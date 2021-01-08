using System.Diagnostics;
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

    }
}