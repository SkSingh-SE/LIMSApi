using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LaboratoryTestAnalysisTypeService : ILaboratoryTestAnalysisTypeService
    {
        private readonly ILaboratoryTestAnalysisTypeRepository _repository;
        private readonly ILogger<LaboratoryTestAnalysisTypeService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestAnalysisTypeService(
            ILaboratoryTestAnalysisTypeRepository repository,
            ILogger<LaboratoryTestAnalysisTypeService> logger,
            LIMSContext context)
        {
            _repository = repository;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Create(LaboratoryTestAnalysisType model)
        {
            Validate(model);

            if (await _repository.ExistsByNamePerSubGroup(model.Name, model.LaboratoryTestSubGroupID, null))
                throw new InvalidOperationException("An analysis type with the same name already exists under this sub-group!");

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
            _logger.LogInformation("Analysis Type '{Name}' created for SubGroup {SubGroupId}.", model.Name, model.LaboratoryTestSubGroupID);
        }

        public async Task Modify(LaboratoryTestAnalysisType model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Analysis Type ID should not be empty!");

            Validate(model);

            if (await _repository.ExistsByNamePerSubGroup(model.Name, model.LaboratoryTestSubGroupID, model.ID))
                throw new InvalidOperationException("An analysis type with the same name already exists under this sub-group!");

            var existing = await _repository.GetById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("Analysis Type not found!");

            existing.Name = model.Name;
            existing.MetalClassificationID = model.MetalClassificationID;
            existing.TestDuration = model.TestDuration;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            // Sync Allowed Techniques (Junction)
            _context.LaboratoryTestAnalysisTypeTechniques.RemoveRange(existing.AllowedTechniques);
            foreach (var tech in model.AllowedTechniques)
            {
                tech.LaboratoryTestAnalysisTypeID = model.ID;
                await _context.LaboratoryTestAnalysisTypeTechniques.AddAsync(tech);
            }

            // Sync Parameters
            _context.LaboratoryTestAnalysisTypeParameters.RemoveRange(existing.Parameters);
            foreach (var param in model.Parameters)
            {
                param.LaboratoryTestAnalysisTypeID = model.ID;
                await _context.LaboratoryTestAnalysisTypeParameters.AddAsync(param);
            }

            // Sync Test Methods
            _context.LaboratoryTestAnalysisTypeMethods.RemoveRange(existing.TestMethods);
            foreach (var method in model.TestMethods)
            {
                method.LaboratoryTestAnalysisTypeID = model.ID;
                if (method.TestMethodSpecificationVersionID.HasValue)
                {
                    var version = await _context.TestMethodSpecificationVersions.AsNoTracking().FirstOrDefaultAsync(v => v.ID == method.TestMethodSpecificationVersionID.Value);
                    if (version != null)
                    {
                        method.TestMethodSpecificationID = version.TestMethodSpecificationID;
                    }
                }
                await _context.LaboratoryTestAnalysisTypeMethods.AddAsync(method);
            }

            // Sync Equipments
            _context.LaboratoryTestAnalysisTypeEquipments.RemoveRange(existing.Equipments);
            foreach (var eq in model.Equipments)
            {
                eq.LaboratoryTestAnalysisTypeID = model.ID;
                await _context.LaboratoryTestAnalysisTypeEquipments.AddAsync(eq);
            }

            // Sync Specifications
            _context.LaboratoryTestAnalysisTypeSpecifications.RemoveRange(existing.Specifications);
            foreach (var spec in model.Specifications)
            {
                spec.LaboratoryTestAnalysisTypeID = model.ID;
                if (spec.SpecificationGradeID.HasValue && !spec.SpecificationHeaderID.HasValue)
                {
                    var grade = await _context.SpecificationGrades.AsNoTracking().FirstOrDefaultAsync(g => g.ID == spec.SpecificationGradeID.Value);
                    if (grade != null)
                    {
                        spec.SpecificationHeaderID = grade.SpecificationHeaderID;
                    }
                }
                await _context.LaboratoryTestAnalysisTypeSpecifications.AddAsync(spec);
            }

            // Sync Invoice Cases (Decoupled Billing Configuration Mappings)
            _context.LaboratoryTestAnalysisTypeInvoiceCases.RemoveRange(existing.InvoiceCases);
            foreach (var ic in model.InvoiceCases)
            {
                ic.LaboratoryTestAnalysisTypeID = model.ID;
                await _context.LaboratoryTestAnalysisTypeInvoiceCases.AddAsync(ic);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Analysis Type '{Name}' updated successfully.", model.Name);
        }

        public async Task Remove(long id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
                throw new InvalidOperationException("Analysis Type not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<LaboratoryTestAnalysisType>(_context, id, "Laboratory Test Analysis Type");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.Update(existing);
            _logger.LogInformation("Analysis Type ID '{Id}' soft-deleted.", id);
        }

        public async Task<LaboratoryTestAnalysisType> GetDetails(long id)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                throw new InvalidOperationException("Analysis Type not found!");
            return entity;
        }

        public async Task<PagedResponse<object>> FetchList(PageFilter filter)
        {
            return await _repository.GetAll(filter);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(long subGroupId)
        {
            return await _repository.GetDropdown(subGroupId);
        }

        public async Task<List<LaboratoryTestAnalysisType>> GetBySubGroupId(long subGroupId)
        {
            return await _repository.GetBySubGroupId(subGroupId);
        }

        private static void Validate(LaboratoryTestAnalysisType model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name should not be empty!");
            if (model.LaboratoryTestSubGroupID == 0)
                throw new ArgumentException("Sub-Group is required!");

            model.Name = model.Name.Trim();
        }
    }
}
