using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace English
{
    public class frmTestBrowser : frmBase, IForm
    {
        //public IWinFormsWebBrowser Browser { get; private set; }
        public ChromiumWebBrowser Browser { get; private set; }
        const string URL = "https://www.eslfast.com/";
        readonly StringBuilder LogBuilder;
        public frmTestBrowser()
        {
            LogBuilder = new StringBuilder();
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

                Browser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
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
            browser.IsBrowserInitializedChanged += (se, ev) =>
            {
                //browser.ShowDevTools();
            };
            browser.MenuHandler = new MenuHandler(this);
            //browser.RenderProcessMessageHandler = new RenderProcessMessageHandler2();
            //browser.JsDialogHandler = new JsDialogHandler();
            var requestResource = new RequestResourceHandlerFactory();
            requestResource.OnEventUrlArrived += (string url) =>
            {
                LogBuilder.AppendLine(Environment.NewLine);
                LogBuilder.AppendLine(url);
            };
            browser.ResourceHandlerFactory = requestResource;
            browser.LifeSpanHandler = new BrowserLifeSpanHandler();
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

        ~frmTestBrowser()
        {
            this.Browser.Dispose();
        }

        public void RaiseEventMenuBrowser(int menuCode)
        {
            switch (menuCode)
            {
                case 27004: // Open log request resource
                    File.WriteAllText("request.txt", LogBuilder.ToString());
                    Process.Start("request.txt");
                    break;
                case 27005: // clear log 
                    LogBuilder.Clear();
                    break;
                case 27100: // exit application
                    this.CrossThreadCalls(() => this.CloseForm());
                    break;
            }
        }

        public void CloseForm()
        {
            this.Close();
        }
    }

    public class RequestResourceHandlerFactory : IResourceHandlerFactory
    {
        /// <summary>
        /// Raised when a request resource event arrives.
        /// </summary>
        public event Action<string> OnEventUrlArrived;

        bool IResourceHandlerFactory.HasHandlers
        {
            get { return true; }
        }

        void SendLogUrl(string url)
        {
            if (url.Contains("chrome-devtools")) return;
            if (OnEventUrlArrived != null) OnEventUrlArrived(url);
        }

        IResourceHandler IResourceHandlerFactory.GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            string url = request.Url;

            if (url.Contains("google") || url.Contains("facebook") || url.Contains("sharethis"))
            {
                SendLogUrl("##> " + request.Method + ": " + url);
                return new RequestResourceCanceler();
            }

            SendLogUrl("--> " + request.Method + ": " + url);
            return null;
        }
    }

    public class RequestResourceCanceler : ResourceHandler
    {
        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            callback.Continue();
            return true;
        }
    }

    public class RequestResourceHandler : ResourceHandler
    {
        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            return false;

            ////Task.Run(() =>
            ////{
            ////    using (callback)
            ////    {
            ////        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://samples.mplayerhq.hu/SWF/zeldaADPCM5bit.swf");

            ////        var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            ////        // Get the stream associated with the response.
            ////        var receiveStream = httpWebResponse.GetResponseStream();
            ////        var mime = httpWebResponse.ContentType;

            ////        var stream = new MemoryStream();
            ////        receiveStream.CopyTo(stream);
            ////        httpWebResponse.Close();

            ////        //Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
            ////        stream.Position = 0;

            ////        //Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)
            ////        ResponseLength = stream.Length;
            ////        MimeType = mime;
            ////        StatusCode = (int)HttpStatusCode.OK;
            ////        Stream = stream;

            ////        callback.Continue();
            ////    }
            ////});
            ////return true;
        }
    }

    public class BrowserLifeSpanHandler : ILifeSpanHandler
    {
        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName,
            WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo,
            IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            // Preserve new windows to be opened and load all popup urls in the same browser view
            browserControl.Load(targetUrl);
            return true;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            //
        }

        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            //nothing
        }
    }

    public class JsDialogHandler : IJsDialogHandler
    {
        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            return false;
        }

        bool IJsDialogHandler.OnBeforeUnloadDialog(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            //Custom implementation would look something like
            // - Create/Show dialog on UI Thread
            // - execute callback once user has responded
            // - callback.Continue(true);
            // - return true

            //NOTE: Returning false will trigger the default behaviour, no need to execute the callback if you return false.
            return false;
        }

        void IJsDialogHandler.OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {

        }

        void IJsDialogHandler.OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {

        }
    }


    public class RenderProcessMessageHandler2 : IRenderProcessMessageHandler
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
