using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Helpers.StatusFlow;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LIMSApi.Repositories
{
    public class CuttingRepository : ICuttingRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CuttingRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }
        public async Task<CuttingChargeHeader> CreateAsync(CuttingChargeHeader model)
        {
            using var trx = await _context.Database.BeginTransactionAsync();

            try
            {
                // Sanitize FK values: 0 is not a valid FK, convert to null
                foreach (var sample in model.Samples ?? Enumerable.Empty<CuttingChargeSample>())
                {
                    if (sample.MetalClassificationID == 0) sample.MetalClassificationID = null;
                    if (sample.SpecimenTypeId == 0) sample.SpecimenTypeId = null;
                }

                await _context.CuttingChargeHeaders.AddAsync(model);
                await _context.SaveChangesAsync();
                await trx.CommitAsync();
                return model;

            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
        public async Task<PagedResponse<object>> GetAllCuttingList(PageFilter filter)
        {
            var query =
                from i in _context.SampleInwards

                where i.IsActive
                      && i.CompanyCode == loggedInUser.CompanyCode
                      && i.SampleDetails.Any(s => s.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))))

                join c in _context.CuttingChargeHeaders
                    on i.ID equals c.InwardID into cuttingJoin
                from c in cuttingJoin.DefaultIfEmpty()

                join e in _context.EmployeeMasters
                    on c.ModifiedBy equals e.ID into empJoin
                from e in empJoin.DefaultIfEmpty()

                select new
                {
                    Id = c != null ? c.ID : 0,
                    InwardId = i.ID,
                    CaseNo = i.CaseNo,

                    GrandTotal = c != null ? c.GrandTotal : (decimal?)null,

                    ModifiedOn = c != null ? c.ModifiedOn : (DateTime?)null,

                    ModifiedBy = e != null ? e.Name : string.Empty,

                    // 🔥 Preparation stats
                    TotalRequired = i.SampleDetails.Count(s => s.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired)))),
                    CompletedCount = c != null
                        ? _context.CuttingChargeSamples.Count(x => x.CuttingChargeHeaderID == c.ID)
                        : 0,

                    // 🔥 Preparation Status
                    PreparationStatus =
                        i.SampleDetails.Any(s => s.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))))
                            ? (
                                _context.CuttingChargeSamples
                                    .Count(x => x.CuttingChargeHeaderID == c.ID)
                                >= i.SampleDetails.Count(s => s.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))))
                                ? "Completed"
                                : "Pending"
                              )
                            : "Not Required",

                    // 🔥 Current stage
                    CurrentStageStatus = i.InwardStatus,

                    // 🔥 Action status
                    ActionStatus =
                        i.SampleDetails.Any(s =>
                            s.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))) &&
                            s.SampleStatus == SampleStatus.PREPARATION_COMPLETED.ToString()
                        )
                        ? ActionStatus.COMPLETED.ToString()
                        : ActionStatus.PENDING.ToString(),

                    // 🔥 Invoice lock — edit disabled after invoice
                    IsInvoiced = i.IsInvoiceGenerated
                };

            // Filters
            query = query.ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                // Rely on SQL Server CI collation — no LOWER() wrap.
                // For numeric columns (GrandTotal), parse search as decimal and do exact equality
                // rather than CAST(col AS varchar) LIKE '%..%' which is non-sargable.
                var search = filter.searchTerm.Trim();
                decimal? searchAmount = decimal.TryParse(search, out var amt) ? amt : (decimal?)null;
                query = query.Where(x =>
                    x.CaseNo.Contains(search) ||
                    (searchAmount != null && x.GrandTotal == searchAmount)
                );
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Pagination
            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(
                items.Cast<object>().ToList(),
                totalRecords,
                filter.PageNumber,
                filter.PageSize
            );
        }


        public async Task<CuttingChargeHeader?> GetByIdAsync(long id)
        {
            var cuttingChargeHeader = await _context.CuttingChargeHeaders
                .Include(c => c.Samples)
                .ThenInclude(c => c.CuttingChargeDetails)
                .FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);

            return cuttingChargeHeader;
        }

        public async Task<CuttingChargeHeader?> GetByInwardIdAsync(long inwardId)
        {
            var cuttingChargeHeader = await _context.CuttingChargeHeaders
               .Include(c => c.Samples)
               .ThenInclude(c => c.CuttingChargeDetails)
               .FirstOrDefaultAsync(c => c.InwardID == inwardId && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);

            return cuttingChargeHeader;
        }

        public async Task UpdateAsync(CuttingChargeHeader model)
        {
            using var trx = await _context.Database.BeginTransactionAsync();

            try
            {
                // Sanitize FK values: 0 is not a valid FK, convert to null
                foreach (var sample in model.Samples ?? Enumerable.Empty<CuttingChargeSample>())
                {
                    if (sample.MetalClassificationID == 0) sample.MetalClassificationID = null;
                    if (sample.SpecimenTypeId == 0) sample.SpecimenTypeId = null;
                }

                // Detach any tracked instances to avoid "entity already tracked" errors
                _context.ChangeTracker.Clear();

                _context.CuttingChargeHeaders.Update(model);
                await _context.SaveChangesAsync();
                await trx.CommitAsync();

            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> ExistsByInwardIdAsync(long inwardId)
        {
            return await _context.CuttingChargeHeaders
                .AnyAsync(c => c.InwardID == inwardId && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<CuttingChargeSample?> GetSampleByIdAsync(long sampleId)
        {
            return await _context.CuttingChargeSamples
                .FirstOrDefaultAsync(s => s.ID == sampleId);
        }

        public async Task UpdateSampleAsync(CuttingChargeSample sample)
        {
            _context.CuttingChargeSamples.Update(sample);
            await _context.SaveChangesAsync();
        }
    }
}
