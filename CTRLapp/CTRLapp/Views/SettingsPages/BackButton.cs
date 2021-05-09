
using Xamarin.Forms;

namespace CTRLapp.Views.SettingsPages
{
    public class BackButton : ContentPage
    {
        public BackButton()
        {

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}