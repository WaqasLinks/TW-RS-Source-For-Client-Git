

using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shipping.WebApi.Utility
{
    public class SMSManager
    {
        public static async Task<bool> SendSmsAsync(string toNumber, string message, string from = null)
        {
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["SMS:URL"].ToString());
                    var response = await _httpClient.GetAsync("/messages/http/send?apiKey=" + ConfigurationManager.AppSettings["SMS:APIKEY"].ToString() + "&to=" + toNumber + "&content=" + message + "");
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
