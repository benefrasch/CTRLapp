using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages.GUI.edit_layouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Matrix : ContentView
    {
        public Matrix()
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