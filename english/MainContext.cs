using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;
using Fleck;
using System.Web.Hosting;
using CefSharp;
using English.Properties;
using CefSharp.WinForms;
using CefSharp.SchemeHandler;

namespace English
{
    public class MainContext : ApplicationContext, IContext
    {
        #region [ Crucial Variables ]

        const string PROXY_URL = "http://+:9000/";
        const string PROXY_PATH_ROOT = @"C:\WebUI_v3\DEV\dev\source\FUM_dev\trunk_multi";

        //For the task tray icon.
        NotifyIcon _mainIcon = new NotifyIcon();

        //Check if it's time to update.
        Timer _timerRefresh = new Timer() { Interval = 1000 * 60 };
        bool _connectivityIssues = false;
        int _connectCheckCounter = 0;

        readonly frmBrowser _browser;
        readonly frmDictionary _dictionary;
        readonly frmMediaPlayer _player;

        #endregion

        public string CoreCSS
        {
            get
            {
                if (File.Exists("core.css")) return @" <style type=""text/css""> " + File.ReadAllText("core.css") + " </style> ";
                return string.Empty;
            }
        }
        public string CoreJS
        {
            get
            {
                if (File.Exists("core.js")) return @" <script type=""text/javascript""> " + File.ReadAllText("core.js") + " </script> ";
                return string.Empty;
            }
        }

