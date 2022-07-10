using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

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

        public static async void Send(string ctr, string act, string data=null)
        {
            try
            {
                string constr = constring + ctr + '/' + act;
                Trace.WriteLine("POST " + constr);
                if (data != null)
                {
                    Trace.WriteLine("POST " + data);
                    StringContent stringContent = new StringContent(data);
                    HttpResponseMessage request = await client.PostAsync(constr, stringContent);
                    Trace.WriteLine(request);
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync(constr);
                    response.EnsureSuccessStatusCode();
                    Trace.WriteLine("POST  " + response);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine("RESPONSE" + responseBody.ToString());
                    //ParsData(ctr, window, responseBody);
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                //MessageBox.Show("Не удается подключится к серверу");
            }
        }
    }
}
