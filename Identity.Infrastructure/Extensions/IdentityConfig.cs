using System.Reflection;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Data.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using IdentityConstants = Microsoft.AspNetCore.Identity.IdentityConstants;

namespace Identity.Infrastructure.Extensions;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentity<AppUser, AppRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                    
                    options.User.RequireUniqueEmail = true;
                    
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                    
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddRoles<AppRole>() // From Pixel
            .AddRoleStore<AppRoleStore>()
            .AddUserStore<AppUserStore>()
            .AddDefaultTokenProviders();

        services
            .Configure<IdentityOptions>(options =>
                {
                    // Configure Identity to use the same JWT claims as OpenIdDict instead
                    // of the legacy WS-Federation claims it uses by default (ClaimTypes),
                    // which saves you from doing the mapping in your authorization controller.
                    // options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Email
                    
                    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
                    options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;

                    // Note: to require account confirmation before login,
                    // register an email sender service (IEmailSender) and
                    // set options.SignIn.RequireConfirmedAccount to true.
                    // For more information, visit https://aka.ms/aspaccountconf.
                    
                    options.SignIn.RequireConfirmedAccount = false;
                });

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme; //IdentityOIdc
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme; //IdentityOIdc

                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme; //Identity Fsh
                
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // IdentityPlus
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // IdentityPlus
            })
            // IdentityOIdc
            // .AddIdentityCookies(options =>
            // {
            //     options?.ApplicationCookie?.Configure(c =>
            //     {
            //         c.LoginPath = "/Login";
            //         //c.
            //     });
            // });
            .AddCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

        services
            .AddAuthorization();
        
        // app.Services
        //     .AddAuthorization(options =>
        //     {
        //         options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        //         options.AddPolicy("UserOnly", policy => policy.RequireRole("Basic"));
        //     });

        return services;
    }
    
}