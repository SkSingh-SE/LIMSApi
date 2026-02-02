namespace LIMSApi.Dtos
{
    public class Update2FADto
    {
        public long EmployeeId { get; set; }
        public bool Enabled { get; set; }
        public string Method { get; set; } = string.Empty;
    }
}
