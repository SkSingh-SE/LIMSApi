using LIMSApi.Models;

namespace LIMSApi.Dtos
{
    public class NablTestMethodValidationDto
    {
        public string? TestMethodName { get; set; }
        public string? RevIssue { get; set; }
        public string? ReferenceStandard { get; set; }
        public string? Humidity { get; set; }
        public string? Temperature { get; set; }
        public string? EquipmentId { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? VerifiedBy { get; set; }
        public string? EquipmentName { get; set; }
        public List<CrmParameters> CrmMaterialParameters { get; set; }
    }
}
