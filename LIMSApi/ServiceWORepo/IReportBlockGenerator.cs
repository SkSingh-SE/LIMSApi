using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface IReportBlockGenerator
    {
        //Task<List<ReportBlock>> GenerateBlocksAsync(ReportTemplate template,TestResultHeader testHeader, object testPreviewPayload,string reportNo,string certificateNo);
        List<ReportBlock> GenerateBlocksFromPayload(TestReportPayload payload,bool pageBreak);
    }
}
