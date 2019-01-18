// Copyright © 2014 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using System.Windows.Forms;

namespace English
{
    internal class MenuHandler : IContextMenuHandler
    {
        readonly Form Parent;
        public MenuHandler(Form parent) : base() { this.Parent = parent; }

        private const int HrSpace = 26500;

        private const int ShowDevTools = 26501;
        private const int CloseDevTools = 26502;

        private const int ReloadPage = 26503;

        private const int ExitApplication = 26504;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            // model.Clear();

            //Removing existing menu item
            bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option
            model.Remove(CefMenuCommand.Print); // Remove menu Print

            // Add a separator
            //model.AddSeparator();

            //Add new custom menu items             
            model.AddItem((CefMenuCommand)ReloadPage, "Reload");
            model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
            model.AddItem((CefMenuCommand)CloseDevTools, "Close DevTools");

            // Add a separator
            model.AddSeparator();

            model.AddItem((CefMenuCommand)ExitApplication, "Exit");
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            switch ((int)commandId)
            {
                case ShowDevTools:
                    browser.ShowDevTools();
                    break;
                case CloseDevTools:
                    browser.CloseDevTools();
                    break;
                case ReloadPage:
                    browser.Reload();
                    break;
                case ExitApplication:
                    this.Parent.CrossThreadCalls(() => this.Parent.Close());
                    break;
            }
            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}