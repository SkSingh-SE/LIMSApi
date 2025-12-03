using LIMSApi.Helpers.Enums;

namespace LIMSApi.Services.Interface
{
    public interface ISampleStatusService
    {
        Task<(bool ok, string msg)> UpdateStatusAsync(long sampleId, SampleStatus newStatus, long empId);
        Task ForceAutoStatusAsync(long sampleId, SampleStatus newStatus, long empId);
        Task<bool> UpdateInwardStatus(long inwardId, long empId);
    }
}
