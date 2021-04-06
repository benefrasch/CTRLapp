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
        private Objects.Object obj;

        private bool deleted = false;

        public Object_page(int master_menu, int bottom_menu, int obj_index)
        {
            this.master_menu = master_menu; this.bottom_menu = bottom_menu; this.obj_index = obj_index;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            obj = Json_string.Array[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];
            grid.BindingContext = obj;

            View view = Object_view.View(master_menu, bottom_menu, obj_index);
            view.TranslateTo(0, 0);
            object_grid.Children.Add(view, 1, 1);

            edit_stack.BindingContextChanged += (s, e) => 
            {
                Debug.WriteLine("test");
            };

            //if (obj.Arguments[] != null) Primary_color_picker.SelectedColor = Color.FromHex(obj.Color_primary);
            //if (obj.Color_secondary != null) Secondary_color_picker.SelectedColor = Color.FromHex(obj.Color_secondary);

            base.OnAppearing();
        }




        protected override void OnDisappearing()
        {
            var temp = Json_string.Array;
            if (!deleted)
            {
                //obj.Color_primary = Primary_color_picker.SelectedColor.ToHex();
                //obj.Color_secondary = Secondary_color_picker.SelectedColor.ToHex();

                temp[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index] = obj;
            }
            else
            {
                temp[master_menu].Bottom_Menu_Items[bottom_menu].Objects.RemoveAt(obj_index);
            }
            Json_string.Array = temp;
            base.OnDisappearing();
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