using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.General
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class General : ContentPage
    {
        public General()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            LoadSettings();
        }
        protected override void OnDisappearing()
        {
            SaveSettings(null, null);
        }


        private void LoadSettings()
        {
            //---Broker settings
            brokerIp.Text = Preferences.Get("brokerIp", "");
            deviceName.Text = Preferences.Get("deviceName", "");
            brokerUsername.Text = Preferences.Get("brokerUsername", "");
            brokerPassword.Text = Preferences.Get("brokerPassword", "");
            //---Master Password
            settingsPasword.Text = Preferences.Get("SettingsPassword", "");
            //---DarkMode
            darkModeSwitch.IsToggled = (Application.Current.RequestedTheme == OSAppTheme.Dark);
        }

        private void SaveSettings(object _, EventArgs e)
        {
            //---Broker settings
            Preferences.Set("brokerIp", brokerIp.Text);
            Preferences.Set("deviceName", deviceName.Text);
            Preferences.Set("brokerUsername", brokerUsername.Text);
            Preferences.Set("brokerPassword", brokerPassword.Text);
            //---Master Password
            Preferences.Set("settingsPassword", settingsPasword.Text);
        }

        // show passwords
        private void ShowSettingsPassword(object _, EventArgs e)
        {
            settingsPasword.IsPassword = !settingsPasword.IsPassword;
        }
        private void ShowBrokerPassword(object _, EventArgs e)
        {
            brokerPassword.IsPassword = !brokerPassword.IsPassword;
        }

        private void DarkModeSwitchToggled(object _, ToggledEventArgs e)
        {
            if (e.Value == false)
                Application.Current.UserAppTheme = OSAppTheme.Light;
            else
                Application.Current.UserAppTheme = OSAppTheme.Dark;
        }
    }
}