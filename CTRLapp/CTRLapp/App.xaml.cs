using CTRLapp.Views;
using Xamarin.Forms;

namespace CTRLapp
{
    public partial class App : Application
    {

        public App()
        {
            Device.SetFlags(new string[] { "DragAndDrop_Experimental" });
            InitializeComponent();
            MainPage =  new NavigationPage( new MainPage());
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
