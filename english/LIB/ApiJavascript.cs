using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace English
{
    public class ApiJavascript
    {
        readonly IContext _app;
        public ApiJavascript(IContext app) { this._app = app; }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public void playMp3FromUrl(String url, Int32 repeat)
        {
            ////_app.playMp3FromUrl("https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/hello_gb_1.mp3");
            ////_app.playMp3FromUrl(url, repeat);
            //var playThread = new Thread(timeout => _app.playMp3FromUrl("http://audio.oxforddictionaries.com/en/mp3/ranker_gb_1_8.mp3", (int)timeout));
            //playThread.IsBackground = true;
            //playThread.Start(10000);
        }

        public void speechSentence(String text, Int32 repeat)
        {
            //_app.speechSentence(text, repeat);
        }

        public void speechWords(String text, Int32 repeat)
        {
            //_app.speechWords(text, repeat);
        }

        public void speechWord(String text, Int32 repeat)
        {
            //_app.speechWord(text, repeat);
        }

        public void speechCancel()
        {
            //_app.speechCancel();
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public String fetchResponse(String url)
        {
            //return _app.fetchResponse(url);
            return string.Empty;
        }

        public void fetchHttp(String url)
        {
            //_app.fetchHttp(url);
        }

        public void fetchHttps(String url)
        {
            //_app.fetchHttps(url);
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public void appOpenDevtool(String menu)
        {
            //_app.webViewMain_ShowDevTools();
        }

        public void appOpenUrl(String url, String title)
        {
            //_app.webViewMain_Load(url);
        }

        public void appReload()
        {
            //_app.webViewMain_Reload();
        }

        /*/////////////////////////////////////////////////////////////*/
        /*/////////////////////////////////////////////////////////////*/

        public Boolean dirCreate(String path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public Boolean dirExist(String path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            try
            {
                return Directory.Exists(path);
            }
            catch { }
            return false;
        }

        //========================================================================

        public String fileRead(String file)
        {
            if (File.Exists(file)) return File.ReadAllText(file);
            return JsonConvert.SerializeObject(new { Ok = false, Message = "Cannot find the file: " + file });
        }
                
        public Boolean fileWrite(String file, String data)
        {
            if (string.IsNullOrEmpty(file)) return false;
            string fi = file;
            if (fi[0] == '/' || fi[0] == '\\') fi = fi.Substring(1).Replace('\\', '/').Trim();

            try
            {
                fi = Path.GetFullPath(fi);
                string dir = Path.GetDirectoryName(fi);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(fi, data);
                return true;
            }
            catch { }
            return false;
        }

        public Boolean fileExist(String file)
        {
            if (string.IsNullOrEmpty(file)) return false;
            string fi = file;
            if (fi[0] == '/' || fi[0] == '\\') fi = fi.Substring(1).Replace('\\', '/').Trim();

            try
            {
                fi = Path.GetFullPath(fi);
                return File.Exists(fi);
            }
            catch { }
            return false;
        }
    }
}
