namespace PersonnelManagement.Models.Entities
{
    public class RolePersonnel
    {
        public Guid Id { get; set; }
        public required string RoleName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<Personnel> Personnels { get; set; } = new List<Personnel>();
    }
}
