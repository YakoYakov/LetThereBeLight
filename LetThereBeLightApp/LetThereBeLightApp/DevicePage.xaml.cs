using LetThereBeLightApp.ExtrnalCalls;
using LetThereBeLightApp.Models;
using SkiaSharp.Views.Forms;
using SQLite;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static LetThereBeLightApp.Constants.Constants;

namespace LetThereBeLightApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicesPage : ContentPage
    {
        private SmartBulb _device { get; set; }

        public DevicesPage(SmartBulb device)
        {
            InitializeComponent();
            _device = device;
            BindingContext = _device;
            brightnessSlider.Value = _device.Brightness;
            warmSlider.Value = _device.ColorTemperature;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await ChangeBrightnessToolTipPosition();
            await ChangeWarmToolTipPosition();
        }

        private async void SwitchOnOff(object sender, EventArgs e)
        {
            TurnOnLoader();
            var payload = new { SmartBulbId = _device.Id };
            var result = await SmartBulbClient.SendCommandAsync(TOGGLE_ENDPOINT, payload);
            TurnOffLoader();

            if (result.Success)
            {
                using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
                {
                    connection.CreateTable<SmartBulb>();
                    _device.Power = _device.Power == "On" ? _device.Power = "Off" : _device.Power = "On";
                    connection.Update(_device);
                }
            }
            else
            {
                await DisplayAlert(
                    "Operation Failed!",
                    result.Message,
                    "Got It");
            }
        }

        private async void BrightnessSlider_DragCompleted(object sender, EventArgs e)
        {
            TurnOnLoader();
            var payload = new { SmartBulbId = _device.Id, Brightness = (int)Math.Round(brightnessSlider.Value) };
            var result = await SmartBulbClient.SendCommandAsync(CHANGE_BRIGHTNESS_ENDPOINT, payload);
            TurnOffLoader();

            if (result.Success)
            {
                using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
                {
                    connection.CreateTable<SmartBulb>();
                    _device.Brightness = (int)Math.Round(brightnessSlider.Value);
                    connection.Update(_device);
                }
            }
            else
            {
                await DisplayAlert(
                    "Operation Failed!",
                    result.Message,
                    "Got It");
            }
        }

        private async void WarmSlider_DragCompleted(object sender, EventArgs e)
        {
            TurnOnLoader();
            var payload = new { SmartBulbId = _device.Id, ColorTemperature = (int)Math.Round(warmSlider.Value) };
            var result = await SmartBulbClient.SendCommandAsync(CHANGE_COLOR_TEMPERATURE_ENDPOINT, payload);
            TurnOffLoader();

            if (result.Success)
            {
                using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
                {
                    connection.CreateTable<SmartBulb>();
                    _device.ColorTemperature = (int)Math.Round(warmSlider.Value);
                    connection.Update(_device);
                }
            }
            else
            {
                await DisplayAlert(
                    "Operation Failed!",
                    result.Message,
                    "Got It");
            }
        }

        private async void UpdateColor(object sender, EventArgs e)
        {
            TurnOnLoader();
            var selectedColor = ColorWheel1.SelectedColor.ToSKColor();
            var payload = new { SmartBulbId = _device.Id, R = selectedColor.Red, G = selectedColor.Green, B = selectedColor.Blue };
            var result = await SmartBulbClient.SendCommandAsync(CHANGE_RGB_ENDPOINT, payload);
            TurnOffLoader();

            if (!result.Success)
            {

                await DisplayAlert(
                    "Operation Failed!",
                    result.Message,
                    "Got It");
            }
        }

        private async void UpdateName(object sender, EventArgs e)
        {
            if (deviceName.Text != null)
            {
                TurnOnLoader();
                var payload = new { SmartBulbId = _device.Id, Name = deviceName.Text.Trim() };
                var result = await SmartBulbClient.SendCommandAsync(CHANGE_NAME_ENDPOINT, payload);
                TurnOffLoader();

                if (result.Success)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
                    {
                        connection.CreateTable<SmartBulb>();
                        _device.Name = deviceName.Text.Trim();
                        connection.Update(_device);
                    }
                }
                else
                {
                    await DisplayAlert(
                        "Operation Failed!",
                        result.Message,
                        "Got It");
                }
            }
        }

        private async void brightnessSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            await ChangeBrightnessToolTipPosition();
        }

        private async void warmSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            await ChangeWarmToolTipPosition();
        }
        private async Task ChangeBrightnessToolTipPosition()
        {
            brightnessToolTip.Text = Math.Round(brightnessSlider.Value).ToString();
            await brightnessToolTip.TranslateTo(brightnessSlider.Value * ((brightnessSlider.Width - 40) / brightnessSlider.Maximum), 0, 100);
        }

        private async Task ChangeWarmToolTipPosition()
        {
            colorTemperatureToolTip.Text = Math.Round(warmSlider.Value).ToString();
            await colorTemperatureToolTip.TranslateTo(
                (warmSlider.Value - 2700) * ((warmSlider.Width - 40) / 3800), 0, 100);
        }

        private void TurnOnLoader() 
        {
            ai.IsRunning = true;
            loader.IsVisible = true;
            loaderImage.IsVisible = true;
        }

        private void TurnOffLoader() 
        {
            ai.IsRunning = false;
            loader.IsVisible = false;
            loaderImage.IsVisible = false;
        }
    }
}