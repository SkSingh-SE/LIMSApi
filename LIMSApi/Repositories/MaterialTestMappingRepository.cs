using System.Linq;
using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class MaterialTestMappingRepository : IMaterialTestMappingRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public MaterialTestMappingRepository(LIMSContext _context)
        {
            this._context = _context;
            this.loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<PagedResponse<object>> GetAllAsync(PageFilter filter)
        {

            var _query = from m in _context.MaterialTestMappings
                         where m.IsActive && m.CompanyCode == loggedInUser.CompanyCode
                         join mc in _context.MetalClassificationMasters on m.MetalClassificationID equals mc.ID into metalClass
                         from mc in metalClass.DefaultIfEmpty()
                         join pc in _context.ProductConditionMasters on m.ProductConditionID equals pc.ID into prodCond
                         from pc in prodCond.DefaultIfEmpty()
                         join g in _context.SpecificationGrades on m.GradeID equals g.ID into grade
                         from g in grade.DefaultIfEmpty()
                         join e in _context.EmployeeMasters on m.CreatedBy equals e.ID into emp
                         from e in emp.DefaultIfEmpty()
                         join ml in _context.MappingLaboratoryTests on m.ID equals ml.TestMappingID into mapLabTest
                         from ml in mapLabTest.DefaultIfEmpty()
                         join lt in _context.LaboratoryTests on ml.LaboratoryTestID equals lt.ID into labTest
                         from lt in labTest.DefaultIfEmpty()
                         select new
                         {
                             m.ID,
                             MetalClassification = mc != null ? mc.Name : "",
                             ProductCondition = pc != null ? pc.Name : "",
                             Grade = g != null ? g.Grade : "",
                             LaboratoryTest = lt != null ? lt.SubGroup : "",
                             m.CreatedOn,
                             CreatedBy = e != null ? e.Name : ""
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.MetalClassification != null && x.MetalClassification.ToLower().Contains(search)) ||
                    (x.ProductCondition != null && x.ProductCondition.ToLower().Contains(search)) ||
                    (x.Grade != null && x.Grade.ToLower().Contains(search)) ||
                    (x.LaboratoryTest != null && x.LaboratoryTest.ToLower().Contains(search)) ||
                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(search)) ||
                    x.CreatedOn.ToString().Contains(search)
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            int totalRecords = await _query.CountAsync();

           
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }


        public async Task<MaterialTestMapping?> GetByIdAsync(long id)
        {
            return await _context.MaterialTestMappings
                .Include(x => x.MetalClassification)
                .Include(x => x.ProductCondition)
                .Include(x => x.Grade)
                .Include(x => x.LaboratoryTests)
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<MaterialTestMapping>> GetByClassificationAsync(long metalClassificationId)
        {
            return await _context.MaterialTestMappings
                .Where(x => x.MetalClassificationID == metalClassificationId && x.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(MaterialTestMapping entity)
        {
            await _context.MaterialTestMappings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaterialTestMapping entity)
        {
            _context.MaterialTestMappings.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _context.MaterialTestMappings.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = false;
                _context.MaterialTestMappings.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(long? metalClassificationId, long? productConditionId, long? gradeId)
        {
            var query = _context.MaterialTestMappings
                .AsNoTracking()
                .Where(m => m.IsActive);


            if (metalClassificationId.HasValue)
                query = query.Where(m => m.MetalClassificationID == metalClassificationId);

            if (productConditionId.HasValue)
                query = query.Where(m => m.ProductConditionID == productConditionId);

            if (gradeId.HasValue)
                query = query.Where(m => m.GradeID == gradeId);

            

            return await query.AnyAsync();
        }

        public async Task<List<DropdwonSelector>> GetSuggestedTestsAsync(TestSuggestionRequest request)
        {
            //  Handle multiple grade IDs (comma-separated)
            var gradeIds = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.GradeIDs))
                gradeIds = request.GradeIDs.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(g => g.Trim().ToLower())
                                           .ToList();

            //  Build base query
            var query = from m in _context.MaterialTestMappings
                        join ml in _context.MappingLaboratoryTests on m.ID equals ml.TestMappingID
                        join lt in _context.LaboratoryTests on ml.LaboratoryTestID equals lt.ID
                        where m.IsActive
                        select new
                        {
                            m.GradeID,
                            m.MetalClassificationID,
                            m.ProductConditionID,
                            ml.LaboratoryTestID,
                            LaboratoryTestSubGroup = lt.SubGroup
                        };

            // Filter by MetalClassificationID (if provided)
            if (request.MetalClassificationID.HasValue)
                query = query.Where(x => x.MetalClassificationID == request.MetalClassificationID);

            // Filter by ProductConditionID (if provided)
            if (request.ProductConditionID.HasValue)
                query = query.Where(x => x.ProductConditionID == request.ProductConditionID);

            // Filter by Grade(s)
            if (gradeIds.Any())
                query = query.Where(x => gradeIds.Contains(x.GradeID.ToString().ToLower()));

            // Get distinct tests
            var results = await query
                .Select(x => new DropdwonSelector
                {
                    Id = x.LaboratoryTestID,
                    Name = x.LaboratoryTestSubGroup
                })
                .Distinct()
                .ToListAsync();

            return results;
        }

        public async Task<List<DropdwonSelector>> GetSuggestedGradeAsync(GradeSuggestionRequest request)
        {
            //  Handle multiple grade IDs (comma-separated)
            
            //  Build base query
            var query = from m in _context.MaterialTestMappings
                        join g in _context.SpecificationGrades on m.GradeID equals g.ID into gm
                        from g in gm.DefaultIfEmpty()
                        where m.IsActive
                        select new
                        {
                            m.GradeID,
                            g.Grade,
                            m.MetalClassificationID,
                            m.ProductConditionID
                        };

            // Filter by MetalClassificationID (if provided)
            if (request.MetalClassificationID.HasValue)
                query = query.Where(x => x.MetalClassificationID == request.MetalClassificationID);

            // Filter by ProductConditionID (if provided)
            if (request.ProductConditionID.HasValue)
                query = query.Where(x => x.ProductConditionID == request.ProductConditionID);


            // Get distinct tests
            var results = await query
                .Select(x => new DropdwonSelector
                {
                    Id = (long)x.GradeID,
                    Name = x.Grade
                })
                .Distinct()
                .ToListAsync();

            return results;
        }

    }
}
