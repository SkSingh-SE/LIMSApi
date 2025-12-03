namespace LIMSApi.Helpers.Enums
{
    public enum SampleStatus
    {
        // --- INWARD / REGISTRATION ---
        SAMPLE_INWARD_REGISTERED,       // Sample Arrived
        AWAITING_MISSING_INFORMATION,   // 12-hour reminder loop
        INWARD_COMPLETED,               // All details captured

        UNDER_PLANNING,                 // Sample under planning phase
        // --- REVIEW OF REQUEST ---
        UNDER_REVIEW_REQUEST,           // Method/Specs review
        REQUEST_REJECTED,               // If incorrect info
        REQUEST_APPROVED,               // Review successful

        // --- SAMPLE PREPARATION (Before PI) ---
        PREPARATION_REQUIRED,
        PREPARATION_IN_PROGRESS,
        PREPARATION_COMPLETED,

        // --- PI GENERATION (After Preparation Cost Known) ---
        PI_GENERATED,
        ADVANCE_PAYMENT_PENDING,
        ADVANCE_PAYMENT_COMPLETED,

        // --- TPI (Optional) ---
        TPI_WAITING_FOR_AGENT,
        TPI_IN_PROGRESS,
        TPI_COMPLETED,

        // --- TESTING ---
        TESTING_IN_PROGRESS,
        TESTING_COMPLETED,

        // --- REPORTING ---
        REPORT_GENERATION_IN_PROGRESS,
        REPORT_GENERATED,
        REPORT_UNDER_REVIEW,
        REPORT_REJECTED_BY_INTERNAL,

        // --- CUSTOMER REVIEW / AMENDMENT ---
        REPORT_SENT_FOR_CUSTOMER_REVIEW,
        CUSTOMER_REQUESTED_AMENDMENT,
        AMENDMENT_IN_PROGRESS,
        AMENDMENT_COMPLETED,

        // --- PAYMENT (If required after report) ---
        PAYMENT_PENDING,
        PAYMENT_COMPLETED,

        // --- FINAL APPROVAL & CLOSURE ---
        FINAL_REPORT_APPROVED,
        REPORT_DISPATCHED,
        CASE_CLOSED
    }


}
