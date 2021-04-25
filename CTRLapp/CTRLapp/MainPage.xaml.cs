﻿using CTRLapp.Views.Settings_pages;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        int master_menu, bottom_menu;

        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MQTT.DisconnectMQTT();

            master_menu = 0; bottom_menu = 0; //reset both menus to 0

            if (Variables.Variables.Layout != null) Master_List.ItemsSource = Variables.Variables.Layout;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[master_menu].Bottom_Menu_Items != null) Bottom_List.ItemsSource = Variables.Variables.Layout[master_menu].Bottom_Menu_Items;
            Load_Objects();
            BackgroundImageSource = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].BackgroundImage;
        }


        private void Master_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            master_menu = e.SelectedItemIndex;
            bottom_menu = 0;
            Load_Objects();
        }
        private void Bottom_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            bottom_menu = e.SelectedItemIndex;
            Load_Objects();
        }

        private void Load_Objects()
        {
            Main_Layout.Children.Clear();
            if (Variables.Variables.Layout == null || Variables.Variables.Layout[master_menu].Bottom_Menu_Items == null || Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects == null) return;
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