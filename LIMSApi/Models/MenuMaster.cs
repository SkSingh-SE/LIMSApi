using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class MenuMaster
    {
        [Key]
        public long ID { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsExpanded { get; set; } = false;
        public string? Route { get; set; }
        public string? Color { get; set; }

        public long? ParentID { get; set; }
        public MenuMaster? Parent { get; set; }
        public ICollection<MenuMaster> SubMenu { get; set; } = new List<MenuMaster>();
    }
}
