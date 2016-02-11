using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using AdvancedTimer.Forms.Plugin.Abstractions;

namespace BandHue
{
    public partial class MainPage : ContentPage
    {
        MainViewModel ViewModel;
        IAdvancedTimer heartTimer;
        private bool IsTimerConfigured = false;

        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = ViewModel;
            heartTimer = DependencyService.Get<IAdvancedTimer>();
            ConfigureTimer();
            TestValue.ValueChanged += TestValue_ValueChanged;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

//            ViewModel.Beat += async delegate(int obj)
//            {
//                    var b = await AnimateHeart();
//
//            };
//            MessagingCenter.Subscribe<MainViewModel, int> (this, "Beat", async (sender, arg) =>  {
//                // do something whenever the "Hi" message is sent
//                // using the 'arg' parameter which is a string
//                await AnimateHeart();
//            });
        }

        void TestValue_ValueChanged (object sender, ValueChangedEventArgs e)
        {
            ViewModel.BeatsPerMinute = (int)e.NewValue;
        }

        void ViewModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine(e.PropertyName);

            if (e.PropertyName == nameof(ViewModel.BeatsPerMinute) || e.PropertyName == nameof(ViewModel.BeatQuality))
                UpdateBeats();
            
//            if (e.PropertyName == "HrValue")
//            {
//                // update BPM
//
//            }
        }

        async public void AnimateHeart_Clicked(object sender, EventArgs e)
        {
            await AnimateHeart();
        }

        async private Task<bool> AnimateHeart()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(
                async () =>
                {
                    await HeartImage.ScaleTo(1.2, 50, Easing.SinInOut);
                    await HeartImage.ScaleTo(1, 50, Easing.SinInOut);
                });
            return true;
        }

        async private void TimerElapsed(object sender, EventArgs e)
        {
            heartTimer.stopTimer();

            //MessagingCenter.Send<MainViewModel, int> (this, "Beat", Pulse);
//            Beat(Pulse);

            await AnimateHeart();

            if (ViewModel.BeatsPerMinute > 100)
                LightController.PulseLight(255, 0, 0);
            else if (ViewModel.BeatsPerMinute > 60)
                LightController.PulseLight(0, 255, 0);
            else if (ViewModel.BeatsPerMinute > 40)
                LightController.PulseLight(0, 0, 255);
            else
                LightController.PulseLight(255, 255, 255);

            if (ViewModel.MsPerBeat > 0)
            {
                heartTimer.setInterval(ViewModel.MsPerBeat);
                heartTimer.startTimer();
            }
        }

        private void ConfigureTimer()
        {
            if (!IsTimerConfigured)
            {
                heartTimer.initTimer(3000, TimerElapsed, false);
            }
            IsTimerConfigured = true;
        }

        private void UpdateBeats()
        {
            // if the timer is not going set it to fire
            if ((!heartTimer.isTimerEnabled()))
            {
                if (ViewModel.MsPerBeat != 0)
                {
                    // set the interval
                    heartTimer.setInterval(ViewModel.MsPerBeat);
                    heartTimer.startTimer();
                }
            }
        }
    }
}

