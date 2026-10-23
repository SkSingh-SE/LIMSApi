namespace LIMSApi.Dtos
{
    public class MenuPermissionGroupDto
    {
        public string MenuTitle { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
    public class PermissionDto
    {
        public long ID { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public long? MenuID { get; set; }
        public string MenuTitle { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public bool IsOverride { get; set; }
    }
    public class UserPermissionUpdateDto
    {
        public long PermissionID { get; set; }
        public bool IsGranted { get; set; }
    }

}
