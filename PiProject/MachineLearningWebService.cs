using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PiProject
{
    public class MachineLearningWebServiceResponse
    {
        public Dictionary<string, List<Dictionary<string, string>>> Results { get; set; }
    }

    public static class MachineLearningWebService
    {
        public static async Task<string> ScoreMeasurement(Measurement mes)
        {
            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                    {
                        "measurement",
                        mes.ToJSON()
                    }
                },
                GlobalParameters = new Dictionary<string, string>() { }
            };

            string labelName = null;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AzureMLApiKey);
                client.BaseAddress = new Uri(Settings.AzureMLAddress);

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseObj = JsonConvert.DeserializeObject<MachineLearningWebServiceResponse>(responseContent);

                    labelName = responseObj.Results["label"][0]["Scored Labels"];
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog($"Error:\n{responseContent}").ShowAsync();
                    return null;
                }
            }

            return labelName;
        }
    }
}
