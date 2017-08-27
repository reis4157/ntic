using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HepsiBenimMi.Utility
{
    public class WebOperations
    {
        public static Byte[] GetImage(string Url)
        {
            using (WebClient webClient = new WebClient())
            {
                return webClient.DownloadData(Url);
            }
            return new Byte[] { };
        }

        public static String GetSourceCode(string Url)
        {
            int tryCount = 0;
            while (tryCount < 3)
            {
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
                    myRequest.Method = "GET";
                    WebResponse myResponse = myRequest.GetResponse();
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                    string result = sr.ReadToEnd();
                    sr.Close();
                    myResponse.Close();

                    return result;
                }
                catch (Exception ex)
                {
                    tryCount++;
                    Thread.Sleep(500);
                }
            }
            return "";
           
        }

        public static void GetFile(string Url)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(Url, @"c:\Users\Jon\Test\foo.txt");
            }
        }
    }
}
