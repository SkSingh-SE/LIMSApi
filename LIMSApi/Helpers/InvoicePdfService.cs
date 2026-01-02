using LIMSApi.Dtos;
using LIMSApi.Models;
using QuestPDF.Fluent;

namespace LIMSApi.Helpers
{
    public class InvoicePdfService
    {
        private readonly IWebHostEnvironment _env;

        public InvoicePdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> GenerateTaxInvoicePdfAsync(
            TaxInvoice invoice,
            TaxInvoicePdfModelDto model)
        {
            var folder = Path.Combine(_env.WebRootPath, "invoices");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{invoice.InvoiceNo}.pdf";
            var filePath = Path.Combine(folder, fileName);

            var doc = new ProformaInvoiceDocument(
                model,
                Path.Combine(_env.WebRootPath, "assets/logo.png"),
                Path.Combine(_env.WebRootPath, "assets/signature.png")
            );

            doc.GeneratePdf(filePath);

            return $"/invoices/{fileName}";
        }
    }
}
