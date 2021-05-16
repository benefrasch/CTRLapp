using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.GUI.EditLayouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SwitchButton : ContentView
    {
        public SwitchButton()
        {
            InitializeComponent();
        }

        public event EventHandler UpdateEvent;

        public void UpdatePreview(object _, EventArgs e)
        {
            UpdateEvent.Invoke(null, null);
        }
    }
}