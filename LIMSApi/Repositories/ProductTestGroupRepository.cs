using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductTestGroupRepository : IProductTestGroupRepository
    {
        private readonly LIMSContext _context;

        public ProductTestGroupRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddProductTestGroup(ProductTestGroup model)
        {
            await _context.ProductTestGroups.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductTestGroup(long id)
        {
            var existing = await _context.ProductTestGroups.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existing != null)
            {
                existing.IsActive = false;
                existing.ModifiedOn = DateTime.UtcNow;
                _context.ProductTestGroups.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProductTestGroup?> GetProductTestGroupById(long id)
        {
            return await _context.ProductTestGroups.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateProductTestGroup(ProductTestGroup model)
        {
            _context.ProductTestGroups.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllProductTestGroups(PageFilter filter)
        {
            var _query = (from ptg in _context.ProductTestGroups

                          join ps in _context.ProductSpecifications on ptg.ProductSpecificationID equals ps.ID into psGroup
                          from ps in psGroup.DefaultIfEmpty()

                          join lt in _context.LaboratoryTests on ptg.LaboratoryTestID equals lt.ID into ltGroup
                          from lt in ltGroup.DefaultIfEmpty()

                          join tms in _context.TestMethodStandards on ptg.TestMethodStandardID equals tms.ID into tmsGroup
                          from tms in tmsGroup.DefaultIfEmpty()

                          where ptg.IsActive

                          select new
                          {
                              ptg.ID,
                              ptg.ProductSpecificationID,
                              ProductSpecificationName = ps != null ? ps.SpecificationName : string.Empty,
                              ptg.LaboratoryTestID,
                              LaboratoryTestName = lt != null ? lt.Name : string.Empty,
                              ptg.TestMethodStandardID,
                              TestMethodSpecificationName = tms != null ? tms.Name : string.Empty,
                              ptg.IsPerBatch,
                              ptg.Year,
                              ptg.Remark,
                              ptg.ModifiedOn
                          })
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.ProductSpecificationName != null && x.ProductSpecificationName.ToLower().Contains(search))
                    || (x.LaboratoryTestName != null && x.LaboratoryTestName.ToLower().Contains(search))
                    || (x.TestMethodSpecificationName != null && x.TestMethodSpecificationName.ToLower().Contains(search))
                );
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductTestGroupDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from ptg in _context.ProductTestGroups
                         join lt in _context.LaboratoryTests on ptg.LaboratoryTestID equals lt.ID into ltGroup
                         from lt in ltGroup.DefaultIfEmpty()
                         where ptg.IsActive
                         select new { ptg.ID, Name = lt != null ? lt.Name : string.Empty };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => x.Name.ToLower().Contains(search) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<List<object>> GetByProductSpecificationId(long productSpecId)
        {
            var data = await (from ptg in _context.ProductTestGroups

                              join lt in _context.LaboratoryTests on ptg.LaboratoryTestID equals lt.ID into ltGroup
                              from lt in ltGroup.DefaultIfEmpty()

                              join tms in _context.TestMethodStandards on ptg.TestMethodStandardID equals tms.ID into tmsGroup
                              from tms in tmsGroup.DefaultIfEmpty()

                              where ptg.IsActive && ptg.ProductSpecificationID == productSpecId

                              select new
                              {
                                  ptg.ID,
                                  ptg.ProductSpecificationID,
                                  ptg.LaboratoryTestID,
                                  LaboratoryTestName = lt != null ? lt.Name : string.Empty,
                                  ptg.TestMethodStandardID,
                                  TestMethodSpecificationName = tms != null ? tms.Name : string.Empty,
                                  ptg.IsPerBatch,
                                  ptg.Year,
                                  ptg.Remark
                              }).ToListAsync();

            return data.Cast<object>().ToList();
        }
    }
}
