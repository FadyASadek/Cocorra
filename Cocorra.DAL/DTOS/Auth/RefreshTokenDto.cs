using System.ComponentModel.DataAnnotations;

namespace Cocorra.DAL.DTOS.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
