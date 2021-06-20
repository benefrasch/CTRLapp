using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.General
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class General : ContentView
    {
        public General()
        {
            InitializeComponent();
            LoadSettings();
        }

        private bool load;
        private void LoadSettings()
        {
            load = true;
            //---Broker settings
            brokerIp.Text = Preferences.Get("brokerIp", "");
            deviceName.Text = Preferences.Get("deviceName", "");
            brokerUsername.Text = Preferences.Get("brokerUsername", "");
            brokerPassword.Text = Preferences.Get("brokerPassword", "");
            //---Master Password
            settingsPasword.Text = Preferences.Get("SettingsPassword", "");
            //---DarkMode
            darkModeSwitch.IsToggled = Application.Current.RequestedTheme == OSAppTheme.Dark;
            load = false;
        }

        private void SaveSettings(object _, EventArgs e)
        {
            if (load) return;
            Debug.WriteLine("save");
            //---Broker settings
            Preferences.Set("brokerIp", brokerIp.Text);
            Preferences.Set("deviceName", deviceName.Text);
            Preferences.Set("brokerUsername", brokerUsername.Text);
            Preferences.Set("brokerPassword", brokerPassword.Text);
            //---Master Password
            Preferences.Set("SettingsPassword", settingsPasword.Text);
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