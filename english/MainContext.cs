using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;

namespace English
{
    public class MainContext : ApplicationContext
    {
        #region [ Crucial Variables ]

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

        /// <summary>
        /// Gets this whole application rolling
        /// </summary>
        public MainContext()
        {
            setupIcon();

            _dictionary = new frmDictionary();
            _dictionary.FormClosing += (se, ev) => {
                _dictionary.Hide();
                ev.Cancel = true;
            };

            _player = new frmMediaPlayer();
            _player.FormClosing += (se, ev) => {
                _player.Hide();
                ev.Cancel = true;
            };

            _browser = new frmBrowser();
            _browser.FormClosing += (se, ev) => {
                _browser.Hide();
                ev.Cancel = true;
            };
            _browser.Show();

            _timerRefresh.Tick += tm_refresh_Tick;
            tm_refresh_Tick(_timerRefresh, new EventArgs()); //Fire the first event
            _timerRefresh.Start();
        }

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
            _mainIcon.MouseClick += (se,ev)=>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    onOpenDictionaryClicked(null, null);
                }
            };

            _mainIcon.Visible = true;
        }

        protected override void ExitThreadCore()
        {
            _mainIcon.Visible = false;
            _mainIcon.Dispose();

            _timerRefresh.Stop();
            _timerRefresh.Dispose();

            base.ExitThreadCore();
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
    }
}
