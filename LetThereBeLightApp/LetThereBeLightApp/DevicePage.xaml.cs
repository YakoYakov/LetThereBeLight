using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetThereBeLightApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicesPage : ContentPage
    {
        public DevicesPage()
        {
            InitializeComponent();
        }


        private void BrightnessSlider_DragCompleted(object sender, EventArgs e)
        {
            //TODO send  command for changing brightness
        }
    }

}