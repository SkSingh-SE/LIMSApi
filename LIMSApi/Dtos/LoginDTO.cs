using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class LoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
