namespace LIMSApi.Helpers.Enums
{
    public enum MessageTemplateKey
    {
        // =====================================================
        // 1. SAMPLE INWARD
        // =====================================================
        SAMPLE_INWARD_ACK,
        SAMPLE_INWARD_INCOMPLETE_REMINDER,
        SAMPLE_INWARD_UPDATED,

        // =====================================================
        // 2. TECHNICAL REVIEW / PLANNING
        // =====================================================
        TECHNICAL_CLARIFICATION_REQUEST,
        PLAN_APPROVED,
        PLAN_REJECTED,

        // =====================================================
        // 3. TESTING
        // =====================================================
        TESTING_STARTED,
        TESTING_PARTIALLY_COMPLETED,
        TESTING_COMPLETED,

        // =====================================================
        // 4. REPORT DRAFT / REVIEW / APPROVAL
        // =====================================================
        REPORT_DRAFT_READY,
        REPORT_APPROVED,
        REPORT_REJECTED,

        // =====================================================
        // 5. PRICING & BILLING
        // =====================================================
        PRICE_CALCULATED,
        PROFORMA_INVOICE_SENT,
        FINAL_INVOICE_GENERATED,
        FINAL_INVOICE_POST_TESTING,

        // =====================================================
        // 6. PAYMENT
        // =====================================================
        PAYMENT_LINK,
        PAYMENT_RECEIVED,
        PAYMENT_PARTIAL_RECEIVED,
        PAYMENT_FAILED,
        PAYMENT_OVERDUE_REMINDER,

        // =====================================================
        // 7. REPORT DISPATCH
        // =====================================================
        REPORT_DISPATCHED,
        REPORT_READY_FOR_DOWNLOAD,

        // =====================================================
        // 8. AMENDMENT
        // =====================================================
        INTERNAL_AMENDMENT_APPLIED,
        CUSTOMER_AMENDMENT_REQUESTED,
        CUSTOMER_AMENDMENT_APPROVED,
        CUSTOMER_AMENDMENT_INVOICE_SENT,
        CUSTOMER_AMENDMENT_PAYMENT_RECEIVED,
        AMENDED_REPORT_READY,

        // =====================================================
        // 9. CASE CLOSURE
        // =====================================================
        CASE_CLOSED,

        //=====================================================
        // Additional
        //=====================================================
        SEND_OTP,

        // =====================================================
        // 10. VERSION REVIEW
        // =====================================================
        VERSION_REVIEW_DUE
    }
}
