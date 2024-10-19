using Entity;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

namespace AIIL.Services.Api.Middlewares
{
    public class LockoutCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public LockoutCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Lấy UserManager từ IServiceProvider trong HttpContext
                    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null && await userManager.IsLockedOutAsync(user))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync("Your account is locked. Please contact support.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
