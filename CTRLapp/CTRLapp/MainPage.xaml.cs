using CTRLapp.Objects;
using CTRLapp.Variables;
using CTRLapp.Views.Settings_pages;
using Newtonsoft.Json;
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
        int master_menu, bottom_menu;
        //private static List<Master_Menu_Item> Config
        //{
        //    get { return JsonConvert.DeserializeObject<List<Master_Menu_Item>>(Json_string.Config); }
        //}

        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            master_menu = 0; bottom_menu = 0; //reset both menus to 0
            if (Json_string.Array != null) Master_List.ItemsSource = Json_string.Array;
            if (Json_string.Array != null && Json_string.Array[master_menu].Bottom_Menu_Items != null) Bottom_List.ItemsSource = Json_string.Array[master_menu].Bottom_Menu_Items;
            Load_Objects();
            Debug.WriteLine(Json_string.Config);
        }

        private void Master_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            master_menu = e.SelectedItemIndex;
            Bottom_List.ItemsSource = Json_string.Array[master_menu].Bottom_Menu_Items;
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
            //List<Master_Menu_Item> config = JsonConvert.DeserializeObject<List<Master_Menu_Item>>(Json_string.Config);
            List<Master_Menu_Item> config = Json_string.Array;

            if (config == null || config[master_menu].Bottom_Menu_Items == null || config[master_menu].Bottom_Menu_Items[bottom_menu].Objects == null) return;
            List<Objects.Object> object_list = config[master_menu].Bottom_Menu_Items[bottom_menu].Objects;

            foreach ((Objects.Object obj, int index) in object_list.Select((v, i) => (v, i)))
            {
                Debug.WriteLine(obj);
                //Layout temp = Build(obj);
                Main_Layout.Children.Add(Object_view.View(master_menu, bottom_menu, index));
            }
        }


        private async void Settings_button_Pressed(object sender, EventArgs e)
        {
             await Navigation.PushAsync(new Settings_page(), true);
        }


    }
}