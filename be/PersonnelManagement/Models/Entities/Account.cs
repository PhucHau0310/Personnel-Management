using System.Data;

namespace PersonnelManagement.Models.Entities
{

    public class Account
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public Guid RoleAccountId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<Token> RefreshTokens { get; set; } = new List<Token>();
        public RoleAccount? RoleAccount { get; set; }
    }
}
