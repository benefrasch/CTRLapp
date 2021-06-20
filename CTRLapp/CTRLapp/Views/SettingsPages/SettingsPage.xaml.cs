using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            content.Content = new General.General();
            generalButton.BackgroundColor = Color.FromHex("#80404040");
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine(JsonConvert.SerializeObject(Variables.Variables.Layout));
            File.WriteAllText(Variables.Variables.configLocation, JsonConvert.SerializeObject(Variables.Variables.Layout));

        }

        private void BackClicked(object _, System.EventArgs e)
        {
            Navigation.PopAsync();
        }
        private void GeneralClicked(object _, System.EventArgs e)
        {
            content.Content = new General.General();
            generalButton.BackgroundColor = Color.FromHex("#80404040");
            guiButton.BackgroundColor = Color.Transparent;
            devicesButton.BackgroundColor = Color.Transparent;
        }
        private void GuiClicked(object _, System.EventArgs e)
        {
            content.Content = new GUI.Gui();
            generalButton.BackgroundColor = Color.Transparent;
            guiButton.BackgroundColor = Color.FromHex("#80404040");
            devicesButton.BackgroundColor = Color.Transparent;
        }
        private void DevicesClicked(object _, System.EventArgs e)
        {
            content.Content = new Devices.Devices();
            generalButton.BackgroundColor = Color.Transparent;
            guiButton.BackgroundColor = Color.Transparent;
            devicesButton.BackgroundColor = Color.FromHex("#80404040");
        }
    }
}