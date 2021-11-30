using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace NetCore.CustomMiddleware.Middleware
{
    public class MyCustomMiddleware
    {
        private RequestDelegate next;
        private IOptions<MyCustomMiddlewareOptions> options;

        //Use configuration options
        //Pass information to another middleware component -> HttpContext.Items <object, object> dictionary
        public MyCustomMiddleware(RequestDelegate next, IOptions<MyCustomMiddlewareOptions> options)
        {
            this.next = next;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["message"] = "The weather is great today!";
            await context.Response.WriteAsync($"==MyCustomMiddleware is handling the request course name: { options.Value.CourseName}==");
            await this.next.Invoke(context);
        }
    }
}
