using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Framework.Infrastructure.Hosting;

public static class HostingConfig
{
    public static IApplicationBuilder UseUrlsFromConfig(this WebApplication app)
    {
        var httpsRedirection = app.Configuration.GetSection("Hosting:HttpsRedirection")?.Get<bool>() != false;
        if (httpsRedirection)
            app.UseHttpsRedirection();

        var urls = app.Configuration.GetSection("Hosting:Urls")?.Get<string[]>();
        if (urls != null && urls.Any())
        {
            app.Urls.Clear();
            foreach (var url in urls)
            {
                app.Urls.Add(url);
            }
        }
        return app;
    }
    
    public static IHostApplicationBuilder ConfigureReverseProxySupport(this IHostApplicationBuilder app)
    {

        var rProxySupport = app.Configuration.GetSection("Hosting:ReverseProxySupport")?.Get<bool>() != false;
        if(rProxySupport)
            app.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });

        return app;
    }

    public static WebApplication UseReverseProxySupport(this WebApplication app)
    {
        var rProxySupport = app.Configuration.GetSection("Hosting:ReverseProxySupport")?.Get<bool>() != false;

        if(rProxySupport)
        {
            app.UseCertificateForwarding();
            app.UseForwardedHeaders();
        }

        var assumeEveryRequestHttps = app.Configuration.GetSection("Hosting:AssumeEveryRequestHttps")?.Get<bool>() == true;
        if (assumeEveryRequestHttps)
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";

                return next();
            });

        return app;
    }

}