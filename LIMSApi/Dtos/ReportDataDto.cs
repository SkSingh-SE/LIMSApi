namespace LIMSApi.Dtos
{
    /// <summary>
    /// Carries all data needed for PDF generation via QuestPDF.
    /// Built by ReportService.BuildReportDataAsync() from database joins.
    /// </summary>
    public class ReportDataDto
    {
        // ── Report Identity ──
        public long ReportId { get; set; }
        public long ReportHeaderId { get; set; }
        public string ReportNo { get; set; } = string.Empty;
        public string? CertificateNo { get; set; }
        public DateTime ReportDate { get; set; }

        // ── Customer ──
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerGST { get; set; } = string.Empty;

        // ── Sample ──
        public string CaseNo { get; set; } = string.Empty;
        public string SampleNo { get; set; } = string.Empty;
        public string SampleDescription { get; set; } = string.Empty;
        public string MaterialSpec { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;

        // ── Dates ──
        public string DateReceived { get; set; } = string.Empty;
        public string DateTested { get; set; } = string.Empty;
        public string DateReported { get; set; } = string.Empty;

        // ── Tests ──
        public List<ReportDataTestSection> TestSections { get; set; } = new();

        // ── Remarks ──
        public string? Remarks { get; set; }

        // ── Signatures ──
        public string TestedByName { get; set; } = string.Empty;
        public string? TestedByDesignation { get; set; }
        public string? TestedBySignaturePath { get; set; }

        public string ReviewedByName { get; set; } = string.Empty;
        public string? ReviewedByDesignation { get; set; }
        public string? ReviewedBySignaturePath { get; set; }

        public string AuthorizedByName { get; set; } = string.Empty;
        public string? AuthorizedByDesignation { get; set; }
        public string? AuthorizedBySignaturePath { get; set; }

        // ── QR ──
        public string? QrCodeData { get; set; }

        // ── NABL ──
        public bool IsNabl { get; set; }
        public string? NablCertNo { get; set; }
    }

    /// <summary>
    /// One test section inside a report (e.g., Tensile Test, Chemical Analysis).
    /// </summary>
    public class ReportDataTestSection
    {
        public long TestResultHeaderId { get; set; }
        public string TestName { get; set; } = string.Empty;

        /// <summary>General or Chemical — drives the table layout.</summary>
        public string TestType { get; set; } = "General";

        public string? SpecificationName { get; set; }

        public List<ReportDataParameter> Parameters { get; set; } = new();
        public List<ReportDataImage> Images { get; set; } = new();
    }

    /// <summary>
    /// A single parameter row in a test-results table.
    /// </summary>
    public class ReportDataParameter
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? SpecMin { get; set; }
        public string? SpecMax { get; set; }
        public string? Result { get; set; }
        public string Status { get; set; } = string.Empty; // Pass / Fail / N/A
    }

    /// <summary>
    /// A test-result image (microstructure, etc.).
    /// </summary>
    public class ReportDataImage
    {
        public string Url { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }
}
