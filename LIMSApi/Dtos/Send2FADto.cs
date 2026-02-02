namespace LIMSApi.Dtos
{
    public class Send2FADto
    {
        public long EmployeeId { get; set; }
        public string Method { get; set; } = string.Empty; // email | sms
    }
    public class Verify2FADto
    {
        public long EmployeeId { get; set; }
        public string Otp { get; set; } = string.Empty;
    }
}
