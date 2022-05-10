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
            grid.BindingContext = this; 
        }

        public string BrokerIp
        {
            get { return Preferences.Get("brokerIp", ""); }
            set { Preferences.Set("brokerIp", value); }
        }
        public string DeviceName
        {
            get { return Preferences.Get("deviceName", ""); }
            set { Preferences.Set("deviceName", value); }
        }
        public string BrokerUsername
        {
            get { return Preferences.Get("brokerUsername", ""); }
            set { Preferences.Set("brokerUsername", value); }
        }
        public string BrokerPassword
        {
            get { return Preferences.Get("brokerPassword", ""); }
            set { Preferences.Set("brokerPassword", value); }
        }
        public string SettingsPassword
        {
            get { return Preferences.Get("settingsPassword", ""); }
            set { Preferences.Set("settingsPassword", value); }
        }


        private void DarkModeSwitchToggled(object _, ToggledEventArgs e)
        {
            if (e.Value == false)
                Application.Current.UserAppTheme = OSAppTheme.Light;
            else
                Application.Current.UserAppTheme = OSAppTheme.Dark;
        }

        private void OpenJSONConfig(object _, EventArgs e)
        {
            Navigation.PushAsync(new JSONconfig());

        }
    }
}