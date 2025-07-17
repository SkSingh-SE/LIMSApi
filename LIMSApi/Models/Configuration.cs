using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class Configuration : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public string KeyName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string ValueType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}
