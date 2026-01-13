namespace LIMSApi.Dtos
{
    public class OrgNodeDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string Department { get; set; } = default!;
        public string Initial { get; set; } = default!;
        public string Type { get; set; } = default!; // Designation
        public string? ImageUrl { get; set; }
        public string? Color { get; set; }
        public bool IsExpanded { get; set; } = false;
        public List<OrgNodeDto> Children { get; set; } = new();
    }
}
