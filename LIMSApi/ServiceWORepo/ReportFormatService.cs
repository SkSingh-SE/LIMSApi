using System.Linq.Dynamic.Core;
using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class ReportFormatService : IReportFormatService
    {
        private readonly LIMSContext _db;

        public ReportFormatService(LIMSContext db)
        {
            _db = db;
        }

        // ── LIST (paginated) ──
        public async Task<PagedResponse<object>> GetFormatListAsync(PageFilter filter)
        {
            var query = _db.ReportFormats
                .Where(f => f.IsActive)
                .Select(f => new
                {
                    f.ID,
                    f.FormatCode,
                    f.FormatName,
                    f.Description,
                    f.PageLayout,
                    f.IsDefault,
                    f.IsLocked,
                    SectionCount = f.Sections.Count(s => s.IsActive),
                    MappingCount = f.Mappings.Count(m => m.IsActive),
                    f.CreatedOn,
                    f.ModifiedOn
                });

            // Search
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var term = filter.searchTerm.ToLower();
                query = query.Where(f =>
                    f.FormatCode.ToLower().Contains(term) ||
                    f.FormatName.ToLower().Contains(term) ||
                    (f.Description != null && f.Description.ToLower().Contains(term)));
            }

            // Sort
            var sortCol = string.IsNullOrWhiteSpace(filter.SortByColumn) ? "ID" : filter.SortByColumn;
            var sortDir = string.IsNullOrWhiteSpace(filter.SortOrder) ? "desc" : filter.SortOrder;
            query = query.OrderBy($"{sortCol} {sortDir}");

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(
                items.Cast<object>().ToList(),
                totalRecords,
                filter.PageNumber,
                filter.PageSize);
        }

        // ── GET BY ID ──
        public async Task<ReportFormatDto?> GetFormatByIdAsync(long id)
        {
            var entity = await _db.ReportFormats
                .Include(f => f.Sections.Where(s => s.IsActive))
                .Include(f => f.Mappings.Where(m => m.IsActive))
                    .ThenInclude(m => m.LaboratoryTest)
                .Include(f => f.Mappings.Where(m => m.IsActive))
                    .ThenInclude(m => m.TestMethod)
                .FirstOrDefaultAsync(f => f.ID == id && f.IsActive);

            return entity == null ? null : MapToDto(entity);
        }

        // ── CREATE ──
        public async Task<long> CreateFormatAsync(ReportFormatDto dto)
        {
            // Check unique code
            var exists = await _db.ReportFormats.AnyAsync(f =>
                f.FormatCode == dto.FormatCode && f.IsActive);
            if (exists)
                throw new ArgumentException($"Format code '{dto.FormatCode}' already exists.");

            var entity = new ReportFormat
            {
                FormatCode = dto.FormatCode,
                FormatName = dto.FormatName,
                Description = dto.Description,
                PageLayout = dto.PageLayout,
                PageSize = dto.PageSize,
                HeaderConfigJson = dto.HeaderConfig?.GetRawText(),
                IsDefault = dto.IsDefault,
                IsLocked = false,
                IsActive = true
            };

            // If setting as default, unset any existing default
            if (dto.IsDefault)
                await UnsetExistingDefault();

            // Add sections
            foreach (var sec in dto.Sections)
            {
                entity.Sections.Add(new ReportFormatSection
                {
                    SectionType = (ReportSectionType)sec.SectionType,
                    SortOrder = sec.SortOrder,
                    ConfigJson = sec.Config?.GetRawText(),
                    IsVisible = sec.IsVisible,
                    IsActive = true
                });
            }

            _db.ReportFormats.Add(entity);
            await _db.SaveChangesAsync();
            return entity.ID;
        }

        // ── UPDATE ──
        public async Task UpdateFormatAsync(long id, ReportFormatDto dto)
        {
            var entity = await _db.ReportFormats
                .Include(f => f.Sections)
                .FirstOrDefaultAsync(f => f.ID == id && f.IsActive);

            if (entity == null)
                throw new KeyNotFoundException("Report format not found.");
            if (entity.IsLocked)
                throw new InvalidOperationException("Locked format cannot be modified.");

            // Check unique code (excluding self)
            var codeExists = await _db.ReportFormats.AnyAsync(f =>
                f.FormatCode == dto.FormatCode && f.IsActive && f.ID != id);
            if (codeExists)
                throw new ArgumentException($"Format code '{dto.FormatCode}' already exists.");

            entity.FormatCode = dto.FormatCode;
            entity.FormatName = dto.FormatName;
            entity.Description = dto.Description;
            entity.PageLayout = dto.PageLayout;
            entity.PageSize = dto.PageSize;
            entity.HeaderConfigJson = dto.HeaderConfig?.GetRawText();
            entity.IsDefault = dto.IsDefault;

            if (dto.IsDefault)
                await UnsetExistingDefault(id);

            // Replace sections: remove old, add new
            _db.ReportFormatSections.RemoveRange(entity.Sections);
            entity.Sections.Clear();

            foreach (var sec in dto.Sections)
            {
                entity.Sections.Add(new ReportFormatSection
                {
                    SectionType = (ReportSectionType)sec.SectionType,
                    SortOrder = sec.SortOrder,
                    ConfigJson = sec.Config?.GetRawText(),
                    IsVisible = sec.IsVisible,
                    IsActive = true
                });
            }

            await _db.SaveChangesAsync();
        }

        // ── DELETE (soft) ──
        public async Task DeleteFormatAsync(long id)
        {
            var entity = await _db.ReportFormats
                .Include(f => f.Sections)
                .Include(f => f.Mappings)
                .FirstOrDefaultAsync(f => f.ID == id && f.IsActive);

            if (entity == null)
                throw new KeyNotFoundException("Report format not found.");
            if (entity.IsLocked)
                throw new InvalidOperationException("Locked format cannot be deleted.");

            entity.IsActive = false;
            foreach (var s in entity.Sections) s.IsActive = false;
            foreach (var m in entity.Mappings) m.IsActive = false;

            await _db.SaveChangesAsync();
        }

        // ── CLONE ──
        public async Task<long> CloneFormatAsync(long id)
        {
            var entity = await _db.ReportFormats
                .Include(f => f.Sections.Where(s => s.IsActive))
                .Include(f => f.Mappings.Where(m => m.IsActive))
                .FirstOrDefaultAsync(f => f.ID == id && f.IsActive);

            if (entity == null)
                throw new KeyNotFoundException("Report format not found.");

            var clone = new ReportFormat
            {
                FormatCode = entity.FormatCode + "_COPY",
                FormatName = entity.FormatName + " (Copy)",
                Description = entity.Description,
                PageLayout = entity.PageLayout,
                PageSize = entity.PageSize,
                HeaderConfigJson = entity.HeaderConfigJson,
                IsDefault = false,
                IsLocked = false,
                IsActive = true
            };

            foreach (var sec in entity.Sections)
            {
                clone.Sections.Add(new ReportFormatSection
                {
                    SectionType = sec.SectionType,
                    SortOrder = sec.SortOrder,
                    ConfigJson = sec.ConfigJson,
                    IsVisible = sec.IsVisible,
                    IsActive = true
                });
            }

            foreach (var map in entity.Mappings)
            {
                clone.Mappings.Add(new ReportFormatMapping
                {
                    LaboratoryTestID = map.LaboratoryTestID,
                    TestMethodID = map.TestMethodID,
                    Priority = map.Priority,
                    IsActive = true
                });
            }

            _db.ReportFormats.Add(clone);
            await _db.SaveChangesAsync();
            return clone.ID;
        }

        // ── FORMAT RESOLUTION ──
        public async Task<ReportFormat?> ResolveFormatForSampleAsync(long sampleId)
        {
            // Get test method IDs and laboratory test IDs for this sample
            var testInfo = await _db.TestResultHeaders
                .Where(h => h.SampleID == sampleId && h.IsActive)
                .Select(h => new { h.LaboratoryTestID })
                .Distinct()
                .ToListAsync();

            var labTestIds = testInfo.Select(t => t.LaboratoryTestID).Distinct().ToList();

            // Get test method spec IDs from test plans (StandardID in GeneralTestMethod)
            var testMethodIds = await _db.TestPlans
                .Where(p => p.SampleID == sampleId)
                .SelectMany(p => p.GeneralTests)
                .SelectMany(g => g.Methods)
                .Where(m => !m.Cancel)
                .Select(m => m.StandardID)
                .Distinct()
                .ToListAsync();

            // Priority 1: TestMethod-level match (most specific)
            if (testMethodIds.Any())
            {
                var methodMatch = await _db.ReportFormatMappings
                    .Where(m => m.IsActive && m.TestMethodID != null && testMethodIds.Contains(m.TestMethodID.Value))
                    .OrderByDescending(m => m.Priority)
                    .Include(m => m.ReportFormat)
                    .FirstOrDefaultAsync(m => m.ReportFormat != null && m.ReportFormat.IsActive);

                if (methodMatch?.ReportFormat != null)
                {
                    return await LoadFormatWithSections(methodMatch.ReportFormat.ID);
                }
            }

            // Priority 2: LaboratoryTest-level match
            if (labTestIds.Any())
            {
                var testMatch = await _db.ReportFormatMappings
                    .Where(m => m.IsActive && m.LaboratoryTestID != null && labTestIds.Contains(m.LaboratoryTestID.Value))
                    .OrderByDescending(m => m.Priority)
                    .Include(m => m.ReportFormat)
                    .FirstOrDefaultAsync(m => m.ReportFormat != null && m.ReportFormat.IsActive);

                if (testMatch?.ReportFormat != null)
                {
                    return await LoadFormatWithSections(testMatch.ReportFormat.ID);
                }
            }

            // Priority 3: Default format
            var defaultFormat = await _db.ReportFormats
                .Where(f => f.IsDefault && f.IsActive)
                .FirstOrDefaultAsync();

            return defaultFormat != null ? await LoadFormatWithSections(defaultFormat.ID) : null;
        }

        // ── MAPPINGS ──
        public async Task<List<ReportFormatMappingDto>> GetMappingsForFormatAsync(long formatId)
        {
            return await _db.ReportFormatMappings
                .Where(m => m.ReportFormatID == formatId && m.IsActive)
                .Include(m => m.LaboratoryTest)
                .Include(m => m.TestMethod)
                .OrderByDescending(m => m.Priority)
                .Select(m => new ReportFormatMappingDto
                {
                    Id = m.ID,
                    LaboratoryTestID = m.LaboratoryTestID,
                    LaboratoryTestName = m.LaboratoryTest != null ? m.LaboratoryTest.Name : null,
                    TestMethodID = m.TestMethodID,
                    TestMethodName = m.TestMethod != null ? m.TestMethod.Name : null,
                    Priority = m.Priority
                })
                .ToListAsync();
        }

        public async Task SaveMappingsAsync(long formatId, List<ReportFormatMappingDto> mappings)
        {
            var format = await _db.ReportFormats
                .Include(f => f.Mappings)
                .FirstOrDefaultAsync(f => f.ID == formatId && f.IsActive);

            if (format == null)
                throw new KeyNotFoundException("Report format not found.");

            // Remove existing mappings
            _db.ReportFormatMappings.RemoveRange(format.Mappings);
            format.Mappings.Clear();

            // Add new mappings
            foreach (var dto in mappings)
            {
                format.Mappings.Add(new ReportFormatMapping
                {
                    LaboratoryTestID = dto.LaboratoryTestID,
                    TestMethodID = dto.TestMethodID,
                    Priority = dto.Priority,
                    IsActive = true
                });
            }

            await _db.SaveChangesAsync();
        }

        // ── DROPDOWN ──
        public async Task<List<object>> GetFormatDropdownAsync(string? searchTerm, int pageNo, int pageSize)
        {
            var query = _db.ReportFormats
                .Where(f => f.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(f =>
                    f.FormatCode.ToLower().Contains(term) ||
                    f.FormatName.ToLower().Contains(term));
            }

            return await query
                .OrderBy(f => f.FormatName)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Select(f => (object)new { id = f.ID, name = f.FormatName, code = f.FormatCode })
                .ToListAsync();
        }

        // ── SECTION TYPES ──
        public List<object> GetSectionTypes()
        {
            return Enum.GetValues<ReportSectionType>()
                .Select(t => (object)new { value = (int)t, name = t.ToString() })
                .ToList();
        }

        // ── PRIVATE HELPERS ──

        private async Task<ReportFormat> LoadFormatWithSections(long formatId)
        {
            return (await _db.ReportFormats
                .Include(f => f.Sections.Where(s => s.IsActive && s.IsVisible))
                .FirstOrDefaultAsync(f => f.ID == formatId && f.IsActive))!;
        }

        private async Task UnsetExistingDefault(long? excludeId = null)
        {
            var existing = await _db.ReportFormats
                .Where(f => f.IsDefault && f.IsActive && (excludeId == null || f.ID != excludeId))
                .ToListAsync();

            foreach (var f in existing)
                f.IsDefault = false;
        }

        private static ReportFormatDto MapToDto(ReportFormat entity)
        {
            return new ReportFormatDto
            {
                Id = entity.ID,
                FormatCode = entity.FormatCode,
                FormatName = entity.FormatName,
                Description = entity.Description,
                PageLayout = entity.PageLayout,
                PageSize = entity.PageSize,
                HeaderConfig = string.IsNullOrWhiteSpace(entity.HeaderConfigJson)
                    ? null
                    : JsonDocument.Parse(entity.HeaderConfigJson).RootElement,
                IsDefault = entity.IsDefault,
                IsLocked = entity.IsLocked,
                IsActive = entity.IsActive,
                Sections = entity.Sections
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SortOrder)
                    .Select(s => new ReportFormatSectionDto
                    {
                        Id = s.ID,
                        SectionType = (int)s.SectionType,
                        SectionTypeName = s.SectionType.ToString(),
                        SortOrder = s.SortOrder,
                        Config = string.IsNullOrWhiteSpace(s.ConfigJson)
                            ? null
                            : JsonDocument.Parse(s.ConfigJson).RootElement,
                        IsVisible = s.IsVisible
                    })
                    .ToList(),
                Mappings = entity.Mappings
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.Priority)
                    .Select(m => new ReportFormatMappingDto
                    {
                        Id = m.ID,
                        LaboratoryTestID = m.LaboratoryTestID,
                        LaboratoryTestName = m.LaboratoryTest?.Name,
                        TestMethodID = m.TestMethodID,
                        TestMethodName = m.TestMethod?.Name,
                        Priority = m.Priority
                    })
                    .ToList()
            };
        }
    }
}
