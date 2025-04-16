namespace PersonnelManagement.Models.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
