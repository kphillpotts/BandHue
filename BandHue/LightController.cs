using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;


namespace BandHue
{
    public class LightController
    {
        public LightController()
        {
        }

        public static void PulseLight(int i, int i2, int i3)
        {
            Debug.WriteLine($"Pulse {i} {i2} {i3}");
        }

        private static void LightCommand(bool lightStatus)
        {
            // connection information
            string username = "3ee51605220dae071f6a8fd11df35b33";
            string ipAdress = "192.168.1.130";
            Uri uri = new Uri(string.Format("http://{0}/api/{1}/lights/1/state", ipAdress, username));

            //  jSON request to send to the light
            var lightRequest = new 
                {
                    on = lightStatus,
                    bri=255,
                    sat=255,
                    effect="colorloop"
                };
            var jsonLightRequest = JsonConvert.SerializeObject(lightRequest);
            var httpContent = new StringContent(jsonLightRequest, System.Text.Encoding.UTF8, "application/json");

            // post request to hue system
            HttpClient client = new HttpClient();
            client.PutAsync(uri,httpContent);
        }  

    }
}

