namespace PersonnelManagement.Security
{
    public static class Endpoints
    {
        public static readonly string[] PublicEndpoints =
        {
           // Auth Controller
            "/api/auth/login",
            "/api/auth/refresh-token",
            "/api/account/forgot-password",
            "/api/account/reset-password",
            "/api/account/verify-code",
            "/api/personnel/check-authen-image",
            "/api/personnel/check-in-out",
        };
    }
}
