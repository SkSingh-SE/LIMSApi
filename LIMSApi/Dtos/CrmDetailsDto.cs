namespace LIMSApi.Dtos
{
    public class CrmDetailsDto
    {
        public long ReferenceMaterialId { get; set; }
        public string? DocumentNo { get; set; }
        public string? RmCode { get; set; }
        public string? RmName { get; set; }
        public string? Type { get; set; }
        public string? MaterialClassification { get; set; }
        public string? BatchNo { get; set; }
        public string? CertificateNo { get; set; }
        public DateTime? ValidityDate { get; set; }
        public DateTime? Date { get; set; }
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public string? Manufacturer { get; set; }
        public string? PreparedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? PreparedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ReviewedBy { get; set; }

    }
}
