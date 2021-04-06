using CTRLapp.Views;
using System.Diagnostics;
using Xamarin.Forms;

namespace CTRLapp
{
    public partial class App : Application
    {

        public App()
        {
            Device.SetFlags(new string[] { "DragAndDrop_Experimental" });
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            Debug.WriteLine("start");
        }

        protected override void OnSleep()
        {
            Debug.WriteLine("sleep");
        }

        protected override void OnResume()
        {
            Debug.WriteLine("resume");
        }



    }
}
