using System;
using Xamarin.Forms;

namespace LetThereBeLightApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void DiscoveryButton_Clicked(object sender, EventArgs e)
        {
           // TODO run discovery and save found devices
        }

        private void Device_Clicked(object sender, EventArgs e)
        {
            // TODO run discovery and save found devices
            Navigation.PushAsync(new DevicesPage(), true);
        }
    }
}
