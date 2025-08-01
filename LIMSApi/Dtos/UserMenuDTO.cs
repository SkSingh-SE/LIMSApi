namespace LIMSApi.Dtos
{
    public class UserMenuDTO
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Route { get; set; }
        public long? ParentMenuID { get; set; }
        public List<UserMenuDTO> Children { get; set; } = new List<UserMenuDTO>();
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
