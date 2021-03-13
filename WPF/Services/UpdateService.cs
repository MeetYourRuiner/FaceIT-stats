using FaceitStats.WPF.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FaceitStats.WPF.Services
{
    class UpdateService : IUpdateService
    {
        private static string URL = "https://api.github.com/repos/MeetYourRuiner/FaceIT-stats/releases/latest";
        private static string updateLink;

        public string CurrentVersion
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public async Task<bool> CheckForUpdate()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.UserAgent = "request";
            JObject deserializedResponse;
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    Trace.WriteLine(((HttpWebResponse)response).StatusDescription);
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseFromServer = reader.ReadToEnd();
                            deserializedResponse = JObject.Parse(responseFromServer);
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Could not find the update");
            }
            var latestVersion = ((string)deserializedResponse["tag_name"]).Substring(1);
            var currentVersion = CurrentVersion;
            if (latestVersion == currentVersion)
                return false;

            var latestVersionNums = latestVersion.Split('.').Select(n => int.Parse(n)).ToArray(); // 1.0.0.0
            var currentVersionNums = currentVersion.Split('.').Select(n => int.Parse(n)).ToArray();
            for (int i = 0; i < 4; i++)
            {
                if (latestVersionNums[i] < currentVersionNums[i])
                {
                    return false;
                }
            }
            updateLink = (string)deserializedResponse["assets"][0]["browser_download_url"];
            return true;
        }

        public async Task UpdateAsync(Action<string> updateProgressSetter)
        {

            string newfilename = "update.exe";
            string oldfilename = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (o, args) =>
                {
                    updateProgressSetter(args.ProgressPercentage.ToString());
                };
                await wc.DownloadFileTaskAsync(new System.Uri(updateLink), newfilename);
            }
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
    }
}
