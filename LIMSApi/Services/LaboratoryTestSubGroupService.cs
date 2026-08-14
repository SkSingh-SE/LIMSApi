using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LaboratoryTestSubGroupService : ILaboratoryTestSubGroupService
    {
        private readonly ILaboratoryTestSubGroupRepository _repository;
        private readonly ILogger<LaboratoryTestSubGroupService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestSubGroupService(
            ILaboratoryTestSubGroupRepository repository,
            ILogger<LaboratoryTestSubGroupService> logger,
            LIMSContext context)
        {
            _repository = repository;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Create(LaboratoryTestSubGroup model)
        {
            Validate(model);

            if (await _repository.ExistsByNamePerLabTest(model.Name, model.LaboratoryTestID, null))
                throw new InvalidOperationException("A sub-group with the same name already exists under this laboratory test!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            foreach (var spec in model.Specifications)
            {
                if (spec.SpecificationGradeID.HasValue && !spec.SpecificationHeaderID.HasValue)
                {
                    var grade = await _context.SpecificationGrades.AsNoTracking().FirstOrDefaultAsync(g => g.ID == spec.SpecificationGradeID.Value);
                    if (grade != null)
                    {
                        spec.SpecificationHeaderID = grade.SpecificationHeaderID;
                    }
                }
            }

            foreach (var method in model.TestMethods)
            {
                if (method.TestMethodSpecificationVersionID.HasValue)
                {
                    var version = await _context.TestMethodSpecificationVersions.AsNoTracking().FirstOrDefaultAsync(v => v.ID == method.TestMethodSpecificationVersionID.Value);
                    if (version != null)
                    {
                        method.TestMethodSpecificationID = version.TestMethodSpecificationID;
                    }
                }
            }

            await _repository.Add(model);
            _logger.LogInformation("Sub-Group '{Name}' created for LabTest {LabTestId}.", model.Name, model.LaboratoryTestID);
        }

        public async Task Modify(LaboratoryTestSubGroup model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Sub-Group ID should not be empty!");

            Validate(model);

            if (await _repository.ExistsByNamePerLabTest(model.Name, model.LaboratoryTestID, model.ID))
                throw new InvalidOperationException("A sub-group with the same name already exists under this laboratory test!");

            var existing = await _repository.GetById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("Sub-Group not found!");

            existing.Name = model.Name;
            existing.ReportTestName = model.ReportTestName;
            existing.TestDuration = model.TestDuration;
            existing.MetalClassificationID = model.MetalClassificationID;
            existing.DisplayOrder = model.DisplayOrder;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            // Sync Parameters
            _context.LaboratoryTestSubGroupParameters.RemoveRange(existing.Parameters);
            foreach (var param in model.Parameters)
            {
                param.LaboratoryTestSubGroupID = model.ID;
                await _context.LaboratoryTestSubGroupParameters.AddAsync(param);
            }

            // Sync Test Methods
            _context.LaboratoryTestSubGroupMethods.RemoveRange(existing.TestMethods);
            foreach (var method in model.TestMethods)
            {
                method.LaboratoryTestSubGroupID = model.ID;
                if (method.TestMethodSpecificationVersionID.HasValue)
                {
                    var version = await _context.TestMethodSpecificationVersions.AsNoTracking().FirstOrDefaultAsync(v => v.ID == method.TestMethodSpecificationVersionID.Value);
                    if (version != null)
                    {
                        method.TestMethodSpecificationID = version.TestMethodSpecificationID;
                    }
                }
                await _context.LaboratoryTestSubGroupMethods.AddAsync(method);
            }

            // Sync Equipments
            _context.LaboratoryTestSubGroupEquipments.RemoveRange(existing.Equipments);
            foreach (var eq in model.Equipments)
            {
                eq.LaboratoryTestSubGroupID = model.ID;
                await _context.LaboratoryTestSubGroupEquipments.AddAsync(eq);
            }

            // Sync Specifications
            _context.LaboratoryTestSubGroupSpecifications.RemoveRange(existing.Specifications);
            foreach (var spec in model.Specifications)
            {
                spec.LaboratoryTestSubGroupID = model.ID;
                if (spec.SpecificationGradeID.HasValue && !spec.SpecificationHeaderID.HasValue)
                {
                    var grade = await _context.SpecificationGrades.AsNoTracking().FirstOrDefaultAsync(g => g.ID == spec.SpecificationGradeID.Value);
                    if (grade != null)
                    {
                        spec.SpecificationHeaderID = grade.SpecificationHeaderID;
                    }
                }
                await _context.LaboratoryTestSubGroupSpecifications.AddAsync(spec);
            }

            // Sync Invoice Cases (Decoupled Billing)
            _context.LaboratoryTestSubGroupInvoiceCases.RemoveRange(existing.InvoiceCases);
            foreach (var ic in model.InvoiceCases)
            {
                ic.LaboratoryTestSubGroupID = model.ID;
                await _context.LaboratoryTestSubGroupInvoiceCases.AddAsync(ic);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Sub-Group '{Name}' updated successfully.", model.Name);
        }

        public async Task Remove(long id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
                throw new InvalidOperationException("Sub-Group not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<LaboratoryTestSubGroup>(_context, id, "Laboratory Test Sub-Group");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.Update(existing);
            _logger.LogInformation("Sub-Group ID '{Id}' deleted.", id);
        }

        public async Task<LaboratoryTestSubGroup> GetDetails(long id)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                throw new InvalidOperationException("Sub-Group not found!");
            return entity;
        }

        public async Task<PagedResponse<object>> FetchList(PageFilter filter)
        {
            return await _repository.GetAll(filter);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(long labTestId)
        {
            return await _repository.GetDropdown(labTestId);
        }

        public async Task<List<LaboratoryTestSubGroup>> GetByLabTestId(long labTestId)
        {
            return await _repository.GetByLabTestId(labTestId);
        }

        public async Task<List<DropdwonSelector>> GetStandardsBySubGroupId(long subGroupId)
        {
            var standards = await _context.LaboratoryTestSubGroupMethods
                .AsNoTracking()
                .Include(m => m.TestMethodSpecification)
                .Where(m => m.LaboratoryTestSubGroupID == subGroupId && m.TestMethodSpecification != null && !m.TestMethodSpecification.IsDisabled)
                .Select(m => new DropdwonSelector
                {
                    Id = m.TestMethodSpecificationID,
                    Name = m.TestMethodSpecification!.DisplayTitle ?? m.TestMethodSpecification.Name
                })
                .Distinct()
                .ToListAsync();

            return standards;
        }

        private static void Validate(LaboratoryTestSubGroup model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name should not be empty!");
            if (model.LaboratoryTestID == 0)
                throw new ArgumentException("Laboratory Test is required!");
            if (string.IsNullOrWhiteSpace(model.ReportTestName))
                throw new ArgumentException("Report Test Name should not be empty!");

            model.Name = model.Name.Trim();
            model.ReportTestName = model.ReportTestName.Trim();
        }
    }
}
