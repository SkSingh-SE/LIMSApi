using LIMSApi.Helpers.Enums;
using LIMSApi.Services;

namespace LIMSApi.Helpers.StatusFlow.Extensions
{
    public static class SampleStatusExtensions
    {
        public static bool IsVisible(
            string status,
            WorkflowListType listType)
        {
            if (!Enum.TryParse<SampleStatus>(status, out var st))
                return false;

            if (!SampleStatusService.WorkflowPriority.TryGetValue(st, out var p))
                return false;

            return listType switch
            {
                WorkflowListType.Inward => p == 1 || p == 2,
                WorkflowListType.Planning => p == 4,
                WorkflowListType.Testing => p == 4 || p == 5 || p == 99,
                WorkflowListType.Reporting => p == 6 || p == 99,
                WorkflowListType.Completed => p == 7,
                WorkflowListType.Rejected => p == 99,
                _ => false
            };
        }
    }



}

