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

        // ── Lab/Company Identity ──
        public string LabName { get; set; } = string.Empty;
        public string LabAddress { get; set; } = string.Empty;
        public string LabPhone { get; set; } = string.Empty;
        public string LabEmail { get; set; } = string.Empty;
        public string? LabLogoPath { get; set; }
        public string? CIN { get; set; }
        public string? NablLogoPath { get; set; }
        public string? CompanyStampPath { get; set; }

        // ── Certificate Identity ──
        public string? UlrNo { get; set; }
        public string? DateOfIssue { get; set; }
        public string? SampleReceivedDate { get; set; }
        public string? TestPerformedAt { get; set; }

        // ── Customer ──
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerGST { get; set; } = string.Empty;

        // ── Customer Provided Info ──
        public string? CustomerReference { get; set; }
        public string? StampedAs { get; set; }
        public string? NatureOfSample { get; set; }
        public string? SampleDrawnBy { get; set; }

        // ── Sample ──
        public string CaseNo { get; set; } = string.Empty;
        public string SampleNo { get; set; } = string.Empty;
        public string SampleDescription { get; set; } = string.Empty;
        public string MaterialSpec { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string? ProductForm { get; set; }
        public string? SpecimenOrientation { get; set; }
        public string? HeatTreatment { get; set; }
        public string? HeatNo { get; set; }
        public string? BatchNo { get; set; }
        public int? Quantity { get; set; }

        // ── Sample Dimensions ──
        public decimal? Thickness { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? CrossSectionArea { get; set; }
        public decimal? GaugeLength { get; set; }

        // ── Test Conditions (per first test header) ──
        public decimal? RoomTemperature { get; set; }
        public decimal? RoomHumidity { get; set; }
        public string? EquipmentUsed { get; set; }
        public string? LabRoom { get; set; }

        // ── Dates ──
        public string DateReceived { get; set; } = string.Empty;
        public string DateTested { get; set; } = string.Empty;
        public string DateReported { get; set; } = string.Empty;

        // ── Footer Conditions ──
        public List<string> ReportConditions { get; set; } = new();

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
        public NablReportInfo? NablInfo { get; set; }

        // ── Conformity Assessment ──
        public string? StatementOfConformity { get; set; }
        public string? DecisionRule { get; set; }
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

        /// <summary>Higher-level category: "CHEMICAL", "MECHANICAL + METALS & ALLOYS", etc.</summary>
        public string TestCategory { get; set; } = string.Empty;

        public string? SpecificationName { get; set; }

        /// <summary>Test method used (e.g., "IS 1608:2005", "JIS G 1253:2002")</summary>
        public string? TestMethod { get; set; }

        /// <summary>Date test was completed (formatted string)</summary>
        public string? DateOfTesting { get; set; }

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
        public bool? IsWithinNablScope { get; set; }
        public string? NablScopeStatus { get; set; } // WithinScope / OutsideScope / NotAccredited
        public decimal? ExpandedUncertainty { get; set; }
        public decimal? CoverageFactor { get; set; }
        public string? ConformityResult { get; set; } // Conforms / Does not conform / null

        /// <summary>For chemical multi-column pivot: identifies which test group (e.g., "GTAW", "SMAW")</summary>
        public string? SubGroup { get; set; }
    }

    /// <summary>
    /// Carries out-of-scope parameter names for the report footer note.
    /// </summary>
    public class NablReportInfo
    {
        public bool IsPartialScope { get; set; }
        public List<string> OutOfScopeParameterNames { get; set; } = new();
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
