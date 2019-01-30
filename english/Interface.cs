using System;
using System.Collections.Generic;
using System.Text;

namespace English
{

    public interface IForm
    {
        IContext Context { get; }
        void RaiseEventMenuBrowser(int menuCode);
        void CloseForm();
        void ClearLog();

        string URL_NEXT { get; set; }
        void Go(string url);
    }

    public interface IContext
    {
        void jobPush();
        string CoreCSS { get; }
        string CoreJS { get; }
    }

    public class oProxy
    {
        public oProxyTarget Target { set; get; }
        public oProxyDestination Destination { set; get; }
    }

    public class oProxyTarget
    {
        public string UrlBase { set; get; }
        public oProxyTarget() {
            this.UrlBase = "http://localhost:321";
        }
    }
    
    public class oProxyDestination
    {
        public bool hasLogAllUrl { set; get; }

        public string UrlWesocket { set; get; }
        public string UrlHttp { set; get; }
        public string PathRoot { set; get; }

        public oProxyDestinationSettings Settings { set; get; }

        public oProxyDestination() {
            this.UrlWesocket = "ws://localhost:33333";
            this.UrlHttp = "http://localhost:11111";
            this.PathRoot = "/.";
        }
    }
    public class oProxyDestinationSettings
    {
        public string Url { set; get; }
        public string[] appendUrl { set; get; }
    }


    public interface IApp
    {
        int socketPort { get; }
        Fleck.IWebSocketConnection socketCurrent { get; }

        void socketSendMessage(string message);
        void socketPushMessage(string message);

        bool dicWordPhraseAdd(string name, string phonics, string mean);
        bool dicSentenceAdd(string text);

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        string fetchResponse(string url);

        void fetchHttp(string url);
        void fetchHttps(string url);

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        void downloadMp3(string url, int timeOutDownloadMp3 = 30000);

        void playMp3FromUrl(string url, int repeat, bool isRunOnline = false, int timeOutDownloadMp3 = 30000);

        void speechSentence(string text, int repeat);
        void speechWords(string text, int repeat);
        void speechWord(string text, int repeat);
        void speechCancel();

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        oApp appInfo { get; }
        void writeLog(string text);

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        void webViewMain_Load(string url);
        void webViewMain_Reload();
        void webViewMain_ShowDevTools();
        void webViewMain_Stop();
    }

    public class oAppCoreJs
    {
        public string Scheme { set; get; }
        public string Host { set; get; }
        public string fileName { set; get; }
        public string[] appendFiles { set; get; }
    }

    public class oApp
    {
        public string[] storePaths { get; set; }
        public string storePathCurrent { get; set; }

        public int socketPort { get; set; }

        public bool alwayOnTop { set; get; }
        public bool hasWriteLog { set; get; }
        public string Url { set; get; }

        public int Width { set; get; }
        public int Height { set; get; }
        public int Top { set; get; }
        public int Left { set; get; }

        public oAppCoreJs coreJs { set; get; }
        public string[] disableHosts { set; get; }

        public oApp()
        {
            this.storePathCurrent = string.Empty;
            this.storePaths = new string[] { @"G:\GoogleDrive" };

            this.Width = 600;
            this.Height = 480;
            this.alwayOnTop = false;
            this.hasWriteLog = true;
            //this.disableHosts = new string[] { "devtools", "play.google.com" };
            this.disableHosts = new string[] { };
            //===========================================================================================================
            this.coreJs = new oAppCoreJs()
            {
                Scheme = "https",
                Host = "www.google-analytics.com",
                fileName = "analytics.js",
                appendFiles = new string[] {
                    "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css", "w2ui.min.css", "jquery.min.js", "w2ui.min.js", "vue.min.js",
                    "translate/core.js", "translate/core.css", "translate/components.js", "translate/components.css"
                }
            };
            this.Url = "https://translate.google.com/#view=home&op=translate&sl=en&tl=vi";
            //-----------------------------------------------------------------------------------------------------------
            //this.coreJs = new oAppCoreJs()
            //{
            //    Scheme = "https",
            //    Host = "www.google-analytics.com",
            //    fileName = "analytics.js"
            //};
            //this.Url = "https://www.google.com/cse?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd&ad=n9&num=10&rurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Djavascript%2B%2522commonly%2Bused%2522%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd&siteurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Dcommon%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd";
            //-----------------------------------------------------------------------------------------------------------
            //this.Url = "https://translate.google.com/#vi/en/";
            //this.Url = "https://www.thefreedictionary.com/_/search/google.aspx?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd";
            //this.Url = "https://www.google.com/cse?q=javascript+%22commonly+used%22&cof=FORID:11&ie=UTF-8&sa=Search&cx=partner-pub-2694630391511205:bm47g3z09jd&ad=n9&num=10&rurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Djavascript%2B%2522commonly%2Bused%2522%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd&siteurl=https%3A%2F%2Fwww.thefreedictionary.com%2F_%2Fsearch%2Fgoogle.aspx%3Fq%3Dcommon%26cof%3DFORID%3A11%26ie%3DUTF-8%26sa%3DSearch%26cx%3Dpartner-pub-2694630391511205%3Abm47g3z09jd";
            //===========================================================================================================
        }
    }
}
