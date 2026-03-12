namespace Cocorra.DAL.DTOS.ProfileDto
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int Age { get; set; }
    }
}