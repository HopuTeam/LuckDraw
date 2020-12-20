using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LuckDraw.Handles
{
    public static class NetAction
    {
        public static string GetInfo(string url)
        {
            string responseData;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //HttpContext.Session.SetString("responseData", string.Empty);
                responseData = string.Empty;
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseData = reader.ReadToEnd();
                    reader.Close();
                }
                return responseData;
            }
            catch
            {
                return "responseData";
            }
        }
    }
}
