namespace PersonnelManagement.Models.Entities
{
    public class Token
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime? UpdatedAt { get; set; }

        // Relationships
        public Account? Account { get; set; }
    }
}
