using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class General : ContentPage
    {
        public General()
        {
            InitializeComponent();
            Load_settings();
        }

        protected override void OnAppearing()
        {
            Load_settings();
            MQTT.DisconnectMQTT();
        }
        protected override void OnDisappearing()
        {
            Save_settings(null, null);
        }


        private void Load_settings()
        {
            //---Broker settings
            broker_ip.Text = Preferences.Get("broker_ip", "");
            device_name.Text = Preferences.Get("device_name", "");
            broker_username.Text = Preferences.Get("broker_username", "");
            broker_password.Text = Preferences.Get("broker_password", "");
            //---Master Password
            Settings_password.Text = Preferences.Get("Settings_password_list", "");
        }

        private void Save_settings(object sender, EventArgs e)
        {
            //---Broker settings
            Preferences.Set("broker_ip", broker_ip.Text);
            Preferences.Set("device_name", device_name.Text);
            Preferences.Set("broker_username", broker_username.Text);
            Preferences.Set("broker_password", broker_password.Text);
            //---Master Password
            Preferences.Set("Settings_password_list", Settings_password.Text);
        }

        // show passwords
        private void Show_settings_password(object sender, EventArgs e)
        {
            Settings_password.IsPassword = !Settings_password.IsPassword;
        }
        private void Show_broker_password(object sender, EventArgs e)
        {
            broker_password.IsPassword = !broker_password.IsPassword;
        }
    }
}