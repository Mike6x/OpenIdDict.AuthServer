using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Quartz;
using Framework.Core.Options;
using Identity.Domain.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Framework.Core.Auth.OpenIdDict;

public static class OpenIdDictConfig
{
    public static IServiceCollection AddAuthValidation(this IServiceCollection services, IConfiguration config)
    {
        
        var authOptions = services.BindValidateReturn<OpenIdDictOptions>(config);

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer(authOptions.IssuerUrl!);
                options.UseIntrospection()
                       .SetClientId(authOptions.ClientId!)
                       .SetClientSecret(authOptions.ClientSecret!);
                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();
        return services;
    }

    public static void ConfigureOpenIdDict<T>(this WebApplicationBuilder builder, Assembly dbContextAssembly, string connectionName = "DefaultConnection") 
        where T : DbContext
    {
        builder.Services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });
        
        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    
        builder.Services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<T>();
                options.UseQuartz();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetIntrospectionEndpointUris("/connect/introspect")
                       .SetEndSessionEndpointUris("/connect/endsession")
                       .SetUserInfoEndpointUris("connect/userinfo")
                       .SetTokenEndpointUris("/connect/token")
                       .SetDeviceAuthorizationEndpointUris("/connect/deviceauthorization")
                       .SetRevocationEndpointUris("/connect/revoke")
                       .SetJsonWebKeySetEndpointUris("/.well-known/jwks.json")
                       .SetEndUserVerificationEndpointUris("/connect/enduserverification");
                
                //when integration with third-party APIs/resource servers is desired
                options.DisableAccessTokenEncryption();
                
                options
                    .SetAccessTokenLifetime(TimeSpan.FromSeconds(3600))
                    .SetRefreshTokenLifetime(TimeSpan.FromSeconds(86400));
                
                //allowed grant types
                options
                    .AllowAuthorizationCodeFlow()
                    //.RequireProofKeyForCodeExchange()
                    .AllowClientCredentialsFlow() // For Machine-to-Machine Authentication
                    .AllowPasswordFlow() // For Resource Owner Password Credentials Flow - Net8
                    .AllowHybridFlow()
                    .AllowRefreshTokenFlow()
                    .AllowDeviceAuthorizationFlow();

                
                //PKCE
                options.RequireProofKeyForCodeExchange();
                
                // var scopes = new List<string>
                // {
                //     Permissions.Scopes.Email,
                //     Permissions.Scopes.Profile,
                //     Permissions.Scopes.Roles,
                // }.Select(s => s.Replace(Permissions.Prefixes.Scope, ""));
                // options.RegisterScopes(scopes.ToArray());
                options.RegisterScopes(
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles,
                    Scopes.OfflineAccess, // Net 8
                    "api"
                );
                
                if (builder.Environment.IsDevelopment())
                {
                    options
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                }
                
                # region Check PKCE key
                
                var openIdDictSettings = builder.Configuration.GetSection("OpenIdDict").Get<OpenIdDictSettingsConfig>();

                if(!string.IsNullOrEmpty(openIdDictSettings?.Encryption?.Key))
                {
                    options.AddEncryptionKey(
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(openIdDictSettings.Encryption.Key ?? ""))
                            );
                } else if (openIdDictSettings?.Encryption?.Cert != null)
                {
                    var path = openIdDictSettings.Encryption?.Cert?.Path ?? "./cert.pfx";

                    if (openIdDictSettings?.Encryption?.Cert?.GenerateIfEmpty == true)
                        GenerateCertificate(path, openIdDictSettings?.Encryption?.Cert, CertificateType.Encryption);

                    if (!File.Exists(openIdDictSettings?.Encryption?.Cert?.Path))
                    {
                        throw new FileNotFoundException($"Certificate not found at {path}");
                    }

                    var cert = X509CertificateLoader.LoadPkcs12FromFile(path, openIdDictSettings.Signing.Cert.Password);
                    options.AddEncryptionCertificate(cert);
                }

                if (!string.IsNullOrEmpty(openIdDictSettings?.Signing?.Key))
                {
                    options.AddSigningKey(
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(openIdDictSettings.Signing.Key ?? ""))
                            );
                }
                else if (openIdDictSettings?.Signing?.Cert != null)
                {
                    var path = openIdDictSettings.Signing?.Cert?.Path ?? "./cert.pfx";

                    if (openIdDictSettings?.Signing?.Cert?.GenerateIfEmpty == true)
                        GenerateCertificate(path, openIdDictSettings?.Signing?.Cert, CertificateType.Signing);
                    if (!File.Exists(openIdDictSettings?.Signing?.Cert?.Path))
                    {
                        throw new FileNotFoundException($"Certificate not found at {path}");
                    }
                    var cert = X509CertificateLoader.LoadPkcs12FromFile(path, openIdDictSettings.Signing.Cert.Password);
                    options.AddSigningCertificate(cert);
                }
                
                #endregion
                
                var aspBuilder = 
                    options.UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough()
                        .DisableTransportSecurityRequirement();
                
                if(openIdDictSettings?.OnlyAllowHttps != true)
                {
                    aspBuilder.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
            //.AddValidation(options =>
            //{
            //    // Note: the validation handler uses OpenID Connect discovery
            //    // to retrieve the address of the introspection endpoint.
            //    //options.SetClientId(openIddictSettings.IdentityClientId);

            //    // Import the configuration from the local OpenIddict server instance.
            //    options.UseLocalServer();

            //    // Register the System.Net.Http. integration
            //    options.UseSystemNetHttp();

            //    // Register the ASP.NET Framework.Core host.
            //    options.UseAspNetCore();
            //});

        // Move to IdentityConfig
        // builder.Services
        //     .AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        // builder.Services
        //     .AddAuthorization();

        #region Register Database
        // string? connectionString = builder.Configuration.GetConnectionString(connectionName);
        // if (!builder.Environment.IsDevelopment() && connectionString == null)
        //     throw new ArgumentNullException(nameof(connectionString));
        //
        // builder.Services.AddDbContext<T>(options =>
        // {
        //     if (builder.Environment.IsDevelopment())
        //     {
        //         options.UseInMemoryDatabase("authDb");
        //     }
        //     else
        //     {
        //         options.UseNpgsql(connectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
        //     }
        //     
        //     options.UseNpgsql(connectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
        //     options.UseOpenIddict();
        // });
        #endregion
    }
    
    public static void ConfigureOpenIdDictDbContext<T>(this WebApplicationBuilder builder, Assembly dbContextAssembly, string connectionName = "DefaultConnection") 
        where T : DbContext
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionName) 
                               ?? throw new InvalidOperationException($"Connection string {connectionName} not found.");

        builder.Services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(connectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
            options.UseOpenIddict();
        });
    }
    

    private static void GenerateCertificate(string path, CertConfig? cert, CertificateType type)
    {
        if(File.Exists(path))
        {
            //check validity

            var certLoaded = X509CertificateLoader.LoadPkcs12FromFile(path, cert?.Password);
            if (certLoaded.NotAfter.AddDays(-5) > DateTimeOffset.UtcNow)
            {
                var days = (certLoaded.NotAfter - DateTimeOffset.UtcNow).Days;
                Console.WriteLine($"Certificate at {path} is still valid for {days} days");
                return;
            }
            Console.WriteLine($"Certificate at {path} is expired, generating new one");
            if(File.Exists(path+ ".bak"))
            {
                File.Delete(path + ".bak");
            }
            File.Move(path, path + ".bak");
        }

        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = new X500DistinguishedName($"CN={cert?.Issuer ?? "OpenIddictSelfSigned"}");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        switch(type)
        {
            case CertificateType.Encryption:
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));
                break;
            case CertificateType.Signing:
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));
                break;
        }



        var validityInMonths = cert?.ValidityMonths ?? 1;
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMonths(validityInMonths).AddDays(1));

        File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, cert?.Password));
    }

    private enum CertificateType
    {
        Encryption,
        Signing
    }
    
        
    private const string AllowAllOrigins = "AllowAll";
    public static IApplicationBuilder UseOpenIdDict(this WebApplication app)
    {
        app.UseCors(AllowAllOrigins);
        return app;
    }


}
