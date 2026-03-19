using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ProductSpecificationGradeRepository : IProductSpecificationGradeRepository
    {
        private readonly LIMSContext _context;

        public ProductSpecificationGradeRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddProductSpecificationGrade(ProductSpecificationGrade model)
        {
            await _context.ProductSpecificationGrades.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductSpecificationGrade(long id)
        {
            var existing = await _context.ProductSpecificationGrades.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existing != null)
            {
                existing.IsActive = false;
                existing.ModifiedOn = DateTime.UtcNow;
                _context.ProductSpecificationGrades.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProductSpecificationGrade?> GetProductSpecificationGradeById(long id)
        {
            return await _context.ProductSpecificationGrades.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateProductSpecificationGrade(ProductSpecificationGrade model)
        {
            _context.ProductSpecificationGrades.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllProductSpecificationGrades(PageFilter filter)
        {
            var _query = (from psg in _context.ProductSpecificationGrades

                          join ps in _context.ProductSpecifications on psg.ProductSpecificationID equals ps.ID into psGroup
                          from ps in psGroup.DefaultIfEmpty()

                          join sg in _context.SpecificationGrades on psg.SpecificationGradeID equals sg.ID into sgGroup
                          from sg in sgGroup.DefaultIfEmpty()

                          join sh in _context.SpecificationHeaders on sg.SpecificationHeaderID equals sh.ID into shGroup
                          from sh in shGroup.DefaultIfEmpty()

                          where psg.IsActive

                          select new
                          {
                              psg.ID,
                              psg.ProductSpecificationID,
                              ProductSpecificationName = ps != null ? ps.SpecificationName : string.Empty,
                              psg.SpecificationGradeID,
                              GradeName = sg != null ? sg.Grade : string.Empty,
                              MaterialSpecification = sh != null && sg != null ? $"{sh.AliasName}-{sg.Grade}" : string.Empty,
                              psg.AliasName
                          })
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.ProductSpecificationName != null && x.ProductSpecificationName.ToLower().Contains(search))
                    || (x.GradeName != null && x.GradeName.ToLower().Contains(search))
                    || (x.MaterialSpecification != null && x.MaterialSpecification.ToLower().Contains(search))
                    || (x.AliasName != null && x.AliasName.ToLower().Contains(search))
                );
            }
            if (filter.SortByColumn != null)
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

        public async Task<List<object>> GetByProductSpecificationId(long productSpecId)
        {
            var data = await (from psg in _context.ProductSpecificationGrades

                              join sg in _context.SpecificationGrades on psg.SpecificationGradeID equals sg.ID into sgGroup
                              from sg in sgGroup.DefaultIfEmpty()

                              join sh in _context.SpecificationHeaders on sg.SpecificationHeaderID equals sh.ID into shGroup
                              from sh in shGroup.DefaultIfEmpty()

                              where psg.IsActive && psg.ProductSpecificationID == productSpecId

                              select new
                              {
                                  psg.ID,
                                  psg.ProductSpecificationID,
                                  psg.SpecificationGradeID,
                                  GradeName = sg != null ? sg.Grade : string.Empty,
                                  MaterialSpecification = sh != null && sg != null ? $"{sh.AliasName}-{sg.Grade}" : string.Empty,
                                  psg.AliasName
                              }).ToListAsync();

            return data.Cast<object>().ToList();
        }
    }
}
