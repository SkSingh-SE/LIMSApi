using static LIMSApi.Reporting.ReportDocument;

namespace LIMSApi.Dtos
{
    public class ReportDTO
    {
    }
    public class ReportCreateFromSampleDto
    {
        public long SampleId { get; set; }
        public List<long> TestResultHeaderIds { get; set; } = new();
        public string? ReportNo { get; set; }
    }

    public class ReportBlockDto
    {
        public string BlockType { get; set; }
        public int SortOrder { get; set; }
        public object Payload { get; set; }
    }

    public class ReportReadDto
    {
        public long Id { get; set; }
        public long SampleId { get; set; }
        public string ReportNo { get; set; } = string.Empty;
        public string SampleNo { get; set; } = string.Empty;
        public string? CertificateNo { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<ReportBlockReadDto> Blocks { get; set; }
            = new();
    }

    public class ReportBlockReadDto
    {
        public long Id { get; set; }
        public string BlockType { get; set; }
        public int SortOrder { get; set; }
        public string PayloadJson { get; set; }
    }

    public class ReportPreviewDto
    {
        public long ReportHeaderId { get; set; }
        public long WorkflowInstanceId { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string CaseNo { get; set; }
        public string Customer { get; set; }
        public string Material { get; set; }
        public string Condition { get; set; }
        public string ReportNo { get; set; }
        public string Status { get; set; }
        public List<ReportTestDto> MechanicalTests { get; set; }
        public List<ReportTestDto> ChemicalTests { get; set; }
        public List<ReportLongTermTestDto> LongTermTests { get; set; }
        public List<ReportActionDto> Actions { get; set; }
        public ReportAmendmentPreviewDto? Amendment { get; set; }

        // Sample details for approval review
        public string? Grade { get; set; }
        public string? HeatNo { get; set; }
        public string? BatchNo { get; set; }
        public string? SampleDescription { get; set; }
        public string? Quantity { get; set; }
        public DateTime? SampleReceivedDate { get; set; }
        public DateTime? TestStartDate { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public string? StatementOfConformity { get; set; }
        public string? DecisionRule { get; set; }
        public string? UlrNo { get; set; }
    }
    public class ReportAmendmentPreviewDto
    {
        public long AmendmentRequestId { get; set; }
        public string Reason { get; set; } = "";
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public DateTime RequestedOn { get; set; }
    }
    public class ReportTestDto
    {
        public long TestResultHeaderId { get; set; }
        public string? TestName { get; set; }
        public string ReportNo { get; set; }
        public string? Specification1Name { get; set; }
        public string? Specification2Name { get; set; }
        public string Status { get; set; }
        public List<ReportTestParameterDto> Parameters { get; set; }
        public List<ReportTestImageDto> Images { get; set; } = new();
    }

    public class ReportTestImageDto
    {
        public long ID { get; set; }
        public string? FilePath { get; set; }
        public string? Caption { get; set; }
    }

    public class ReportTestParameterDto
    {
        public long ParameterID { get; set; }
        public string ParameterName { get; set; }
        public decimal? Value { get; set; }
        public string Unit { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public bool IsWithinLimit { get; set; }
        public string? ResultStatus { get; set; }
        public string Remarks { get; set; }

        // Unit conversion & display
        public int DecimalPrecision { get; set; } = 2;
        public decimal ConversionFactor { get; set; } = 1;
        public decimal? ConvertedValue { get; set; }
        public string? SelectedUnit { get; set; }
    }

    public class ReportLongTermTestDto
    {
        public long LongTermTestId { get; set; }
        public string TestName { get; set; }
        public int DurationHours { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Status { get; set; }
        public List<ReportLongTermParameterDto> Parameters { get; set; }
        public List<ReportLongTermReadingDto> Readings { get; set; }
    }

    public class ReportLongTermParameterDto
    {
        public string ParameterName { get; set; }
        public string Value { get; set; }
    }

    public class ReportLongTermReadingDto
    {
        public DateTime RecordedAt { get; set; }
        public object? Parsed { get; set; }
    }

    public class ReportActionDto
    {
        public long Id { get; set; } // WorkflowInstanceId
        public string Name { get; set; } // Button label
        public string Action { get; set; } // Next / Cancel / Back
    }

    // for reporting

    public class ReportRenderPayload
    {
        public long SampleId { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string ReportNo { get; set; }
        public string CertificateNo { get; set; }

        public List<TestReportPayload> Tests { get; set; } = new();
    }

    public class TestReportPayload
    {
        public string TestName { get; set; }

        public HeaderPayload Header { get; set; }
        public SectionTitlePayload TestTitle { get; set; }

        public KeyValueTablePayload CustomerDetails { get; set; }
        public KeyValueTablePayload SampleDetails { get; set; }

        public TablePayload? ResultTable { get; set; }
        public ObservationPayload? Observation { get; set; }
        public StatementPayload? Statement { get; set; }

        public LongTermMatrixPayload? LongTerm { get; set; }
        public ImageGalleryPayload? Images { get; set; }

        public EndFooterPayload EndFooter { get; set; }
    }

    public class HeaderPayload
    {
        public string CertificateNo { get; set; }
        public string ReportNo { get; set; }
        public string DateOfIssue { get; set; }

        public long SampleId { get; set; }
        public string SampleNo { get; set; } = string.Empty;

        public string TestName { get; set; }
        public string TestMethod { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public string SampleReceivedOn { get; set; }
        public string TestPerformedAt { get; set; }
    }

    public class SectionTitlePayload
    {
        public string Title { get; set; }
    }

    public class KeyValueTablePayload
    {
        public string Title { get; set; }
        public List<KeyValueRow> Rows { get; set; } = new();
    }

    public class KeyValueRow
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
    public class TablePayload
    {
        public string Title { get; set; }
        public string[] Columns { get; set; }
        public List<TableRowPayload> Rows { get; set; } = new();
    }

    public class TableRowPayload
    {
        public string Parameter { get; set; }
        public string Result { get; set; }
        public string Unit { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
    }
    public class ObservationPayload
    {
        public string Title { get; set; } = "Observation";
        public List<ObservationField> Fields { get; set; } = new();
    }

    public class ObservationField
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
    public class StatementPayload
    {
        public string Text { get; set; }
    }
    public class LongTermMatrixPayload
    {
        public bool ShowAverage { get; set; }
        public List<LongTermTestPayload> Tests { get; set; } = new();
    }

    public class LongTermTestPayload
    {
        public string TestName { get; set; }
        public List<LongTermRowPayload> Rows { get; set; } = new();
    }

    public class LongTermRowPayload
    {
        public string Parameter { get; set; }
        public List<object> Values { get; set; } = new();
        public decimal? Average { get; set; }
    }
    public class ImageGalleryPayload
    {
        public string Title { get; set; } = "Test Images";
        public List<ImagePayload> Images { get; set; } = new();
    }

    public class ImagePayload
    {
        public string Url { get; set; }
        public string Caption { get; set; }
    }
    public class EndFooterPayload
    {
        public SignatoryPayload TestedBy { get; set; } = new();
        public SignatoryPayload ReviewedBy { get; set; } = new();
        public SignatoryPayload WitnessedBy { get; set; } = new();
    }

    public class SignatoryPayload
    {
        public string Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? Organization { get; set; }
        public string SignaturePath { get; set; } = string.Empty;
    }
    public class AmendmentRequestDto
    {
        public long ReportHeaderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

}
