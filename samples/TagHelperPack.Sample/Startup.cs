using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if !NET471
using Microsoft.Extensions.Hosting;
#endif
using TagHelperPack.Sample.Services;

namespace TagHelperPack.Sample
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AspNetCoreVersion>();

            // Add framework services.
            services.AddAuthentication("QueryAuth")
                    .AddScheme<AuthenticationSchemeOptions, QueryAuthScheme>("QueryAuth", options => { });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("IsAdmin", "true");
                });
            });

#if !NET471
            services.AddRazorPages();
#else
            services.AddMvc();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#if !NET471
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#else
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#endif
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

#if !NET471
            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapRazorPages();
            });
#else
            app.UseMvc();
#endif
        }
    }
}
