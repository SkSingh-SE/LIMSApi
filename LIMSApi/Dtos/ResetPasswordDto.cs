namespace LIMSApi.Dtos
{
    public class ResetPasswordDto
    {
        public long EmployeeId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
