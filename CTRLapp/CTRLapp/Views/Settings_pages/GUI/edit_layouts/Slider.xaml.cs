using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages.GUI.edit_layouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Slider : ContentView
    {
        public Slider()
        {
            InitializeComponent();
        }

        public event EventHandler UpdateEvent;

        public void UpdatePreview(Object sender, EventArgs e)
        {
            UpdateEvent.Invoke(null, null);
        }
    }
}