using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArsShina_Bot.Http
{
    class Post
    {
        private static string constring;
        static readonly HttpClient client = new HttpClient();
        public Post()
        {
            constring = Config.GetString();

        }

        public static async Task<string> Send(string ctr, string act, string data=null)
        {
            try
            {
                string constr = constring + ctr + '/' + act;
                Trace.WriteLine("POST " + constr);
                if (data != null)
                {
                    Trace.WriteLine("POST " + data);
                    var poststr = data.Trim('/');
                    StringContent stringContent = new StringContent(poststr);
                    HttpResponseMessage request = await client.PostAsync(constr, new StringContent(new JavaScriptSerializer().Serialize(data), Encoding.UTF8, "application/json"));
                    var ddd = request.Content.ReadAsStringAsync().Result;
                    Trace.WriteLine(ddd);
                    return request.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync(constr);
                    response.EnsureSuccessStatusCode();
                    Trace.WriteLine("POST  " + response);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine("RESPONSE" + responseBody.ToString());
                    return responseBody;
                }
                return "";

            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return "Не удается подключится к серверу";
            }
        }
    }
}
