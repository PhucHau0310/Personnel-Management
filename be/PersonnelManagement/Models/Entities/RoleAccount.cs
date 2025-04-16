namespace PersonnelManagement.Models.Entities
{
    public class RoleAccount
    {
        public Guid Id { get; set; }
        public required string RoleName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
