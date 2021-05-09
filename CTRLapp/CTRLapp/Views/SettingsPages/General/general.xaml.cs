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
            LoadSettings();
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
            broker_ip.Text = Preferences.Get("brokerIp", "");
            device_name.Text = Preferences.Get("deviceName", "");
            broker_username.Text = Preferences.Get("brokerUsername", "");
            broker_password.Text = Preferences.Get("brokerPassword", "");
            //---Master Password
            Settings_password.Text = Preferences.Get("SettingsPassword", "");
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            //---Broker settings
            Preferences.Set("brokerIp", broker_ip.Text);
            Preferences.Set("deviceName", device_name.Text);
            Preferences.Set("brokerUsername", broker_username.Text);
            Preferences.Set("brokerPassword", broker_password.Text);
            //---Master Password
            Preferences.Set("settingsPassword", Settings_password.Text);
        }

        // show passwords
        private void ShowSettingsPassword(object sender, EventArgs e)
        {
            Settings_password.IsPassword = !Settings_password.IsPassword;
        }
        private void ShowBrokerPassword(object sender, EventArgs e)
        {
            broker_password.IsPassword = !broker_password.IsPassword;
        }
    }
}