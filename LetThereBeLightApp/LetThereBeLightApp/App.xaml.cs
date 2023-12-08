using Xamarin.Forms;

namespace LetThereBeLightApp
{
    public partial class App : Application
    {
        public static string DatabaseConnectionString = string.Empty;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string connectionString)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            DatabaseConnectionString = connectionString;
        }



        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
