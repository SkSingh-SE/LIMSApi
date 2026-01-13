using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class MessageTemplate : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string TemplateKey { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = string.Empty;  // "Email" | "WhatsApp"

        [MaxLength(200)]
        public string? Subject { get; set; }  // Email only, nullable for WhatsApp
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }  // Admin description
        
        public int Version { get; set; } = 1;
    }

}
