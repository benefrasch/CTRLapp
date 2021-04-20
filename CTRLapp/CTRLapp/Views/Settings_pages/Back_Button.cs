
using Xamarin.Forms;

namespace CTRLapp.Views.Settings_pages
{
    public class Back_Button : ContentPage
    {
        public Back_Button()
        {

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Navigation.PopModalAsync();
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}