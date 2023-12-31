﻿using LetThereBeLightApp.ExtrnalCalls;
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
            ai.IsRunning = true;
            try
            {
                var result = await SmartBulbClient.DiscoverDevices();
                ai.IsRunning = false;

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

                using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
                {
                    connection.CreateTable<SmartBulb>();

                    foreach (var smartBulb in smartBulbsFromResult)
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Got It");
                ai.IsRunning = false;
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
