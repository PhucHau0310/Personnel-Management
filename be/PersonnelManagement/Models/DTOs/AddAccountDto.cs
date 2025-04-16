namespace PersonnelManagement.Models.DTOs
{
    public class AddAccountDto
    {
        public string? Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public Guid RoleId { get; set; }
    }
}
