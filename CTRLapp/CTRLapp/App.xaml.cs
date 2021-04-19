using CTRLapp.Objects;
using CTRLapp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            string Config = "";
            if (File.Exists(Variables.Variables.configLocation))
                Config = File.ReadAllText(Variables.Variables.configLocation);
            Debug.WriteLine(Config);
            Variables.Variables.Layout = JsonConvert.DeserializeObject<List<Master_Menu_Item>>(Config);
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }



    }
}
