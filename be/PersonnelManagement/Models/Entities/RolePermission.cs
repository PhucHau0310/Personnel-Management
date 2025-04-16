namespace PersonnelManagement.Models.Entities
{
    public class RolePermission
    {
        public Guid Id { get; set; }
        public Guid RoleAccountId { get; set; }
        public Guid PermissionId { get; set; }
        public RoleAccount? RoleAccount { get; set; }
        public Permission? Permission { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
