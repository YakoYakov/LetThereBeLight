﻿using LetThereBeLightApp.ExtrnalCalls;
using LetThereBeLightApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
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

        private async void DiscoveryButton_Clicked(object sender, EventArgs e)
        {
            // TODO run discovery and save found devices
            //var result = await SmartBulbClient.DiscoverDevices();
            
            //var smartBulbsFromResult = result.Select(
            //    r => new SmartBulb
            //    {
            //        Id = r.deviceProperties.id,
            //        Brightness = r.deviceProperties.brightness,
            //        ColorMode = r.deviceProperties.colorMode,
            //        ColorTemperature = r.deviceProperties.colorTemperature,
            //        Hue = r.deviceProperties.hue,
            //        Saturation = r.deviceProperties.saturation,
            //        Location = r.deviceProperties.location,
            //        Power = r.deviceProperties.power == 0 ? "On" : "Off",
            //        Name = r.deviceProperties.name,
            //        RGB = r.deviceProperties.rgb,
            //    }
            //);

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();
                var smartBulbsFromResult = new List<SmartBulb>();
                //TODO remove used just for testing
                smartBulbsFromResult.Add(
                    new SmartBulb
                    {
                        Id = 33,
                        Brightness = 80,
                        ColorMode = 0,
                        ColorTemperature = 3300,
                        Hue = 0,
                        Saturation = 0,
                        Location = "fghfgh",
                        Power = "On",
                        Name = "Benny lamp",
                        RGB = 123,
                    });

                smartBulbsFromResult.ToList().Add(
                   new SmartBulb
                   {
                       Id = 354,
                       Brightness = 80,
                       ColorMode = 0,
                       ColorTemperature = 3300,
                       Hue = 0,
                       Saturation = 0,
                       Location = "fghfgh",
                       Power = "Off",
                       Name = "Yako`s lamp",
                       RGB = 123,
                   });

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

        private void Device_Selected(object sender, EventArgs e)
        {
            if (devicesListView.SelectedItem is SmartBulb currentDevice)
            {
                Navigation.PushAsync(new DevicesPage(currentDevice), true);
            }
        }
    }
}
