using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LIMSApi.ServiceWORepo
{
    public class ReportTemplateService : IReportTemplateService
    {
        private readonly LIMSContext _db;

        public ReportTemplateService(LIMSContext db)
        {
            _db = db;
        }

        // -------------------------------
        // GET LIST
        // -------------------------------
        public async Task<List<ReportTemplateDto>> GetTemplatesAsync(string? testType = null)
        {
            var query = _db.ReportTemplates
                .Include(t => t.Blocks)
                .Where(t => t.IsActive);

            if (!string.IsNullOrWhiteSpace(testType))
                query = query.Where(t => t.TestType == testType);

            return await query
                .OrderByDescending(t => t.IsDefault)
                .ThenBy(t => t.DisplayName)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        // -------------------------------
        // GET BY ID
        // -------------------------------
        public async Task<ReportTemplateDto?> GetTemplateByIdAsync(long templateId)
        {
            var template = await _db.ReportTemplates
                .Include(t => t.Blocks)
                .FirstOrDefaultAsync(t => t.ID == templateId && t.IsActive);

            return template == null ? null : MapToDto(template);
        }

        // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<long> CreateTemplateAsync(ReportTemplateDto dto)
        {
            var template = new ReportTemplate
            {
                TemplateCode = dto.TemplateCode,
                DisplayName = dto.DisplayName,
                TestType = dto.TestType,
                TestTypeID = dto.TestTypeID,
                Version = dto.Version,
                IsLocked = dto.IsLocked,
                IsDefault = dto.IsDefault,
                IsActive = true
            };

            foreach (var block in dto.Blocks)
            {
                template.Blocks.Add(new ReportTemplateBlock
                {
                    BlockType = block.BlockType,
                    SortOrder = block.SortOrder,
                    ConfigJson = block.Config != null ? block.Config?.GetRawText() : string.Empty
                });
            }

            _db.ReportTemplates.Add(template);
            await _db.SaveChangesAsync();

            return template.ID;
        }

        // -------------------------------
        // UPDATE
        // -------------------------------
        public async Task UpdateTemplateAsync(long templateId, ReportTemplateDto dto)
        {
            var template = await _db.ReportTemplates
                .Include(t => t.Blocks)
                .FirstOrDefaultAsync(t => t.ID == templateId);

            if (template == null)
                throw new Exception("Template not found");

            if (template.IsLocked)
                throw new Exception("Locked template cannot be modified");

            template.DisplayName = dto.DisplayName;
            template.TestType = dto.TestType;
            template.IsDefault = dto.IsDefault;

            // Replace blocks
            _db.ReportTemplateBlocks.RemoveRange(template.Blocks);
            template.Blocks.Clear();

            foreach (var block in dto.Blocks)
            {
                template.Blocks.Add(new ReportTemplateBlock
                {
                    BlockType = block.BlockType,
                    SortOrder = block.SortOrder,
                    ConfigJson = block.Config?.GetRawText()
                });
            }

            await _db.SaveChangesAsync();
        }

        // -------------------------------
        // DELETE (SOFT)
        // -------------------------------
        public async Task DeleteTemplateAsync(long templateId)
        {
            var template = await _db.ReportTemplates.FindAsync(templateId);

            if (template == null)
                throw new Exception("Template not found");

            if (template.IsLocked)
                throw new Exception("Locked template cannot be deleted");

            template.IsActive = false;
            await _db.SaveChangesAsync();
        }

        // -------------------------------
        // CLONE
        // -------------------------------
        public async Task<long> CloneTemplateAsync(long templateId)
        {
            var template = await _db.ReportTemplates
                .Include(t => t.Blocks)
                .FirstOrDefaultAsync(t => t.ID == templateId);

            if (template == null)
                throw new Exception("Template not found");

            var clone = new ReportTemplate
            {
                TemplateCode = template.TemplateCode + "_CLONE",
                DisplayName = template.DisplayName + " (Copy)",
                TestType = template.TestType,
                Version = template.Version + 1,
                IsLocked = false,
                IsDefault = false,
                IsActive = true
            };

            foreach (var block in template.Blocks)
            {
                clone.Blocks.Add(new ReportTemplateBlock
                {
                    BlockType = block.BlockType,
                    SortOrder = block.SortOrder,
                    ConfigJson = block.ConfigJson
                });
            }

            _db.ReportTemplates.Add(clone);
            await _db.SaveChangesAsync();

            return clone.ID;
        }

        // -------------------------------
        // MAPPER
        // -------------------------------
        private static ReportTemplateDto MapToDto(ReportTemplate t)
        {
            return new ReportTemplateDto
            {
                Id = t.ID,
                TemplateCode = t.TemplateCode,
                DisplayName = t.DisplayName,
                TestType = t.TestType,
                TestTypeID = t.TestTypeID,
                Version = t.Version,
                IsLocked = t.IsLocked,
                IsDefault = t.IsDefault,
                IsActive = t.IsActive,
                Blocks = t.Blocks
                    .OrderBy(b => b.SortOrder)
                    .Select(b => new ReportTemplateBlockDto
                    {
                        Id = b.ID,
                        BlockType = b.BlockType,
                        SortOrder = b.SortOrder,
                        Config = string.IsNullOrWhiteSpace(b.ConfigJson) ? null: JsonDocument.Parse(b.ConfigJson).RootElement
                    })
                    .ToList()
            };
        }
    }
}
