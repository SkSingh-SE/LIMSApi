using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class SettingsService : ISettingsService
    {
        private readonly LIMSContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IFileUploadService _fileUploadService;
        private readonly IFinancialYearService _fyService;

        private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^[\d\s\+\-\(\)]{7,20}$", RegexOptions.Compiled);
        private static readonly Regex GstinRegex = new(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[A-Z\d]{1}[Z]{1}[A-Z\d]{1}$", RegexOptions.Compiled);

        public SettingsService(LIMSContext db, IWebHostEnvironment env, IFileUploadService fileUploadService, IFinancialYearService fyService)
        {
            _db = db;
            _env = env;
            _fileUploadService = fileUploadService;
            _fyService = fyService;
        }

        // =====================
        // Validation Helpers
        // =====================

        private static void ValidateOrganization(string labName, string labCode, string labAddress, string contactEmail, string contactPhone)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(labName))
                errors.Add("Lab Name is required");
            else if (labName.Length > 200)
                errors.Add("Lab Name must not exceed 200 characters");

            if (string.IsNullOrWhiteSpace(labCode))
                errors.Add("Lab Code is required");
            else if (labCode.Length > 50)
                errors.Add("Lab Code must not exceed 50 characters");

            if (string.IsNullOrWhiteSpace(labAddress))
                errors.Add("Lab Address is required");
            else if (labAddress.Length > 500)
                errors.Add("Lab Address must not exceed 500 characters");

            if (string.IsNullOrWhiteSpace(contactEmail))
                errors.Add("Contact Email is required");
            else if (!EmailRegex.IsMatch(contactEmail.Trim()))
                errors.Add("Contact Email format is invalid");
            else if (contactEmail.Length > 200)
                errors.Add("Contact Email must not exceed 200 characters");

            if (string.IsNullOrWhiteSpace(contactPhone))
                errors.Add("Contact Phone is required");
            else if (!PhoneRegex.IsMatch(contactPhone.Trim()))
                errors.Add("Contact Phone format is invalid (7-20 digits allowed)");

            if (errors.Count > 0)
                throw new ArgumentException(string.Join("; ", errors));
        }

        private static void ValidateGst(GstDto gstDto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(gstDto.Gstin))
                errors.Add("GSTIN is required when GST is applicable");
            else if (!GstinRegex.IsMatch(gstDto.Gstin.Trim().ToUpper()))
                errors.Add("GSTIN format is invalid (expected 15-character format e.g., 22AAAAA0000A1Z5)");

            if (string.IsNullOrWhiteSpace(gstDto.StateCode))
                errors.Add("State Code is required when GST is applicable");

            if (gstDto.DefaultGstRate < 0 || gstDto.DefaultGstRate > 100)
                errors.Add("Default GST Rate must be between 0 and 100");

            if (errors.Count > 0)
                throw new ArgumentException(string.Join("; ", errors));
        }

        private static void ValidateFinancialYear(DateOnly startDate, DateOnly endDate)
        {
            var errors = new List<string>();

            if (startDate == default)
                errors.Add("Financial Year Start Date is required");
            if (endDate == default)
                errors.Add("Financial Year End Date is required");

            if (startDate != default && endDate != default)
            {
                if (endDate <= startDate)
                    errors.Add("Financial Year End Date must be after Start Date");

                var monthsDiff = ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month);
                if (monthsDiff < 11 || monthsDiff > 13)
                    errors.Add("Financial Year duration must be approximately 12 months (11-13 months allowed)");
            }

            if (errors.Count > 0)
                throw new ArgumentException(string.Join("; ", errors));
        }

        private async Task ValidateFinancialYearOverlapAsync(DateOnly startDate, DateOnly endDate, long? excludeId, long organizationId, CancellationToken cancellationToken)
        {
            var start = startDate.ToDateTime(TimeOnly.MinValue);
            var end = endDate.ToDateTime(TimeOnly.MinValue);

            var overlapping = await _db.FinancialYears
                .Where(fy => fy.OrganizationId == organizationId
                    && (excludeId == null || fy.Id != excludeId)
                    && fy.StartDate < end && fy.EndDate > start)
                .FirstOrDefaultAsync(cancellationToken);

            if (overlapping != null)
                throw new ArgumentException($"Financial Year overlaps with existing year '{overlapping.Year}' ({overlapping.StartDate:dd-MMM-yyyy} to {overlapping.EndDate:dd-MMM-yyyy})");
        }

        private static void ValidateSignatories(SignatoryDto[] signatories)
        {
            for (int i = 0; i < signatories.Length; i++)
            {
                var s = signatories[i];
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(s.SignatoryName))
                    errors.Add($"Signatory #{i + 1}: Name is required");
                else if (s.SignatoryName.Length > 200)
                    errors.Add($"Signatory #{i + 1}: Name must not exceed 200 characters");

                if (string.IsNullOrWhiteSpace(s.Designation))
                    errors.Add($"Signatory #{i + 1}: Designation is required");
                else if (s.Designation.Length > 200)
                    errors.Add($"Signatory #{i + 1}: Designation must not exceed 200 characters");

                if (errors.Count > 0)
                    throw new ArgumentException(string.Join("; ", errors));
            }

            // Check for duplicate signatory names
            var duplicateNames = signatories
                .Where(s => !string.IsNullOrWhiteSpace(s.SignatoryName))
                .GroupBy(s => s.SignatoryName.Trim().ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.First().SignatoryName)
                .ToList();

            if (duplicateNames.Count > 0)
                throw new ArgumentException($"Duplicate signatory names found: {string.Join(", ", duplicateNames)}");
        }

        private static void ValidateNabl(NablDto nablDto)
        {
            if (!nablDto.NablEnabled) return;

            if (string.IsNullOrWhiteSpace(nablDto.NablTcNumber))
                throw new ArgumentException("NABL Certificate/TC Number is required when NABL is enabled");
        }

        // =====================
        // Core Data Access
        // =====================

        public async Task<(Organization, NablAccreditation?, NumberingConfig[], GstConfig?, FinancialYear?, AuthorizedSignatory[])> GetAllAsync(long organizationId = 0, CancellationToken cancellationToken = default)
        {
            if (organizationId == 0)
            {
                var firstOrg = await _db.Organizations.FirstOrDefaultAsync(cancellationToken);
                if (firstOrg == null)
                {
                    firstOrg = new Organization
                    {
                        LabName = "Default Lab",
                        LabCode = "LAB001",
                        LabAddress = "Enter your lab address",
                        ContactEmail = "admin@example.com",
                        ContactPhone = "0000000000"
                    };
                    _db.Organizations.Add(firstOrg);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                organizationId = firstOrg.Id;
            }

            var org = await _db.Organizations.FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken)
                ?? throw new KeyNotFoundException("Organization not found");
            var nabl = await _db.NablAccreditations.FirstOrDefaultAsync(x => x.OrganizationId == organizationId, cancellationToken);
            var numbering = await _db.NumberingConfigs.Where(x => x.OrganizationId == organizationId).ToArrayAsync(cancellationToken);
            var gst = await _db.GstConfigs.FirstOrDefaultAsync(x => x.OrganizationId == organizationId, cancellationToken);
            var year = await _db.FinancialYears.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.IsCurrent, cancellationToken);
            var signatories = await _db.AuthorizedSignatories.Where(x => x.OrganizationId == organizationId).ToArrayAsync(cancellationToken);
            return (org, nabl, numbering, gst, year, signatories);
        }

        // =====================
        // Entity-level Save (with validation + tracking-safe)
        // =====================

        public async Task<Organization> SaveOrganizationAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            ValidateOrganization(organization.LabName, organization.LabCode, organization.LabAddress, organization.ContactEmail, organization.ContactPhone);

            if (organization.Id == 0)
                _db.Organizations.Add(organization);
            else if (_db.Entry(organization).State == EntityState.Detached)
                _db.Organizations.Update(organization);

            await _db.SaveChangesAsync(cancellationToken);
            return organization;
        }

        public async Task<NablAccreditation> SaveNablAsync(NablAccreditation nabl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nabl.CertificateNumber))
                throw new ArgumentException("NABL Certificate Number is required");

            if (nabl.Id == 0)
                _db.NablAccreditations.Add(nabl);
            else if (_db.Entry(nabl).State == EntityState.Detached)
                _db.NablAccreditations.Update(nabl);

            await _db.SaveChangesAsync(cancellationToken);
            return nabl;
        }

        public async Task<NumberingConfig> SaveNumberingAsync(NumberingConfig numbering, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(numbering.ModuleName))
                throw new ArgumentException("Module Name is required");
            if (string.IsNullOrWhiteSpace(numbering.Prefix))
                throw new ArgumentException($"Prefix is required for module '{numbering.ModuleName}'");
            if (numbering.Prefix.Length > 100)
                throw new ArgumentException($"Prefix must not exceed 100 characters for module '{numbering.ModuleName}'");

            if (numbering.Id == 0)
                _db.NumberingConfigs.Add(numbering);
            else if (_db.Entry(numbering).State == EntityState.Detached)
                _db.NumberingConfigs.Update(numbering);

            await _db.SaveChangesAsync(cancellationToken);
            return numbering;
        }

        public async Task<GstConfig> SaveGstAsync(GstConfig gst, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(gst.GstNumber))
                throw new ArgumentException("GST Number is required");
            if (!GstinRegex.IsMatch(gst.GstNumber.Trim().ToUpper()))
                throw new ArgumentException("GSTIN format is invalid (expected 15-character format e.g., 22AAAAA0000A1Z5)");
            if (gst.DefaultGstRate < 0 || gst.DefaultGstRate > 100)
                throw new ArgumentException("Default GST Rate must be between 0 and 100");

            if (gst.Id == 0)
                _db.GstConfigs.Add(gst);
            else if (_db.Entry(gst).State == EntityState.Detached)
                _db.GstConfigs.Update(gst);

            await _db.SaveChangesAsync(cancellationToken);
            return gst;
        }

        public async Task<FinancialYear> SaveFinancialYearAsync(FinancialYear year, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(year.Year))
                throw new ArgumentException("Financial Year label is required");
            if (year.EndDate <= year.StartDate)
                throw new ArgumentException("Financial Year End Date must be after Start Date");

            // Check overlap
            await ValidateFinancialYearOverlapAsync(
                DateOnly.FromDateTime(year.StartDate),
                DateOnly.FromDateTime(year.EndDate),
                year.Id == 0 ? null : year.Id,
                year.OrganizationId,
                cancellationToken);

            if (year.Id == 0)
                _db.FinancialYears.Add(year);
            else if (_db.Entry(year).State == EntityState.Detached)
                _db.FinancialYears.Update(year);

            await _db.SaveChangesAsync(cancellationToken);

            if (year.IsCurrent)
            {
                var currentUserId = LIMSApi.Helpers.LoggedInUserProvider.CurrentUser?.EmployeeID ?? 0;
                await _fyService.SetCurrentFinancialYearAsync(year.Id, currentUserId, "Changed via Settings");
            }

            return year;
        }

        public async Task<AuthorizedSignatory> SaveSignatoryAsync(AuthorizedSignatory signatory, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(signatory.Name))
                throw new ArgumentException("Signatory Name is required");
            if (signatory.Name.Length > 200)
                throw new ArgumentException("Signatory Name must not exceed 200 characters");
            if (string.IsNullOrWhiteSpace(signatory.Designation))
                throw new ArgumentException("Signatory Designation is required");
            if (signatory.Designation.Length > 200)
                throw new ArgumentException("Signatory Designation must not exceed 200 characters");

            if (signatory.Id == 0)
                _db.AuthorizedSignatories.Add(signatory);
            else if (_db.Entry(signatory).State == EntityState.Detached)
                _db.AuthorizedSignatories.Update(signatory);

            await _db.SaveChangesAsync(cancellationToken);
            return signatory;
        }

        public async Task SaveAllAsync(
            Organization organization,
            NablAccreditation? nabl,
            NumberingConfig[] numbering,
            GstConfig? gst,
            FinancialYear? year,
            AuthorizedSignatory[] signatories,
            CancellationToken cancellationToken = default)
        {
            using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await SaveOrganizationAsync(organization, cancellationToken);
                if (nabl != null) await SaveNablAsync(nabl, cancellationToken);
                foreach (var num in numbering) await SaveNumberingAsync(num, cancellationToken);
                if (gst != null) await SaveGstAsync(gst, cancellationToken);
                if (year != null) await SaveFinancialYearAsync(year, cancellationToken);
                foreach (var sign in signatories) await SaveSignatoryAsync(sign, cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // =====================
        // File Uploads
        // =====================

        public async Task<string> UploadOrganizationLogoAsync(long organizationId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Organization, null, organizationId.ToString());
            var relativePath = uploaded.FilePath;

            var org = await _db.Organizations.FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);
            if (org != null)
            {
                org.OrganizationLogo = relativePath;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return relativePath;
        }

        public async Task<string> UploadOrganizationLogoAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            var org = await _db.Organizations.FirstOrDefaultAsync(cancellationToken);
            if (org == null) throw new KeyNotFoundException("Organization not found");
            return await UploadOrganizationLogoAsync(org.Id, file, env, cancellationToken);
        }

        public async Task<string> UploadNablCertificateAsync(long nablId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Nabl, null, nablId.ToString());
            var relativePath = uploaded.FilePath;

            var nabl = await _db.NablAccreditations.FirstOrDefaultAsync(x => x.Id == nablId, cancellationToken);
            if (nabl != null)
            {
                nabl.CertificatePath = relativePath;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return relativePath;
        }

        public async Task<string> UploadNablCertificateAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            var org = await _db.Organizations.FirstOrDefaultAsync(cancellationToken);
            if (org == null) throw new KeyNotFoundException("Organization not found");
            var nabl = await _db.NablAccreditations.FirstOrDefaultAsync(x => x.OrganizationId == org.Id, cancellationToken);
            if (nabl == null)
            {
                nabl = new NablAccreditation { CertificateNumber = string.Empty, IssueDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddYears(1), OrganizationId = org.Id };
                _db.NablAccreditations.Add(nabl);
                await _db.SaveChangesAsync(cancellationToken);
            }
            return await UploadNablCertificateAsync(nabl.Id, file, env, cancellationToken);
        }

        public async Task<string> UploadSignatureAsync(long signatoryId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, FileType.Signatory, null, signatoryId.ToString());
            var relativePath = uploaded.FilePath;

            var sign = await _db.AuthorizedSignatories.FirstOrDefaultAsync(x => x.Id == signatoryId, cancellationToken);
            if (sign != null)
            {
                sign.SignaturePath = relativePath;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return relativePath;
        }

        public async Task<string> UploadSignatureAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Signatory, null, string.Empty);
            var relativePath = uploaded.FilePath;
            return relativePath;
        }

        // =====================
        // DTO-based helpers
        // =====================

        public async Task<SettingsSaveAllDto> GetAllDtoAsync(long organizationId = 0, CancellationToken cancellationToken = default)
        {
            var (org, nabl, numbering, gst, year, signatories) = await GetAllAsync(organizationId, cancellationToken);

            var orgDto = new OrganizationDto
            {
                Id = org.Id,
                LabName = org.LabName,
                LabCode = org.LabCode,
                LabAddress = org.LabAddress,
                ContactEmail = org.ContactEmail,
                ContactPhone = org.ContactPhone,
                OrganizationLogo = org.OrganizationLogo
            };

            var nablDto = new NablDto
            {
                NablEnabled = nabl != null,
                NablTcNumber = nabl?.CertificateNumber,
                NablCertificate = nabl?.CertificatePath,
                NablLogo = nabl?.LogoPath
            };

            var numberingDto = new NumberingDto();
            var tc = numbering.FirstOrDefault(x => x.ModuleName == "TC");
            if (tc != null)
            {
                numberingDto.TcBaseNumber = tc.Prefix;
                numberingDto.YearCode = DateTime.UtcNow.Year.ToString().Substring(2);
                numberingDto.RunningCounter = tc.CurrentNumber;
            }
            var report = numbering.FirstOrDefault(x => x.ModuleName == "REPORT");
            if (report != null)
            {
                numberingDto.ReportNumberPrefix = report.Prefix;
            }

            var gstDto = new GstDto
            {
                GstApplicable = gst != null,
                Gstin = gst?.GstNumber,
                PanNumber = gst?.PanNumber,
                StateCode = gst?.State,
                DefaultGstRate = (int)(gst?.DefaultGstRate ?? 18),
                PIGstApplicable = gst?.PIGstApplicable ?? true
            };

            var fyDto = new FinancialYearDto();
            if (year != null)
            {
                fyDto.StartDate = DateOnly.FromDateTime(year.StartDate);
                fyDto.EndDate = DateOnly.FromDateTime(year.EndDate);
            }

            var signDtos = signatories.Select(s => new SignatoryDto
            {
                Id = s.Id,
                SignatoryName = s.Name,
                Designation = s.Designation,
                SignatureImage = s.SignaturePath,
                Status = s.IsActive,
                ApplicableFor = s.ApplicableFor
            }).ToArray();

            return new SettingsSaveAllDto
            {
                OrganizationInfo = orgDto,
                NablAccreditation = nablDto,
                Numbering = numberingDto,
                GstConfig = gstDto,
                FinancialYear = fyDto,
                Signatories = signDtos.ToList()
            };
        }

        public async Task<Organization> SaveOrganizationAsync(OrganizationDto organizationDto, CancellationToken cancellationToken = default)
        {
            ValidateOrganization(organizationDto.LabName, organizationDto.LabCode, organizationDto.LabAddress, organizationDto.ContactEmail, organizationDto.ContactPhone);

            var (existingOrg, _, _, _, _, _) = await GetAllAsync(0, cancellationToken);
            existingOrg.LabName = organizationDto.LabName.Trim();
            existingOrg.LabCode = organizationDto.LabCode.Trim();
            existingOrg.LabAddress = organizationDto.LabAddress.Trim();
            existingOrg.ContactEmail = organizationDto.ContactEmail.Trim();
            existingOrg.ContactPhone = organizationDto.ContactPhone.Trim();
            existingOrg.OrganizationLogo = organizationDto.OrganizationLogo;
            await _db.SaveChangesAsync(cancellationToken);
            return existingOrg;
        }

        public async Task<NablAccreditation> SaveNablAsync(NablDto nablDto, CancellationToken cancellationToken = default)
        {
            ValidateNabl(nablDto);

            var (org, existingNabl, _, _, _, _) = await GetAllAsync(0, cancellationToken);
            var model = existingNabl ?? new NablAccreditation { OrganizationId = org.Id };
            model.CertificateNumber = nablDto.NablTcNumber?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nablDto.NablCertificate)) model.CertificatePath = nablDto.NablCertificate;
            if (!string.IsNullOrWhiteSpace(nablDto.NablLogo)) model.LogoPath = nablDto.NablLogo;
            return await SaveNablAsync(model, cancellationToken);
        }

        public async Task<object> SaveNumberingAsync(NumberingDto numberingDto, CancellationToken cancellationToken = default)
        {
            var (org, _, existingNumbering, _, _, _) = await GetAllAsync(0, cancellationToken);
            var tc = existingNumbering.FirstOrDefault(x => x.ModuleName == "TC") ?? new NumberingConfig { ModuleName = "TC", OrganizationId = org.Id };
            tc.Prefix = numberingDto.TcBaseNumber?.Trim() ?? string.Empty;

            var report = existingNumbering.FirstOrDefault(x => x.ModuleName == "REPORT") ?? new NumberingConfig { ModuleName = "REPORT", OrganizationId = org.Id };
            report.Prefix = numberingDto.ReportNumberPrefix?.Trim() ?? string.Empty;

            var savedTc = await SaveNumberingAsync(tc, cancellationToken);
            var savedReport = await SaveNumberingAsync(report, cancellationToken);
            return new { TcConfig = savedTc, ReportConfig = savedReport };
        }

        public async Task<GstConfig> SaveGstAsync(GstDto gstDto, CancellationToken cancellationToken = default)
        {
            if (gstDto.GstApplicable)
                ValidateGst(gstDto);

            var (org, _, _, existingGst, _, _) = await GetAllAsync(0, cancellationToken);
            var model = existingGst ?? new GstConfig { OrganizationId = org.Id };
            model.GstNumber = gstDto.Gstin?.Trim() ?? string.Empty;
            model.State = gstDto.StateCode?.Trim() ?? string.Empty;
            model.PanNumber = gstDto.PanNumber?.Trim()?.ToUpper();
            model.Address = null;
            model.DefaultGstRate = gstDto.DefaultGstRate;
            model.PIGstApplicable = gstDto.PIGstApplicable;
            return await SaveGstAsync(model, cancellationToken);
        }

        public async Task<FinancialYear> SaveFinancialYearAsync(FinancialYearDto yearDto, CancellationToken cancellationToken = default)
        {
            ValidateFinancialYear(yearDto.StartDate, yearDto.EndDate);

            var (org, _, _, _, existingYear, _) = await GetAllAsync(0, cancellationToken);

            await ValidateFinancialYearOverlapAsync(yearDto.StartDate, yearDto.EndDate, existingYear?.Id, org.Id, cancellationToken);

            FinancialYear model;
            if (existingYear != null)
            {
                existingYear.OrganizationId = org.Id;
                existingYear.StartDate = yearDto.StartDate.ToDateTime(TimeOnly.MinValue);
                existingYear.EndDate = yearDto.EndDate.ToDateTime(TimeOnly.MinValue);
                existingYear.Year = $"{yearDto.StartDate.Year}-{yearDto.EndDate.Year}";
                existingYear.IsCurrent = true;
                model = existingYear;
            }
            else
            {
                model = new FinancialYear
                {
                    OrganizationId = org.Id,
                    StartDate = yearDto.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = yearDto.EndDate.ToDateTime(TimeOnly.MinValue),
                    Year = $"{yearDto.StartDate.Year}-{yearDto.EndDate.Year}",
                    IsCurrent = true
                };
            }
            return await SaveFinancialYearAsync(model, cancellationToken);
        }

        public async Task<AuthorizedSignatory[]> SaveSignatoriesAsync(SignatoryDto[] signatoryDtos, CancellationToken cancellationToken = default)
        {
            ValidateSignatories(signatoryDtos);

            var (org, _, _, _, _, _) = await GetAllAsync(0, cancellationToken);
            var results = new List<AuthorizedSignatory>();
            foreach (var signDto in signatoryDtos)
            {
                var sign = new AuthorizedSignatory
                {
                    Id = signDto.Id ?? 0,
                    Name = signDto.SignatoryName.Trim(),
                    Designation = signDto.Designation.Trim(),
                    SignaturePath = signDto.SignatureImage,
                    ApplicableFor = signDto.ApplicableFor,
                    IsActive = signDto.Status,
                    OrganizationId = org.Id
                };
                results.Add(await SaveSignatoryAsync(sign, cancellationToken));
            }
            return results.ToArray();
        }

        public async Task SaveAllAsync(SettingsSaveAllDto payload, CancellationToken cancellationToken = default)
        {
            var dto = payload;

            // Validate all sections upfront before touching DB
            ValidateOrganization(dto.OrganizationInfo.LabName, dto.OrganizationInfo.LabCode, dto.OrganizationInfo.LabAddress, dto.OrganizationInfo.ContactEmail, dto.OrganizationInfo.ContactPhone);
            ValidateNabl(dto.NablAccreditation);
            if (dto.GstConfig.GstApplicable) ValidateGst(dto.GstConfig);
            if (dto.FinancialYear.StartDate != default && dto.FinancialYear.EndDate != default)
                ValidateFinancialYear(dto.FinancialYear.StartDate, dto.FinancialYear.EndDate);
            if (dto.Signatories.Count > 0) ValidateSignatories(dto.Signatories.ToArray());

            var (existingOrg, existingNabl, existingNumbering, existingGst, existingFy, _) = await GetAllAsync(0, cancellationToken);

            // Financial year overlap check
            if (dto.FinancialYear.StartDate != default && dto.FinancialYear.EndDate != default)
                await ValidateFinancialYearOverlapAsync(dto.FinancialYear.StartDate, dto.FinancialYear.EndDate, existingFy?.Id, existingOrg.Id, cancellationToken);

            existingOrg.LabName = dto.OrganizationInfo.LabName.Trim();
            existingOrg.LabCode = dto.OrganizationInfo.LabCode.Trim();
            existingOrg.LabAddress = dto.OrganizationInfo.LabAddress.Trim();
            existingOrg.ContactEmail = dto.OrganizationInfo.ContactEmail.Trim();
            existingOrg.ContactPhone = dto.OrganizationInfo.ContactPhone.Trim();
            existingOrg.OrganizationLogo = dto.OrganizationInfo.OrganizationLogo;
            var org = existingOrg;

            NablAccreditation? nabl = null;
            if (dto.NablAccreditation.NablEnabled)
            {
                nabl = existingNabl ?? new NablAccreditation { OrganizationId = org.Id };
                nabl.CertificateNumber = dto.NablAccreditation.NablTcNumber?.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(dto.NablAccreditation.NablCertificate)) nabl.CertificatePath = dto.NablAccreditation.NablCertificate;
                if (!string.IsNullOrWhiteSpace(dto.NablAccreditation.NablLogo)) nabl.LogoPath = dto.NablAccreditation.NablLogo;
            }

            var numberingList = new List<NumberingConfig>();
            var tc = existingNumbering.FirstOrDefault(x => x.ModuleName == "TC") ?? new NumberingConfig { ModuleName = "TC", OrganizationId = org.Id };
            tc.Prefix = dto.Numbering.TcBaseNumber?.Trim() ?? string.Empty;
            var report = existingNumbering.FirstOrDefault(x => x.ModuleName == "REPORT") ?? new NumberingConfig { ModuleName = "REPORT", OrganizationId = org.Id };
            report.Prefix = dto.Numbering.ReportNumberPrefix?.Trim() ?? string.Empty;
            numberingList.Add(tc);
            numberingList.Add(report);

            GstConfig? gst = null;
            if (dto.GstConfig.GstApplicable)
            {
                gst = existingGst ?? new GstConfig { OrganizationId = org.Id };
                gst.GstNumber = dto.GstConfig.Gstin?.Trim() ?? string.Empty;
                gst.State = dto.GstConfig.StateCode?.Trim() ?? string.Empty;
                gst.PanNumber = dto.GstConfig.PanNumber?.Trim()?.ToUpper();
                gst.DefaultGstRate = dto.GstConfig.DefaultGstRate;
                gst.PIGstApplicable = dto.GstConfig.PIGstApplicable;
            }

            FinancialYear? fy = null;
            if (dto.FinancialYear.StartDate != default && dto.FinancialYear.EndDate != default)
            {
                if (existingFy != null)
                {
                    existingFy.OrganizationId = org.Id;
                    existingFy.StartDate = dto.FinancialYear.StartDate.ToDateTime(TimeOnly.MinValue);
                    existingFy.EndDate = dto.FinancialYear.EndDate.ToDateTime(TimeOnly.MinValue);
                    existingFy.Year = $"{dto.FinancialYear.StartDate.Year}-{dto.FinancialYear.EndDate.Year}";
                    existingFy.IsCurrent = true;
                    fy = existingFy;
                }
                else
                {
                    fy = new FinancialYear
                    {
                        OrganizationId = org.Id,
                        StartDate = dto.FinancialYear.StartDate.ToDateTime(TimeOnly.MinValue),
                        EndDate = dto.FinancialYear.EndDate.ToDateTime(TimeOnly.MinValue),
                        Year = $"{dto.FinancialYear.StartDate.Year}-{dto.FinancialYear.EndDate.Year}",
                        IsCurrent = true
                    };
                }
            }

            var signatories = dto.Signatories.Select(s => new AuthorizedSignatory
            {
                Id = s.Id ?? 0,
                Name = s.SignatoryName.Trim(),
                Designation = s.Designation.Trim(),
                SignaturePath = s.SignatureImage,
                ApplicableFor = s.ApplicableFor,
                IsActive = s.Status,
                OrganizationId = org.Id
            }).ToArray();

            await SaveAllAsync(org, nabl, numberingList.ToArray(), gst, fy, signatories, cancellationToken);
        }

        // =====================
        // Convenience wrappers
        // =====================

        public async Task<string> UploadOrganizationLogoAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await UploadOrganizationLogoAsync(file, _env, cancellationToken);
        }

        public async Task<string> UploadNablCertificateAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await UploadNablCertificateAsync(file, _env, cancellationToken);
        }

        public async Task<string> UploadSignatureAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await UploadSignatureAsync(file, _env, cancellationToken);
        }

        public async Task DeleteSignatoryAsync(long signatoryId, CancellationToken cancellationToken = default)
        {
            var sign = await _db.AuthorizedSignatories.FirstOrDefaultAsync(x => x.Id == signatoryId, cancellationToken)
                ?? throw new KeyNotFoundException("Signatory not found");
            _db.AuthorizedSignatories.Remove(sign);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
