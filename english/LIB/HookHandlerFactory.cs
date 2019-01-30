using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using CefSharp;
using Newtonsoft.Json;

namespace English
{
    public class HookHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "http";
        public const string Host = "f";

        //static string PATH_ROOT = Path.GetDirectoryName(Application.ExecutablePath);
        static string PATH_ROOT = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            string url = request.Url.Trim().ToLower(), domain = string.Empty;
            if (url.Contains("?")) domain = url.Split('?')[1].Trim();

            Console.WriteLine(url);

            Uri uri = new Uri(url);
            string fileName = uri.AbsolutePath,
                file = fileName.Replace('/', '\\'),
                resource = string.Empty;
            if (file[0] == '\\') file = file.Substring(1);

            string basePath = Path.Combine(PATH_ROOT, "hook\\base");
            switch (file)
            {
                case "w.css":
                    string[] css = Directory.GetFiles(basePath, "*.css").Select(x=> File.ReadAllText(x)).ToArray();
                    resource = string.Join(Environment.NewLine, css);
                    if (!string.IsNullOrWhiteSpace(domain) && File.Exists("hook\\" + domain + ".css"))
                        resource += Environment.NewLine + File.ReadAllText("hook\\" + domain + ".css");
                    return ResourceHandler.FromString(resource, ".css");
                case "w.js":
                    string[] js = Directory.GetFiles(basePath, "*.js").Select(x => File.ReadAllText(x)).ToArray();
                    resource = string.Join(Environment.NewLine, js);
                    if (!string.IsNullOrWhiteSpace(domain) && File.Exists("hook\\" + domain + ".js"))
                        resource += Environment.NewLine + File.ReadAllText("hook\\" + domain + ".js");
                    return ResourceHandler.FromString(resource, ".js"); 

                //default:
                //    file = Path.Combine("hook", file);
                //    if (File.Exists(file))
                //    {
                //        if (url.Contains(".css") || url.EndsWith(".js"))
                //        {
                //            resource = File.ReadAllText(file);
                //            var fileExtension = Path.GetExtension(fileName);
                //            return ResourceHandler.FromString(resource, fileExtension);
                //        }
                //    }
                //    break;
            }

            return null;
        }
    }

}
