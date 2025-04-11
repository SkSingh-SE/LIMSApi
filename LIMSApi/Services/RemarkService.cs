using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class RemarkService : IRemarkService
    {
        private readonly IRemarkRepository _remarkRepository;
        private readonly ILogger<RemarkService> _logger;
        private LoggedInUserDTO loggedInUser;

        public RemarkService(IRemarkRepository RemarkRepo, ILogger<RemarkService> logger)
        {
            _remarkRepository = RemarkRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateRemark(RemarkMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Remark name should not be empty!");

            bool exists = await _remarkRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Remark already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _remarkRepository.AddRemark(model);
            _logger.LogInformation("Remark '{RemarkName}' created successfully.", model.Name);
        }

        public async Task ModifyRemark(RemarkMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Remark ID should not be empty!");

            bool exists = await _remarkRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Remark already exists!");

            var existingRemark = await _remarkRepository.GetRemarkById(model.ID);
            if (existingRemark == null)
                throw new InvalidOperationException("Remark not found!");


            existingRemark.Name = model.Name;
            existingRemark.Description = model.Description;
            existingRemark.ModifiedOn = DateTime.UtcNow;
            existingRemark.ModifiedBy = loggedInUser.EmployeeID;

            await _remarkRepository.UpdateRemark(existingRemark);
            _logger.LogInformation("Remark '{RemarkName}' updated successfully.", model.Name);
        }

        public async Task RemoveRemark(long id)
        {
            var existingRemark = await _remarkRepository.GetRemarkById(id);
            if (existingRemark == null)
                throw new InvalidOperationException("Remark not found!");

            existingRemark.IsActive = false;
            existingRemark.ModifiedOn = DateTime.UtcNow;
            existingRemark.ModifiedBy = loggedInUser.EmployeeID;

            await _remarkRepository.DeleteRemark(existingRemark);
            _logger.LogInformation("Remark with ID '{RemarkId}' deleted successfully.", id);
        }

        public async Task<RemarkMaster> GetRemarkDetails(long id)
        {
            var classification = await _remarkRepository.GetRemarkById(id);
            if (classification == null)
                throw new InvalidOperationException("Remark not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchRemarkList(PageFilter filter)
        {
            return await _remarkRepository.GetAllRemarks(filter);
        }

        public async Task<List<DropdwonSelector>> GetRemarkDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _remarkRepository.GetRemarkDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
