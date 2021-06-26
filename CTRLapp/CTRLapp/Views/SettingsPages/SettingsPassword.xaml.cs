using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPassword : ContentPage
    {
        readonly Page settingspage;
        readonly string adminPassword = Preferences.Get("SettingsPassword", "");
        private bool wasThere; //so it just dissapears when leaving settings

        public SettingsPassword(Page settingspage)
        {
            InitializeComponent();
            this.settingspage = settingspage;
        }

        protected override void OnAppearing()
        {

            if (wasThere)
            {
                Navigation.PopAsync();
                return;
            }
            wasThere = true;
            if (adminPassword == "") PushSettingsPage();
            base.OnAppearing();
        }

        private async void PushSettingsPage()
        {
            await Navigation.PushAsync(settingspage);
        }

        private void PassowrdEntryCompleted(object _, EventArgs e)
        {
            if (passowrdEntry.Text == adminPassword) PushSettingsPage();
            else DisplayAlert("wrong passowrd", "", "OK");
        }

        private async void BackButtonPressed(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}