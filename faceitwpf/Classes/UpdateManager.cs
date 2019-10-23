using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace faceitwpf.Classes
{
    class UpdateManager
    {

        private static string URL = "https://api.github.com/repos/MeetYourRuiner/FaceIT-stats/releases/latest";
        public static bool CheckForUpdate()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.UserAgent = "request";
            WebResponse response = request.GetResponse();
            JObject deserializedResponse;
            Trace.WriteLine(((HttpWebResponse)response).StatusDescription);
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                deserializedResponse = JObject.Parse(responseFromServer);
            }
            response.Close();
            var latestVersion = ((string)deserializedResponse["tag_name"]).Substring(1); // 1.0.0.0
            if (latestVersion != GetCurrentVersion())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetCurrentVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}
