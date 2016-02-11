using System;
using System.Windows.Input;
using Xamarin.Forms;
using Microsoft.Band.Portable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band.Portable.Sensors;
using AdvancedTimer.Forms.Plugin.Abstractions;
using Microsoft.Band.Portable.Personalization;

namespace BandHue
{


    public class MainViewModel : BaseViewModel
    {

        public BandClient Client { get; set; }

        private bool isConnected = false;

        public bool IsConnected
        {
            get
            {
                if (Client == null)
                    return false;

                return Client.IsConnected;
            }
        }

        string lastMessage;

        public string LastMessage
        {
            get
            {
                return lastMessage;
            }
            set
            {
                lastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }


        public ICommand ConnectCommand { get; set; }

        public ICommand ConnectHRCommand { get; set; }

        public enum ReadingQuality
        {
            NotConneted = 0,
            Unknown = 1,
            Aquiring = 2,
            Locked = 3
        }

        private int beatsPerMinute;

        public int BeatsPerMinute
        {
            get
            {
                return beatsPerMinute;
            }
            set
            {
                if (beatsPerMinute == value)
                    return;

                beatsPerMinute = value;
                OnPropertyChanged(nameof(BeatsPerMinute));
            }
        }

        public int MsPerBeat
        {
            get
            {
                if (BeatsPerMinute == 0)
                    return 0;
                else
                    return 1000 * 60 / BeatsPerMinute;
            }
        }

        private HeartRateQuality beatQuality = HeartRateQuality.Unknown;

        public HeartRateQuality BeatQuality
        {
            get
            {
                return beatQuality;
            }
            set
            {
                if (beatQuality == value)
                    return;

                beatQuality = value;
                OnPropertyChanged(nameof(BeatQuality));
            }
        }


        public event Action<int> Beat;

        public MainViewModel()
        {
            ConnectCommand = new Command(Connect);
            ConnectHRCommand = new Command(async() => await ConnectHr());
        }

        async public void Connect()
        {
            // if we are already connected, do nothing
            if (IsConnected)
                return;

            var bandClientManager = BandClientManager.Instance;
            // query the service for paired devices
            var pairedBands = await bandClientManager.GetPairedBandsAsync();

            if (pairedBands.Count() == 0)
            {
                LastMessage = "No bands paired to this device";
                OnPropertyChanged("IsConnected");
                return;
            }

            // connect to the first device
            var bandInfo = pairedBands.FirstOrDefault();

            Client = await bandClientManager.ConnectAsync(bandInfo);
            LastMessage = bandInfo.Name;
            OnPropertyChanged("IsConnected");
        }

        async public Task<bool> ConnectHr()
        {
            if (!IsConnected)
            {
                LastMessage = "Can't connect to HR, band not connected";
                return false;
            }

            var sensorManager = Client.SensorManager;
            // get the heart rate sensor
            var heartRate = sensorManager.HeartRate;
            // add a handler
            heartRate.ReadingChanged += (o, args) =>
            {
                    BeatQuality = args.SensorReading.Quality;

                    BeatsPerMinute = args.SensorReading.HeartRate;
//                if (args.SensorReading.Quality == HeartRateQuality.Locked)
//                {
//                    Pulse = (int)args.SensorReading.HeartRate;
//                    int calculated = 1000 * 60 / (int)args.SensorReading.HeartRate;
//                    UpdateBeats(calculated);
//                }
//                else
//                {
//                    UpdateBeats(0);
//                }
//                HrConnectionDisplay = args.SensorReading.Quality.ToString();
//                HrValue =  args.SensorReading.HeartRate.ToString();
            };
            if (heartRate.UserConsented == UserConsent.Unspecified)
            {
                bool granted = await heartRate.RequestUserConsent();
            }
            if (heartRate.UserConsented == UserConsent.Granted)
            {
                // start reading, with the interval
                await heartRate.StartReadingsAsync(BandSensorSampleRate.Ms16);
            }
            else
            {
                // user declined
            }
            // stop reading
            //await heartRate.StopReadingsAsync();
            return true;
        }

    }
}

