using CTRLapp.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.General
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JSONconfig : ContentPage
    {
        public JSONconfig()
        {
            InitializeComponent();

            jsonEntry.Text = JsonConvert.SerializeObject(Variables.Variables.Layout);

        }

        protected override void OnDisappearing()
        {
            Variables.Variables.Layout = JsonConvert.DeserializeObject<List<MasterMenuItem>>(jsonEntry.Text);

            base.OnDisappearing();
        }
    }
}