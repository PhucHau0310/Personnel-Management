namespace PersonnelManagement.Models.DTOs
{
    public class LoginAuthReq
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
