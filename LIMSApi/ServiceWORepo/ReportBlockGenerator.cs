using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LIMSApi.ServiceWORepo
{
    public class ReportBlockGenerator : IReportBlockGenerator
    {
        private readonly LIMSContext _db;

        public ReportBlockGenerator(LIMSContext db)
        {
            _db = db;
        }

        // =========================
        // SECTION → FIELD MAPS
        // =========================

        //private static readonly Dictionary<string, Dictionary<string, string>> SectionFieldMaps =
        //    new()
        //    {
        //        ["CustomerDetails"] = new()
        //        {
        //            ["Customer Name"] = "sample.sampleInward.customer.name",
        //            ["Customer Address"] = "sample.sampleInward.customer.address"
        //        },

        //        ["SampleDetails"] = new()
        //        {
        //            ["Sample No"] = "sample.sampleNo",
        //            ["Case No"] = "sample.sampleInward.caseNo",
        //            ["Material"] = "sample.metalClassification",
        //            ["Condition"] = "sample.productCondition"
        //        }
        //    };

        //// =========================
        //// ENTRY POINT
        //// =========================

        //public async Task<List<ReportBlock>> GenerateBlocksAsync(
        //    ReportTemplate template,
        //    TestResultHeader header,
        //    object previewPayload,
        //    string reportNo,
        //    string certificateNo)
        //{
        //    var blocks = new List<ReportBlock>();

        //    foreach (var section in template.Blocks.OrderBy(x => x.SortOrder))
        //    {
        //        ReportBlock? block = section.BlockType switch
        //        {
        //            "Header" => BuildHeader(section.SortOrder, header, reportNo, certificateNo),

        //            "CustomerDetails" => BuildKeyValueSection(
        //                section.SortOrder,
        //                "Customer Details",
        //                "CustomerDetails",
        //                previewPayload
        //            ),

        //            "SampleDetails" => BuildKeyValueSection(
        //                section.SortOrder,
        //                "Sample Details",
        //                "SampleDetails",
        //                previewPayload
        //            ),

        //            "ResultTable" => BuildResultTable(section.SortOrder, header),

        //            "Observation" => BuildObservation(section.SortOrder, header),

        //            "Statement" => BuildStatement(section.SortOrder, header),

        //            "LongTermMatrix" => BuildLongTermMatrix(section.SortOrder, header),

        //            "EndFooter" => BuildEndFooter(section.SortOrder),

        //            _ => null
        //        };

        //        if (block != null)
        //            blocks.Add(block);
        //    }

        //    return blocks;
        //}

        //// =========================
        //// HEADER
        //// =========================

        //private ReportBlock BuildHeader(
        //    int order,
        //    TestResultHeader header,
        //    string reportNo,
        //    string certificateNo)
        //{
        //    return new ReportBlock
        //    {
        //        BlockType = "Header",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            CertificateNo = certificateNo,
        //            ReportNo = reportNo,
        //            DateOfIssue = header.CompletedAt?.ToString("dd-MM-yyyy"),
        //            SampleNo = header.Sample?.SampleNo,
        //            TestName = header.LaboratoryTest?.Name,
        //            CustomerName = header.Sample?.SampleInward?.Customer?.Name,
        //            CustomerAddress = header.Sample?.SampleInward?.Customer?.Address,
        //            SampleReceivedOn = header.Sample?.SampleInward?.CollectionTime.ToString("dd-MM-yyyy"),
        //            TestPerformedAt = "DMSPL, Ahmedabad"
        //        })
        //    };
        //}

        //// =========================
        //// KEY VALUE SECTION (CORE)
        //// =========================

        //private ReportBlock? BuildKeyValueSection(
        //    int order,
        //    string title,
        //    string sectionKey,
        //    object payload)
        //{
        //    if (!SectionFieldMaps.TryGetValue(sectionKey, out var map))
        //        return null;

        //    var rows = new List<object>();

        //    using var doc = JsonDocument.Parse(JsonSerializer.Serialize(payload));

        //    foreach (var kv in map)
        //    {
        //        var value = ResolvePath(doc.RootElement, kv.Value);
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            rows.Add(new
        //            {
        //                Label = kv.Key,
        //                Value = value
        //            });
        //        }
        //    }

        //    if (!rows.Any())
        //        return null;

        //    return new ReportBlock
        //    {
        //        BlockType = "KeyValueTable",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            Title = title,
        //            Rows = rows
        //        })
        //    };
        //}

        //// =========================
        //// RESULT TABLE
        //// =========================

        //private ReportBlock BuildResultTable(int order, TestResultHeader header)
        //{
        //    return new ReportBlock
        //    {
        //        BlockType = "ResultTable",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            Title = "Test Results",
        //            Columns = new[] { "Parameter", "Result", "Unit", "Min", "Max" },
        //            Rows = header.Parameters.Select(p => new
        //            {
        //                Parameter = p.ParameterName,
        //                Result = p.Value?.ToString() ?? "",
        //                Unit = p.Unit,
        //                Min = p.MinValue?.ToString() ?? "",
        //                Max = p.MaxValue?.ToString() ?? ""
        //            })
        //        })
        //    };
        //}

        //// =========================
        //// OBSERVATION
        //// =========================

        //private ReportBlock? BuildObservation(int order, TestResultHeader header)
        //{
        //    var fields = header.Parameters
        //        .Where(p => !string.IsNullOrWhiteSpace(p.Remarks))
        //        .Select(p => new
        //        {
        //            Label = p.ParameterName,
        //            Value = p.Remarks
        //        })
        //        .ToList();

        //    if (!fields.Any())
        //        return null;

        //    return new ReportBlock
        //    {
        //        BlockType = "Observation",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            Title = "Observation",
        //            Fields = fields
        //        })
        //    };
        //}

        //// =========================
        //// STATEMENT
        //// =========================

        //private ReportBlock BuildStatement(int order, TestResultHeader header)
        //{
        //    bool pass = header.Parameters.All(p => (bool)p.IsWithinLimit);

        //    return new ReportBlock
        //    {
        //        BlockType = "Statement",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            Text = pass
        //                ? "The result meets the specified requirements."
        //                : "The result does not meet the specified requirements."
        //        })
        //    };
        //}

        //// =========================
        //// LONG TERM MATRIX
        //// =========================

        //private ReportBlock? BuildLongTermMatrix(int order, TestResultHeader header)
        //{
        //    var tests = _db.LongTermTests
        //        .Where(x => x.TestResultHeaderID == header.ID)
        //        .Include(x => x.TestResultHeader)
        //            .ThenInclude(h => h.Parameters)
        //        .Include(x => x.Records)
        //        .ToList();

        //    if (!tests.Any())
        //        return null;

        //    return new ReportBlock
        //    {
        //        BlockType = "LongTermMatrix",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            Tests = tests.Select(t => new
        //            {
        //                TestName = t.TestResultHeader!.LaboratoryTest!.Name,
        //                Rows = t.TestResultHeader.Parameters.Select(p => new
        //                {
        //                    Parameter = p.ParameterName,
        //                    Values = t.Records
        //                        .Select(r =>
        //                            JsonSerializer.Deserialize<Dictionary<string, object>>(r.DataJson)!
        //                                .GetValueOrDefault(p.ParameterName))
        //                        .ToList()
        //                })
        //            })
        //        })
        //    };
        //}

        //// =========================
        //// END FOOTER
        //// =========================

        //private ReportBlock BuildEndFooter(int order)
        //{
        //    return new ReportBlock
        //    {
        //        BlockType = "EndFooter",
        //        SortOrder = order,
        //        PayloadJson = JsonSerializer.Serialize(new
        //        {
        //            TestedBy = new { Name = "Aaron Christie", Designation = "Lab Analyst", SignaturePath = "Assets/signature.png" },
        //            ReviewedBy = new { Name = "Janakkumar Kakadiya", Designation = "Technical Manager", SignaturePath = "Assets/signature.png" },
        //            WitnessedBy = new { Name = "Rahul R. Prasad", Organization = "TUV India", SignaturePath = "Assets/signature.png" }
        //        })
        //    };
        //}

        //// =========================
        //// JSON PATH RESOLVER
        //// =========================

        //private static string ResolvePath(JsonElement root, string path)
        //{
        //    foreach (var part in path.Split('.'))
        //    {
        //        if (root.ValueKind == JsonValueKind.Object &&
        //            root.TryGetProperty(part, out var next))
        //            root = next;
        //        else
        //            return string.Empty;
        //    }

        //    return root.ToString();
        //}

        public List<ReportBlock> GenerateBlocksFromPayload(
        TestReportPayload payload,
        bool pageBreak)
        {
            var blocks = new List<ReportBlock>();

            // -------------------------------------------------
            // Header (page break before header except first test)
            // -------------------------------------------------
            blocks.Add(CreateBlock(
                blockType: "Header",
                payload: payload.Header,
                pageBreak: pageBreak
            ));

            // -------------------------------------------------
            // Customer Details
            // -------------------------------------------------
            if (payload.CustomerDetails != null)
            {
                blocks.Add(CreateBlock(
                    "KeyValueTable",
                    payload.CustomerDetails
                ));
            }

            // -------------------------------------------------
            // Sample Details
            // -------------------------------------------------
            if (payload.SampleDetails != null)
            {
                blocks.Add(CreateBlock(
                    "KeyValueTable",
                    payload.SampleDetails
                ));
            }

            // -------------------------------------------------
            // Test Title (before result table)
            // -------------------------------------------------
            blocks.Add(CreateBlock(
                "SectionTitle",
                new SectionTitlePayload
                {
                    Title = payload.TestName
                }
            ));

            // -------------------------------------------------
            // Result Table
            // -------------------------------------------------
            if (payload.ResultTable != null)
            {
                blocks.Add(CreateBlock(
                    "ResultTable",
                    payload.ResultTable
                ));
            }

            // -------------------------------------------------
            // Observation
            // -------------------------------------------------
            if (payload.Observation != null)
            {
                blocks.Add(CreateBlock(
                    "Observation",
                    payload.Observation
                ));
            }

            // -------------------------------------------------
            // Statement
            // -------------------------------------------------
            if (payload.Statement != null)
            {
                blocks.Add(CreateBlock(
                    "Statement",
                    payload.Statement
                ));
            }

            // -------------------------------------------------
            // Long Term Matrix
            // -------------------------------------------------
            if (payload.LongTerm != null)
            {
                blocks.Add(CreateBlock(
                    "LongTermMatrix",
                    payload.LongTerm
                ));
            }

            // -------------------------------------------------
            // Images
            // -------------------------------------------------
            if (payload.Images != null && payload.Images.Images.Any())
            {
                blocks.Add(CreateBlock(
                    "ImageGallery",
                    payload.Images
                ));
            }

            // -------------------------------------------------
            // End Footer (Signatures)
            // -------------------------------------------------
            if (payload.EndFooter != null)
            {
                blocks.Add(CreateBlock(
                    "EndFooter",
                    payload.EndFooter
                ));
            }

            return blocks;
        }

        // =====================================================
        // Helper
        // =====================================================
        private static ReportBlock CreateBlock(
            string blockType,
            object payload,
            bool pageBreak = false)
        {
            return new ReportBlock
            {
                BlockType = blockType,
                PayloadJson = JsonSerializer.Serialize(payload)
            };
        }
    }
}
