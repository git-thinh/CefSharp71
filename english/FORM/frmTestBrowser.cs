using CefSharp;
using CefSharp.Handler;
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
        const string URL = "https://www.rong-chang.com/easyspeak/es/school01.htm";
        //const string URL = "https://www.eslfast.com/";
        //const string URL = "https://dictionary.cambridge.org/";
        //const string URL = "https://youtube.com";
        readonly StringBuilder LogBuilder;
        public frmTestBrowser(IContext context) : base(context)
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
                browser.ShowDevTools();
            };
            browser.RequestHandler = new RequestHandler(this);
            browser.MenuHandler = new MenuHandler(this);
            browser.JsDialogHandler = new JsDialogHandler();
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
                    this.ClearLog();
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

        public void ClearLog() { LogBuilder.Clear(); }
    }

    public class RequestHandler : DefaultRequestHandler
    {
        readonly IForm Parent;
        public RequestHandler(IForm parent) : base() { this.Parent = parent; }

        public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            this.Parent.ClearLog();
            return false;
        }

        private Dictionary<UInt64, MemoryStreamResponseFilter> responseDictionary = new Dictionary<UInt64, MemoryStreamResponseFilter>();

        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var url = request.Url;
            if (url == browserControl.Address)
            {
                //if (request.Url.Equals(CefExample.ResponseFilterTestUrl, StringComparison.OrdinalIgnoreCase))
                //{
                //    return new FindReplaceResponseFilter("REPLACE_THIS_STRING", "This is the replaced string!");
                //}

                //if (request.Url.Equals("custom://cefsharp/assets/js/jquery.js", StringComparison.OrdinalIgnoreCase))
                //{
                //    return new AppendResponseFilter(System.Environment.NewLine + "//CefSharp Appended this comment.");
                //}

                ////Only called for our customScheme
                //var dataFilter = new MemoryStreamResponseFilter();
                //responseDictionary.Add(request.Identifier, dataFilter);
                //return dataFilter;

                return new AppendResponseFilter(System.Environment.NewLine + 
                    "<!-- CefSharp Appended this comment. -->" + 
                    this.Parent.Context.CoreCSS + this.Parent.Context.CoreJS);
            }

            //return new PassThruResponseFilter();
            return null;
        }

        //public override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        //{
        //    var url = request.Url;
        //    if (url == browserControl.Address)
        //    {
        //        MemoryStreamResponseFilter filter;
        //        if (responseDictionary.TryGetValue(request.Identifier, out filter))
        //        {
        //            //TODO: Do something with the data here
        //            var data = filter.Data;
        //            var dataLength = filter.Data.Length;
        //            //NOTE: You may need to use a different encoding depending on the request
        //            var dataAsUtf8String = Encoding.UTF8.GetString(data);
        //        }
        //    }
        //}
    }

    public class AppendResponseFilter : IResponseFilter
    {
        private static Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Overflow from the output buffer.
        /// </summary>
        private List<byte> overflow = new List<byte>();

        public AppendResponseFilter(string stringToAppend)
        {
            //Add the encoded string into the overflow.
            overflow.AddRange(encoding.GetBytes(stringToAppend));
        }

        bool IResponseFilter.InitFilter()
        {
            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                return FilterStatus.Done;
            }

            //We'll read all the data
            dataInRead = dataIn.Length;
            dataOutWritten = Math.Min(dataInRead, dataOut.Length);

            if (dataIn.Length > 0)
            {
                //Copy all the existing data first
                dataIn.CopyTo(dataOut);
            }

            // If we have overflow data and remaining space in the buffer then write the overflow.
            if (overflow.Count > 0)
            {
                // Number of bytes remaining in the output buffer.
                var remainingSpace = dataOut.Length - dataOutWritten;
                // Maximum number of bytes we can write into the output buffer.
                var maxWrite = Math.Min(overflow.Count, remainingSpace);

                // Write the maximum portion that fits in the output buffer.
                if (maxWrite > 0)
                {
                    dataOut.Write(overflow.ToArray(), 0, (int)maxWrite);
                    dataOutWritten += maxWrite;
                }

                if (maxWrite == 0 && overflow.Count > 0)
                {
                    //We haven't yet got space to append our data
                    return FilterStatus.NeedMoreData;
                }

                if (maxWrite < overflow.Count)
                {
                    // Need to write more bytes than will fit in the output buffer. 
                    // Remove the bytes that were written already
                    overflow.RemoveRange(0, (int)(maxWrite - 1));
                }
                else
                {
                    overflow.Clear();
                }
            }

            if (overflow.Count > 0)
            {
                return FilterStatus.NeedMoreData;
            }

            return FilterStatus.Done;
        }

        public void Dispose()
        {

        }
    }

    /// <summary>
    /// MemoryStreamResponseFilter - copies all data from IResponseFilter.Filter
    /// to a MemoryStream. This is provided as an example to get you started and has not been
    /// production tested. If you experience problems you should refer to the CEF documentation
    /// and ask any questions you have on http://magpcss.org/ceforum/index.php
    /// Make sure to ask your question in the context of the CEF API (remember that CefSharp is just a wrapper).
    /// https://magpcss.org/ceforum/apidocs3/projects/(default)/CefResponseFilter.html#Filter(void*,size_t,size_t&,void*,size_t,size_t&)
    /// </summary>
    public class MemoryStreamResponseFilter : IResponseFilter
    {
        private MemoryStream memoryStream;

        bool IResponseFilter.InitFilter()
        {
            //NOTE: We could initialize this earlier, just one possible use of InitFilter
            memoryStream = new MemoryStream();

            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                return FilterStatus.Done;
            }

            //Calculate how much data we can read, in some instances dataIn.Length is
            //greater than dataOut.Length
            dataInRead = Math.Min(dataIn.Length, dataOut.Length);
            dataOutWritten = dataInRead;

            var readBytes = new byte[dataInRead];
            dataIn.Read(readBytes, 0, readBytes.Length);
            dataOut.Write(readBytes, 0, readBytes.Length);

            //Write buffer to the memory stream
            memoryStream.Write(readBytes, 0, readBytes.Length);

            //If we read less than the total amount avaliable then we need
            //return FilterStatus.NeedMoreData so we can then write the rest
            if (dataInRead < dataIn.Length)
            {
                return FilterStatus.NeedMoreData;
            }

            return FilterStatus.Done;
        }

        void IDisposable.Dispose()
        {
            memoryStream.Dispose();
            memoryStream = null;
        }

        public byte[] Data
        {
            get { return memoryStream.ToArray(); }
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
            if (url.Contains("chrome-devtools") || url.Contains("chrome-extension")) return;
            if (OnEventUrlArrived != null) OnEventUrlArrived(url);
        }

        IResourceHandler IResourceHandlerFactory.GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            string url = request.Url;

            if (url.Contains("facebook")
                //|| url.Contains("google") 
                || url.Contains("google-analytics")
                || url.Contains("googlesyndication")
                || url.Contains("adservice.google.com")
                || url.Contains("googletagservices")
                || url.Contains("click")
                || url.Contains("sharethis")
                || url.Contains("counter")
                || url.Contains("adserver")
                || url.Contains("reach")
                || url.Contains("visitor"))
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
            //browserControl.Load(targetUrl);
            //frame.ExecuteJavaScriptAsync("setTimeout(function () { location.href = '" + targetUrl + "'; }, 1);");
            frame.ExecuteJavaScriptAsync(" location.href = '" + targetUrl + "'; ");
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
}
