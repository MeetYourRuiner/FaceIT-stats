using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace faceitwpf.Classes
{
    class UpdateManager
    {
        private static string URL = "https://api.github.com/repos/MeetYourRuiner/FaceIT-stats/releases/latest";
        private static string updateLink;
        public static async Task<string> CheckForUpdate()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.UserAgent = "request";
            WebResponse response = await request.GetResponseAsync();
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
            var latestVersion = ((string)deserializedResponse["tag_name"]).Substring(1);
            var currentVersion = GetCurrentVersion();
            if (latestVersion == currentVersion)
                return null;

            var latestVersionNums = latestVersion.Split('.'); // 1.0.0.0
            var currentVersionNums = GetCurrentVersion().Split('.');
            for (int i = 0; i < 4; i++)
            {
                if (int.Parse(latestVersionNums[i]) < int.Parse(currentVersionNums[i]))
                {
                    return null;
                }
            }
            updateLink = (string)deserializedResponse["assets"][0]["browser_download_url"];
            return latestVersion;
        }

        public static async Task Update()
        {
            string newfilename = "update.exe";
            string oldfilename = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            using (WebClient wc = new WebClient())
                await wc.DownloadFileTaskAsync(new System.Uri(updateLink), newfilename);
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            using (StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.GetEncoding(866)))
            {
                streamWriter.WriteLine($"RENAME \"{oldfilename}\" \"old.exe\"");
                streamWriter.WriteLine($"RENAME \"{newfilename}\" \"{oldfilename}\"");
                streamWriter.WriteLine($"\"{oldfilename}\" -updated");
            }
            process.WaitForExit();
            process.Close();
            System.Windows.Application.Current.Shutdown();
        }

        public static string GetCurrentVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}
