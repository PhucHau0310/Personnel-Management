namespace PersonnelManagement.Models.DTOs
{
    public class UpdateRoleAccountDto
    {
        public string RoleName { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }
}
