/*
 * Copyright - VMware, Inc.
 * SPDX-License-Identifier: EPL-2.0
 */

using System;
using System.IO;
using System.Net;
using System.Text;

namespace SingletonClient.Implementation.Support
{
    public class SingletonAccessService : IAccessService
    {
        public string HttpGet(string url)
        {
            string result = "";

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }

        public string HttpPost(string url, string text)
        {
            if (text == null)
            {
                return null;
            }

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            byte[] bs = Encoding.UTF8.GetBytes(text);
            string responseData = null;
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = bs.Length;

            try
            {
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return responseData;
        }
    }
}