        static void CefInit()
        {
            Cef.EnableHighDPISupport();
            AbstractCefSettings settings = new CefSettings();
            settings.ExternalMessagePump = false;

            // Set Google API keys, used for Geolocation requests sans GPS.  See http://www.chromium.org/developers/how-tos/api-keys
            // Environment.SetEnvironmentVariable("GOOGLE_API_KEY", "");
            // Environment.SetEnvironmentVariable("GOOGLE_DEFAULT_CLIENT_ID", "");
            // Environment.SetEnvironmentVariable("GOOGLE_DEFAULT_CLIENT_SECRET", "");

            // Widevine CDM registration - pass in directory where Widevine CDM binaries and manifest.json are located.
            // For more information on support for DRM content with Widevine see: https://github.com/cefsharp/CefSharp/issues/1934
            //Cef.RegisterWidevineCdm(@".\WidevineCdm");

            //Chromium Command Line args
            //http://peter.sh/experiments/chromium-command-line-switches/
            //NOTE: Not all relevant in relation to `CefSharp`, use for reference purposes only.

            //settings.RemoteDebuggingPort = 9222;
            //The location where cache data will be stored on disk. If empty an in-memory cache will be used for some features and a temporary disk cache for others.
            //HTML5 databases such as localStorage will only persist across sessions if a cache path is specified. 
            settings.CachePath = "cache";

            //settings.CachePath = null;
            //settings.CefCommandLineArgs.Add("disable-application-cache", "1");

            //settings.UserAgent = "CefSharp Browser" + Cef.CefSharpVersion; // Example User Agent
            //settings.CefCommandLineArgs.Add("renderer-process-limit", "1");
            //settings.CefCommandLineArgs.Add("renderer-startup-dialog", "1");
            //settings.CefCommandLineArgs.Add("enable-media-stream", "1"); //Enable WebRTC
            //settings.CefCommandLineArgs.Add("no-proxy-server", "1"); //Don't use a proxy server, always make direct connections. Overrides any other proxy server flags that are passed.
            //settings.CefCommandLineArgs.Add("debug-plugin-loading", "1"); //Dumps extra logging about plugin loading to the log file.
            //settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1"); //Disable discovering third-party plugins. Effectively loading only ones shipped with the browser plus third-party ones as specified by --extra-plugin-dir and --load-plugin switches
            //settings.CefCommandLineArgs.Add("enable-system-flash", "1"); //Automatically discovered and load a system-wide installation of Pepper Flash.
            settings.CefCommandLineArgs.Add("allow-running-insecure-content", "1"); //By default, an https page cannot run JavaScript, CSS or plugins from http URLs. This provides an override to get the old insecure behavior. Only available in 47 and above.

            //settings.CefCommandLineArgs.Add("enable-logging", "1"); //Enable Logging for the Renderer process (will open with a cmd prompt and output debug messages - use in conjunction with setting LogSeverity = LogSeverity.Verbose;)
            //settings.LogSeverity = LogSeverity.Verbose; // Needed for enable-logging to output messages
            //settings.CefCommandLineArgs.Add("enable-logging", "0"); //Enable Logging for the Renderer process (will open with a cmd prompt and output debug messages - use in conjunction with setting LogSeverity = LogSeverity.Verbose;)
            settings.LogSeverity = LogSeverity.Disable; // Needed for enable-logging to output messages

            //settings.CefCommandLineArgs.Add("disable-extensions", "1"); //Extension support can be disabled
            //settings.CefCommandLineArgs.Add("disable-pdf-extension", "1"); //The PDF extension specifically can be disabled

            //Load the pepper flash player that comes with Google Chrome - may be possible to load these values from the registry and query the dll for it's version info (Step 2 not strictly required it seems)
            //settings.CefCommandLineArgs.Add("ppapi-flash-path", @"C:\Program Files (x86)\Google\Chrome\Application\47.0.2526.106\PepperFlash\pepflashplayer.dll"); //Load a specific pepper flash version (Step 1 of 2)
            //settings.CefCommandLineArgs.Add("ppapi-flash-version", "20.0.0.228"); //Load a specific pepper flash version (Step 2 of 2)

            //NOTE: For OSR best performance you should run with GPU disabled:
            // `--disable-gpu --disable-gpu-compositing --enable-begin-frame-scheduling`
            // (you'll loose WebGL support but gain increased FPS and reduced CPU usage).
            // http://magpcss.org/ceforum/viewtopic.php?f=6&t=13271#p27075
            //https://bitbucket.org/chromiumembedded/cef/commits/e3c1d8632eb43c1c2793d71639f3f5695696a5e8

            //NOTE: The following function will set all three params
            //settings.SetOffScreenRenderingBestPerformanceArgs();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            ////settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");

            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1"); //Disable Vsync

            //Disables the DirectWrite font rendering system on windows.
            //Possibly useful when experiencing blury fonts.
            //settings.CefCommandLineArgs.Add("disable-direct-write", "1");

            // The following options control accessibility state for all frames.
            // These options only take effect if accessibility state is not set by IBrowserHost.SetAccessibilityState call.
            // --force-renderer-accessibility enables browser accessibility.
            // --disable-renderer-accessibility completely disables browser accessibility.
            //settings.CefCommandLineArgs.Add("force-renderer-accessibility", "1");
            //settings.CefCommandLineArgs.Add("disable-renderer-accessibility", "1");

            //-------------------------------------------------------------------------------------
            //settings.CefCommandLineArgs.Add("touch-events", "enabled");
            //settings.CefCommandLineArgs.Add("mouse-events", "disabled");
            settings.CefCommandLineArgs.Add("js-flags", "--harmony-proxies");
            settings.MultiThreadedMessageLoop = true;
            //-------------------------------------------------------------------------------------

            //Enables Uncaught exception handler
            settings.UncaughtExceptionStackSize = 10;

            // Off Screen rendering (WPF/Offscreen)
            if (settings.WindowlessRenderingEnabled)
            {
                //Disable Direct Composition to test https://github.com/cefsharp/CefSharp/issues/1634
                //settings.CefCommandLineArgs.Add("disable-direct-composition", "1");

                // DevTools doesn't seem to be working when this is enabled
                // http://magpcss.org/ceforum/viewtopic.php?f=6&t=14095
                //settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");
            }

            ////var proxy = ProxyConfig.GetProxyInformation();
            ////switch (proxy.AccessType)
            ////{
            ////    case InternetOpenType.Direct:
            ////        {
            ////            //Don't use a proxy server, always make direct connections.
            ////            settings.CefCommandLineArgs.Add("no-proxy-server", "1");
            ////            break;
            ////        }
            ////    case InternetOpenType.Proxy:
            ////        {
            ////            settings.CefCommandLineArgs.Add("proxy-server", proxy.ProxyAddress);
            ////            break;
            ////        }
            ////    case InternetOpenType.PreConfig:
            ////        {
            ////            settings.CefCommandLineArgs.Add("proxy-auto-detect", "1");
            ////            break;
            ////        }
            ////}

            //settings.LogSeverity = LogSeverity.Verbose;

            //////if (DebuggingSubProcess)
            //////{
            //////    var architecture = Environment.Is64BitProcess ? "x64" : "x86";
            //////    settings.BrowserSubprocessPath = "..\\..\\..\\..\\CefSharp.BrowserSubprocess\\bin\\" + architecture + "\\Debug\\CefSharp.BrowserSubprocess.exe";
            //////}
            settings.BrowserSubprocessPath = "cefclient.exe";

            //////settings.RegisterScheme(new CefCustomScheme
            //////{
            //////    SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
            //////    SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
            //////    IsSecure = true //treated with the same security rules as those applied to "https" URLs
            //////    //SchemeHandlerFactory = new InMemorySchemeAndResourceHandlerFactory()
            //////});

            //////settings.RegisterScheme(new CefCustomScheme
            //////{
            //////    SchemeName = CefSharpSchemeHandlerFactory.SchemeNameTest,
            //////    SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
            //////    IsSecure = true //treated with the same security rules as those applied to "https" URLs
            //////});

            ////////You can use the http/https schemes - best to register for a specific domain
            //////settings.RegisterScheme(new CefCustomScheme
            //////{
            //////    SchemeName = "https",
            //////    SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
            //////    DomainName = "cefsharp.com",
            //////    IsSecure = true //treated with the same security rules as those applied to "https" URLs
            //////});

            //////settings.RegisterScheme(new CefCustomScheme
            //////{
            //////    SchemeName = "localfolder",
            //////    SchemeHandlerFactory = new FolderSchemeHandlerFactory(rootFolder: @"..\..\..\..\CefSharp.Example\Resources",
            //////                                                        schemeName: "localfolder", //Optional param no schemename checking if null
            //////                                                        hostName: "cefsharp", //Optional param no hostname checking if null
            //////                                                        defaultPage: "home.html") //Optional param will default to index.html
            //////});

            //////string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //////var dir = System.IO.Path.GetDirectoryName(path);
            //////string pathWWW = Path.Combine(dir, "test");
            ////string pathWWW = "test";
            ////settings.RegisterScheme(new CefCustomScheme
            ////{
            ////    SchemeName = "localfolder",
            ////    SchemeHandlerFactory = new FolderSchemeHandlerFactory(rootFolder: pathWWW,
            ////                                                        schemeName: "localfolder", //Optional param no schemename checking if null
            ////                                                        hostName: "cefsharp", //Optional param no hostname checking if null
            ////                                                        defaultPage: "home.html") //Optional param will default to index.html
            ////});

            if (Directory.Exists("hook") == false) Directory.CreateDirectory("hook");
            if (Directory.Exists("hook/base") == false) Directory.CreateDirectory("hook/base");
            if (File.Exists("hook/base/www.js") == false) File.WriteAllText("hook/base/www.js", string.Empty);
            if (File.Exists("hook/base/www.css") == false) File.WriteAllText("hook/base/www.css", string.Empty);
            ////settings.RegisterScheme(new CefCustomScheme
            ////{
            ////    SchemeName = "http",
            ////    SchemeHandlerFactory = new FolderSchemeHandlerFactory(rootFolder: "hook", schemeName: "http", hostName: "hook")
            ////});
            //You can use the http/https schemes - best to register for a specific domain
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = HookHandlerFactory.SchemeName,
                SchemeHandlerFactory = new HookHandlerFactory(),
                DomainName = HookHandlerFactory.Host,
                IsSecure = false //treated with the same security rules as those applied to "https" URLs
            });

