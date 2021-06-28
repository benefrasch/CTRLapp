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
        public static List<string> topicList = new List<string>();

        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = MQTT.ConnectMQTT();

            Debug.WriteLine("onappearing");

            masterMenu = 0; bottomMenu = 0; //reset both menus to 0
            masterList.ItemsSource = null; bottomList.ItemsSource = null;

            if (Variables.Variables.Layout != null)
                masterList.ItemsSource = Variables.Variables.Layout;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenu].BottomMenuItems != null)
                bottomList.ItemsSource = Variables.Variables.Layout[masterMenu].BottomMenuItems;
            LoadObjects();
        }


        private void MasterListItemSelected(object _, ItemTappedEventArgs e)
        {
            masterMenu = e.ItemIndex;
            bottomMenu = 0; //default to first page
            bottomList.ItemsSource = null;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenu].BottomMenuItems != null)
            {
                bottomList.ItemsSource = Variables.Variables.Layout[masterMenu].BottomMenuItems;
                bottomList.HeightRequest = Variables.Variables.Layout[masterMenu].BottomMenuItems.Count * bottomList.RowHeight;
            }
            LoadObjects();
        }
        private void BottomListItemSelected(object _, ItemTappedEventArgs e)
        {
            bottomMenu = e.ItemIndex;
            LoadObjects();
        }

        private async void LoadObjects()
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
            List<Objects.Object> objectList = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects;

            foreach ((Objects.Object obj, int index) in objectList.Select((v, i) => (v, i)))
            {
                mainLayout.Children.Add(new ObjectView(masterMenu, bottomMenu, index)
                {
                    TranslationX = obj.X,
                    TranslationY = obj.Y,
                    Rotation = obj.Rotation,
                });
            }

            if (topicList.Count >= 1)
                await MQTT.SubscribeMQTT(topicList);

        }


        private async void SettingsButtonPressed(object _, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPassword(new SettingsPage()), true);
        }


    }
}