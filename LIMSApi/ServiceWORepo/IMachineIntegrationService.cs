using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    public interface IMachineIntegrationService
    {
        Task<MachineDataResultDto> ProcessMachineData(MachineDataDto dto);
        Task<List<object>> GetPendingTestsForEquipment(long equipmentId);
        Task<List<object>> GetMachineDataLogs(long headerId);
    }
}
