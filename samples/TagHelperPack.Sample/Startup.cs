using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NET6_0_OR_GREATER
using Microsoft.Extensions.Hosting;
#endif
using TagHelperPack.Sample.Services;

namespace TagHelperPack.Sample;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

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
                policy.RequireAssertion(handler =>
                {
                    var httpContext = handler.Resource switch
                    {
                        ViewContext vc => vc.HttpContext, // AuthzTagHelper
                        HttpContext hc => hc, // AuthorizationMiddleware
                        _ => throw new InvalidOperationException("Could not resolve HttpContext"),
                    };

                    return httpContext is not null;
                });
            });
            options.AddPolicy("PermissionPolicy", policy =>
            {
                List<string> standardPermissions = new List<string>() { "ViewUsers"};
                List<string> adminPermissions = new List<string>(standardPermissions) { "ManageUsers" };

                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(handler =>
                {
                    var permission = handler.Resource as string;
                    if (handler.User.IsInRole("admin"))
                    {
                        return adminPermissions.Contains(permission);
                    }
                    else //standard role
                    {
                        return standardPermissions.Contains(permission);
                    }
                });
            });
        });

        // Optional optimizations to avoid Reflection
        services.AddTagHelperPack();

#if NET6_0_OR_GREATER
        services.AddRazorPages();
#else
        services.AddMvc();
#endif
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#if NET6_0_OR_GREATER
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

#if NET6_0_OR_GREATER
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(routes =>
        {
            routes.MapRazorPages();
        });
#else
        app.UseMvc();
#endif
    }
}
