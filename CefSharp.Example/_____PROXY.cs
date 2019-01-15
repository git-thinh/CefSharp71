// Copyright © 2013 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CefSharp.Example.Properties;

namespace CefSharp.Example
{
    public class CefSharpSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public static string SchemeName = "http";

        static StringBuilder _log = new StringBuilder();
        //static string PATH_ROOT = Path.GetDirectoryName(Application.ExecutablePath);
        //static string PATH_ROOT = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        static string PATH_ROOT = File.ReadAllLines("proxy.txt").Select(x => x.Trim()).Where(x => x.Length > 0).ToArray()[0].ToLower();
        static string[] URLS_STUB = File.ReadAllLines("proxy.txt").Select(x => x.Trim().ToLower()).Where((x, index) => index > 0).ToArray();

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            SchemeName = schemeName;
            string url = request.Url.Trim().ToLower();

            if (URLS_STUB.Count(x => x == url) == 0)
            {
                return null;
            }

            Uri uri = new Uri(url);
            string fileName = uri.AbsolutePath, file = fileName.Replace('/', '\\');

            if (file[0] == '\\') file = file.Substring(1);
            file = Path.Combine(PATH_ROOT, file);

            if (File.Exists(file))
            {
                if (url.Contains(".html") || url.Contains(".css") || url.EndsWith(".js"))
                {
                    string resource = File.ReadAllText(file);
                    var fileExtension = Path.GetExtension(fileName);
                    return ResourceHandler.FromString(resource, fileExtension);
                }
            }

