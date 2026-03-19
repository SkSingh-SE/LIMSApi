using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface ITpiService
    {
        Task<List<TpiInspectionDto>> GetInspectionsBySampleInward(long sampleInwardId);
        Task<TpiInspectionDto?> GetInspectionDetails(long inspectionId);
        Task<TpiInspectionDto> CreateInspection(CreateTpiInspectionDto dto);
        Task<TpiInspectionDto> UpdateInspectionStatus(long inspectionId, UpdateTpiStatusDto dto);
        Task<bool> IsTpiCleared(long sampleInwardId, TpiStage stage);
        Task<List<TpiInspectionDto>> GetPendingInspections();
    }
}
