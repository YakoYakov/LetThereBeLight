using LetThereBeLightApp.ExtrnalCalls;
using LetThereBeLightApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
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
            var test = await SmartBulbClient.DiscoverDevices();


            // sett the list view correctly with the discovered devices
            devicesListView.ItemsSource = new List<SmartBulb>();


            SmartBulb smartBulb = new SmartBulb();

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseConnectionString))
            {
                connection.CreateTable<SmartBulb>();

                //TODO check if lamb is allready inserted
                int rows = connection.Insert(smartBulb);

                if (rows > 0)
                {
                    DisplayAlert("Found new smart devices", "Added the new devices to the APP", "Got It");
                }
                else
                {
                    DisplayAlert("Note", "No new devices were found!", "Got It");
                }
            }
        }

        private void Device_Selected(object sender, EventArgs e)
        {
            var currentDevice = devicesListView.SelectedItem as SmartBulb;

            if (currentDevice != null)
            {
                Navigation.PushAsync(new DevicesPage(currentDevice), true);
            }

        }
    }
}
