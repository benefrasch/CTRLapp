using CTRLapp.Views.Settings_pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public static int master_menu, bottom_menu;

        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await MQTT.ConnectMQTT();


            Debug.WriteLine("onappearing");

            master_menu = 0; bottom_menu = 0; //reset both menus to 0
            Master_List.ItemsSource = null; Bottom_List.ItemsSource = null;

            if (Variables.Variables.Layout != null)
                Master_List.ItemsSource = Variables.Variables.Layout;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[master_menu].Bottom_Menu_Items != null)
                Bottom_List.ItemsSource = Variables.Variables.Layout[master_menu].Bottom_Menu_Items;
            Load_Objects();
        }


        private void Master_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            master_menu = e.SelectedItemIndex;
            bottom_menu = 0; //default to first page
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[master_menu].Bottom_Menu_Items != null) Bottom_List.ItemsSource = Variables.Variables.Layout[master_menu].Bottom_Menu_Items;
            Load_Objects();
        }
        private void Bottom_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            bottom_menu = e.SelectedItemIndex;
            Load_Objects();
        }

        private void Load_Objects()
        {
            MQTT.RemoveMQTTHandelers();
            Main_Layout.Children.Clear();

            BackgroundImageSource = null;
            if (Variables.Variables.Layout == null || Variables.Variables.Layout[master_menu].Bottom_Menu_Items == null) return;
            if (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].BackgroundImageSource != "")
            {
                BackgroundImageSource = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].BackgroundImageSource;
            }

            if (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects == null) return;
            List<Objects.Object> object_list = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects;

            foreach ((Objects.Object obj, int index) in object_list.Select((v, i) => (v, i)))
            {
                Main_Layout.Children.Add(new Object_view(master_menu, bottom_menu, index)
                {
                    TranslationX = obj.X,
                    TranslationY = obj.Y,
                    Rotation = obj.Rotation,
                });
            }

        }


        private async void Settings_button_Pressed(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings_page(), true);
        }


    }
}