            //settings.RegisterExtension(new V8Extension("english", Resources.extension));

            //This must be set before Cef.Initialized is called
            CefSharpSettings.FocusedNodeChangedEnabled = true;

            //Experimental option where bound async methods are queued on TaskScheduler.Default.
            CefSharpSettings.ConcurrentTaskExecution = true;

            //Legacy Binding Behaviour doesn't work for cross-site navigation (navigating to a different domain)
            //See issue https://github.com/cefsharp/CefSharp/issues/1203 for details
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            //Exit the subprocess if the parent process happens to close
            //This is optional at the moment
            //https://github.com/cefsharp/CefSharp/pull/2375/
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            //NOTE: Set this before any calls to Cef.Initialize to specify a proxy with username and password
            //One set this cannot be changed at runtime. If you need to change the proxy at runtime (dynamically) then
            //see https://github.com/cefsharp/CefSharp/wiki/General-Usage#proxy-resolution
            //CefSharpSettings.Proxy = new ProxyOptions(ip: "127.0.0.1", port: "8080", username: "cefsharp", password: "123");

            //////if (!Cef.Initialize(settings, performDependencyCheck: !DebuggingSubProcess, browserProcessHandler: browserProcessHandler))
            //////{
            //////    throw new Exception("Unable to Initialize Cef");
            //////}
            if (!Cef.Initialize(settings))
            {
                if (Environment.GetCommandLineArgs().Contains("--type=renderer"))
                {
                    Environment.Exit(0);
                }
                else
                {
                    return;
                }
            }
            ////Cef.AddCrossOriginWhitelistEntry(BaseUrl, "https", "cefsharp.com", false);
            //Cef.AddCrossOriginWhitelistEntry("http://hook/", "http", "hook", false);
        }

        /// <summary>
        /// Gets this whole application rolling
        /// </summary>
        public MainContext()
        {
            CefInit();

            ProxyWebServer.Start(PROXY_URL, PROXY_PATH_ROOT);

            setupIcon();

            var f = new frmTestBrowser(this);
            f.FormClosing += (se, ev) => { 
                ev.Cancel = true;
                ExitThreadCore();
            };
            f.Show();

            _dictionary = new frmDictionary(this);
            _dictionary.FormClosing += (se, ev) =>
            {
                _dictionary.Hide();
                ev.Cancel = true;
                //??????????????????????????????????
                this.ExitThreadCore();
            };
            //_dictionary.Show();

            _player = new frmMediaPlayer();
            _player.FormClosing += (se, ev) =>
            {
                _player.Hide();
                ev.Cancel = true;
            };

            _browser = new frmBrowser();
            _browser.FormClosing += (se, ev) =>
            {
                _browser.Hide();
                ev.Cancel = true;
            };
            //_browser.Show();

            _timerRefresh.Tick += tm_refresh_Tick;
            tm_refresh_Tick(_timerRefresh, new EventArgs()); //Fire the first event
            _timerRefresh.Start();
        }

        public void freeResource()
        {
            _mainIcon.Visible = false;
            _mainIcon.Dispose();

            _timerRefresh.Stop();
            _timerRefresh.Dispose();

            ProxyWebServer.Stop();
        }

        protected override void ExitThreadCore()
        {
            freeResource();
            base.ExitThreadCore();
        }

        #region [ Websocket ]

        void websocket_Init()
        {
            var server = new WebSocketServer("ws://0.0.0.0:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () => Console.WriteLine("Open!");
                socket.OnClose = () => Console.WriteLine("Close!");
                socket.OnMessage = message => socket.Send(message);
            });
        }

        #endregion

        #region [ Voids ]

        private void onOpenBrowserClicked(object sender, EventArgs e)
        {
            _browser.Show();
        }

        private void onOpenDictionaryClicked(object sender, EventArgs e)
        {
            _dictionary.Show();
        }

        private void onOpenMediaPlayerClicked(object sender, EventArgs e)
        {
            _player.Show();
        }

        private void Main_icon_BalloonTipClicked(object sender, EventArgs e)
        {
            onOpenBrowserClicked(null, null);
        }

        /// <summary>
        /// Places the Black-Sink icon in the Windows Task Tray
        /// </summary>
        private void setupIcon()
        {
            _mainIcon.Text = "English";
            _mainIcon.Icon = English.Properties.Resources.icon;
            _mainIcon.ContextMenu = new ContextMenu();
            _mainIcon.ContextMenu.MenuItems.Add("Browser...", onOpenBrowserClicked);
            _mainIcon.ContextMenu.MenuItems.Add("-");
            _mainIcon.ContextMenu.MenuItems.Add("Dictionary...", onOpenDictionaryClicked);
            _mainIcon.ContextMenu.MenuItems.Add("Media Player", onOpenMediaPlayerClicked);
            _mainIcon.ContextMenu.MenuItems.Add("-");
            _mainIcon.ContextMenu.MenuItems.Add("About...", onAboutClicked);
            _mainIcon.ContextMenu.MenuItems.Add("-");
            _mainIcon.ContextMenu.MenuItems.Add("Quit", onQuitClicked);
            _mainIcon.BalloonTipClicked += Main_icon_BalloonTipClicked;
            _mainIcon.MouseClick += (se, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    onOpenDictionaryClicked(null, null);
                }
            };

            _mainIcon.Visible = true;
        }


        private void onQuitClicked(object sender, EventArgs e)
        {
            this.ExitThreadCore();
        }

        private void onAboutClicked(object sender, EventArgs e)
        {
            MessageBox.Show("Learn English - Mr Thinh: http://iot.vn");
        }


        private void tm_refresh_Tick(object sender, EventArgs e)
        {
            //bool active = Properties.Settings.Default.is_setup;
            if (checkInternet())
            {
                _timerRefresh.Interval = 1000 * 60;
                if (_connectivityIssues)
                {
                    _mainIcon.Icon = English.Properties.Resources.icon;
                    _mainIcon.Text = "English\r\nLast Synchronized " + DateTime.Now.ToString("t");
                    Application.DoEvents();
                }
                _connectivityIssues = false;
            }
            else
            {
                _timerRefresh.Interval = 1000 * 5;
                _mainIcon.Icon = English.Properties.Resources.offline;
                _mainIcon.Text = "English\r\nNo Internet Connection";
                _connectivityIssues = true;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Algorithm to check internet connection.
        /// [Broken] [Level -1] The state of the system when internet has been declared unavailable.
        /// [Level 0] If the system reports no cable/wifi connection, there is no connection
        /// [Level 1] If the system reports cable/wifi, we can assume there is a connection for now
        /// [Level 2] The system reported cable/wifi, be believed it, but let's check by pinging Google
        /// [Level 3] If the system still reports cable/wifi, but a Google ping failed to work, we'll hope for the best and give it one more shot
        /// [Level 4] Failed twice or more, so there is no connection because something's broken.
        /// </summary>
        /// <returns></returns>
        private bool checkInternet()
        {
            if (_connectCheckCounter == -1)
            {
                //We have a problem. Major check til it works.
                _connectCheckCounter = InternetConnectivity.strongInternetConnectionTest() ? 0 : -1;
                return _connectCheckCounter == 0;
            }

            if (InternetConnectivity.IsConnectionAvailable())
            {
                ++_connectCheckCounter;
                if (_connectCheckCounter == 4)
                {
                    _connectCheckCounter = InternetConnectivity.strongInternetConnectionTest() ? 0 : _connectCheckCounter + 1;
                    Console.WriteLine("[Level 2] Working Internet Connection - Timer Check");
                    return true;
                }
                else if (_connectCheckCounter > 5)
                {
                    _connectCheckCounter = InternetConnectivity.strongInternetConnectionTest() ? 0 : _connectCheckCounter + 1;
                    if (_connectCheckCounter > 7)
                    {
                        _connectCheckCounter = -1;
                        Console.WriteLine("[Level 4] No Internet Connection - Timer Check");
                        return false;
                    }
                    else
                    {
                        //Let's give it one more shot
                        Console.WriteLine("[Level 3] Working Internet Connection - Timer Check");
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("[Level 1] Working Internet Connection - Timer Check");
                    return true;
                }
            }
            else
            {
                Console.WriteLine("[Level 0] No Connection - Timer Check");
                _connectCheckCounter = -1;
                return false;
            }
        }

        #endregion

        public void jobPush()
        {
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => new Worker().StartProcessing(cancellationToken));
        }
    }
}
