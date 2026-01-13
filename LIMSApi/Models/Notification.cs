using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class Notification:AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long? UserID { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public NotificationType Type { get; set; }
        public string? Email { get; set; }
        public long? EntityID { get; set; }
        public string? EntityType { get; set; }
        public long? WorkflowID { get; set; }
        public long? StepID { get; set; }
        public string? Action { get; set; }
        public bool IsRead { get; set; } = false;
    }
    public enum NotificationType
    {
        System = 0,
        General = 1,
        Workflow = 2,
        Email = 3,
        WhatsApp = 4
    }
}
