using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public interface ISnapshotIntegrityValidator
    {
        Task<SnapshotIntegrityResult> ValidateIntegrityAsync(long testExecutionId);
    }

    public class SnapshotIntegrityResult
    {
        public bool IsValid { get; set; }
        public bool ContentHashValid { get; set; }
        public bool LineageValid { get; set; }
        public string StoredHash { get; set; } = string.Empty;
        public string ComputedHash { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new();
        public List<string> Errors => ValidationErrors;
    }

    public class SnapshotIntegrityValidator : ISnapshotIntegrityValidator
    {
        private readonly LIMSContext _context;

        public SnapshotIntegrityValidator(LIMSContext context)
        {
            _context = context;
        }

        public async Task<SnapshotIntegrityResult> ValidateIntegrityAsync(long testExecutionId)
        {
            var result = new SnapshotIntegrityResult { IsValid = true, ContentHashValid = true, LineageValid = true };

            var execution = await _context.TestExecutions
                .Include(e => e.ExecutionConfigSnapshot)
                .Include(e => e.UniversalTestGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ID == testExecutionId);

            if (execution == null)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"TestExecution #{testExecutionId} not found.");
                return result;
            }

            if (execution.ExecutionConfigSnapshot == null)
            {
                result.IsValid = false;
                result.LineageValid = false;
                result.ValidationErrors.Add($"TestExecution #{testExecutionId} has no linked ExecutionConfigSnapshot.");
                return result;
            }

            var snapshot = execution.ExecutionConfigSnapshot;
            result.StoredHash = snapshot.SnapshotHash ?? string.Empty;

            // 1. Content Integrity Check (Canonical SHA-256)
            if (string.IsNullOrWhiteSpace(snapshot.ConfigJson))
            {
                result.IsValid = false;
                result.ContentHashValid = false;
                result.ValidationErrors.Add("Snapshot ConfigJson is null or empty.");
            }
            else
            {
                try
                {
                    // Ensure valid JSON syntax
                    using (JsonDocument.Parse(snapshot.ConfigJson)) { }

                    // Compute SHA-256 directly from the stored canonical JSON
                    string recomputedHash = CanonicalJsonSerializer.ComputeSha256(snapshot.ConfigJson);
                    result.ComputedHash = recomputedHash;

                    if (!string.Equals(result.StoredHash, recomputedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        result.IsValid = false;
                        result.ContentHashValid = false;
                        result.ValidationErrors.Add($"Content hash mismatch! Stored: '{result.StoredHash}', Recomputed: '{recomputedHash}'. Tamper detected.");
                    }
                }
                catch (JsonException ex)
                {
                    result.IsValid = false;
                    result.ContentHashValid = false;
                    result.ValidationErrors.Add($"Snapshot ConfigJson is corrupted or invalid JSON: {ex.Message}");
                }
                catch (Exception ex)
                {
                    result.IsValid = false;
                    result.ContentHashValid = false;
                    result.ValidationErrors.Add($"Failed to verify snapshot content hash: {ex.Message}");
                }
            }

            // 2. Lineage / Relational Integrity Check
            if (execution.UniversalTestGroupID != execution.UniversalTestGroup?.ID)
            {
                result.IsValid = false;
                result.LineageValid = false;
                result.ValidationErrors.Add("Execution UniversalTestGroupID does not match linked UniversalTestGroup record.");
            }

            if (execution.ExecutionConfigSnapshotID != snapshot.ID)
            {
                result.IsValid = false;
                result.LineageValid = false;
                result.ValidationErrors.Add("Execution ExecutionConfigSnapshotID does not match linked snapshot record.");
            }

            if (execution.CompanyCode != snapshot.CompanyCode)
            {
                result.IsValid = false;
                result.LineageValid = false;
                result.ValidationErrors.Add("Execution CompanyCode does not match snapshot CompanyCode.");
            }

            result.IsValid = result.ContentHashValid && result.LineageValid;
            return result;
        }
    }
}
