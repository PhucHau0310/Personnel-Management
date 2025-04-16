namespace PersonnelManagement.Models.Entities
{
    public class PersonnelHistory
    {
        public Guid Id { get; set; }
        public required string PersonnelId { get; set; }
        public Status Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow.AddHours(7);
        public string? Note { get; set; }

        public Personnel? Personnel { get; set; }
    }
}
