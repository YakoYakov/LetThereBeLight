using LetThereBeLightApp.Models;
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
    }
}