using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCore.CustomMiddleware.Middleware;

namespace NetCore.CustomMiddleware
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);

            this.Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Pass options to the middleware
            services.AddOptions();
            var myCustomMiddlewareOptions = this.Configuration.GetSection("MyCustomMiddleware");
            services.Configure<MyCustomMiddlewareOptions>(myCustomMiddlewareOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});


            //Options: Use, Run, Map and MapWhen


            //The order is determined by the sequence in which the components are added - top to bottom
            //The components are visited again during the return jorney

            //Call the next component in the middleware
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("==Hello from the component one - andata=="); //Called on going ahead

                await next.Invoke(); //Invoke the next

                //??? Not recommended once the response has started to write again to the response ???
                await context.Response.WriteAsync("==Hello from the component one - ritorno=="); //Called again on the return jorney

                //If we do not invoke the next piece in the chain, it will terminate the pipeline and short-circuit the pipeline
            });



            //Branch the middleware - inside we can add as many middleware components as we wish
            //If the branch is selected, subsequent elements outside the Map branch statement will not be executed
            //!!! INSIDE USE THE appBuilder PASSED THROUGH THE DELEGATE AND NOT THE APP FROM OUTSIDE
            app.Map("/branch1", appBuilder =>
            {
                //Here we can add as many middleware components as we wish

                //We can even create additional branches within this one

                appBuilder.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Greetings from my middleware branch1");
                });
            });


            //MapWhen
            //!!! INSIDE USE THE appBuilder PASSED THROUGH THE DELEGATE AND NOT THE APP FROM OUTSIDE
            app.MapWhen(context => context.Request.Query.ContainsKey("querybranch"), appBuilder =>
            {
                appBuilder.Run(async (context) =>
                {
                    await context.Response.WriteAsync("You have arrived at your querybranch MapWhen");
                });
            });


            //Add custom middleware
            app.UseMyCustomMiddleware();

            //Run method terminates the pipeline (last one) to the pipeline, it generates a response and returns
            app.Run(async (context) =>
            {
                string msg = context.Items["message"].ToString();
                await context.Response.WriteAsync("==Hello World!==");
                await context.Response.WriteAsync($"Custom middleware msg {msg}");
            });
        }
    }
}
