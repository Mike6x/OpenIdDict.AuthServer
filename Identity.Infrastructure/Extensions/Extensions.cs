// using BuildingBlocks.Auth.OpenIdDict;
// using Identity.Domain.Models;
// using Identity.Infrastructure.Persistence;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.DependencyInjection;
// using OpenIddict.Abstractions;
//
// namespace Identity.Infrastructure.Extensions;
//
//
// public static class Extensions
// {
//     public static WebApplicationBuilder AddIdentityInfraServices(this WebApplicationBuilder builder)
//     {
//         ArgumentNullException.ThrowIfNull(builder);
//
//         builder.Services.AddIdentityExtensions();
//                 
//         var dbContextAssembly = typeof(AppDbContext).Assembly;
//         builder.ConfigureOpenIdDict<AppDbContext>(dbContextAssembly);
//         builder.Services.AddHostedService<SeedClientsAndScopes>();
//         
//         return builder;
//     }
//
//     #region Move to Identity Config
//     
//     private static IServiceCollection AddIdentityExtensions(this IServiceCollection services)
//     {
//         services
//             .AddIdentity<AppUser, AppRole>(options =>
//                 {
//                     options.SignIn.RequireConfirmedAccount = true;
//                     options.User.RequireUniqueEmail = true;
//                     options.SignIn.RequireConfirmedEmail = false;
//                     options.SignIn.RequireConfirmedPhoneNumber = false;
//                 
//                     options.Password.RequireDigit = true;
//                     options.Password.RequireLowercase = true;
//                     options.Password.RequireNonAlphanumeric = false;
//                     options.Password.RequireUppercase = true;
//                     options.Password.RequiredLength = 6;
//                     options.Password.RequiredUniqueChars = 1;
//                     
//                     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
//                     options.Lockout.MaxFailedAccessAttempts = 5;
//                 })
//             .AddEntityFrameworkStores<AppDbContext>()
//             .AddDefaultTokenProviders();
//
//         services.Configure<IdentityOptions>(options =>
//         {
//             // Configure Identity to use the same JWT claims as OpenIdDict instead
//             // of the legacy WS-Federation claims it uses by default (ClaimTypes),
//             // which saves you from doing the mapping in your authorization controller.
//             // options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Email;
//             options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
//             options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
//             options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
//             options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
//
//             // Note: to require account confirmation before login,
//             // register an email sender service (IEmailSender) and
//             // set options.SignIn.RequireConfirmedAccount to true.
//             //
//             // For more information, visit https://aka.ms/aspaccountconf.
//             options.SignIn.RequireConfirmedAccount = false;
//         });
//
//         return services;
//     }
//     
//     #endregion 
// }
