using System.Linq;
using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Helpers.StatusFlow;
using LIMSApi.Helpers.StatusFlow.Extensions;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SampleInwardRepository : ISampleInwardRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public SampleInwardRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddSampleInward(SampleInward model)
        {
            model.CreatedOn = DateTime.UtcNow;
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.SampleInwards.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSampleInward(long id)
        {
            var existingSampleInward = await _context.SampleInwards.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSampleInward != null)
            {
                existingSampleInward.IsActive = false;
                existingSampleInward.ModifiedOn = DateTime.UtcNow;
                _context.SampleInwards.Update(existingSampleInward);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SampleInward?> GetSampleInwardById(long id)
        {
            var sampleInward = await _context.SampleInwards
                                .AsNoTracking()
                                .AsSplitQuery()
                                .Include(x => x.DispatchModes)
                                .Include(x => x.Contacts)
                                .Include(x => x.Addresses)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.AdditionalDetails)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.MetalClassification)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.ProductCondition)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.Discipline)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.SpecimenOrientation)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.ProductForm)
                                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                                    .ThenInclude(sd => sd.TestPlans)
                                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                                            .ThenInclude(utg => utg.LaboratoryTest)
                                                .ThenInclude(lt => lt.Discipline)
                                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            return sampleInward;
        }

        public async Task<SampleInward?> GetSampleInwardWithPlans(long id)
        {
            var sampleInward = await _context.SampleInwards
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.AdditionalDetails)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.MetalClassification)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.ProductCondition)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.ProductMaster)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.ProductSizeMaster)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.SpecificationGrade)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.AssignedGrade)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.Discipline)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.Histories)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.GeneralTests)
                            .ThenInclude(gt => gt.Methods)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.ChemicalTests)
                            .ThenInclude(ct => ct.Methods)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.ChemicalTests)
                            .ThenInclude(ct => ct.Elements)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.ChemicalTests)
                            .ThenInclude(ct => ct.TestTypes)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(utg => utg.LaboratoryTest)
                                .ThenInclude(lt => lt.LabDepartment)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(utg => utg.TestMethodSpecification)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(utg => utg.TestMethodSpecificationVersion)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(utg => utg.SpecificationHeader)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(utg => utg.SpecificationGrade)
                .Include(x => x.SampleDetails.Where(sd => sd.IsActive))
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.UniversalTestGroups.Where(utg => utg.IsActive))
                            .ThenInclude(utg => utg.TestExecutions)
                .FirstOrDefaultAsync(x =>
                    x.ID == id &&
                    x.IsActive &&
                    x.CompanyCode == loggedInUser.CompanyCode);

            return sampleInward;
        }



        public async Task UpdateSampleInward(SampleInward model)
        {
            model.ModifiedOn = DateTime.UtcNow;
            model.CompanyCode = loggedInUser.CompanyCode;
            _context.SampleInwards.Update(model);
            await _context.SaveChangesAsync();
        }


        public async Task<PagedResponse<object>> GetInwardList(PageFilter filter)
        {
            var baseQuery = _context.SampleInwards
                .Where(c => c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);

            if (!loggedInUser.CanViewAllBranches && loggedInUser.BranchID.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.BranchID == loggedInUser.BranchID.Value);
            }

            var query = baseQuery
                .Select(c => new
                {
                    c.ID,
                    c.CaseNo,
                    c.CustomerID,
                    CustomerName = c.Customer.Name,
                    ContactPersonName = c.Contacts.OrderBy(x => x.ID).Select(x => x.Name).FirstOrDefault(),
                    ContactEmail = c.Contacts.OrderBy(x => x.ID).Select(x => x.EmailId).FirstOrDefault(),
                    ContactPhone = c.Contacts.OrderBy(x => x.ID).Select(x => x.MobileNo).FirstOrDefault(),
                    c.CollectionTime,
                    //  IMPORTANT
                    InwardStatus = c.InwardStatus,
                    CurrentStageStatus = c.InwardStatus,
                    ActionStatus = ActionStatusResolver.Resolve(WorkflowListType.Inward, c.InwardStatus).ToString(),

                    ModifiedOn = c.ModifiedOn,
                    ModifiedBy = _context.EmployeeMasters
                                   .Where(e => e.ID == c.ModifiedBy)
                                   .Select(e => e.Name)
                                   .FirstOrDefault()
                });
            query = query.AsQueryable().ApplyFilters(filter.Filter);

            return await ApplyPagingFilteringSorting(query, filter);
        }
        public async Task<PagedResponse<object>> GetPlanList(PageFilter filter)
        {
            var baseQuery = _context.SampleInwards
                .Where(c => c.IsActive && c.CompanyCode == loggedInUser.CompanyCode)
                .Where(c => c.InwardStatus != InwardStatus.CANCELLED.ToString() && c.InwardStatus != InwardStatus.REJECTED.ToString());

            if (!loggedInUser.CanViewAllBranches && loggedInUser.BranchID.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.BranchID == loggedInUser.BranchID.Value);
            }

            var query = baseQuery
                .Select(c => new
                {
                    c.ID,
                    c.CaseNo,
                    c.CustomerID,
                    CustomerName = c.Customer.Name,
                    ContactPersonName = c.Contacts.OrderBy(x => x.ID).Select(x => x.Name).FirstOrDefault(),
                    ContactEmail = c.Contacts.OrderBy(x => x.ID).Select(x => x.EmailId).FirstOrDefault(),
                    ContactPhone = c.Contacts.OrderBy(x => x.ID).Select(x => x.MobileNo).FirstOrDefault(),
                    c.CollectionTime,
                    FirstSampleID = c.SampleDetails.Where(s => s.IsActive).OrderBy(s => s.ID).Select(s => (long?)s.ID).FirstOrDefault(),
                    PlanStatus = 
                        _context.UniversalTestGroups.Any(u => u.IsActive && u.SampleTestPlan.SampleDetail.InwardID == c.ID)
                            ? (
                                _context.UniversalTestGroups.Where(u => u.IsActive && u.SampleTestPlan.SampleDetail.InwardID == c.ID).All(u => u.Status == "Approved")
                                    ? "Approved"
                                    : (
                                        _context.UniversalTestGroups.Where(u => u.IsActive && u.SampleTestPlan.SampleDetail.InwardID == c.ID).All(u => u.Status == "Verified" || u.Status == "Approved")
                                            ? "Verified"
                                            : (
                                                _context.UniversalTestGroups.Where(u => u.IsActive && u.SampleTestPlan.SampleDetail.InwardID == c.ID).All(u => u.Status == "Completed" || u.Status == "Verified" || u.Status == "Approved")
                                                    ? "Completed"
                                                    : (
                                                        _context.UniversalTestGroups.Any(u => u.IsActive && u.SampleTestPlan.SampleDetail.InwardID == c.ID && (u.Status == "InProgress" || u.Status == "Started"))
                                                            ? "InProgress"
                                                            : "Submitted"
                                                      )
                                              )
                                      )
                              )
                            : (
                                _context.TestPlans.Any(p => p.SampleDetail.InwardID == c.ID && p.PlanStatus == "Submitted")
                                    ? "Submitted"
                                    : (
                                        _context.TestPlans.Any(p => p.SampleDetail.InwardID == c.ID && p.PlanStatus == "Draft")
                                            ? "UNDER_PLANNING"
                                            : c.InwardStatus
                                      )
                              ),
                    CurrentStageStatus = c.InwardStatus,
                    ActionStatus = ActionStatusResolver.Resolve(WorkflowListType.Planning, c.InwardStatus).ToString(),
                    ModifiedOn = c.ModifiedOn,
                    ModifiedBy = _context.EmployeeMasters
                                   .Where(e => e.ID == c.ModifiedBy)
                                   .Select(e => e.Name)
                                   .FirstOrDefault()
                });

            query = query.AsQueryable().ApplyFilters(filter.Filter);
            return await ApplyPagingFilteringSorting(query, filter);
        }

        public async Task<PagedResponse<object>> GetReviewList(PageFilter filter)
        {
            var userId = loggedInUser.EmployeeID;

            // Only the latest workflow instance per inward — prevents duplicate rows when
            // the same inward was submitted for review more than once.
            var latestInstanceIds = _context.WorkflowInstances
                .Where(w => w.EntityType == WorkFlowEntityTypeExtensions.GetEntityType(WorkFlowEntityType.Request_Review))
                .GroupBy(w => w.EntityID)
                .Select(g => g.Max(w => w.ID));

            var query =
                from inward in _context.SampleInwards
                where inward.IsActive
                      && inward.CompanyCode == loggedInUser.CompanyCode
                      && (loggedInUser.CanViewAllBranches || (loggedInUser.BranchID.HasValue && inward.BranchID == loggedInUser.BranchID.Value))

                join instance in _context.WorkflowInstances
                    .Where(w => latestInstanceIds.Contains(w.ID) && (w.IsActive || w.Status == "Completed"))
                    on new
                    {
                        inward.ID,
                        EntityType = WorkFlowEntityTypeExtensions.GetEntityType(
                            WorkFlowEntityType.Request_Review)
                    }
                    equals new
                    {
                        ID = instance.EntityID,
                        instance.EntityType
                    }

                join step in _context.WorkflowSteps
                    on instance.CurrentStepID equals step.ID

                select new
                {
                    inward.ID,
                    inward.CaseNo,
                    inward.CustomerID,
                    CustomerName = inward.Customer != null ? inward.Customer.Name : string.Empty,

                    InwardStatus = inward.InwardStatus,
                    CurrentStageStatus = inward.InwardStatus,

                    ActionStatus = ActionStatusResolver.Resolve(
                        WorkflowListType.Review,
                        inward.InwardStatus
                    ),

                    Reviewer = inward.ReviewedBy,
                    ReviewStatus = inward.ReviewStatus,

                    CurrentStep = step.Name,
                    AssignedToValue = step.AssignedToValue,

                    // 🔥 ACTION VISIBILITY RULE
                    CanTakeAction =
                        instance.IsActive &&
                        FilterHelper.IsUserApprover(step.AssignedToValue, userId) &&
                        inward.InwardStatus == InwardStatus.UNDER_REVIEW.ToString(),

                    Actions =
                        instance.IsActive &&
                        FilterHelper.IsUserApprover(step.AssignedToValue, userId) &&
                        inward.InwardStatus == InwardStatus.UNDER_REVIEW.ToString()
                        ? step.Transitions
                            .Where(t => t.IsActive)
                            .Select(t => new
                            {
                                ID = instance.ID,
                                Name = t.Alias ?? t.Action,
                                Action = t.Action
                            })
                        : null,

                    ModifiedOn = inward.ModifiedOn,
                    ModifiedBy = _context.EmployeeMasters
                        .Where(e => e.ID == inward.ModifiedBy)
                        .Select(e => e.Name)
                        .FirstOrDefault()
                };

            query = query.AsQueryable().ApplyFilters(filter.Filter);
            return await ApplyPagingFilteringSorting(query, filter);
        }


        private async Task<PagedResponse<object>> ApplyPagingFilteringSorting(IQueryable<object> query, PageFilter filter)
        {

            // Search
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(EF.Property<string>(x, "CaseNo") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "CustomerName") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "ContactPersonName") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "ContactEmail") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "ContactPhone") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "InwardStatus") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "ReviewStatus") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<string>(x, "ModifiedBy") ?? "", $"%{search}%") ||
                    EF.Functions.Like(EF.Property<DateTime?>(x, "CollectionTime")
                        .ToString() ?? "", $"%{search}%")
                );
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                string order = filter.SortOrder == "asc" ? "ascending" : "descending";
                query = query.OrderBy($"{filter.SortByColumn} {order}");
            }

            return await query.ToPagedAsync(filter);
        }


        public async Task<List<DropdwonSelector>> GetSampleInwardDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = _context.SampleInwards.Include(x => x.SampleDetails).Where(a => a.IsActive && a.CompanyCode == loggedInUser.CompanyCode);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                _query = _query.Where(x => (x.CaseNo != null && x.CaseNo.Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.CaseNo} ({x.SampleDetails.Count} Samples)",
            })).ToListAsync();

            return data;
        }

        public async Task<List<DropdwonSelector>> GetSamplePreparationInwardDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = _context.SampleInwards
                .Include(x => x.SampleDetails)
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.GeneralTests)
                            .ThenInclude(gt => gt.Methods)
                .Include(x => x.SampleDetails)
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.ChemicalTests)
                            .ThenInclude(ct => ct.Methods)
                .Where(a => a.IsActive && a.CompanyCode == loggedInUser.CompanyCode &&
                            a.SampleDetails.Any(sd => !sd.IsCancelled && sd.TestPlans.Any(tp =>
                                tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) ||
                                tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))
                            )));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.CaseNo != null && x.CaseNo.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.CaseNo} ({x.SampleDetails.Count(sd => !sd.IsCancelled && sd.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))))} Prep Samples)",
            })).ToListAsync();

            return data;
        }

        public async Task<object> GetCaseNoAndSampleNo()
        {
            var year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            // Fetch LabCode from Organization settings, extract prefix before first "-"
            var fullLabCode = await _context.Organizations
                .Where(o => o.IsActive && o.CompanyCode == loggedInUser.CompanyCode)
                .Select(o => o.LabCode)
                .FirstOrDefaultAsync() ?? "DMSPL";
            var casePrefix = fullLabCode.Contains('-') ? fullLabCode.Split('-')[0] : fullLabCode;

            var existingCases = await _context.SampleInwards
                .Where(s => s.CaseNo.StartsWith(casePrefix + "-"))
                .Select(s => s.CaseNo)
                .ToListAsync();

            long maxCaseNumber = 0;
            foreach (var c in existingCases)
            {
                var parts = c.Split('-');
                if (parts.Length >= 2 && long.TryParse(parts[1], out long num))
                {
                    if (num > maxCaseNumber) maxCaseNumber = num;
                }
            }
            long nextCaseNumber = maxCaseNumber + 1;
            while (await _context.SampleInwards.AnyAsync(s => s.CaseNo == $"{casePrefix}-{nextCaseNumber:D6}"))
            {
                nextCaseNumber++;
            }

            var existingSamples = await _context.SampleDetails
                .Where(s => s.SampleNo.StartsWith(year + "-"))
                .Select(s => s.SampleNo)
                .ToListAsync();

            long maxSampleNumber = 0;
            foreach (var s in existingSamples)
            {
                var parts = s.Split('-');
                if (parts.Length >= 2 && long.TryParse(parts[1], out long num))
                {
                    if (num > maxSampleNumber) maxSampleNumber = num;
                }
            }
            long nextSampleNumber = maxSampleNumber + 1;
            while (await _context.SampleDetails.AnyAsync(s => s.SampleNo == $"{year}-{nextSampleNumber:D6}"))
            {
                nextSampleNumber++;
            }

            var res = new
            {
                caseNo = $"{casePrefix}-{nextCaseNumber:D6}",
                sampleNo = $"{year}-{nextSampleNumber:D6}",
                nextSampleCounter = nextSampleNumber
            };

            return res;
        }


    }
}
