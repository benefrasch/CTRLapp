using CTRLapp.Views.SettingsPages;
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
        public static int masterMenu, bottomMenu;

        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await MQTT.ConnectMQTT();


            Debug.WriteLine("onappearing");

            masterMenu = 0; bottomMenu = 0; //reset both menus to 0
            masterList.ItemsSource = null; bottomList.ItemsSource = null;

            if (Variables.Variables.Layout != null)
                masterList.ItemsSource = Variables.Variables.Layout;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenu].BottomMenuItems != null)
                bottomList.ItemsSource = Variables.Variables.Layout[masterMenu].BottomMenuItems;
            LoadObjects();
        }


        private void MasterListItemSelected(object sender, ItemTappedEventArgs e)
        {
            masterMenu = e.ItemIndex;
            bottomMenu = 0; //default to first page
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenu].BottomMenuItems != null) 
                bottomList.ItemsSource = Variables.Variables.Layout[masterMenu].BottomMenuItems;
            bottomList.SelectedItem = null;
            LoadObjects();
        }
        private void BottomListItemSelected(object sender, ItemTappedEventArgs e)
        {
            bottomMenu = e.ItemIndex;
            LoadObjects();
        }

        private void LoadObjects()
        {
            MQTT.RemoveMQTTHandelers();
            mainLayout.Children.Clear();

            BackgroundImageSource = null;
            if (Variables.Variables.Layout == null || Variables.Variables.Layout[masterMenu].BottomMenuItems == null) return;
            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].BackgroundImageSource != "")
            {
                BackgroundImageSource = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].BackgroundImageSource;
            }

            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects == null) return;
            List<Objects.Object> object_list = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects;

            foreach ((Objects.Object obj, int index) in object_list.Select((v, i) => (v, i)))
            {
                mainLayout.Children.Add(new ObjectView(masterMenu, bottomMenu, index)
                {
                    TranslationX = obj.X,
                    TranslationY = obj.Y,
                    Rotation = obj.Rotation,
                });
            }

        }


        private async void SettingsButtonPressed(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(), true);
        }


    }
}