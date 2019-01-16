using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace English
{
    public class frmBrowser : Form
    {
        public IWinFormsWebBrowser Browser { get; private set; }
        const string URL = "https://google.com.vn";

        public frmBrowser()
        {
            this.Text = "Browser";

            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);

            this.Shown += (se, ev) =>
            {
                this.Top = 0;
                this.Left = 0;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            };

            var browser = new ChromiumWebBrowser(URL)
            {
                Dock = DockStyle.Fill,
                BrowserSettings =
                {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled,
                    WebSecurity = CefState.Disabled,
                }
            };
            this.Controls.Add(browser);
            this.Browser = browser;
            browser.MenuHandler = new MenuHandler();
        }

        ~frmBrowser()
        {
            this.Browser.Dispose();
        }
    }
}