            return null;
        }

        public IResourceHandler Create_BACKUP(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            SchemeName = schemeName;
            string url = request.Url;
            //STORE.link_Add(url);

            //if (url.Contains("spa_005_001_ULU002.tmpl.html"))
            //    Thread.Sleep(5000);
            //if (url.Contains("_005_001_ULU002"))
            //    Thread.Sleep(5000);
            //if (url.Contains("AppReqGetBoxInfoDetails"))
            //    Thread.Sleep(5000);
            //if (url.Contains("lang_err_En.json"))
            //    Thread.Sleep(5000);


            if (request.Method == "POST")
            {
                ////////IDictionary<string, string> dict = new Dictionary<string, string>();
                ////////foreach (var k in request.Headers.AllKeys)
                ////////    dict.Add(k, request.Headers[k]);
                ////////string headers = Newtonsoft.Json.JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
                ////////_log.Append(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                ////////_log.Append(url);
                ////////_log.Append(Environment.NewLine);
                ////////_log.Append(headers);
                ////////_log.Append(Environment.NewLine);

                ////////if (request.PostData != null)
                ////////{
                ////////    if (url == "http://192.168.56.102:30000/wcd/api/AppReqSetCustomMessage/_013_000_DIP000") {
                ////////        //IFrame frame = browser.GetMainFrame();
                ////////        IRequest request2 = frame.CreateRequest();

                ////////        request2.Url = "http://192.168.56.102:30000/wcd/user.cgi";
                ////////        request2.Method = "POST";

                ////////        request2.InitializePostData();
                ////////        var element = request2.PostData.CreatePostDataElement();
                ////////        element.Bytes = request.PostData.Elements[0].Bytes;
                ////////        //element.Type = request.PostData.Elements[0].Type;"X-Requested-With": "XMLHttpRequest"
                ////////        request2.PostData.AddElement(element);

                ////////        NameValueCollection headers2 = new NameValueCollection();
                ////////        headers2.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                ////////        headers2.Add("Origin", "http://192.168.56.102");
                ////////        headers2.Add("Upgrade-Insecure-Requests", "1");
                ////////        headers2.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.79 Safari/537.36");
                ////////        headers2.Add("X-Requested-With", "XMLHttpRequest");
                ////////        request2.Headers = headers2;
                ////////        //request2.Headers = request.Headers;

                ////////        frame.LoadRequest(request2);
                ////////        return null;
                ////////    }


                ////////    if (url.Contains("i_file"))
                ////////    {
                ////////        string s0 = request.PostData.Elements[0].GetBody();
                ////////        string[] a = s0.Split(new string[] { "\r\n\r\non\r\n" }, StringSplitOptions.None);
                ////////        string s2 = a[0] + "\r\n\r\n1\r\n" + a[0] + "\r\n\r\non\r\n" + a[1];
                ////////        request.PostData.Elements[0].Bytes = System.Text.Encoding.UTF8.GetBytes(s2);
                ////////    }

                ////////    int count = request.PostData.Elements.Count;
                ////////    for (int i = 0; i < count; i++)
                ////////    {
                ////////        var po = request.PostData.Elements[i];
                ////////        _log.Append(Environment.NewLine);
                ////////        if (po.Type == PostDataElementType.Bytes)
                ////////            _log.Append(po.GetBody());
                ////////        else if (po.Type == PostDataElementType.File)
                ////////            _log.Append("->SEN_FILE: " + po.File);
                ////////        else if (po.Type == PostDataElementType.Empty)
                ////////            _log.Append("->SEN_EMPTY: " + po.GetBody());
                ////////    }
                ////////}

                ////////if (url.Contains("i_file"))
                ////////{
                ////////    //string s0 = request.PostData.Elements[0].GetBody();
                ////////    //string[] a = s0.Split(new string[] { "\r\n\r\non\r\n" }, StringSplitOptions.None);
                ////////    //string s2 = a[0] + "\r\n\r\n9\r\n" + a[0] + "\r\n\r\non\r\n" + a[1];
                ////////    //request.PostData.Elements[0].Bytes = ASCIIEncoding.ASCII.GetBytes(s2);

                ////////    //byte[] postDataBytes = ASCIIEncoding.UTF8.GetBytes(s2);
                ////////    //var element = request.PostData.CreatePostDataElement();
                ////////    //element.Bytes = postDataBytes;
                ////////    //request.PostData.AddElement(element);

                ////////    string s = _log.ToString();
                ////////    File.WriteAllText(@"C:\log_cgi.txt", s);
                ////////    _log.Clear();
                ////////}
                return null;
            }

            //if (url.Contains("v_print.xml"))
            //{
            //    var uri = new Uri(url);
            //    var fileName = uri.AbsolutePath;
            //    string file = fileName.Replace('/', '\\');
            //    if (file[0] == '\\') file = file.Substring(1);
            //    file = Path.Combine(PATH_ROOT, file);

            //    if (File.Exists(file))
            //    {
            //        var fileExtension = Path.GetExtension(fileName);
            //        return ResourceHandler.FromFilePath(file, "text/xml");
            //    }
            //    return null;
            //}

            //if (url.Contains("lang_err_Ja.json") || url.Contains("lang_005_Ja.json"))
            //{
            //    var uri = new Uri(url);
            //    var fileName = uri.AbsolutePath;
            //    string file = fileName.Replace('/', '\\');
            //    if (file[0] == '\\') file = file.Substring(1);
            //    file = Path.Combine(PATH_ROOT, file);

            //    if (File.Exists(file))
            //    {
            //        var fileExtension = Path.GetExtension(fileName);
            //        return ResourceHandler.FromFilePath(file, "text/plain;charset=utf-8");
            //    }
            //    return null;
            //}

            if (url.Contains("/api/") || url.Contains(".json") || url.Contains(".xml")) return null;

            //if (url.Contains(".gif"))
            //{
            //    var uri = new Uri(url);
            //    var fileName = uri.AbsolutePath; 
            //    string file = fileName.Replace('/', '\\');
            //    if (file[0] == '\\') file = file.Substring(1);
            //    file = Path.Combine(PATH_ROOT, file);

            //    if (File.Exists(file))
            //    {
            //        var fileExtension = Path.GetExtension(fileName);
            //        return ResourceHandler.FromFilePath(file, "image/gif");
            //    }
            //    return null;
            //}


            //if (url.Contains(".xml") || url.Contains(".xsl") || url.Contains("inspectcheck_data.txt"))
            //{
            //    var uri = new Uri(url);
            //    var fileName = uri.AbsolutePath;

            //    //Debug.WriteLine("----> " + url);
            //    //if (url.Contains(".html") || url.Contains(".txt") || url.Contains(".js") || url.Contains(".css"))

            //    string resource;
            //    string file = fileName.Replace('/', '\\');
            //    if (file[0] == '\\') file = file.Substring(1);
            //    //file = Path.Combine(PATH_ROOT, file);
            //    //file = Path.Combine(@"C:\Projects\WebUI\dev\source\Phase8\Sprint#4\proxy_winx86_xsl", file);

            //    if (File.Exists(file))
            //    {
            //        resource = File.ReadAllText(file);
            //        var fileExtension = Path.GetExtension(fileName);
            //        return ResourceHandler.FromString(resource, fileExtension);
            //    }
            //    return null;
            //}

            if (url.Contains(".html") || url.Contains(".css") || url.EndsWith(".js"))
            {
                var uri = new Uri(url);
                var fileName = uri.AbsolutePath;

                //Debug.WriteLine("----> " + url);
                //if (url.Contains(".html") || url.Contains(".txt") || url.Contains(".js") || url.Contains(".css"))

                string resource;
                string file = fileName.Replace('/', '\\');
                if (file[0] == '\\') file = file.Substring(1);
                file = Path.Combine(PATH_ROOT, file);

                if (File.Exists(file))
                {
                    resource = File.ReadAllText(file);
                    var fileExtension = Path.GetExtension(fileName);


                    return ResourceHandler.FromString(resource, fileExtension);
                }
            }

            return null;
        }
    }
}