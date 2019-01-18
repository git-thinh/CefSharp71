// Copyright © 2014 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using System.Windows.Forms;

namespace English
{
    internal class MenuHandler : IContextMenuHandler
    {
        readonly IForm Parent;
        public MenuHandler(IForm parent) : base() { this.Parent = parent; }

        private const int HrSpace = 27500;

        private const int ReloadPage = 27003;
        private const int ShowDevTools = 27001;
        private const int CloseDevTools = 27002;
        private const int OpenLogRequestResource = 27004;
        private const int ClearLogRequestResource = 27005;


        private const int ExitApplication = 27100;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            // model.Clear();

            //Removing existing menu item
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option
            model.Remove(CefMenuCommand.Print); // Remove menu Print

            // Add a separator
            //model.AddSeparator();

            //Add new custom menu items             
            model.AddItem((CefMenuCommand)ReloadPage, "Reload");
            model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
            model.AddItem((CefMenuCommand)CloseDevTools, "Close DevTools");
            model.AddItem((CefMenuCommand)OpenLogRequestResource, "Open Log");
            model.AddItem((CefMenuCommand)ClearLogRequestResource, "Clear Log");

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
                case ClearLogRequestResource:
                case OpenLogRequestResource:
                case ExitApplication:
                    this.Parent.RaiseEventMenuBrowser((int)commandId);
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