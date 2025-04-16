namespace PersonnelManagement.Models.Entities
{
    public enum Status
    {
        CheckIn = 1,
        CheckOut = 2
    }

    public class Personnel
    {
        public string? Id { get; set; }
        public required string FullName { get; set; }
        public required string NumberId { get; set; }
        public required string Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Address { get; set; }
        public string? DateCreatedCccd { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public Guid RolePersonnel { get; set; }
        public Status Status { get; set; } = Status.CheckOut;

        public RolePersonnel? RolePersonnels { get; set; }
        public ICollection<PersonnelHistory> CheckHistory { get; set; } = new List<PersonnelHistory>();
    }
}
