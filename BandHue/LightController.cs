using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;

namespace BandHue
{
    public static class LightController
    {
        static bool _lightStatus = false;

        static public void ToggleLight()
        {
            _lightStatus = !_lightStatus;
            LightCommand(_lightStatus);
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

        public static bool _pulseState = false;

        public static void PulseLight(int red, int green, int blue)
        {


            _pulseState = !_pulseState;

            var xyColor = RGBtoXY(red,green,blue);

          //  Console.WriteLine("PulseLight - " + xyColor.ToString());
            // connection information
            string username = "3ee51605220dae071f6a8fd11df35b33";
            string ipAdress = "192.168.1.130";
            Uri uri = new Uri(string.Format("http://{0}/api/{1}/lights/1/state", ipAdress, username));

            string jsonLightRequest;
            if (_pulseState)
            {
                //  jSON request to send to the light
                var lightRequest = new 
                    {
                        on = true,
                        bri = 255,
                        sat = 255,
                        //                    hue = 10000,
                        xy = xyColor,
                        transitiontime = 1
                    };
                jsonLightRequest = JsonConvert.SerializeObject(lightRequest);
            }
            else
            {
                //  jSON request to send to the light
                var lightRequest = new 
                    {
                        on = false,
                        transitiontime = 1
                    };
                jsonLightRequest = JsonConvert.SerializeObject(lightRequest);
            }


            var httpContent = new StringContent(jsonLightRequest, System.Text.Encoding.UTF8, "application/json");
            // post request to hue system
            HttpClient client = new HttpClient();
            client.PutAsync(uri,httpContent);
        }  

        public static List<Double> RGBtoXY(int ired, int igreen, int iblue) 
        {
            // For the hue bulb the corners of the triangle are:
            // -Red: 0.675, 0.322
            // -Green: 0.4091, 0.518
            // -Blue: 0.167, 0.04
            double[] normalizedToOne = new double[3];
            float cred, cgreen, cblue;
            //            cred = c.getRed();
            //            cgreen = c.getGreen();
            //            cblue = c.getBlue();
            normalizedToOne[0] = (ired / 255);
            normalizedToOne[1] = (igreen / 255);
            normalizedToOne[2] = (iblue / 255);
            float red, green, blue;

            // Make red more vivid
            if (normalizedToOne[0] > 0.04045) {
                red = (float) Math.Pow(
                    (normalizedToOne[0] + 0.055) / (1.0 + 0.055), 2.4);
            } else {
                red = (float) (normalizedToOne[0] / 12.92);
            }

            // Make green more vivid
            if (normalizedToOne[1] > 0.04045) {
                green = (float) Math.Pow((normalizedToOne[1] + 0.055)
                    / (1.0 + 0.055), 2.4);
            } else {
                green = (float) (normalizedToOne[1] / 12.92);
            }

            // Make blue more vivid
            if (normalizedToOne[2] > 0.04045) {
                blue = (float) Math.Pow((normalizedToOne[2] + 0.055)
                    / (1.0 + 0.055), 2.4);
            } else {
                blue = (float) (normalizedToOne[2] / 12.92);
            }

            float X = (float) (red * 0.649926 + green * 0.103455 + blue * 0.197109);
            float Y = (float) (red * 0.234327 + green * 0.743075 + blue * 0.022598);
            float Z = (float) (red * 0.0000000 + green * 0.053077 + blue * 1.035763);

            float x = X / (X + Y + Z);
            float y = Y / (X + Y + Z);

            double[] xy = new double[2];
            xy[0] = x;
            xy[1] = y;

            List<Double> xyAsList = new List<double> { x, y };

            return xyAsList;
        }
    }
}

