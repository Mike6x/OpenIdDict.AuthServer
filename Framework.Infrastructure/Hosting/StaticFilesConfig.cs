using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Framework.Core.Hosting;

public static class StaticFilesConfig
{

    public static WebApplication UseVueStaticFiles(this WebApplication app)
    {
        var cachePeriod = app.Environment.IsDevelopment() ? "600" : "604900";
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = new List<string> { "index.html" },
        });

        //app.MapStaticAssets(); //todo
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                if (ctx.File.Name.EndsWith(".html"))
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
                    ctx.Context.Response.Headers.Append("Expires", "-1");
                }
                else
                {
                    ctx.Context.Response.Headers.Append(
                        "Cache-Control", $"public, max-age={cachePeriod}"
                    );
                }
            }
        });

        return app;
    }

    public static WebApplication UseVueFallbackSpa(this WebApplication app)
    {
        //app.MapStaticAssets();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseWhen(
            context => context.GetEndpoint() == null
            && !context.Request.Path.StartsWithSegments("/api")
            && !context.Request.Path.StartsWithSegments("/.well-known"),
            then => then.UseSpa(spa =>
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:5173/");


                //spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                //{
                //    OnPrepareResponse = ctx =>
                //        {
                //            var headers = ctx.Context.Response.GetTypedHeaders();
                //            headers.CacheControl = new CacheControlHeaderValue
                //            {
                //                NoCache = true,
                //                NoStore = true,
                //                MustRevalidate = true,
                //                MaxAge = TimeSpan.Zero
                //            };
                //        }
                //};
            }));
        }
        else
        {
            app.MapFallbackToFile("index.html");
            //app.UseFileServer(fs => fs);

        }

        return app;
    }

}
