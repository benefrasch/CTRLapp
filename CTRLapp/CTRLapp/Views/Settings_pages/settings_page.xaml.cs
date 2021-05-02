using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings_page : TabbedPage
    {
        public Settings_page()
        {
            InitializeComponent();
            CurrentPage = Children[1];
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine(JsonConvert.SerializeObject(Variables.Variables.Layout));
            File.WriteAllText(Variables.Variables.configLocation, JsonConvert.SerializeObject(Variables.Variables.Layout));

        }

    }
}