using CTRLapp.Objects;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gui : ContentPage
    {
        private int Master_Menu_selected, Bottom_Menu_selected;


        public Gui()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Reload_Master_stack();
            Reload_Bottom_Stack();
        }

        private void Reload_Master_stack()
        {
            master_list.ItemsSource = null;
            if (Variables.Variables.Layout != null) master_list.ItemsSource = Variables.Variables.Layout;
        } //reloads Master_Stack
        private void Reload_Bottom_Stack()
        {
            bottom_list.ItemsSource = null;
            // if (Master_Menu_selected == -1) return;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items != null) bottom_list.ItemsSource = Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items;
        } //reloads Bottom_Stack
        private void Relaod_Edit_Stack(string type)
        {
            edit_stack.IsVisible = true;
            edit_type.Text = type;
            if (type == "master_menu_item")
            {
                Edit_gui.IsVisible = false;
                Type_label.Text = "Main Menu";
                edit_stack.BindingContext = Variables.Variables.Layout[Master_Menu_selected];
            }
            else if (type == "bottom_menu_item")
            {
                Edit_gui.IsVisible = true;
                Type_label.Text = "Secondary Menu";
                edit_stack.BindingContext = Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items[Bottom_Menu_selected];
            }


        }//reload Edit Stack

        private void Master_list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItemIndex == -1) return;
            Master_Menu_selected = e.SelectedItemIndex;
            Reload_Bottom_Stack();
            bottom_stack.IsVisible = true;
            Relaod_Edit_Stack("master_menu_item");
        } //if different Master_Manu_Item gets selected
        private void Bottom_list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Bottom_Menu_selected = e.SelectedItemIndex;
            master_list.SelectedItem = null;
            Relaod_Edit_Stack("bottom_menu_item");
        } //if different Bottom_Manu_Item gets selected

        private void Add_Master_Menu_Item(object sender, EventArgs e)
        {
            if (Variables.Variables.Layout == null) Variables.Variables.Layout = new List<Master_Menu_Item>();
            int number = Variables.Variables.Layout.Count;        //gets number of master_menu_items and assigns the next one to the new one
            Master_Menu_Item item = new Master_Menu_Item
            {
                Name = "Menu_" + number,
                Icon_path = "quadrat.png"
            };


            Variables.Variables.Layout.Add(item);
            Master_Menu_selected = number;
            Reload_Master_stack();
            Reload_Bottom_Stack();
            //Json_string.Config = JsonConvert.SerializeObject(config);
        } //add new Master_Menu_Item
        private void Add_Bottom_Menu_Item(object sender, EventArgs e)
        {
            if (Master_Menu_selected == -1) return;
            if (Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items == null) Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items = new List<Bottom_Menu_Item>();
            int number = Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items.Count;    //gets number of master_menu_items and assigns the next one to the new one
            Bottom_Menu_Item item = new Bottom_Menu_Item
            {
                Name = "Menu_" + number,
                Icon_path = "quadrat.png"
            };

            Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items.Add(item);
            Reload_Bottom_Stack();
            //Json_string.Config = JsonConvert.SerializeObject(config);
        } //add new Bottom_Menu_Item


        private async void Delete_button_Pressed(object sender, EventArgs e)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Attention", "do you really want to delete this object", "yes", "no");
            if (!confirm) return; //if "no" is pressed in alert return
            if (edit_type.Text == "master_menu_item")
            {
                Variables.Variables.Layout.RemoveAt(Master_Menu_selected); //removes the specified object

                Reload_Master_stack();
                bottom_stack.IsVisible = false;
                edit_stack.IsVisible = false;
            }
            else if (edit_type.Text == "bottom_menu_item")
            {
                Variables.Variables.Layout[Master_Menu_selected].Bottom_Menu_Items.RemoveAt(Bottom_Menu_selected); //removes the specified object

                Reload_Bottom_Stack();
                edit_stack.IsVisible = false;
            }
        }

        private async void Edit_gui_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Gui_page(Master_Menu_selected, Bottom_Menu_selected));
        }

        private void Edit_stack_changed(object sender, EventArgs e)
        {
        }
    }
}