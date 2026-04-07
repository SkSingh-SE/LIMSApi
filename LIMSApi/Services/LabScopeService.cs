using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LabScopeService : ILabScopeService
    {
        private readonly ILabScopeRepository _labScopeRepository;
        private readonly ILogger<LabScopeService> _logger;
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO _loggedInUser;

        public LabScopeService(ILabScopeRepository labScopeRepo, ILogger<LabScopeService> logger, LIMSContext context)
        {
            _labScopeRepository = labScopeRepo;
            _logger = logger;
            _context = context;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateLabScope(LabScopeMaster model)
        {
            // G1: Prevent duplicate scope for same Laboratory Test
            var exists = await _context.LabScopeMasters
                .AnyAsync(x => x.LaboratoryTestID == model.LaboratoryTestID && x.IsActive);
            if (exists)
                throw new InvalidOperationException("A scope already exists for this Laboratory Test. Edit the existing scope instead.");

            await _labScopeRepository.AddLabScope(model);

            // G10: Log creation
            _context.LabScopeChangeLogs.Add(new LabScopeChangeLog
            {
                LabScopeID = model.ID,
                ChangeType = "Created",
                EntityName = $"LabTestID: {model.LaboratoryTestID}",
                NewValue = $"{model.Specifications.Count} specifications, {model.Specifications.SelectMany(s => s.Parameters).Count()} parameters",
                ChangedBy = _loggedInUser.EmployeeID,
                ChangedOn = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            _logger.LogInformation("LabScope '{LabScopeName}' created successfully.", model.LaboratoryTestID);
        }

        public async Task ModifyLabScope(LabScopeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("LabScope ID should not be empty!");

            var existingLabScope = await _labScopeRepository.GetLabScopeById(model.ID);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            // G1: Prevent changing to a LaboratoryTestID that already has another scope
            if (existingLabScope.LaboratoryTestID != model.LaboratoryTestID)
            {
                var duplicate = await _context.LabScopeMasters
                    .AnyAsync(x => x.LaboratoryTestID == model.LaboratoryTestID && x.IsActive && x.ID != model.ID);
                if (duplicate)
                    throw new InvalidOperationException("A scope already exists for this Laboratory Test.");
            }

            existingLabScope.LaboratoryTestID = model.LaboratoryTestID;
            existingLabScope.ValidFrom = model.ValidFrom;
            existingLabScope.ValidUntil = model.ValidUntil;
            existingLabScope.NextReviewDate = model.NextReviewDate;
            existingLabScope.ScopeRemarks = model.ScopeRemarks;
            existingLabScope.ModifiedOn = DateTime.UtcNow;
            existingLabScope.ModifiedBy = _loggedInUser.EmployeeID;

            var changeLogs = new List<LabScopeChangeLog>();

            // --- Remove missing Specifications ---
            var toRemoveSpec = existingLabScope.Specifications
                .Where(x => !model.Specifications.Any(y => y.ID == x.ID))
                .ToList();

            foreach (var spec in toRemoveSpec)
            {
                changeLogs.Add(new LabScopeChangeLog
                {
                    LabScopeID = model.ID,
                    ChangeType = "SpecificationRemoved",
                    EntityName = spec.TestMethodSpecification?.Name ?? $"SpecID: {spec.TestMethodSpecificationID}",
                    OldValue = $"{spec.Parameters.Count} parameters",
                    ChangedBy = _loggedInUser.EmployeeID,
                    ChangedOn = DateTime.UtcNow
                });
                existingLabScope.Specifications.Remove(spec);
            }

            // --- Add or Update Specifications ---
            foreach (var incomingSpec in model.Specifications)
            {
                var existingSpec = existingLabScope.Specifications
                    .FirstOrDefault(x => x.ID == incomingSpec.ID);

                if (existingSpec == null)
                {
                    // New specification
                    changeLogs.Add(new LabScopeChangeLog
                    {
                        LabScopeID = model.ID,
                        ChangeType = "SpecificationAdded",
                        EntityName = $"SpecID: {incomingSpec.TestMethodSpecificationID}",
                        NewValue = $"{incomingSpec.Parameters.Count} parameters",
                        ChangedBy = _loggedInUser.EmployeeID,
                        ChangedOn = DateTime.UtcNow
                    });
                    existingLabScope.Specifications.Add(incomingSpec);
                }
                else
                {
                    existingSpec.TestMethodSpecificationID = incomingSpec.TestMethodSpecificationID;
                    existingSpec.TestMethodSpecificationVersionID = incomingSpec.TestMethodSpecificationVersionID;
                    existingSpec.ModifiedOn = DateTime.UtcNow;
                    existingSpec.ModifiedBy = _loggedInUser.EmployeeID;

                    // --- Remove missing Parameters ---
                    var toRemoveParams = existingSpec.Parameters
                        .Where(p => !incomingSpec.Parameters.Any(ip => ip.ID == p.ID))
                        .ToList();

                    foreach (var param in toRemoveParams)
                    {
                        changeLogs.Add(new LabScopeChangeLog
                        {
                            LabScopeID = model.ID,
                            ChangeType = "ParameterRemoved",
                            EntityName = $"ParamID: {param.ParameterID}",
                            OldValue = $"Limits: {param.LowerLimitValue} - {param.UpperLimitValue}",
                            ChangedBy = _loggedInUser.EmployeeID,
                            ChangedOn = DateTime.UtcNow
                        });
                        existingSpec.Parameters.Remove(param);
                    }

                    // --- Add or Update Parameters ---
                    foreach (var incomingParam in incomingSpec.Parameters)
                    {
                        var existingParam = existingSpec.Parameters
                            .FirstOrDefault(p => p.ID == incomingParam.ID);

                        if (existingParam == null)
                        {
                            changeLogs.Add(new LabScopeChangeLog
                            {
                                LabScopeID = model.ID,
                                ChangeType = "ParameterAdded",
                                EntityName = $"ParamID: {incomingParam.ParameterID}",
                                NewValue = $"Limits: {incomingParam.LowerLimitValue} - {incomingParam.UpperLimitValue}, ISO: {incomingParam.IsUnderISO}",
                                ChangedBy = _loggedInUser.EmployeeID,
                                ChangedOn = DateTime.UtcNow
                            });
                            existingSpec.Parameters.Add(incomingParam);
                        }
                        else
                        {
                            // Track limit changes
                            if (existingParam.LowerLimitValue != incomingParam.LowerLimitValue ||
                                existingParam.UpperLimitValue != incomingParam.UpperLimitValue)
                            {
                                changeLogs.Add(new LabScopeChangeLog
                                {
                                    LabScopeID = model.ID,
                                    ChangeType = "LimitsChanged",
                                    EntityName = $"ParamID: {existingParam.ParameterID}",
                                    OldValue = $"{existingParam.LowerLimitValue} - {existingParam.UpperLimitValue}",
                                    NewValue = $"{incomingParam.LowerLimitValue} - {incomingParam.UpperLimitValue}",
                                    ChangedBy = _loggedInUser.EmployeeID,
                                    ChangedOn = DateTime.UtcNow
                                });
                            }

                            existingParam.ParameterID = incomingParam.ParameterID;
                            existingParam.ParameterUnitID = incomingParam.ParameterUnitID;
                            existingParam.QualitativeQuantitative = incomingParam.QualitativeQuantitative;
                            existingParam.IsUnderISO = incomingParam.IsUnderISO;
                            existingParam.LowerLimit = incomingParam.LowerLimit;
                            existingParam.LowerLimitValue = incomingParam.LowerLimitValue;
                            existingParam.UpperLimit = incomingParam.UpperLimit;
                            existingParam.UpperLimitValue = incomingParam.UpperLimitValue;
                            existingParam.DisciplineID = incomingParam.DisciplineID;
                            existingParam.GroupID = incomingParam.GroupID;
                            existingParam.SubGroupID = incomingParam.SubGroupID;
                            existingParam.ModifiedOn = DateTime.UtcNow;
                            existingParam.ModifiedBy = _loggedInUser.EmployeeID;

                            // --- Remove missing Equipments ---
                            if (existingParam.Equipments == null) existingParam.Equipments = new List<LabScopeSpecificationParameterEquipment>();
                            var toRemoveEquip = existingParam.Equipments
                                .Where(e => !incomingParam.Equipments.Any(ie => ie.EquipmentID == e.EquipmentID))
                                .ToList();

                            foreach (var equip in toRemoveEquip)
                            {
                                existingParam.Equipments.Remove(equip);
                            }

                            // --- Add new Equipments ---
                            foreach (var incomingEquip in incomingParam.Equipments)
                            {
                                if (!existingParam.Equipments.Any(e => e.EquipmentID == incomingEquip.EquipmentID))
                                {
                                    existingParam.Equipments.Add(incomingEquip);
                                }
                            }
                        }
                    }
                }
            }

            // Save changelog
            if (changeLogs.Any())
            {
                _context.LabScopeChangeLogs.AddRange(changeLogs);
            }

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope '{LabScopeName}' updated successfully. {ChangeCount} changes logged.", model.LaboratoryTestID, changeLogs.Count);
        }


        public async Task RemoveLabScope(long id)
        {
            var existingLabScope = await _labScopeRepository.GetLabScopeById(id);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            // G12: Check FK dependencies before delete
            await DeleteValidationHelper.ValidateDeleteAsync<LabScopeMaster>(_context, id, "Lab Scope");

            existingLabScope.IsActive = false;
            existingLabScope.ModifiedOn = DateTime.UtcNow;
            existingLabScope.ModifiedBy = _loggedInUser.EmployeeID;

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope with ID '{LabScopeId}' deleted successfully.", id);
        }

        public async Task<LabScopeMaster> GetLabScopeDetails(long id)
        {
            var classification = await _labScopeRepository.GetLabScopeById(id);
            if (classification == null)
                throw new InvalidOperationException("LabScope not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchLabScopeList(PageFilter filter)
        {
            return await _labScopeRepository.GetAllLabScopes(filter);
        }

    }
}
