using CTRLapp.Objects;
using CTRLapp.Variables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages.GUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Object_page : ContentPage
    {
        private int master_menu, bottom_menu, obj_index;

        private bool deleted = false;

        public Object_page(int master_menu, int bottom_menu, int obj_index)
        {
            this.master_menu = master_menu; this.bottom_menu = bottom_menu; this.obj_index = obj_index;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            grid.BindingContext = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];

            //preview in middle of right side
            View view = Object_view.View(master_menu, bottom_menu, obj_index);
            view.TranslateTo(0, 0, 60);
            object_grid.Children.Add(view, 1, 1);


            base.OnAppearing();
        }


        protected override void OnDisappearing()
        {
            if (deleted)
            {
                Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.RemoveAt(obj_index);
            }
            base.OnDisappearing();
        }


        private void UpdatePreview(object sender, TextChangedEventArgs e)
        {
            object_grid.Children.Clear();
            View view = Object_view.View(master_menu, bottom_menu, obj_index);
            view.TranslateTo(0, 0);
            object_grid.Children.Add(view, 1, 1);
        }

        private void Delete_Button_Pressed(object sender, EventArgs e)
        {
            // List<Master_Menu_Item> temp = JsonConvert.DeserializeObject<List<Master_Menu_Item>>(Json_string.Config);
            //temp[master_menu].Bottom_Menu_Items[bottom_menu].Objects.RemoveAt(obj_index);
            //Json_string.Config = JsonConvert.SerializeObject(temp);
            deleted = true;
            Navigation.PopModalAsync();
        }
    }
}