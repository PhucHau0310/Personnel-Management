using Microsoft.EntityFrameworkCore;
using PersonnelManagement.Data;
using PersonnelManagement.Models.Entities;

namespace PersonnelManagement.Security
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleMiddleware> _logger;

        public RoleMiddleware(RequestDelegate next, ILogger<RoleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            if (context.Request.Path == "/favicon.ico" || context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value?.ToLower();
            _logger.LogInformation($"Processing request for path: {path}");

            // Check if it's a public endpoint
            if (Endpoints.PublicEndpoints.Any(e => path.StartsWith(e)))
            {
                _logger.LogInformation($"Allowing access to public endpoint: {path}");
                await _next(context);
                return;
            }

            // Skip auth check for authentication endpoints
            // !context.User.Identity?.IsAuthenticated ?? true
            if (!context.User.Identity.IsAuthenticated)
            {
                _logger.LogWarning($"Unauthorized access attempt to: {path}");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            using (var scope = scopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userIdClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized - Invalid user claims");
                    return;
                }

                var userId = Guid.Parse(userIdClaim);
                var user = await _dbContext.Accounts
                    .Include(a => a.RoleAccount)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(a => a.Id == userId);

                if (user == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized - User not found");
                    return;
                }

                var requiredPermission = DetermineRequiredPermission(context);
                if (!string.IsNullOrEmpty(requiredPermission))
                {
                    bool hasPermission = HasPermission(user, requiredPermission);
                    if (!hasPermission)
                    {
                        _logger.LogWarning($"Forbidden - Missing required permission: {requiredPermission} for user: {userId}");
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync($"Forbidden - Missing required permission: {requiredPermission}");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private string DetermineRequiredPermission(HttpContext context)
        {
            // Lấy thông tin từ endpoint metadata (cần tích hợp với RequirePermissionAttribute)
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var permissionAttribute = endpoint.Metadata.GetMetadata<RequirePermissionAttribute>();
                return permissionAttribute?.PermissionCode;
            }
            return null;
        }

        private bool HasPermission(Account account, string permissionCode)
        {
            return account.RoleAccount?.RolePermissions
                .Any(rp => rp.Permission.Code == permissionCode) ?? false;
        }
    }

    // Đảm bảo attribute được định nghĩa chính xác
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute
    {
        public string PermissionCode { get; }

        public RequirePermissionAttribute(string permissionCode)
        {
            PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
        }
    }
}
