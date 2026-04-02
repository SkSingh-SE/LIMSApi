using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductSpecificationRepository : IProductSpecificationRepository
    {
        private readonly LIMSContext _context;

        public ProductSpecificationRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddProductSpecification(ProductSpecification model)
        {
            await _context.ProductSpecifications.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductSpecification(long id)
        {
            var existingProductSpecification = await _context.ProductSpecifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingProductSpecification != null)
            {
                existingProductSpecification.IsActive = false;
                existingProductSpecification.ModifiedOn = DateTime.UtcNow;
                _context.ProductSpecifications.Update(existingProductSpecification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProductSpecification?> GetProductSpecificationById(long id)
        {
            return await _context.ProductSpecifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateProductSpecification(ProductSpecification model)
        {
            _context.ProductSpecifications.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllProductSpecifications(PageFilter filter)
        {
          
            var _query = (from c in _context.ProductSpecifications

                          join g in _context.SpecificationGrades on c.GradeID equals g.ID into gradeGroup
                          from g in gradeGroup.DefaultIfEmpty()
                          join s in _context.SpecificationHeaders on g.SpecificationHeaderID equals s.ID into specGroup
                          from s in specGroup.DefaultIfEmpty()

                          join l in _context.LaboratoryTests on c.LaboratoryTestID equals l.ID into labGroup
                          from l in labGroup.DefaultIfEmpty()

                          join m in _context.MetalClassificationMasters on c.MetalClassificationID equals m.ID into metalGroup
                          from m in metalGroup.DefaultIfEmpty()

                          join t in _context.TestMethodSpecifications on c.TestMethodSpecificationID equals t.ID into testMethodGroup
                          from t in testMethodGroup.DefaultIfEmpty()

                          where c.IsActive && c.IsCustom == false

                          select new
                          {
                              c.ID,
                              c.SpecificationName,
                              c.AliasName,
                              c.SpecificationCode,

                              MaterialSpecification = s != null ? $"{s.AliasName}-{g.Grade}" : string.Empty,
                              LaboratoryTestName = l != null ? l.Name : string.Empty,
                              MetalClassificationName = m != null ? m.Name : string.Empty,
                              TestMethodSpecificationName = t != null ? t.Name : string.Empty,
                              c.ModifiedOn
                          })
              .AsQueryable()
              .ApplyFilters(filter.Filter);



            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.SpecificationName != null && x.SpecificationName.ToLower().Contains(search))
                || x.AliasName.Contains(search)
                || x.SpecificationCode.Contains(search)
                || x.MaterialSpecification.Contains(search)
                );
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }
        public async Task<PagedResponse<object>> GetAllCustomProductSpecifications(PageFilter filter)
        {
            var _query = (from c in _context.ProductSpecifications
                          join s in _context.SpecificationHeaders on c.GradeID equals s.ID
                          where c.IsActive && c.IsCustom
                          select new
                          {
                              c.ID,
                              c.SpecificationName,
                              c.AliasName,
                              c.SpecificationCode,
                              MaterialSpecification = s.AliasName,
                              c.ModifiedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.SpecificationName != null && x.SpecificationName.ToLower().Contains(search))
                || x.AliasName.Contains(search)
                || x.SpecificationCode.Contains(search)
                || x.MaterialSpecification.Contains(search)
                );
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductSpecificationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ProductSpecifications where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.SpecificationName != null && x.SpecificationName.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.SpecificationName,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.ProductSpecifications.AnyAsync(x => x.SpecificationName == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.ProductSpecifications.AnyAsync(x => x.SpecificationName == name && x.ID != Id && x.IsActive);
        }
    }
}
