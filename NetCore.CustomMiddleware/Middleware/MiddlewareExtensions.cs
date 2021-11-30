using Microsoft.AspNetCore.Builder;

namespace NetCore.CustomMiddleware.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomMiddleware(this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseMiddleware<MyCustomMiddleware>();
        }
    }
}
