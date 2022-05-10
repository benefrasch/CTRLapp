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

        private void SaveButtonPressed(object _, EventArgs e)
        {
            //convert json back to Layout so app can use it
            Variables.Variables.Layout = JsonConvert.DeserializeObject<List<MasterMenuItem>>(jsonEntry.Text);
            Navigation.PopAsync();
        }

        private async void DiscardButtonPressed(object _, EventArgs e)
        {
            if (await App.Current.MainPage.DisplayAlert("Discard changes", "Do you really want to discard your changes?", "yes", "no"))
            {
                //pop without saving
                _ = Navigation.PopAsync();
            }
        }
    }
}