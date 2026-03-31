using System.ComponentModel.DataAnnotations;

namespace Cocorra.DAL.DTOS.ProfileDto
{
    public class UpdateProfileDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [Range(18, 120)]
        public int Age { get; set; }
    }
}