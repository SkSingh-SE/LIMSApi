using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using QuestPDF.Infrastructure;

namespace LIMSApi.Reporting
{
    /// <summary>
    /// Factory that creates the appropriate QuestPDF document based on report format type.
    /// </summary>
    public static class ReportDocumentFactory
    {
        public static IDocument Create(ReportFormatType formatType, ReportDataDto data, string? watermark = null)
        {
            return formatType switch
            {
                ReportFormatType.TestCertificate => new EnhancedReportDocument(data),
                ReportFormatType.ChemicalAnalysis => new ChemicalAnalysisDocument(data, watermark),
                ReportFormatType.CertificateOfConformance => new CertificateOfConformanceDocument(data, watermark),
                ReportFormatType.FullTestReport => new EnhancedReportDocument(data),
                ReportFormatType.MechanicalTest => new EnhancedReportDocument(data), // uses full report filtered client-side
                ReportFormatType.Metallography => new EnhancedReportDocument(data),
                ReportFormatType.FailureAnalysis => new EnhancedReportDocument(data),
                ReportFormatType.Amendment => new EnhancedReportDocument(data),
                ReportFormatType.Custom => new EnhancedReportDocument(data),
                _ => new EnhancedReportDocument(data)
            };
        }

        /// <summary>
        /// Returns all available report format types with display names.
        /// </summary>
        public static List<ReportFormatInfo> GetAvailableFormats()
        {
            return new List<ReportFormatInfo>
            {
                new() { FormatType = ReportFormatType.TestCertificate, DisplayName = "Test Certificate (MTC)", Description = "Chemical + Mechanical + Dimensional with NABL header" },
                new() { FormatType = ReportFormatType.ChemicalAnalysis, DisplayName = "Chemical Analysis Report", Description = "OES/Wet chemistry element table only" },
                new() { FormatType = ReportFormatType.MechanicalTest, DisplayName = "Mechanical Test Report", Description = "Tensile + Impact + Hardness with specimen details" },
                new() { FormatType = ReportFormatType.FullTestReport, DisplayName = "Full Test Report", Description = "All test sections combined" },
                new() { FormatType = ReportFormatType.CertificateOfConformance, DisplayName = "Certificate of Conformance (COC)", Description = "Conformance statement only, no raw values" },
                new() { FormatType = ReportFormatType.Metallography, DisplayName = "Metallography Report", Description = "Microstructure, grain size, inclusions" },
                new() { FormatType = ReportFormatType.FailureAnalysis, DisplayName = "Failure Analysis Report", Description = "Narrative: Background / Testing / Results / Root Cause" },
                new() { FormatType = ReportFormatType.Amendment, DisplayName = "Amendment Report", Description = "Any format + amendment change table" },
            };
        }
    }

    public class ReportFormatInfo
    {
        public ReportFormatType FormatType { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
