namespace PersonnelManagement.Models.DTOs
{
    public class AccountDtos
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        //public required string Email { get; set; }
        public Guid RoleAccountId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
