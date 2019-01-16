using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace English
{
    public class ProxyWebServer
    {
        public static string PATH_ROOT = @"./";
        static AutoResetEvent _TERMINAL = new AutoResetEvent(false);

        public static void Start(string baseAddress = "http://+:9000/", string pathRoot = "./")
        {
            PATH_ROOT = pathRoot;

            Task.Factory.StartNew(() =>
            {
                WebApp.Start<Startup>(new StartOptions(baseAddress)
                {
                    ServerFactory = "Microsoft.Owin.Host.HttpListener"
                });
                _TERMINAL.WaitOne();
            });
        }

        public static void Stop()
        {
            _TERMINAL.Set();
        }
    }


    class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);

            //var physicalFileSystem = new PhysicalFileSystem(@"./www");
            var physicalFileSystem = new PhysicalFileSystem(ProxyWebServer.PATH_ROOT);
            var options = new FileServerOptions
            {
                RequestPath = new PathString(""),
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html",
                "spaHome.html",
                "spa.html"
            };
            app.UseFileServer(options);
        }
    }

}
