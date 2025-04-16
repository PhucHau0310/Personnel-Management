namespace PersonnelManagement.Models.DTOs
{
    public class CreateRoleAccountDto
    {
        public string RoleName { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }
}
