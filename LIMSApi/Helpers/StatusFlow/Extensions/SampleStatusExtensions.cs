using LIMSApi.Helpers.Enums;

namespace LIMSApi.Helpers.StatusFlow.Extensions
{
    public static class SampleStatusExtensions
    {
        public static string ToDisplayName(this SampleStatus status)
        {
            return status switch
            {
                SampleStatus.REPORT_DISPATCHED => "Report Dispatched",
                _ => status.ToString().Replace("_", " ").ToLower()
            };
        }
        
    }
}
