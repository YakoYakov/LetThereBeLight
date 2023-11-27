using LetThereBeLightApp.Models;
using SQLite;
using System;
using System.Linq;
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
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                // Will create table only if it does not exist.
                connection.CreateTable<SmartBulb>();
                var currentDevice = connection.Table<SmartBulb>().ToList().FirstOrDefault(l => l.Id == _device.Id);
            }
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

        private void Delete_Button_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();
                int rows = connection.Delete(_device);

                //if (rows > 0)
                //{
                //    DisplayAlert("Found new smart devices", "Added the new devices to the APP", "Got It");
                //}
                //else
                //{
                //    DisplayAlert("Note", "No new devices were found!", "Got It");
                //}
            }
        }
    }

}