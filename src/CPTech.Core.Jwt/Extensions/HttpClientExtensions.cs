using CPTech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cptech.Extension
{
    public static class HttpClientExtensions
    {
        //public static async Task<string> GetStringAsync(this HttpClient client, string url)
        //{
        //    var body = await client.GetAsync(url);
        //    return await body.Content.ReadAsStringAsync();
        //}

        public static async Task<string> PostAsync(this HttpClient client, string url, HttpContent content)
        {
            var body = await client.PostAsync(url, content);
            return await body.Content.ReadAsStringAsync();
        }
    }
}
