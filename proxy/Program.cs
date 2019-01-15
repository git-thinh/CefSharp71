// Copyright © 2010 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.Example;
using CefSharp.Example.Handlers;
using CefSharp.WinForms.Example.Handlers;
//using CefSharp.WinForms.Example.Minimal;

namespace CefSharp.WinForms.Example
{
    public class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            //if (!File.Exists("url.txt") || !File.Exists("proxy.txt"))
            //{
            //    MessageBox.Show("Please config files: url.txt, proxy.txt ");
            //    return 0;
            //}

            //if (string.IsNullOrWhiteSpace(File.ReadAllText("url.txt")) || string.IsNullOrWhiteSpace(File.ReadAllText("proxy.txt")))
            //{
            //    MessageBox.Show("Please input files: url.txt, proxy.txt. They must not be empty.");
            //    return 0;
            //}
            
            //if (!Directory.Exists(File.ReadAllLines("proxy.txt").Select(x=>x.Trim()).Where(x=>x.Length > 0).ToArray()[0]))
            //{
            //    MessageBox.Show("[proxy.txt] Cannot path: " + File.ReadAllLines("proxy.txt").Select(x => x.Trim()).Where(x => x.Length > 0).ToArray()[0]);
            //    return 0;
            //}

            Cef.EnableHighDPISupport();

            var browser = new BrowserForm(true);
            IBrowserProcessHandler browserProcessHandler = null;
            browserProcessHandler = new BrowserProcessHandler();

            var settings = new CefSettings();
            //settings.MultiThreadedMessageLoop = true;
            settings.ExternalMessagePump = false;

            //////// http://xxx.com/..
            //////string[] a = File.ReadAllText("url.txt").ToLower().Trim().Split('/');

            //////settings.RegisterScheme(new CefCustomScheme
            //////{
            //////    SchemeName = a[0].Substring(0, a[0].Length - 1),
            //////    DomainName = a[2].Split(':')[0],
            //////    SchemeHandlerFactory = new CefSharpSchemeHandlerFactory()
            //////});

            CefExample.Init(settings, browserProcessHandler: browserProcessHandler);

            Application.Run(browser);

            return 0;
        }
    }
}
