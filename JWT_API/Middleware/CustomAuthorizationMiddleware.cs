using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace JWT_API.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"No tiene el nivel suficiente para acceder a este recurso.\"}");
            }
        }
    }
}
