using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace LIMSApi.Services
{
    /// <summary>
    /// Phase 9 Universal Report — controlled presentation of frozen history only.
    /// Sources: ExecutionConfigSnapshot.ConfigJson + UniversalTestResult(revision) + Review/Approval.
    /// BANNED here: FormulaEvaluator, spec/MU/factor recalculation, live master re-resolution of config.
    /// Display identity (lab/branch/customer) is snapshotted into ReportDataJson at Generate time.
    /// </summary>
    public class UniversalReportService : IUniversalReportService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        private static readonly HashSet<string> PreviewableStates = new(StringComparer.OrdinalIgnoreCase)
            { "Finalized", "UnderReview", "Verified", "Approved", "Rejected", "ReworkRequired" };

        private readonly LIMSContext _context;
        private readonly ISnapshotIntegrityValidator _integrity;
        private readonly IBranchContext _branchContext;

        public UniversalReportService(
            LIMSContext context,
            ISnapshotIntegrityValidator integrity,
            IBranchContext branchContext)
        {
            _context = context;
            _integrity = integrity;
            _branchContext = branchContext;
        }

        public async Task<UniversalReportPreviewDto> PreviewAsync(long testExecutionId, long userId, long branchId, long organizationId, string? requestedFormatCode = null, bool canViewAllBranches = false)
        {
            var (execution, result, snapshot) = await LoadApprovedLineageAsync(testExecutionId, branchId, organizationId, canViewAllBranches, requireApproved: false);
            if (!PreviewableStates.Contains(result.ResultStatus))
                throw new InvalidOperationException($"Report preview requires a finalized result. Current: '{result.ResultStatus}'.");
            var data = await AssembleAsync(execution, result, snapshot, reportNo: null, reportRevisionNo: 0, requestedFormatCode: requestedFormatCode);
            var hash = CanonicalJsonSerializer.ComputeSha256Hash(CanonicalJsonSerializer.SerializeCanonical(data));
            await AddAuditAsync(result.ID, execution.ID, result.RevisionNo, userId, "ReportPreviewed", result.SnapshotHash,
                JsonSerializer.Serialize(new { testExecutionId, resultId = result.ID, revisionNo = result.RevisionNo, format = data.ReportFormatCode }, JsonOptions));
            await _context.SaveChangesAsync();
            return new UniversalReportPreviewDto
            {
                TestExecutionID = execution.ID,
                UniversalTestResultID = result.ID,
                ResultRevisionNo = result.RevisionNo,
                OverallDecision = result.OverallDecision,
                SnapshotHash = result.SnapshotHash,
                IsPreview = true,
                Watermark = "UNCONTROLLED PREVIEW",
                Data = data,
                ReportDataHash = hash,
                ReportFormatCode = data.ReportFormatCode,
                ReportFormatSource = data.ReportFormatSource
            };
        }

        public async Task<UniversalReportDetailDto> GenerateAsync(long testExecutionId, long userId, long branchId, long organizationId, string? remarks = null, string? requestedFormatCode = null, bool canViewAllBranches = false)
        {
            var (execution, result, snapshot) = await LoadApprovedLineageAsync(testExecutionId, branchId, organizationId, canViewAllBranches, requireApproved: true);
            await EnsureReleasableAsync(execution, result);

            // Duplicate unreleased GENERATED report protection:
            var existingGenerated = await _context.UniversalReports
                .Where(r => r.TestExecutionID == execution.ID && r.ResultRevisionNo == result.RevisionNo && r.Status == "GENERATED" && r.IsActive)
                .OrderByDescending(r => r.ReportRevisionNo)
                .FirstOrDefaultAsync();
            if (existingGenerated != null && string.IsNullOrWhiteSpace(remarks))
            {
                return await MapDetailAsync(existingGenerated.ID, branchId, organizationId, canViewAllBranches)
                    ?? throw new KeyNotFoundException("Existing generated report not found.");
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var tenant = _branchContext.ResolveTenantContext(execution.BranchID);
                var reportNo = await NextReportNoAsync(tenant.CompanyCode);
                var nextRev = await _context.UniversalReports
                    .Where(r => r.TestExecutionID == execution.ID && r.ResultRevisionNo == result.RevisionNo)
                    .Select(r => (int?)r.ReportRevisionNo).MaxAsync() ?? 0;
                nextRev++;

                var data = await AssembleAsync(execution, result, snapshot, reportNo, nextRev, requestedFormatCode: requestedFormatCode);
                var canonical = CanonicalJsonSerializer.SerializeCanonical(data);
                var dataHash = CanonicalJsonSerializer.ComputeSha256Hash(canonical);
                var resultHash = CanonicalJsonSerializer.ComputeSha256Hash(CanonicalJsonSerializer.SerializeCanonical(new
                {
                    result.ID,
                    result.RevisionNo,
                    result.SnapshotHash,
                    result.OverallDecision,
                    result.DecisionRule,
                    result.AcceptanceCriteriaCode
                }));

                var entity = new UniversalReport
                {
                    TestExecutionID = execution.ID,
                    UniversalTestResultID = result.ID,
                    ResultRevisionNo = result.RevisionNo,
                    ReportRevisionNo = nextRev,
                    ReportNo = reportNo,
                    SnapshotHash = result.SnapshotHash,
                    ResultRevisionHash = resultHash,
                    ReportDataJson = JsonSerializer.Serialize(data, JsonOptions),
                    ReportDataHash = dataHash,
                    Status = "GENERATED",
                    BranchID = execution.BranchID,
                    OrganizationID = execution.OrganizationID,
                    GeneratedOn = DateTime.UtcNow,
                    GeneratedBy = userId,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = tenant.CompanyCode,
                    IsActive = true
                };
                _context.UniversalReports.Add(entity);
                await _context.SaveChangesAsync();

                // Internal controlled GENERATED document has watermark
                await RenderAndStorePdfAsync(entity, data, watermarkText: "UNCONTROLLED INTERNAL COPY");
                await AddAuditAsync(result.ID, execution.ID, result.RevisionNo, userId, "ReportGenerated", result.SnapshotHash,
                    JsonSerializer.Serialize(new { reportId = entity.ID, reportNo, reportRevisionNo = nextRev, resultRevisionNo = result.RevisionNo, reportDataHash = dataHash, pdfHash = entity.PdfHash, remarks }, JsonOptions));
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return await MapDetailAsync(entity.ID, branchId, organizationId, canViewAllBranches)
                    ?? throw new KeyNotFoundException("Generated report not found after save.");
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<UniversalReportDetailDto> ReleaseAsync(long reportId, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var entity = await LoadOwnedAsync(reportId, branchId, organizationId, canViewAllBranches: false);
            if (entity.Status != "GENERATED")
                throw new InvalidOperationException($"Only GENERATED reports can be released. Current: '{entity.Status}'.");
            var execution = await _context.TestExecutions.AsNoTracking().FirstOrDefaultAsync(e => e.ID == entity.TestExecutionID)
                ?? throw new KeyNotFoundException("Linked execution not found.");
            var result = await _context.UniversalTestResults.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ID == entity.UniversalTestResultID && r.IsActive)
                ?? throw new KeyNotFoundException("Linked result not found.");
            if (result.ResultStatus != "Approved")
                throw new InvalidOperationException($"Release requires the linked result to still be Approved. Current: '{result.ResultStatus}'.");
            await EnsureReleasableAsync(execution, result);

            var tenant = _branchContext.ResolveTenantContext(execution.BranchID);

            // Deserialize frozen data
            var data = string.IsNullOrWhiteSpace(entity.ReportDataJson)
                ? new UniversalReportDataDto()
                : JsonSerializer.Deserialize<UniversalReportDataDto>(entity.ReportDataJson, JsonOptions) ?? new UniversalReportDataDto();

            // Release-time ULR Assignment (NABL 15 June 2026 Policy):
            // Only assign if report is within accredited scope and ULR is applicable (and not exempt)
            if (data.ShowNablMark && data.ULRApplicable && data.ULRRequired && string.IsNullOrWhiteSpace(data.UlrNo))
            {
                var certNo = data.AccreditationCertificateNo ?? data.NablCertNo;
                var ulrNo = await GenerateUlrNumberAsync(tenant.CompanyCode, certNo, DateTime.UtcNow);
                data.UlrNo = ulrNo;
                entity.ReportDataJson = JsonSerializer.Serialize(data, JsonOptions);
                entity.ReportDataHash = CanonicalJsonSerializer.ComputeSha256Hash(CanonicalJsonSerializer.SerializeCanonical(data));
            }

            entity.Status = "RELEASED";
            entity.ReleasedOn = DateTime.UtcNow;
            entity.ReleasedBy = userId;
            entity.ModifiedBy = userId;
            entity.ModifiedOn = DateTime.UtcNow;

            // Official released PDF: watermark removed (null), ULR and QR code embedded
            await RenderAndStorePdfAsync(entity, data, watermarkText: null);

            await AddAuditAsync(result.ID, execution.ID, result.RevisionNo, userId, "ReportReleased", result.SnapshotHash,
                JsonSerializer.Serialize(new { reportId = entity.ID, entity.ReportNo, entity.ReportRevisionNo, ulrNo = data.UlrNo, remarks }, JsonOptions));
            await _context.SaveChangesAsync();
            return await MapDetailAsync(entity.ID, branchId, organizationId, canViewAllBranches: true)
                ?? throw new KeyNotFoundException("Released report not found after save.");
        }

        public async Task<UniversalReportDetailDto> ReissueAsync(long reportId, long userId, long branchId, long organizationId, string? reason = null)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reissue requires a reason.");
            var old = await LoadOwnedAsync(reportId, branchId, organizationId, canViewAllBranches: false);
            if (old.Status != "RELEASED")
                throw new InvalidOperationException($"Only RELEASED reports can be reissued. Current: '{old.Status}'.");

            var oldData = string.IsNullOrWhiteSpace(old.ReportDataJson)
                ? null
                : JsonSerializer.Deserialize<UniversalReportDataDto>(old.ReportDataJson, JsonOptions);

            // Reissue = new report revision from the CURRENT approved result revision (never overwrite)
            var detail = await GenerateAsync(old.TestExecutionID, userId, branchId, organizationId, $"Reissue of {old.ReportNo}: {reason}");

            // Retain previous ULR if applicable under June 2026 amendment rule (same ULR may be retained)
            if (oldData != null && !string.IsNullOrWhiteSpace(oldData.UlrNo))
            {
                var newEntity = await _context.UniversalReports.FirstOrDefaultAsync(r => r.ID == detail.ID);
                if (newEntity != null)
                {
                    var newData = JsonSerializer.Deserialize<UniversalReportDataDto>(newEntity.ReportDataJson, JsonOptions) ?? new UniversalReportDataDto();
                    newData.UlrNo = oldData.UlrNo; // Retain ULR
                    newEntity.ReportDataJson = JsonSerializer.Serialize(newData, JsonOptions);
                    newEntity.ReportDataHash = CanonicalJsonSerializer.ComputeSha256Hash(CanonicalJsonSerializer.SerializeCanonical(newData));
                    await RenderAndStorePdfAsync(newEntity, newData, watermarkText: "UNCONTROLLED INTERNAL COPY");
                    await _context.SaveChangesAsync();
                    detail = await MapDetailAsync(newEntity.ID, branchId, organizationId, canViewAllBranches: true) ?? detail;
                }
            }

            var oldEntity = await _context.UniversalReports.FirstOrDefaultAsync(r => r.ID == reportId);
            if (oldEntity != null)
            {
                oldEntity.Status = "SUPERSEDED";
                oldEntity.ModifiedBy = userId;
                oldEntity.ModifiedOn = DateTime.UtcNow;
                await AddAuditAsync(oldEntity.UniversalTestResultID, oldEntity.TestExecutionID, oldEntity.ResultRevisionNo, userId, "ReportReissued", oldEntity.SnapshotHash,
                    JsonSerializer.Serialize(new { supersededReportId = reportId, newReportId = detail.ID, reason }, JsonOptions));
                await _context.SaveChangesAsync();
                return await MapDetailAsync(detail.ID, branchId, organizationId, canViewAllBranches: true) ?? detail;
            }
            return detail;
        }

        public async Task<UniversalReportDetailDto> VoidAsync(long reportId, long userId, long branchId, long organizationId, string? reason = null)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Void requires a reason.");
            var entity = await LoadOwnedAsync(reportId, branchId, organizationId, canViewAllBranches: false);
            if (entity.Status == "VOID")
                throw new InvalidOperationException("Report is already void.");
            entity.Status = "VOID";
            entity.IsActive = false;
            entity.ModifiedBy = userId;
            entity.ModifiedOn = DateTime.UtcNow;
            await AddAuditAsync(entity.UniversalTestResultID, entity.TestExecutionID, entity.ResultRevisionNo, userId, "ReportVoided", entity.SnapshotHash,
                JsonSerializer.Serialize(new { reportId = entity.ID, entity.ReportNo, reason }, JsonOptions));
            await _context.SaveChangesAsync();
            return await MapDetailAsync(entity.ID, branchId, organizationId, canViewAllBranches: true)
                ?? throw new KeyNotFoundException("Voided report not found after save.");
        }

        public async Task<UniversalReportDetailDto?> GetByIdAsync(long reportId, long branchId, long organizationId, bool canViewAllBranches = false)
            => await MapDetailAsync(reportId, branchId, organizationId, canViewAllBranches);

        public async Task<List<UniversalReportListItemDto>> ListByExecutionAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false)
        {
            var q = _context.UniversalReports.AsNoTracking()
                .Where(r => r.TestExecutionID == testExecutionId && r.OrganizationID == organizationId);
            if (!canViewAllBranches)
                q = q.Where(r => r.BranchID == branchId);
            var rows = await q.OrderByDescending(r => r.ReportRevisionNo).ToListAsync();
            var userIds = rows.SelectMany(r => new long?[] { r.GeneratedBy, r.ReleasedBy, r.ModifiedBy }).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
            var names = await ResolveActorNamesAsync(userIds);
            return rows.Select(r =>
            {
                UniversalReportDataDto? d = null;
                try
                {
                    d = string.IsNullOrWhiteSpace(r.ReportDataJson) ? null : JsonSerializer.Deserialize<UniversalReportDataDto>(r.ReportDataJson, JsonOptions);
                }
                catch {}

                return new UniversalReportListItemDto
                {
                    ID = r.ID,
                    TestExecutionID = r.TestExecutionID,
                    UniversalTestResultID = r.UniversalTestResultID,
                    ResultRevisionNo = r.ResultRevisionNo,
                    ReportRevisionNo = r.ReportRevisionNo,
                    ReportNo = r.ReportNo,
                    Status = r.Status,
                    OverallDecision = d?.OverallDecision,
                    ReportDataHash = r.ReportDataHash,
                    PdfHash = r.PdfHash,
                    UlrNo = d?.UlrNo,
                    ShowNablMark = d?.ShowNablMark ?? false,
                    ReportFormatSource = d?.ReportFormatSource ?? "DEFAULT",
                    GeneratedOn = r.GeneratedOn,
                    ReleasedOn = r.ReleasedOn,
                    GeneratedByName = r.GeneratedBy.HasValue && names.TryGetValue(r.GeneratedBy.Value, out var g) ? g : null,
                    ReleasedByName = r.ReleasedBy.HasValue && names.TryGetValue(r.ReleasedBy.Value, out var rel) ? rel : null,
                    ModifiedByName = null,
                    ModifiedOn = r.ModifiedOn
                };
            }).ToList();
        }

        public async Task<(byte[] PdfBytes, string ReportNo, string FileName)> GeneratePdfBytesAsync(long reportId, long branchId, long organizationId, bool canViewAllBranches = false, string? watermark = null)
        {
            var detail = await MapDetailAsync(reportId, branchId, organizationId, canViewAllBranches)
                ?? throw new KeyNotFoundException("Report not found or access denied.");
            if (detail.Status == "VOID")
                throw new InvalidOperationException("Void reports cannot be downloaded.");
            var info = new Reporting.UniversalReportRenderInfo
            {
                ReportStatus = detail.Status,
                GeneratedOn = detail.GeneratedOn,
                GeneratedByName = detail.GeneratedByName,
                ReleasedOn = detail.ReleasedOn,
                ReleasedByName = detail.ReleasedByName,
                ReportDataHash = detail.ReportDataHash,
                ResultRevisionHash = detail.ResultRevisionHash,
                SnapshotHash = detail.SnapshotHash
            };
            var doc = new Reporting.UniversalReportDocument(detail.Data, watermark ?? (detail.Status == "RELEASED" ? null : "UNCONTROLLED PREVIEW"), info);
            var bytes = doc.GeneratePdf();
            return (bytes, detail.ReportNo, $"{detail.ReportNo}.pdf");
        }

        public async Task<List<ReportFormatOptionDto>> GetAvailableFormatsAsync(long organizationId, string companyCode)
        {
            var list = new List<ReportFormatOptionDto>
            {
                new() { FormatCode = "DEFAULT", FormatName = "Default Universal Report", PageLayout = "Portrait", IsDefault = true }
            };
            var configured = await _context.ReportFormats.AsNoTracking()
                .Where(f => f.IsActive && f.CompanyCode == companyCode)
                .OrderBy(f => f.FormatName)
                .Select(f => new ReportFormatOptionDto
                {
                    FormatCode = f.FormatCode,
                    FormatName = f.FormatName,
                    PageLayout = f.PageLayout,
                    IsDefault = f.IsDefault
                }).ToListAsync();
            list.AddRange(configured);
            return list;
        }

        // ── Lineage: snapshot + approved result + approval only ──

        private async Task<(TestExecution execution, UniversalTestResult result, TestExecutionConfigSnapshotDto snapshot)> LoadApprovedLineageAsync(
            long testExecutionId, long branchId, long organizationId, bool canViewAllBranches, bool requireApproved)
        {
            var execution = await _context.TestExecutions.AsNoTracking()
                .Include(e => e.UniversalTestGroup)
                .FirstOrDefaultAsync(e => e.ID == testExecutionId && e.OrganizationID == organizationId);
            if (execution == null)
                throw new KeyNotFoundException("Execution not found or access denied.");
            if (!canViewAllBranches && execution.BranchID != branchId)
                throw new UnauthorizedAccessException("Cross-branch report access denied.");
            if (execution.ExecutionConfigSnapshotID == null)
                throw new InvalidOperationException("Execution has no frozen snapshot; report cannot be built.");

            var snapshotEntity = await _context.ExecutionConfigSnapshots.AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == execution.ExecutionConfigSnapshotID);
            if (snapshotEntity == null || string.IsNullOrWhiteSpace(snapshotEntity.ConfigJson))
                throw new InvalidOperationException("Frozen execution snapshot is missing.");
            TestExecutionConfigSnapshotDto snapshot;
            try
            {
                snapshot = JsonSerializer.Deserialize<TestExecutionConfigSnapshotDto>(snapshotEntity.ConfigJson, JsonOptions)
                    ?? throw new InvalidOperationException("Snapshot payload is empty.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Frozen snapshot is unreadable: {ex.Message}");
            }

            var rq = _context.UniversalTestResults
                .Include(r => r.Parameters)
                .Include(r => r.Findings)
                .Include(r => r.Audits)
                .Where(r => r.TestExecutionID == testExecutionId && r.OrganizationID == organizationId && r.IsActive);
            if (!canViewAllBranches)
                rq = rq.Where(r => r.BranchID == branchId);
            var result = await rq.OrderByDescending(r => r.RevisionNo).FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("No result exists for this execution.");
            if (requireApproved && result.ResultStatus != "Approved")
                throw new InvalidOperationException($"Report generation requires an Approved result. Current: '{result.ResultStatus}'.");

            var integrity = await _integrity.ValidateIntegrityAsync(execution.ID);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Snapshot integrity failed: {string.Join("; ", integrity.ValidationErrors)}. Report blocked.");
            return (execution, result, snapshot);
        }

        private async Task EnsureReleasableAsync(TestExecution execution, UniversalTestResult result)
        {
            // Gate owner is Phase 8 (approved result + review state). UTG status is NOT a gate.
            if (result.ResultStatus != "Approved")
                throw new InvalidOperationException($"Release requires an Approved result. Current: '{result.ResultStatus}'.");
            var latestId = await _context.UniversalTestResults.AsNoTracking()
                .Where(r => r.TestExecutionID == execution.ID && r.IsActive)
                .OrderByDescending(r => r.RevisionNo).Select(r => r.ID).FirstOrDefaultAsync();
            if (latestId != result.ID)
                throw new InvalidOperationException("Stale result revision: a newer approved revision exists. Regenerate from the latest revision.");
            var blocking = await _context.UniversalReviewFindings.AsNoTracking()
                .AnyAsync(f => f.UniversalTestResultID == result.ID && f.IsBlocking && f.Status == "Open" && f.IsActive);
            if (blocking)
                throw new InvalidOperationException("Release blocked: unresolved blocking review findings exist.");
            var integrity = await _integrity.ValidateIntegrityAsync(execution.ID);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Snapshot integrity failed: {string.Join("; ", integrity.ValidationErrors)}. Release blocked.");
        }

        private async Task<UniversalReport> LoadOwnedAsync(long reportId, long branchId, long organizationId, bool canViewAllBranches)
        {
            var q = _context.UniversalReports.Where(r => r.ID == reportId && r.OrganizationID == organizationId);
            if (!canViewAllBranches)
                q = q.Where(r => r.BranchID == branchId);
            return await q.FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Report not found or access denied.");
        }

        // ── NABL Governance & ULR Resolution Engine (NABL 133 & June 2026 Policy) ──

        private class NablResolution
        {
            public bool IsAccreditedLaboratory { get; set; }
            public string AccreditationStatus { get; set; } = "NotAccredited";
            public string? CertificateNumber { get; set; }
            public DateTime? ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public long? BranchID { get; set; }
            public string? LogoPath { get; set; }
            public bool IsWithinAccreditedScope { get; set; }
            public string? ScopeReference { get; set; }
            public bool NablSymbolAllowed { get; set; }
            public bool ShowNablMark { get; set; }
            public bool ULRApplicable { get; set; }
            public bool ULRRequired { get; set; }
            public string? ULRExceptionReason { get; set; }
            public string? ScopeDisclaimer { get; set; }
        }

        private NablResolution ResolveNablReportStatus(
            TestExecution execution,
            TestExecutionConfigSnapshotDto snapshot,
            SnapshotAccreditationDto? cert,
            DateTime refDateUtc)
        {
            var res = new NablResolution();
            var scopeStatus = snapshot.Scope?.ScopeStatus;
            res.ScopeReference = snapshot.Scope?.LabScopeID?.ToString();

            // 1. Laboratory accreditation check
            if (cert == null || cert.AccreditationStatus != "Active")
            {
                res.IsAccreditedLaboratory = false;
                res.AccreditationStatus = "NotAccredited";
                res.NablSymbolAllowed = false;
                res.ShowNablMark = false;
                res.ULRApplicable = false;
                res.ULRRequired = false;
                res.ULRExceptionReason = "Laboratory is not accredited";
                res.ScopeDisclaimer = "This test is not covered under NABL accreditation; reported as a non-accredited laboratory result.";
                return res;
            }

            res.CertificateNumber = cert.CertificateNumber;
            res.ValidFrom = cert.ValidFrom;
            res.ValidTo = cert.ValidTo;
            res.BranchID = cert.BranchID;
            res.LogoPath = cert.LogoPath;

            // 2. Branch/location isolation check
            // NABL accreditation is location-specific. CAB location must match.
            if (cert.BranchID.HasValue && cert.BranchID.Value != execution.BranchID)
            {
                res.IsAccreditedLaboratory = false;
                res.AccreditationStatus = "BranchNotCovered";
                res.NablSymbolAllowed = false;
                res.ShowNablMark = false;
                res.ULRApplicable = false;
                res.ULRRequired = false;
                res.ULRExceptionReason = $"Branch {execution.BranchID} is not covered under accreditation certificate {cert.CertificateNumber}";
                res.ScopeDisclaimer = $"Accreditation applies only to CAB branch location {cert.BranchID}; not applicable to branch {execution.BranchID}.";
                return res;
            }

            // 3. Date validity check
            bool isDateValid = (cert.ValidFrom == null || cert.ValidFrom <= refDateUtc) &&
                               (cert.ValidTo == null || cert.ValidTo >= refDateUtc);
            if (!isDateValid)
            {
                res.IsAccreditedLaboratory = true;
                res.AccreditationStatus = "Expired";
                res.NablSymbolAllowed = false;
                res.ShowNablMark = false;
                res.ULRApplicable = false;
                res.ULRRequired = false;
                res.ULRExceptionReason = $"Accreditation certificate expired on {cert.ValidTo:dd-MM-yyyy}";
                res.ScopeDisclaimer = "Accreditation certificate is not currently effective; accreditation mark not applied.";
                return res;
            }

            res.IsAccreditedLaboratory = true;
            res.AccreditationStatus = "Active";

            // 4. Accredited scope check from frozen snapshot
            bool isWithinScope = string.Equals(scopeStatus, "WithinScope", StringComparison.OrdinalIgnoreCase)
                || (string.Equals(scopeStatus, "Referenced", StringComparison.OrdinalIgnoreCase) && snapshot.Scope?.LabScopeID != null);
            res.IsWithinAccreditedScope = isWithinScope;

            if (!isWithinScope)
            {
                res.NablSymbolAllowed = false;
                res.ShowNablMark = false;
                res.ULRApplicable = false;
                res.ULRRequired = false;
                res.ULRExceptionReason = "Test is outside accredited scope";
                res.ScopeDisclaimer = scopeStatus switch
                {
                    var s when string.Equals(s, "OutsideScope", StringComparison.OrdinalIgnoreCase)
                        => "Results are outside the accredited scope; accreditation mark not applied.",
                    var s when string.Equals(s, "NotAccredited", StringComparison.OrdinalIgnoreCase)
                        => "This test is not covered under NABL accreditation; reported as a non-accredited laboratory result.",
                    _ => "Accreditation scope reference was not recorded as WithinScope for this execution; accreditation mark not applied."
                };
                return res;
            }

            // 5. Within Accredited Scope + Valid Accreditation:
            // IMPORTANT: NABL symbol allowed regardless of PASS/FAIL!
            res.NablSymbolAllowed = true;
            res.ShowNablMark = true;
            res.ScopeDisclaimer = null;

            // 6. ULR Applicability resolution (NABL 15 June 2026 clarification):
            // ULR is NOT mandatory for:
            // - Soil & Rock
            // - Environmental non-commodity (air, wastewater)
            // - In-house testing
            // - Cell culture, veterinary, dope, forensic
            bool isSoilOrRock = IsSoilOrRockTesting(snapshot);
            if (isSoilOrRock)
            {
                res.ULRApplicable = false;
                res.ULRRequired = false;
                res.ULRExceptionReason = "Soil & Rock testing exempt under NABL ULR policy (Clarification 15.06.2026)";
            }
            else
            {
                res.ULRApplicable = true;
                res.ULRRequired = true;
                res.ULRExceptionReason = null;
            }

            return res;
        }

        private static bool IsSoilOrRockTesting(TestExecutionConfigSnapshotDto snapshot)
        {
            var d = (snapshot.DisciplineName ?? "").ToLowerInvariant();
            var t = (snapshot.LaboratoryTestName ?? "").ToLowerInvariant();
            var tc = (snapshot.LaboratoryTestCode ?? "").ToLowerInvariant();
            var g = (snapshot.GradeName ?? "").ToLowerInvariant();
            return d.Contains("soil") || d.Contains("rock") || d.Contains("geotech")
                || t.Contains("soil") || t.Contains("rock") || t.Contains("cbr") || t.Contains("california bearing")
                || tc.Contains("cbr") || g.Contains("soil");
        }

        // ── Report Format Resolution (Dual Mode: DEFAULT or CONFIGURED) ──

        private async Task<(ReportFormat? format, string formatSource)> ResolveReportFormatAsync(
            string? requestedFormatCode,
            long? laboratoryTestId,
            long? testMethodId,
            string companyCode)
        {
            // 1. Explicit request
            if (!string.IsNullOrWhiteSpace(requestedFormatCode) && !string.Equals(requestedFormatCode, "DEFAULT", StringComparison.OrdinalIgnoreCase))
            {
                var fmt = await _context.ReportFormats.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FormatCode == requestedFormatCode && f.IsActive && f.CompanyCode == companyCode);
                if (fmt != null) return (fmt, "CONFIGURED");
            }

            // 2. Mapped format
            if (laboratoryTestId.HasValue || testMethodId.HasValue)
            {
                var mapping = await _context.ReportFormatMappings.AsNoTracking()
                    .Include(m => m.ReportFormat)
                    .Where(m => m.IsActive && m.CompanyCode == companyCode && m.ReportFormat != null && m.ReportFormat.IsActive
                        && ((laboratoryTestId.HasValue && m.LaboratoryTestID == laboratoryTestId)
                            || (testMethodId.HasValue && m.TestMethodID == testMethodId)))
                    .OrderByDescending(m => m.Priority)
                    .FirstOrDefaultAsync();

                if (mapping?.ReportFormat != null)
                    return (mapping.ReportFormat, "CONFIGURED");
            }

            // 3. Fallback to DEFAULT Universal Report format
            return (null, "DEFAULT");
        }

        // ── Assembly: frozen values only, display identity snapshotted ──

        private async Task<UniversalReportDataDto> AssembleAsync(
            TestExecution execution,
            UniversalTestResult result,
            TestExecutionConfigSnapshotDto snapshot,
            string? reportNo,
            int reportRevisionNo,
            string? requestedFormatCode = null)
        {
            var utg = execution.UniversalTestGroup;
            var tenant = _branchContext.ResolveTenantContext(execution.BranchID);

            var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.ID == execution.BranchID);
            var org = await _context.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == execution.OrganizationID);

            string? deptName = null;
            if (utg?.DepartmentID != null)
                deptName = await _context.DepartmentMasters.AsNoTracking().Where(d => d.ID == utg.DepartmentID).Select(d => d.Name).FirstOrDefaultAsync();

            var actorIds = new List<long?> { execution.ExecutionAnalystID, result.ReviewerID, result.VerifiedBy, result.ApprovedBy, result.FinalizedBy }
                .Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
            var actorProfiles = await ResolveActorProfilesAsync(actorIds);
            var names = actorProfiles.ToDictionary(k => k.Key, v => v.Value.Name);

            // NABL resolution (Historical Provenance Freeze)
            // Use snapshot accreditation. If missing (legacy executions), fallback to live DB but frozen at execution completion time.
            var refDateUtc = execution.CompletedOn ?? execution.CreatedOn;
            SnapshotAccreditationDto? cert = snapshot.Accreditation;
            
            if (cert == null)
            {
                var liveCert = await _context.NablAccreditations.AsNoTracking()
                    .Where(n => n.IsActive && n.CompanyCode == tenant.CompanyCode && n.OrganizationId == execution.OrganizationID
                        && (n.BranchID == null || n.BranchID == execution.BranchID))
                    .OrderByDescending(n => n.BranchID.HasValue ? 1 : 0)
                    .ThenByDescending(n => n.ExpiryDate)
                    .FirstOrDefaultAsync();
                    
                if (liveCert != null)
                {
                    cert = new SnapshotAccreditationDto
                    {
                        AccreditationID = liveCert.Id,
                        CertificateNumber = liveCert.CertificateNumber,
                        ValidFrom = liveCert.IssueDate,
                        ValidTo = liveCert.ExpiryDate,
                        BranchID = liveCert.BranchID,
                        LogoPath = liveCert.LogoPath,
                        AccreditationStatus = (liveCert.IssueDate == default || liveCert.IssueDate <= refDateUtc) &&
                                              (liveCert.ExpiryDate == default || liveCert.ExpiryDate >= refDateUtc) 
                                              ? "Active" : "Expired"
                    };
                }
            }

            var nabl = ResolveNablReportStatus(execution, snapshot, cert, refDateUtc);

            // Report Format Resolution
            long? labTestId = snapshot.LaboratoryTestID != 0 ? snapshot.LaboratoryTestID : execution.UniversalTestGroup?.LaboratoryTestID;
            long? testMethodId = snapshot.TestMethodSpecificationID ?? execution.UniversalTestGroup?.TestMethodSpecificationID;

            var (format, formatSource) = await ResolveReportFormatAsync(
                requestedFormatCode,
                labTestId,
                testMethodId,
                tenant.CompanyCode);

            // Format controls presentation, but it cannot override accreditation policy:
            var showNablMark = nabl.NablSymbolAllowed;

            // Extract customer and sample information dynamically
            string? sampleNo = null, sampleDesc = null, gradeName = snapshot.GradeName, caseNo = null, customerName = null, customerAddress = null;
            string? contactPerson = null, customerRef = null, sampleCondition = null, samplingSource = null, samplingPlan = null;
            DateTime? receivedOn = null;
            if (utg != null)
            {
                var plan = await _context.TestPlans.AsNoTracking()
                    .Include(p => p.SampleDetail).ThenInclude(s => s!.SampleInward)
                    .FirstOrDefaultAsync(p => p.ID == utg.SampleTestPlanID);
                var sample = plan?.SampleDetail;
                if (sample != null)
                {
                    sampleNo = sample.SampleNo;
                    sampleDesc = !string.IsNullOrWhiteSpace(sample.Details) ? sample.Details : sample.Specimen;
                    receivedOn = sample.CreatedOn;
                    sampleCondition = !string.IsNullOrWhiteSpace(sample.Remarks) ? sample.Remarks : "As Received";
                    
                    var inward = sample.SampleInward;
                    if (inward != null)
                    {
                        caseNo = inward.CaseNo;
                        customerRef = !string.IsNullOrWhiteSpace(inward.CaseNo) ? $"PO/Case: {inward.CaseNo}" : null;
                        samplingSource = inward.ReturnSample ? "Customer Supplied (Returnable)" : "Customer Supplied";
                        samplingPlan = "Not Applicable";
                        
                        var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.ID == inward.CustomerID);
                        customerName = customer?.Name;
                        var cp = await _context.ContactPersons.AsNoTracking().FirstOrDefaultAsync(p => p.CustomerID == inward.CustomerID);
                        contactPerson = cp != null ? (!string.IsNullOrWhiteSpace(cp.Salutation) ? $"{cp.Salutation} {cp.Name}" : cp.Name) : null;
                        customerAddress = !string.IsNullOrWhiteSpace(inward.Address)
                            ? string.Join(", ", new[] { inward.Address, inward.Area, inward.City, inward.State, inward.PinCode }.Where(s => !string.IsNullOrWhiteSpace(s)))
                            : (customer != null ? string.Join(", ", new[] { customer.Address, customer.PinCode }.Where(s => !string.IsNullOrWhiteSpace(s))) : null);
                    }
                }
            }

            var data = new UniversalReportDataDto
            {
                ReportNo = reportNo,
                TestExecutionID = execution.ID,
                ExecutionNo = execution.ExecutionNo,
                IsRetest = execution.IsRetest,
                PreviousExecutionID = execution.PreviousExecutionID,
                UniversalTestGroupID = execution.UniversalTestGroupID,
                UniversalTestResultID = result.ID,
                ResultRevisionNo = result.RevisionNo,
                ReportRevisionNo = reportRevisionNo,
                OverallDecision = result.OverallDecision,
                DecisionRule = result.DecisionRule,
                AcceptanceCriteriaCode = result.AcceptanceCriteriaCode,
                SnapshotHash = result.SnapshotHash,
                ExecutionConfigSnapshotID = result.ExecutionConfigSnapshotID,

                // Format metadata
                ReportFormatCode = format?.FormatCode ?? "DEFAULT",
                ReportFormatName = format?.FormatName ?? "Default Universal Report",
                ReportFormatSource = formatSource,

                LabName = org?.LabName ?? "Devine Laboratory",
                LabAddress = branch?.Address ?? org?.LabAddress ?? string.Empty,
                LabPhone = org?.ContactPhone ?? org?.MobileNo ?? string.Empty,
                LabEmail = org?.ContactEmail ?? string.Empty,
                LabWebsite = org?.Website ?? string.Empty,
                LabSubtitle = null,
                LabTagline = null,
                LabLogoPath = org?.OrganizationLogo,
                BranchName = branch?.Name,
                BranchAddress = branch?.Address,
                DepartmentName = deptName,

                CustomerName = customerName ?? "—",
                CustomerAddress = customerAddress ?? "—",
                ContactPerson = contactPerson ?? "—",
                CustomerReference = customerRef ?? "—",
                CaseNo = caseNo,
                SampleNo = sampleNo ?? "—",
                SampleDescription = sampleDesc ?? gradeName ?? "—",
                GradeName = gradeName,
                SampleCondition = sampleCondition ?? "As Received",
                SamplingSource = samplingSource ?? "Customer Supplied",
                SamplingPlan = samplingPlan ?? "Not Applicable",
                SampleReceivedOn = receivedOn ?? execution.CreatedOn,
                TestCommencedOn = execution.StartedOn ?? execution.CreatedOn,
                TestCompletedOn = execution.CompletedOn ?? DateTime.UtcNow,
                DateTested = execution.CompletedOn ?? execution.CreatedOn,

                LaboratoryTestName = snapshot.LaboratoryTestName,
                LaboratoryTestCode = snapshot.LaboratoryTestCode,
                DisciplineName = snapshot.DisciplineName,
                TestMethodName = snapshot.TestMethodName,
                TestMethodStandard = snapshot.TestMethodStandard,
                TestMethodVersion = snapshot.TestMethodVersion,
                SpecificationTitle = snapshot.SpecificationTitle,
                ExecutionLayoutCode = snapshot.ExecutionLayoutCode,
                ExecutionLayoutName = snapshot.ExecutionLayoutName,
                RendererType = snapshot.RendererType,
                TestEquipment = snapshot.Equipment != null && snapshot.Equipment.Count > 0
                    ? string.Join(", ", snapshot.Equipment.Select(e => !string.IsNullOrWhiteSpace(e.Model) ? $"{e.Name} ({e.Model})" : e.Name))
                    : (snapshot.LaboratoryTestName ?? "—"),
                EnvironmentalConditions = snapshot.Conditions != null && snapshot.Conditions.Count > 0
                    ? string.Join(" | ", snapshot.Conditions.Select(c => $"{c.DimensionName}: {c.ActualExecutionValue ?? c.ConfiguredValue1} {c.Unit}".Trim()))
                    : "—",

                Parameters = snapshot.Parameters,
                Conditions = snapshot.Conditions,
                Equipment = snapshot.Equipment,
                Factors = snapshot.Factors,
                MeasurementUncertainty = snapshot.MeasurementUncertainty,
                AcceptanceCriteria = snapshot.AcceptanceCriteria,
                Scope = snapshot.Scope,
                ActualConditionsJson = execution.ActualConditionsJson,
                ActualEquipmentJson = execution.ActualEquipmentJson,

                ResultParameters = result.Parameters.Where(p => p.IsActive).Select(p => new UniversalResultParameterDto
                {
                    ID = p.ID,
                    ParameterMasterID = p.ParameterMasterID,
                    ParameterCode = p.ParameterCode,
                    ParameterName = p.ParameterName,
                    InputType = p.InputType,
                    Unit = p.Unit,
                    DecimalPrecision = p.DecimalPrecision,
                    IsCalculated = p.IsCalculated,
                    IsMandatory = p.IsMandatory,
                    IsReportable = p.IsReportable,
                    DisplayOrder = p.DisplayOrder,
                    RawValue = p.RawValue,
                    RawNumericValue = p.RawNumericValue,
                    AppliedFactorCode = p.AppliedFactorCode,
                    AppliedFactorOperation = p.AppliedFactorOperation,
                    FactoredValue = p.FactoredValue,
                    Formula = p.Formula,
                    SubstitutionTrace = p.SubstitutionTrace,
                    CalculatedValue = p.CalculatedValue,
                    ComplianceValue = p.ComplianceValue,
                    DisplayValue = p.DisplayValue,
                    ReportedValue = p.ReportedValue,
                    SpecMin = p.SpecMin,
                    SpecMax = p.SpecMax,
                    SpecTarget = p.SpecTarget,
                    MinTolerance = p.MinTolerance,
                    MaxTolerance = p.MaxTolerance,
                    EffectiveMin = p.EffectiveMin,
                    EffectiveMax = p.EffectiveMax,
                    ToleranceSource = p.ToleranceSource,
                    ToleranceType = p.ToleranceType,
                    AppliedTolerance = p.AppliedTolerance,
                    ComplianceValueSource = p.ComplianceValueSource,
                    RequirementStatus = p.RequirementStatus,
                    Verdict = p.Verdict,
                    CombinedUncertainty = p.CombinedUncertainty,
                    ExpandedUncertainty = p.ExpandedUncertainty,
                    CoverageFactor = p.CoverageFactor,
                    MUSource = p.MUSource,
                    GuardBandApplied = p.GuardBandApplied,
                    EvaluationNote = p.EvaluationNote
                }).OrderBy(p => p.DisplayOrder).ToList(),
                CalculationTraceJson = result.CalculationTraceJson,
                ComplianceSummaryJson = result.ComplianceSummaryJson,

                // NABL & ULR Governance
                ScopeStatus = nabl.IsWithinAccreditedScope ? "WithinScope" : (snapshot.Scope?.ScopeStatus ?? "OutsideScope"),
                ShowNablMark = showNablMark,
                NablCertNo = nabl.CertificateNumber,
                NablLogoPath = nabl.LogoPath,
                UlrNo = null, // Set on official Release!
                ScopeDisclaimer = nabl.ScopeDisclaimer,
                IsAccreditedLaboratory = nabl.IsAccreditedLaboratory,
                AccreditationStatus = nabl.AccreditationStatus,
                AccreditationCertificateNo = nabl.CertificateNumber,
                AccreditationValidFrom = nabl.ValidFrom,
                AccreditationValidTo = nabl.ValidTo,
                AccreditedBranchID = nabl.BranchID,
                IsWithinAccreditedScope = nabl.IsWithinAccreditedScope,
                ScopeReference = nabl.ScopeReference,
                NablSymbolAllowed = nabl.NablSymbolAllowed,
                ULRApplicable = nabl.ULRApplicable,
                ULRRequired = nabl.ULRRequired,
                ULRExceptionReason = nabl.ULRExceptionReason,

                AnalystID = execution.ExecutionAnalystID,
                AnalystName = execution.ExecutionAnalystID.HasValue && actorProfiles.TryGetValue(execution.ExecutionAnalystID.Value, out var anProfile) ? anProfile.Name : null,
                AnalystDesignation = execution.ExecutionAnalystID.HasValue && actorProfiles.TryGetValue(execution.ExecutionAnalystID.Value, out var anProfile2) ? anProfile2.Designation : null,
                AnalystSignaturePath = execution.ExecutionAnalystID.HasValue && actorProfiles.TryGetValue(execution.ExecutionAnalystID.Value, out var anProfile3) ? anProfile3.DigitalSignature : null,

                ReviewerID = result.ReviewerID,
                ReviewerName = result.ReviewerID.HasValue && actorProfiles.TryGetValue(result.ReviewerID.Value, out var rnProfile) ? rnProfile.Name : null,
                ReviewerDesignation = result.ReviewerID.HasValue && actorProfiles.TryGetValue(result.ReviewerID.Value, out var rnProfile2) ? rnProfile2.Designation : null,
                ReviewerSignaturePath = result.ReviewerID.HasValue && actorProfiles.TryGetValue(result.ReviewerID.Value, out var rnProfile3) ? rnProfile3.DigitalSignature : null,

                VerifiedBy = result.VerifiedBy,
                VerifiedByName = result.VerifiedBy.HasValue && actorProfiles.TryGetValue(result.VerifiedBy.Value, out var vnProfile) ? vnProfile.Name : null,
                VerifiedOn = result.VerifiedOn,

                ApprovedBy = result.ApprovedBy,
                ApprovedByName = result.ApprovedBy.HasValue && actorProfiles.TryGetValue(result.ApprovedBy.Value, out var apnProfile) ? apnProfile.Name : null,
                ApproverDesignation = result.ApprovedBy.HasValue && actorProfiles.TryGetValue(result.ApprovedBy.Value, out var apnProfile2) ? apnProfile2.Designation : null,
                ApproverSignaturePath = result.ApprovedBy.HasValue && actorProfiles.TryGetValue(result.ApprovedBy.Value, out var apnProfile3) ? apnProfile3.DigitalSignature : null,
                ApprovedOn = result.ApprovedOn,
                ReviewRemarks = result.ReviewRemarks,
                ApprovalRemarks = result.ApprovalRemarks,
                Findings = result.Findings.Where(f => f.IsActive).Select(f => new UniversalReviewFindingDto
                {
                    ID = f.ID,
                    UniversalTestResultID = f.UniversalTestResultID,
                    TestExecutionID = f.TestExecutionID,
                    FindingType = f.FindingType,
                    Description = f.Description,
                    Severity = f.Severity,
                    Status = f.Status,
                    IsBlocking = f.IsBlocking,
                    Resolution = f.Resolution,
                    ResolvedBy = f.ResolvedBy,
                    ResolvedOn = f.ResolvedOn
                }).ToList(),
            };
            return data;
        }

        // ── Concurrency-safe numbering: UPDLOCK rowlock, never MAX+1 ──

        private async Task<string> NextReportNoAsync(string companyCode)
        {
            var existing = await _context.NumberingConfigs
                .FromSqlRaw("SELECT * FROM NumberingConfigs WITH (UPDLOCK, ROWLOCK) WHERE ModuleName = 'UREPORT' AND CompanyCode = {0}", companyCode)
                .FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.CurrentNumber++;
                return $"{existing.Prefix}-{DateTime.UtcNow:yyyyMMdd}-{existing.CurrentNumber:D4}";
            }
            var orgId = LoggedInUserProvider.CurrentUser?.OrganizationID ?? 0;
            _context.NumberingConfigs.Add(new NumberingConfig
            {
                ModuleName = "UREPORT",
                Prefix = "URPT",
                StartNumber = 1,
                CurrentNumber = 1,
                OrganizationId = orgId,
                BranchId = null,
                CreatedBy = LoggedInUserProvider.CurrentUser?.UserId ?? 0,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = companyCode,
                IsActive = true
            });
            return $"URPT-{DateTime.UtcNow:yyyyMMdd}-0001";
        }

        // ── ULR Generation Engine (NABL 15 June 2026 Clarification) ──
        // Format: <CertificatePrefix><YY><8-digit sequence>
        // Restarts annually on 1st January. Supports both legacy TC-XXXXX and 2026 certificate formats.

        private async Task<string> GenerateUlrNumberAsync(string companyCode, string? certNo, DateTime refDate)
        {
            var yearStr = refDate.ToString("yy");
            var yearInt = refDate.Year;

            var cleanCert = string.IsNullOrWhiteSpace(certNo)
                ? "TC0000"
                : System.Text.RegularExpressions.Regex.Replace(certNo, @"[^A-Za-z0-9]", "").ToUpperInvariant();

            var moduleKey = $"ULR_{yearInt}";

            var existing = await _context.NumberingConfigs
                .FromSqlRaw("SELECT * FROM NumberingConfigs WITH (UPDLOCK, ROWLOCK) WHERE ModuleName = {0} AND CompanyCode = {1}", moduleKey, companyCode)
                .FirstOrDefaultAsync();

            long nextSeq;
            if (existing != null)
            {
                existing.CurrentNumber++;
                nextSeq = existing.CurrentNumber;
            }
            else
            {
                nextSeq = 1;
                _context.NumberingConfigs.Add(new NumberingConfig
                {
                    ModuleName = moduleKey,
                    Prefix = cleanCert,
                    StartNumber = 1,
                    CurrentNumber = 1,
                    OrganizationId = LoggedInUserProvider.CurrentUser?.OrganizationID ?? 0,
                    BranchId = null,
                    CreatedBy = LoggedInUserProvider.CurrentUser?.UserId ?? 0,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = companyCode,
                    IsActive = true
                });
            }

            return $"{cleanCert}{yearStr}{nextSeq:D8}";
        }

        private async Task RenderAndStorePdfAsync(UniversalReport entity, UniversalReportDataDto data, string? watermarkText = null)
        {
            var names = await ResolveActorNamesAsync(new List<long> { entity.GeneratedBy ?? 0, entity.ReleasedBy ?? 0 }.Where(x => x != 0).ToList());
            var info = new Reporting.UniversalReportRenderInfo
            {
                ReportStatus = entity.Status,
                GeneratedOn = entity.GeneratedOn,
                GeneratedByName = entity.GeneratedBy.HasValue && names.TryGetValue(entity.GeneratedBy.Value, out var g) ? g : null,
                ReleasedOn = entity.ReleasedOn,
                ReleasedByName = entity.ReleasedBy.HasValue && names.TryGetValue(entity.ReleasedBy.Value, out var rel) ? rel : null,
                ReportDataHash = entity.ReportDataHash,
                ResultRevisionHash = entity.ResultRevisionHash,
                SnapshotHash = entity.SnapshotHash
            };
            var doc = new Reporting.UniversalReportDocument(data, watermarkText: watermarkText, info: info);
            var bytes = doc.GeneratePdf();
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "universal-reports");
            Directory.CreateDirectory(dir);
            var fileName = $"{entity.ReportNo}.pdf";
            var fullPath = Path.Combine(dir, fileName);
            await File.WriteAllBytesAsync(fullPath, bytes);
            entity.PdfPath = $"universal-reports/{fileName}";
            using var sha = System.Security.Cryptography.SHA256.Create();
            entity.PdfHash = Convert.ToHexString(sha.ComputeHash(bytes)).ToLowerInvariant();
        }

        private async Task<UniversalReportDetailDto?> MapDetailAsync(long reportId, long branchId, long organizationId, bool canViewAllBranches)
        {
            var q = _context.UniversalReports.AsNoTracking()
                .Where(r => r.ID == reportId && r.OrganizationID == organizationId);
            if (!canViewAllBranches)
                q = q.Where(r => r.BranchID == branchId);
            var r = await q.FirstOrDefaultAsync();
            if (r == null) return null;
            UniversalReportDataDto data;
            try
            {
                data = string.IsNullOrWhiteSpace(r.ReportDataJson)
                    ? new UniversalReportDataDto()
                    : JsonSerializer.Deserialize<UniversalReportDataDto>(r.ReportDataJson, JsonOptions) ?? new UniversalReportDataDto();
            }
            catch (JsonException)
            {
                data = new UniversalReportDataDto();
            }
            var names = await ResolveActorNamesAsync(new List<long> { r.GeneratedBy ?? 0, r.ReleasedBy ?? 0 }.Where(x => x != 0).ToList());
            return new UniversalReportDetailDto
            {
                ID = r.ID,
                TestExecutionID = r.TestExecutionID,
                UniversalTestResultID = r.UniversalTestResultID,
                ResultRevisionNo = r.ResultRevisionNo,
                ReportRevisionNo = r.ReportRevisionNo,
                ReportNo = r.ReportNo,
                Status = r.Status,
                OverallDecision = data.OverallDecision,
                ReportDataHash = r.ReportDataHash,
                PdfHash = r.PdfHash,
                UlrNo = data.UlrNo,
                ShowNablMark = data.ShowNablMark,
                ReportFormatSource = data.ReportFormatSource,
                GeneratedOn = r.GeneratedOn,
                ReleasedOn = r.ReleasedOn,
                GeneratedByName = r.GeneratedBy.HasValue && names.TryGetValue(r.GeneratedBy.Value, out var g) ? g : null,
                ReleasedByName = r.ReleasedBy.HasValue && names.TryGetValue(r.ReleasedBy.Value, out var rel) ? rel : null,
                ModifiedOn = r.ModifiedOn,
                SnapshotHash = r.SnapshotHash,
                ResultRevisionHash = r.ResultRevisionHash,
                PdfPath = r.PdfPath,
                BranchID = r.BranchID,
                OrganizationID = r.OrganizationID,
                Data = data
            };
        }

        private class ActorProfile
        {
            public string Name { get; set; } = string.Empty;
            public string? Designation { get; set; }
            public string? DigitalSignature { get; set; }
        }

        private async Task<Dictionary<long, ActorProfile>> ResolveActorProfilesAsync(List<long> userIds)
        {
            var map = new Dictionary<long, ActorProfile>();
            if (userIds.Count == 0) return map;
            var users = await _context.UserMasters.AsNoTracking().Where(u => userIds.Contains(u.ID)).ToListAsync();
            var empIds = users.Where(u => u.EmployeeID.HasValue).Select(u => u.EmployeeID!.Value).Distinct().ToList();
            var emps = await _context.EmployeeMasters.AsNoTracking()
                .Include(e => e.Designation)
                .Where(e => empIds.Contains(e.ID))
                .ToDictionaryAsync(e => e.ID, e => e);

            foreach (var u in users)
            {
                if (u.EmployeeID.HasValue && emps.TryGetValue(u.EmployeeID.Value, out var emp))
                {
                    map[u.ID] = new ActorProfile
                    {
                        Name = emp.Name,
                        Designation = emp.Designation?.Name,
                        DigitalSignature = emp.DigitalSignature
                    };
                }
                else
                {
                    map[u.ID] = new ActorProfile
                    {
                        Name = u.UserName ?? $"User {u.ID}",
                        Designation = u.RoleName,
                        DigitalSignature = null
                    };
                }
            }
            return map;
        }

        private async Task<Dictionary<long, string>> ResolveActorNamesAsync(List<long> userIds)
        {
            var map = new Dictionary<long, string>();
            if (userIds.Count == 0) return map;
            var users = await _context.UserMasters.AsNoTracking().Where(u => userIds.Contains(u.ID)).ToListAsync();
            var empIds = users.Where(u => u.EmployeeID.HasValue).Select(u => u.EmployeeID!.Value).Distinct().ToList();
            var emps = await _context.EmployeeMasters.AsNoTracking().Where(e => empIds.Contains(e.ID)).ToDictionaryAsync(e => e.ID, e => e.Name);
            foreach (var u in users)
                map[u.ID] = u.EmployeeID.HasValue && emps.TryGetValue(u.EmployeeID.Value, out var n) ? n : $"User {u.ID}";
            return map;
        }

        private async Task AddAuditAsync(long resultId, long executionId, int revision, long actorId, string eventType, string? hash, string? details)
        {
            var names = await ResolveActorNamesAsync(new List<long> { actorId });
            _context.UniversalResultAudits.Add(new UniversalResultAudit
            {
                UniversalTestResultID = resultId,
                TestExecutionID = executionId,
                RevisionNo = revision,
                EventType = eventType,
                ActorID = actorId,
                ActorName = names.TryGetValue(actorId, out var n) ? n : $"User {actorId}",
                EventOn = DateTime.UtcNow,
                SnapshotHash = hash,
                DetailsJson = details,
                CreatedBy = actorId,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = LoggedInUserProvider.CurrentUser?.CompanyCode ?? "LIMS",
                IsActive = true
            });
        }
    }
}
