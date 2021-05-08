using CTRLapp.Objects;
using Plugin.Media;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.GUI
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
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems != null) bottom_list.ItemsSource = Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems;
        } //reloads Bottom_Stack
        private void Relaod_Edit_Stack(string type)
        {
            edit_stack.IsVisible = true;
            edit_type.Text = type;
            if (type == "master_menu_item")
            {
                Edit_gui.IsVisible = false;
                Edit_background.IsVisible = false;
                Type_label.Text = "Main Menu";
                edit_stack.BindingContext = Variables.Variables.Layout[Master_Menu_selected];
            }
            else if (type == "BottomMenuItem")
            {
                Edit_gui.IsVisible = true;
                Edit_background.IsVisible = true;
                Type_label.Text = "Secondary Menu";
                edit_stack.BindingContext = Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems[Bottom_Menu_selected];
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
            Relaod_Edit_Stack("BottomMenuItem");
        } //if different Bottom_Manu_Item gets selected

        private void Add_Master_Menu_Item(object sender, EventArgs e)
        {
            if (Variables.Variables.Layout == null) Variables.Variables.Layout = new List<MasterMenuItem>();
            int number = Variables.Variables.Layout.Count;        //gets number of master_menu_items and assigns the next one to the new one
            MasterMenuItem item = new MasterMenuItem
            {
                Name = "Menu_" + number,
                IconPath = "quadrat.png"
            };


            Variables.Variables.Layout.Add(item);
            Master_Menu_selected = number;
            Reload_Master_stack();
            Reload_Bottom_Stack();
        } //add new Master_Menu_Item
        private void Add_BottomMenuItem(object sender, EventArgs e)
        {
            if (Master_Menu_selected == -1) return;
            if (Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems == null) Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems = new List<BottomMenuItem>();
            int number = Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems.Count;    //gets number of master_menu_items and assigns the next one to the new one
            BottomMenuItem item = new BottomMenuItem
            {
                Name = "Menu_" + number,
                IconPath = "quadrat.png"
            };

            Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems.Add(item);
            Reload_Bottom_Stack();
        } //add new BottomMenuItem


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
            else if (edit_type.Text == "BottomMenuItem")
            {
                Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems.RemoveAt(Bottom_Menu_selected); //removes the specified object

                Reload_Bottom_Stack();
                edit_stack.IsVisible = false;
            }
        }

        private async void Edit_Gui_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new GuiPage(Master_Menu_selected, Bottom_Menu_selected));
        }

        private async void BackgroundImageSelect(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var mediaOptions = new Plugin.Media.Abstractions.PickMediaOptions()
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            if (selectedImageFile == null) return;

            Variables.Variables.Layout[Master_Menu_selected].BottomMenuItems[Bottom_Menu_selected].BackgroundImageSource = selectedImageFile.Path;
        }

    }
}