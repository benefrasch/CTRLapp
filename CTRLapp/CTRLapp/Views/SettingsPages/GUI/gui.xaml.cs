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
        private int masterMenuSelected, bottomMenuSelected;


        public Gui()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ReloadMasterStack();
            Reload_Bottom_Stack();
        }

        private void ReloadMasterStack()
        {
            master_list.ItemsSource = null;
            if (Variables.Variables.Layout != null) master_list.ItemsSource = Variables.Variables.Layout;
        } //reloads Master_Stack
        private void Reload_Bottom_Stack()
        {
            bottom_list.ItemsSource = null;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenuSelected].BottomMenuItems != null) bottom_list.ItemsSource = Variables.Variables.Layout[bottomMenuSelected].BottomMenuItems;
        } //reloads Bottom_Stack
        private void ReloadEditStack(string type)
        {
            edit_stack.IsVisible = true;
            edit_type.Text = type;
            if (type == "MasterMenuItem")
            {
                Edit_gui.IsVisible = false;
                editBackground.IsVisible = false;
                Type_label.Text = "Main Menu";
                edit_stack.BindingContext = Variables.Variables.Layout[masterMenuSelected];
            }
            else if (type == "BottomMenuItem")
            {
                Edit_gui.IsVisible = true;
                editBackground.IsVisible = true;
                Type_label.Text = "Secondary Menu";
                edit_stack.BindingContext = Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected];
            }


        }//reload Edit Stack

        private void MasterListItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.ItemIndex == -1) return;
            masterMenuSelected = e.ItemIndex;
            Reload_Bottom_Stack();
            ReloadEditStack("MasterMenuItem");
        } //if Master_Manu_Item gets selected
        private void BottomListItemTapped(object sender, ItemTappedEventArgs e)
        {
            bottomMenuSelected = e.ItemIndex;
            master_list.SelectedItem = null;
            ReloadEditStack("BottomMenuItem");
        } //if Bottom_Manu_Item gets selected

        private void AddMasterMenuItem(object sender, EventArgs e)
        {
            if (Variables.Variables.Layout == null) Variables.Variables.Layout = new List<MasterMenuItem>();
            int number = Variables.Variables.Layout.Count;        //gets number of master_menu_items and assigns the next one to the new one
            MasterMenuItem item = new MasterMenuItem
            {
                Name = "Menu_" + number,
                IconPath = "quadrat.png"
            };


            Variables.Variables.Layout.Add(item);
            masterMenuSelected = number;
            ReloadMasterStack();
            Reload_Bottom_Stack();
        } //add new Master_Menu_Item
        private void AddBottomMenuItem(object sender, EventArgs e)
        {
            if (masterMenuSelected == -1) return;
            if (Variables.Variables.Layout[masterMenuSelected].BottomMenuItems == null) Variables.Variables.Layout[masterMenuSelected].BottomMenuItems = new List<BottomMenuItem>();
            int number = Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.Count;    //gets number of master_menu_items and assigns the next one to the new one
            BottomMenuItem item = new BottomMenuItem
            {
                Name = "Menu_" + number,
                IconPath = "quadrat.png"
            };

            Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.Add(item);
            Reload_Bottom_Stack();
        } //add new BottomMenuItem


        private async void DeleteButtonPressed(object sender, EventArgs e)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Attention", "do you really want to delete this object", "yes", "no");
            if (!confirm) return; //if "no" is pressed in alert return
            if (edit_type.Text == "MasterMenuItem")
            {
                Variables.Variables.Layout.RemoveAt(masterMenuSelected); //removes the specified object

                ReloadMasterStack();
                bottom_stack.IsVisible = false;
                edit_stack.IsVisible = false;
            }
            else if (edit_type.Text == "BottomMenuItem")
            {
                Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.RemoveAt(bottomMenuSelected); //removes the specified object

                Reload_Bottom_Stack();
                edit_stack.IsVisible = false;
            }
        }

        private async void EditGuiClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new GuiPage(masterMenuSelected, bottomMenuSelected));
        }

        private async void IconImageSelect(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var mediaOptions = new Plugin.Media.Abstractions.PickMediaOptions()
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            if (selectedImageFile == null) return;

            if (Type_label.Text == "Main Menu")
                Variables.Variables.Layout[masterMenuSelected].IconPath = selectedImageFile.Path;
            else
                Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected].IconPath = selectedImageFile.Path;

            ReloadMasterStack();
            Reload_Bottom_Stack();

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

            Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected].BackgroundImageSource = selectedImageFile.Path;
        }

    }
}