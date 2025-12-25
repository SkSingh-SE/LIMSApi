using System;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Helpers.StatusFlow
{
    /// <summary>
    /// Decides what user can do on a record for a given workflow list.
    /// Does NOT decide visibility (that is handled by StatusFilterHelper).
    /// </summary>
    public static class ActionStatusResolver
    {
        public static ActionStatus Resolve(WorkflowListType listType, string currentStatus)
        {
            if (!Enum.TryParse<SampleStatus>(currentStatus, out var status))
                return ActionStatus.NOT_REACHED;

            return listType switch
            {
                WorkflowListType.Inward => ResolveInward(currentStatus),
                WorkflowListType.Planning => ResolvePlanning(currentStatus),
                WorkflowListType.Review => ResolveReview(currentStatus),
                WorkflowListType.Testing => ResolveTesting(status),
                WorkflowListType.Reporting => ResolveReporting(status),

                _ => ActionStatus.NOT_REACHED
            };
        }

        // Inward List

        public static ActionStatus ResolveInward(string inwardStatus)
        {
            if (!Enum.TryParse<InwardStatus>(inwardStatus, out var status))
                return ActionStatus.NOT_REACHED;

            return status switch
            {
                InwardStatus.INWARD_REGISTERED
                or InwardStatus.IN_PROGRESS
                or InwardStatus.INWARD_COMPLETED
                    => ActionStatus.PENDING, // edit inward

                InwardStatus.UNDER_PLANNING
                    => ActionStatus.PENDING, // planning enabled

                InwardStatus.UNDER_REVIEW
                or InwardStatus.COMPLETED
                    => ActionStatus.COMPLETED, // locked

                InwardStatus.REJECTED
                    => ActionStatus.REJECTED,

                _ => ActionStatus.PENDING
            };
        }



        // ======================================================
        // PLANNING LIST
        // Entry point: UNDER_PLANNING
        // ======================================================
        private static ActionStatus ResolvePlanning(string inwardStatus)
        {
            if (!Enum.TryParse<InwardStatus>(inwardStatus, out var status))
                return ActionStatus.NOT_REACHED;

            return status switch
            {
                InwardStatus.INWARD_REGISTERED
                or InwardStatus.INWARD_COMPLETED
                    => ActionStatus.PENDING, // edit inward

                InwardStatus.UNDER_PLANNING
                    => ActionStatus.PENDING, // planning enabled

                InwardStatus.UNDER_REVIEW
                or InwardStatus.COMPLETED
                    => ActionStatus.COMPLETED, // locked

                _ => ActionStatus.PENDING
            };
        }

        // ======================================================
        // Review LIST
        // Entry point: UNDER_REVIEW
        // ======================================================
        private static ActionStatus ResolveReview(string inwardStatus)
        {
            if (!Enum.TryParse<InwardStatus>(inwardStatus, out var status))
                return ActionStatus.NOT_REACHED;

            return status switch
            {
                InwardStatus.UNDER_REVIEW
                    => ActionStatus.PENDING, 

                InwardStatus.REVIEW_COMPLETED
                or InwardStatus.COMPLETED
                    => ActionStatus.COMPLETED, // locked

                _ => ActionStatus.NOT_REACHED
            };
        }

        // ======================================================
        // TESTING LIST
        // Entry point: REQUEST_APPROVED
        // ======================================================
        private static ActionStatus ResolveTesting(SampleStatus status)
        {
            if (!HasReachedTesting(status))
                return ActionStatus.NOT_REACHED;

            return status switch
            {
                SampleStatus.TESTING_COMPLETED
                    => ActionStatus.COMPLETED,

                SampleStatus.REQUEST_REJECTED
                or SampleStatus.REPORT_REJECTED_BY_INTERNAL
                    => ActionStatus.REJECTED,

                _ => ActionStatus.PENDING
            };
        }

        // ======================================================
        // REPORTING LIST
        // Entry point: REPORT_GENERATION_IN_PROGRESS
        // ======================================================
        private static ActionStatus ResolveReporting(SampleStatus status)
        {
            if (!HasReachedReporting(status))
                return ActionStatus.NOT_REACHED;

            return status switch
            {
                SampleStatus.FINAL_REPORT_APPROVED
                or SampleStatus.REPORT_DISPATCHED
                    => ActionStatus.COMPLETED,

                SampleStatus.REPORT_REJECTED_BY_INTERNAL
                    => ActionStatus.REJECTED,

                _ => ActionStatus.PENDING
            };
        }

        // ======================================================
        // ENTRY POINT CHECKS (based on flow order)
        // ======================================================


        private static bool HasReachedTesting(SampleStatus status)
        {
            return status >= SampleStatus.REQUEST_APPROVED;
        }

        private static bool HasReachedReporting(SampleStatus status)
        {
            return status >= SampleStatus.REPORT_GENERATION_IN_PROGRESS;
        }
    }

    /// <summary>
    /// What user can do on a record in a given list
    /// </summary>
    public enum ActionStatus
    {
        NOT_REACHED, // Flow has not reached this stage yet
        PENDING,     // Action is possible
        COMPLETED,   // Stage completed (read-only)
        REJECTED     // Rejected (special handling)
    }
}
