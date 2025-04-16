using PersonnelManagement.Models.Entities;

namespace PersonnelManagement.Models.DTOs
{
    public class UpdatePersonnel
    {
        public string? FullName { get; set; }
        public string? NumberId { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? DateCreatedCccd { get; set; }
        public int Status { get; set; }
        public Guid rolePersonnelId { get; set; }
    }
}
