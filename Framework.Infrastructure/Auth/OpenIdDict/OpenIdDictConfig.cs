using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Framework.Core.Exceptions;
using Framework.Core.Persistence;
using Framework.Infrastructure.Options;
using Framework.Infrastructure.Persistence;
using Hangfire.Server;
using Identity.Domain.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Framework.Infrastructure.Auth.OpenIdDict;

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

    public static IServiceCollection AddOpenIdDictConfig<T>(this IServiceCollection services, IConfiguration configuration)
        where T : DbContext
    {
        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });
        
        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    
        services
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
                
                options.RegisterScopes(
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles,
                    Scopes.OfflineAccess, // Net 8
                    "api"
                );
                
                // only dev
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
                
                
                # region Check PKCE key
                
                var openIdDictSettings = configuration.GetSection("OpenIdDict").Get<OpenIdDictSettingsConfig>();

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
                // RegisterUser the System.Net.Http. integration
                options.UseSystemNetHttp();
            });

        return services;
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
    
}
