using LetThereBeLightApp.ExtrnalCalls;
using LetThereBeLightApp.Models;
using SQLite;
using System;
using System.Linq;
using Xamarin.Forms;

namespace LetThereBeLightApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();

                var devices = connection.Table<SmartBulb>().ToList();
                devicesListView.ItemsSource = connection.Table<SmartBulb>().ToList();
            }
        }

        private async void DiscoveryButton_Clicked(object sender, EventArgs e)
        {
            var result = await SmartBulbClient.DiscoverDevices();

            var smartBulbsFromResult = result.Select(
                r => new SmartBulb
                {
                    Id = r.deviceProperties.id,
                    Brightness = r.deviceProperties.brightness,
                    ColorMode = r.deviceProperties.colorMode,
                    ColorTemperature = r.deviceProperties.colorTemperature,
                    Hue = r.deviceProperties.hue,
                    Saturation = r.deviceProperties.saturation,
                    Location = r.deviceProperties.location,
                    Power = r.deviceProperties.power == 0 ? "On" : "Off",
                    Name = r.deviceProperties.name,
                    RGB = r.deviceProperties.rgb,
                }
            );

            var test = new SmartBulb
            {
                Id = 11,
                Brightness = 11,
                ColorMode = 11,
                ColorTemperature = 11,
                Hue = 11,
                Saturation = 11,
                Location = "",
                Power = "Off",
                Name = "Bennys lamp",
                RGB = 1,
            };

            var smartBulbsFromResult2 = smartBulbsFromResult.ToList();
            smartBulbsFromResult2.Add( test );

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();

                foreach (var smartBulb in smartBulbsFromResult2)
                {
                    // Insert only if the device is new!
                    if (!connection.Table<SmartBulb>().ToList().Any(s => s.Id == smartBulb.Id))
                    {
                        int rows = connection.Insert(smartBulb);

                        if (rows > 0)
                        {
                            await DisplayAlert("Found new smart devices", "Added the new devices to the APP", "Got It");
                        }
                        else
                        {
                            await DisplayAlert("Note", "No new devices were found!", "Got It");
                        }
                    }
                }

                devicesListView.ItemsSource = connection.Table<SmartBulb>().ToList();
            }
        }

        private void Device_Selected(object sender, EventArgs e)
        {
            if (devicesListView.SelectedItem is SmartBulb currentDevice)
            {
                Navigation.PushAsync(new DevicesPage(currentDevice), true);
            }
        }

        private void Delete_Device(object sender, EventArgs e)
        {
            var listItem = sender as ImageButton;
            SmartBulb selectedDevice = (SmartBulb)listItem.CommandParameter;
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();
                int rows = connection.Delete(selectedDevice);

                if (rows > 0)
                {
                    devicesListView.ItemsSource = connection.Table<SmartBulb>().ToList();
                    DisplayAlert("Success", "Device was deleted", "Got It");
                }
                else
                {
                    DisplayAlert("Error", "No Device was deleted", "Got It");
                }
            }
        }
    }
}
