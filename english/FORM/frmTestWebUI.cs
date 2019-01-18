using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace English
{
    public class frmTestWebUI : frmBase
    {
        //public IWinFormsWebBrowser Browser { get; private set; }
        public ChromiumWebBrowser Browser { get; private set; }
        const string URL = "http://localhost:321/spaHome.html";

        public frmTestWebUI()
        {
            this.Text = "";

            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);

            this.Shown += (se, ev) =>
            {
                this.Top = 0;
                this.Left = 0;
                this.Width = 1030;// Screen.PrimaryScreen.WorkingArea.Width / 2;
                this.Height = 610;// Screen.PrimaryScreen.WorkingArea.Height;
                BackColor = Color.Black;
            };

            var browser = new ChromiumWebBrowser(URL)
            {
                Location = new System.Drawing.Point(3, 7),
                Width = 1024,
                Height = 600,
                Dock = DockStyle.None,
                BrowserSettings =
                {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled,
                    WebSecurity = CefState.Disabled,
                    FileAccessFromFileUrls = CefState.Enabled,
                    UniversalAccessFromFileUrls = CefState.Enabled
                }
            };
            //browser.MenuHandler = new MenuHandler(this);
            browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            browser.IsBrowserInitializedChanged += (se, ev) =>
            {
                browser.ShowDevTools();
            };
            this.Controls.Add(browser);
            this.Browser = browser;

            ////Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
            //browser.LoadingStateChanged += (sender, args) =>
            //{
            //    //Wait for the Page to finish loading
            //    if (args.IsLoading == false)
            //    {
            //        //args.Browser.GetHost().ExecuteJavaScriptAsync("alert('All Resources Have Loaded');");
            //    }


            //};

            ////Wait for the MainFrame to finish loading
            //browser.FrameLoadEnd += (sender, args) =>
            //{
            //    //Wait for the MainFrame to finish loading
            //    if (args.Frame.IsMain)
            //    {
            //        args.Frame.ExecuteJavaScriptAsync("console.log('?????????????????: MainFrame finished loading');");
            //    }
            //};


        }

        ~frmTestWebUI()
        {
            this.Browser.Dispose();
        }
    }


    /// <summary>
    /// A class that is used to demonstrate how asynchronous javascript events can be returned to the .Net runtime environment.
    /// </summary>
    /// <seealso cref="ScriptedMethods"/>
    /// <seealso cref="resources/ScriptedMethodsTest.html"/>
    public class ScriptedMethodsBoundObject
    {
        /// <summary>
        /// Raised when a Javascript event arrives.
        /// </summary>
        public event Action<string, object> EventArrived;

        /// <summary>
        /// This method will be exposed to the Javascript environment. It is
        /// invoked in the Javascript environment when some event of interest
        /// happens.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="eventData">Data provided by the invoker pertaining to the event.</param>
        /// <remarks>
        /// By default RaiseEvent will be translated to raiseEvent as a javascript function.
        /// This is configurable when calling RegisterJsObject by setting camelCaseJavascriptNames;
        /// </remarks>
        public void RaiseEvent(string eventName, object eventData = null)
        {
            if (EventArrived != null)
            {
                EventArrived(eventName, eventData);
            }
        }
    }

    public class RenderProcessMessageHandler : IRenderProcessMessageHandler
    {
        public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame) { }
        public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node) { }
        public void OnUncaughtException(IWebBrowser browserControl, IBrowser browser, IFrame frame, JavascriptException exception) { }

        // Wait for the underlying JavaScript Context to be created. This is only called for the main frame.
        // If the page has no JavaScript, no context will be created.
        void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            const string script = "document.addEventListener('DOMContentLoaded', function(){ console.log('????????????????DomLoaded'); });";

            frame.ExecuteJavaScriptAsync(script);
        }
    }
}
