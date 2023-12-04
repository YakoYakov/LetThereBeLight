using LetThereBeLightApp.Models;
using SkiaSharp.Views.Forms;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
        }

        private void BrightnessSlider_DragCompleted(object sender, EventArgs e)
        {
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

        private void WarmSlider_DragCompleted(object sender, EventArgs e)
        {
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


        private void UpdateColor(object sender, EventArgs e)
        {
            var selectedColor = ColorWheel1.SelectedColor.ToSKColor();
        }

        private void UpdateName(object sender, EventArgs e)
        {
            if (deviceName.Text != null)
            {
                var newName = deviceName.Text.Trim();
            }
        }
    }
}