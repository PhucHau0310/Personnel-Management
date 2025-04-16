namespace PersonnelManagement.Models.DTOs
{
    public class RoleAccountDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; } // Danh sách tên quyền
    }
}
