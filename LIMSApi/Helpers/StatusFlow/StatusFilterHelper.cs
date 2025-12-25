using LIMSApi.Helpers.StatusFlow.Extensions;
using LIMSApi.Services;

namespace LIMSApi.Helpers.StatusFlow
{
    public static class StatusFilterHelper
    {
        public static List<string> GetAllowedStatuses(WorkflowListType listType)
        {
            return listType switch
            {
                WorkflowListType.Inward =>
                    GetByPriority(1, 2),

                WorkflowListType.Planning =>
                    GetByPriority(4),

                WorkflowListType.Testing =>
                    GetByPriority(5, 99),

                WorkflowListType.Reporting =>
                    GetByPriority(6, 99),

                WorkflowListType.Completed =>
                    GetByPriority(7),

                WorkflowListType.Rejected =>
                    GetByPriority(99),

                _ => new List<string>()
            };
        }

        private static List<string> GetByPriority(params int[] priorities)
        {
            return SampleStatusService.WorkflowPriority
                .Where(x => priorities.Contains(x.Value))
                .Select(x => x.Key.ToString())
                .ToList();
        }
    }
    public enum WorkflowListType
    {
        Inward,
        Planning,
        Testing,
        Review,
        Reporting,
        Completed,
        Rejected
    }
}
