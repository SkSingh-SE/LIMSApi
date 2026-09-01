using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IPlanComplianceService
    {
        Task<PlanComplianceResultDto> EvaluateComplianceAsync(PlanComplianceRequestDto request);
        Task<List<DropdwonSelector>> GetChemicalParametersForAnalysisTypeAsync(long analysisTypeId);
    }
}
