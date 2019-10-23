using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace faceitwpf.Classes
{
    class UpdateManager
    {
        private static string URL = "https://api.github.com/repos/MeetYourRuiner/FaceIT-stats/releases/latest";
        private static string updateLink;
        public static bool CheckForUpdate()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.UserAgent = "request";
            WebResponse response = request.GetResponse();
            JObject deserializedResponse;
            Trace.WriteLine(((HttpWebResponse)response).StatusDescription);
            using (Stream dataStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    deserializedResponse = JObject.Parse(responseFromServer);
                }
            }
            response.Close();
            var latestVersion = ((string)deserializedResponse["tag_name"]).Substring(1).Split('.'); // 1.0.0.0
            var currentVestion = GetCurrentVersion();
            for (int i = 0; i < 4; i++)
            {
                if (int.Parse(latestVersion[i]) > int.Parse(currentVestion[i]))
                {
                    updateLink = (string)deserializedResponse["assets"][0]["browser_download_url"];
                    return true;
                }
            }
            return false;
        }

        public static async Task Update()
        {
            string newfilename = "newfaceit-100ts.exe";
            string oldfilename = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            using (WebClient wc = new WebClient())
                await wc.DownloadFileTaskAsync(new System.Uri(updateLink), newfilename);
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    RedirectStandardInput = true
                }
            };
            process.Start();
            process.StandardInput.WriteLine($"RENAME {oldfilename} old.exe");
            process.StandardInput.WriteLine($"RENAME {newfilename} {oldfilename}");
            process.StandardInput.WriteLine($"{oldfilename} -updated");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            process.Close();
            System.Windows.Application.Current.Shutdown();
        }

        private static string[] GetCurrentVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion.Split('.');
        }
    }
}
