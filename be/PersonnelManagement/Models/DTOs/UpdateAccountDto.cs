namespace PersonnelManagement.Models.DTOs
{
    public class UpdateAccountDto
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public Guid RoleAccountId { get; set; }
    }
}
