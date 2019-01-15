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
        NotifyIcon main_icon = new NotifyIcon();
        
        //Check if it's time to update.
        Timer tm_refresh = new Timer() { Interval = 1000 * 60 };        
        bool connectivity_issues = false;
        int conn_test_counter = 0;
        
        #endregion

        /// <summary>
        /// Gets this whole application rolling
        /// </summary>
        public MainContext()
        {
            setupIcon();

            frmBrowser frm = new frmBrowser();
            //frm.OnSetupFinished += setupCompleted;
            frm.Show();

            tm_refresh.Tick += tm_refresh_Tick;
            tm_refresh_Tick(tm_refresh, new EventArgs()); //Fire the first event
            tm_refresh.Start();
        }

        #region [ Voids ]

        private void onOpenBrowserClicked(object sender, EventArgs e)
        {

        }

        private void onOpenDictionaryClicked(object sender, EventArgs e)
        {

        }

        private void onOpenMediaPlayerClicked(object sender, EventArgs e)
        {

        }

        private void Main_icon_BalloonTipClicked(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(Properties.Settings.Default.StorageLocation);
        }

        /// <summary>
        /// Places the Black-Sink icon in the Windows Task Tray
        /// </summary>
        private void setupIcon()
        {
            main_icon.Text = "English";
            main_icon.Icon = English.Properties.Resources.icon;
            main_icon.ContextMenu = new ContextMenu();
            main_icon.ContextMenu.MenuItems.Add("Browser...", onOpenBrowserClicked);
            main_icon.ContextMenu.MenuItems.Add("-");
            main_icon.ContextMenu.MenuItems.Add("Dictionary...", onOpenDictionaryClicked);
            main_icon.ContextMenu.MenuItems.Add("About...", onAboutClicked);
            main_icon.ContextMenu.MenuItems.Add("-");
            main_icon.ContextMenu.MenuItems.Add("Media Player", onOpenMediaPlayerClicked);
            main_icon.ContextMenu.MenuItems.Add("-");
            main_icon.ContextMenu.MenuItems.Add("Quit", onQuitClicked);
            main_icon.BalloonTipClicked += Main_icon_BalloonTipClicked;

            main_icon.Visible = true;
        }

        protected override void ExitThreadCore()
        {
            main_icon.Visible = false;
            main_icon.Dispose();

            tm_refresh.Stop();
            tm_refresh.Dispose();

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
                tm_refresh.Interval = 1000 * 60;
                if (connectivity_issues)
                {
                    main_icon.Icon = English.Properties.Resources.icon;
                    main_icon.Text = "English\r\nLast Synchronized " + DateTime.Now.ToString("t");
                    Application.DoEvents();
                }
                connectivity_issues = false;
            }
            else
            {
                tm_refresh.Interval = 1000 * 5;
                main_icon.Icon = English.Properties.Resources.offline;
                main_icon.Text = "English\r\nNo Internet Connection";
                connectivity_issues = true;
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
            if (conn_test_counter == -1)
            {
                //We have a problem. Major check til it works.
                conn_test_counter = InternetConnectivity.strongInternetConnectionTest() ? 0 : -1;
                return conn_test_counter == 0;
            }

            if (InternetConnectivity.IsConnectionAvailable())
            {
                ++conn_test_counter;
                if (conn_test_counter == 4)
                {
                    conn_test_counter = InternetConnectivity.strongInternetConnectionTest() ? 0 : conn_test_counter + 1;
                    Console.WriteLine("[Level 2] Working Internet Connection - Timer Check");
                    return true;
                }
                else if (conn_test_counter > 5)
                {
                    conn_test_counter = InternetConnectivity.strongInternetConnectionTest() ? 0 : conn_test_counter + 1;
                    if (conn_test_counter > 7)
                    {
                        conn_test_counter = -1;
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
                conn_test_counter = -1;
                return false;
            }
        }

        #endregion
        
    }
}
