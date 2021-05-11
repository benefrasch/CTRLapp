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
        private int masterMenuSelected = 0, bottomMenuSelected = 0;


        public Gui()
        {
            InitializeComponent();
            LoadMasterStack();
        }


        private void LoadMasterStack()
        {
            masterList.ItemsSource = null;
            if (Variables.Variables.Layout != null) masterList.ItemsSource = Variables.Variables.Layout;
        } //reloads Master_Stack

        private void LoadBottomStack()
        {
            bottomList.ItemsSource = null;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenuSelected].BottomMenuItems != null) 
                bottomList.ItemsSource = Variables.Variables.Layout[masterMenuSelected].BottomMenuItems;
        } //reloads Bottom_Stack


        private void LoadEditStack(string type)
        {
            editStack.IsVisible = true;
            typeLabel.Text = type;
            if (type == "MasterMenuItem")
            {
                editGui.IsVisible = false;
                editBackground.IsVisible = false;
                typeLabel.Text = "Main Menu";
                editStack.BindingContext = Variables.Variables.Layout[masterMenuSelected];
            }
            else if (type == "BottomMenuItem")
            {
                editGui.IsVisible = true;
                editBackground.IsVisible = true;
                typeLabel.Text = "Secondary Menu";
                editStack.BindingContext = Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected];
            }


        }//reload Edit Stack

        private void MasterListItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.ItemIndex == -1) return;
            masterMenuSelected = e.ItemIndex;
            LoadBottomStack();
            LoadEditStack("MasterMenuItem");
            addBottomMenu.IsVisible = true;
        } 
        private void BottomListItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.ItemIndex == -1) return;
            bottomMenuSelected = e.ItemIndex;
            masterList.SelectedItem = null;
            LoadEditStack("BottomMenuItem");
        }

        private void AddMasterMenuItem(object sender, EventArgs e)
        {
            if (Variables.Variables.Layout == null) Variables.Variables.Layout = new List<MasterMenuItem>();
            MasterMenuItem item = new MasterMenuItem
            {
                Name = "Menu_" + Variables.Variables.Layout.Count,
                IconPath = "quadrat.png"
            };
            Variables.Variables.Layout.Add(item);

            LoadMasterStack();
        } //add new Master_Menu_Item
        private void AddBottomMenuItem(object sender, EventArgs e)
        {
            if (Variables.Variables.Layout[masterMenuSelected].BottomMenuItems == null) Variables.Variables.Layout[masterMenuSelected].BottomMenuItems = new List<BottomMenuItem>();
            BottomMenuItem item = new BottomMenuItem
            {
                Name = "Menu_" + Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.Count,
                IconPath = "quadrat.png"
            };
            Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.Add(item);

            LoadBottomStack();
        } //add new BottomMenuItem


        private async void DeleteButtonPressed(object sender, EventArgs e)
        {
            if (!await App.Current.MainPage.DisplayAlert("Attention", "do you really want to delete this object", "yes", "no")) return; //if "no" is pressed in alert return
            if (typeLabel.Text == "Main Menu")
            {
                Variables.Variables.Layout.RemoveAt(masterMenuSelected); //removes the specified object

                LoadMasterStack();
                editStack.IsVisible = false;
                addBottomMenu.IsVisible = true;
                bottomList.ItemsSource = null;
            }
            else if (typeLabel.Text == "Secondary Menu")
            {
                Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.RemoveAt(bottomMenuSelected); //removes the specified object

                LoadBottomStack();
                editStack.IsVisible = false;
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

            if (typeLabel.Text == "Main Menu")
                Variables.Variables.Layout[masterMenuSelected].IconPath = selectedImageFile.Path;
            else
                Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected].IconPath = selectedImageFile.Path;

            LoadMasterStack();
            LoadBottomStack();

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