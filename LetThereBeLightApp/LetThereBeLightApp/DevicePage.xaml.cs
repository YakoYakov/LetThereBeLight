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
            var payload = new { SmartBulbId = _device.Id };
            var result = await SmartBulbClient.SendCommandAsync(TOGGLE_ENDPOINT, payload);
            ;
            //TODO make call to turn off on lamp
            //Persist data to db
        }

        private async void BrightnessSlider_DragCompleted(object sender, EventArgs e)
        {
            var payload = new { SmartBulbId = _device.Id, Brightness = (int)Math.Round(brightnessSlider.Value) };
            var result = await SmartBulbClient.SendCommandAsync(CHANGE_BRIGHTNESS_ENDPOINT, payload);

            //TODO send  command for changing brightness
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();
                int rows = connection.Update(_device);

                //if (rows > 0)
                //{
                //    DisplayAlert("Success", "Device Brightness was changed", "Got It");

                //}
                //else
                //{
                //    DisplayAlert("Note", "No new devices were found!", "Got It");
                //}
            }
        }

        private async void WarmSlider_DragCompleted(object sender, EventArgs e)
        {
            var payload = new { SmartBulbId = _device.Id, ColorTemperature = (int)Math.Round(warmSlider.Value) };
            var result = await SmartBulbClient.SendCommandAsync(CHANGE_COLOR_TEMPERATURE_ENDPOINT, payload);

            //TODO send  command for changing color temperature
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();
                int rows = connection.Update(_device);

                //if (rows > 0)
                //{
                //    DisplayAlert("Success", "Device Brightness was changed", "Got It");

                //}
                //else
                //{
                //    DisplayAlert("Note", "No new devices were found!", "Got It");
                //}
            }
        }

        private async void UpdateColor(object sender, EventArgs e)
        {
            var selectedColor = ColorWheel1.SelectedColor.ToSKColor();
            var payload = new { SmartBulbId = _device.Id, R = selectedColor.Red, G = selectedColor.Green, B = selectedColor.Blue };
            var result = await SmartBulbClient.SendCommandAsync(CHANGE_RGB_ENDPOINT, payload);

        }

        private async void UpdateName(object sender, EventArgs e)
        {
            if (deviceName.Text != null)
            {
                var newName = deviceName.Text.Trim();

                var payload = new { SmartBulbId = _device.Id, Name = deviceName.Text.Trim()};
                var result = await SmartBulbClient.SendCommandAsync(CHANGE_NAME_ENDPOINT, payload);
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
    }
}