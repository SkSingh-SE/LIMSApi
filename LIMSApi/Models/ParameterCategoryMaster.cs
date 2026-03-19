using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class ParameterCategoryMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
