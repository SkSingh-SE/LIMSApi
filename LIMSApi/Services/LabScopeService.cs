using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class LabScopeService : ILabScopeService
    {
        private readonly ILabScopeRepository _labScopeRepository;
        private readonly ILogger<LabScopeService> _logger;

        public LabScopeService(ILabScopeRepository labScopeRepo, ILogger<LabScopeService> logger)
        {
            _labScopeRepository = labScopeRepo;
            _logger = logger;
        }

        public async Task CreateLabScope(LabScopeMaster model)
        {

            await _labScopeRepository.AddLabScope(model);
            _logger.LogInformation("LabScope '{LabScopeName}' created successfully.", model.LaboratoryTestID);
        }

        public async Task ModifyLabScope(LabScopeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("LabScope ID should not be empty!");

            var existingLabScope = await _labScopeRepository.GetLabScopeById(model.ID);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            existingLabScope.LaboratoryTestID = model.LaboratoryTestID;
            existingLabScope.ModifiedOn = DateTime.UtcNow;

            // --- Remove missing Specifications ---
            var toRemoveSpec = existingLabScope.Specifications
                .Where(x => !model.Specifications.Any(y => y.ID == x.ID))
                .ToList();

            foreach (var spec in toRemoveSpec)
            {
                existingLabScope.Specifications.Remove(spec);
            }

            // --- Add or Update Specifications ---
            foreach (var incomingSpec in model.Specifications)
            {
                var existingSpec = existingLabScope.Specifications
                    .FirstOrDefault(x => x.ID == incomingSpec.ID);

                if (existingSpec == null)
                {
                    // New specification - add
                    existingLabScope.Specifications.Add(incomingSpec);
                }
                else
                {
                    // Update existing specification
                    existingSpec.TestMethodSpecificationID = incomingSpec.TestMethodSpecificationID;

                    // --- Remove missing Parameters ---
                    var toRemoveParams = existingSpec.Parameters
                        .Where(p => !incomingSpec.Parameters.Any(ip => ip.ID == p.ID))
                        .ToList();

                    foreach (var param in toRemoveParams)
                    {
                        existingSpec.Parameters.Remove(param);
                    }

                    // --- Add or Update Parameters ---
                    foreach (var incomingParam in incomingSpec.Parameters)
                    {
                        var existingParam = existingSpec.Parameters
                            .FirstOrDefault(p => p.ID == incomingParam.ID);

                        if (existingParam == null)
                        {
                            existingSpec.Parameters.Add(incomingParam);
                        }
                        else
                        {
                            // Update parameter properties
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

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope '{LabScopeName}' updated successfully.", model.LaboratoryTestID);
        }


        public async Task RemoveLabScope(long id)
        {
            var existingLabScope = await _labScopeRepository.GetLabScopeById(id);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            existingLabScope.IsActive = false;
            existingLabScope.ModifiedOn = DateTime.UtcNow;

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
