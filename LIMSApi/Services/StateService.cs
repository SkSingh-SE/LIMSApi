using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<StateService> _logger;

        public StateService(IStateRepository stateRepository, ILogger<StateService> logger)
        {
            _stateRepository = stateRepository;
            _logger = logger;
        }

        public async Task<StateMaster> CreateState(StateMaster state)
        {
            if (string.IsNullOrWhiteSpace(state.Name))
                throw new ArgumentException("State name should not be empty!");

            bool exists = await _stateRepository.ExistsByName(state.Name);
            if (exists)
                throw new InvalidOperationException("State already exists!");

            await _stateRepository.AddState(state);
            _logger.LogInformation("State '{StateName}' created successfully.", state.Name);
            return state;
        }

        public async Task ModifyState(StateMaster state)
        {
            if (state.ID == 0)
                throw new ArgumentException("State ID should not be empty!");

            bool exists = await _stateRepository.ExistsByNameAndNotId(state.Name, state.ID);
            if (exists)
                throw new InvalidOperationException("Same State already exists!");

            var existingState = await _stateRepository.GetStateById(state.ID);
            if (existingState == null)
                throw new InvalidOperationException("State not found!");

            existingState.Name = state.Name;
            existingState.Code = state.Code;
            existingState.ModifiedOn = DateTime.UtcNow;

            await _stateRepository.UpdateState(existingState);
            _logger.LogInformation("State '{StateName}' updated successfully.", state.Name);
        }

        public async Task RemoveState(long id)
        {
            var existingState = await _stateRepository.GetStateById(id);
            if (existingState == null)
                throw new Exception("State not found!");

            existingState.IsActive = false;
            existingState.ModifiedOn = DateTime.UtcNow;

            await _stateRepository.UpdateState(existingState);
            _logger.LogInformation("State with ID '{StateId}' deleted successfully.", id);
        }

        public async Task<StateMaster> GetStateDetails(long id)
        {
            var state = await _stateRepository.GetStateById(id);
            if (state == null)
                throw new Exception("State not found!");

            return state;
        }

        public async Task<PagedResponse<object>> FetchStates(PageFilter filter)
        {
            return await _stateRepository.GetAllStates(filter);
        }

        public async Task<StateMaster?> GetStateByName(string name)
        {
            return await _stateRepository.GetByName(name);
        }
    }
}
