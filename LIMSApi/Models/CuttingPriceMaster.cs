using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Models
{
    public class CuttingPriceMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long? SpecimenTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CuttingType { get; set; } = string.Empty;
        [MaxLength(100)]
        [Required]
        public string UnitType { get; set; } = string.Empty;

        [Required,Precision(10,2)]
        public decimal RatePerUnit { get; set; }

        public string? Remark { get; set; } = string.Empty;

        [ForeignKey("SpecimenTypeId"), JsonIgnore]
        public virtual SpecimenTypeMaster? SpecimenType { get; set; }
    }
}
