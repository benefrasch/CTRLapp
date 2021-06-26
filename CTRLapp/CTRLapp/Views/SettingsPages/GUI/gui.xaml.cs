using CTRLapp.Objects;
using Plugin.Media;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.GUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gui : ContentView
    {
        private int masterMenuSelected = 0, bottomMenuSelected = 0;
        private string type;

        public Gui()
        {
            InitializeComponent();
            LoadMasterStack();
        }


        private void LoadMasterStack()
        {
            masterList.ItemsSource = null;
            if (Variables.Variables.Layout != null) masterList.ItemsSource = Variables.Variables.Layout;
        }

        private void LoadBottomStack()
        {
            bottomList.ItemsSource = null;
            if (Variables.Variables.Layout != null && Variables.Variables.Layout[masterMenuSelected].BottomMenuItems != null)
                bottomList.ItemsSource = Variables.Variables.Layout[masterMenuSelected].BottomMenuItems;
        }


        private void LoadEditStack(string type) //reload Edit Stack depending on selected type
        {
            editStack.IsVisible = true;
            this.type = type;
            typeLabel.Text = type;
            if (type == "Main Menu")
            {
                editGui.IsVisible = false;
                editBackground.IsVisible = false;
                editStack.BindingContext = Variables.Variables.Layout[masterMenuSelected];
            }
            else if (type == "Secondary Menu")
            {
                editGui.IsVisible = true;
                editBackground.IsVisible = true;
                editStack.BindingContext = Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected];
            }

        }

        private void MasterListItemTapped(object _, ItemTappedEventArgs e)
        {
            if (e.ItemIndex == -1) return;
            masterMenuSelected = e.ItemIndex;
            LoadBottomStack();
            LoadEditStack("Main Menu");
            addBottomMenu.IsVisible = true;
        }
        private void BottomListItemTapped(object _, ItemTappedEventArgs e)
        {
            if (e.ItemIndex == -1) return;
            bottomMenuSelected = e.ItemIndex;
            masterList.SelectedItem = null;
            LoadEditStack("Secondary Menu");
        }

        private void AddMasterMenuItem(object _, EventArgs e)
        {
            if (Variables.Variables.Layout == null) Variables.Variables.Layout = new List<MasterMenuItem>();
            MasterMenuItem item = new MasterMenuItem
            {
                Name = "Menu_" + Variables.Variables.Layout.Count,
                IconPath = "quadrat.png"
            };
            Variables.Variables.Layout.Add(item);

            LoadMasterStack();
        }
        private void AddBottomMenuItem(object _, EventArgs e)
        {
            if (Variables.Variables.Layout[masterMenuSelected].BottomMenuItems == null) Variables.Variables.Layout[masterMenuSelected].BottomMenuItems = new List<BottomMenuItem>();
            BottomMenuItem item = new BottomMenuItem
            {
                Name = "Menu_" + Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.Count,
                IconPath = "quadrat.png"
            };
            Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.Add(item);

            LoadBottomStack();
        }


        private async void DeleteButtonPressed(object _, EventArgs e)
        {
            if (!await App.Current.MainPage.DisplayAlert("Attention", "do you really want to delete this object", "yes", "no")) return; //if "no" is pressed in alert return
            if (type == "Main Menu")
            {
                Variables.Variables.Layout.RemoveAt(masterMenuSelected); //removes the specified object

                LoadMasterStack();
                editStack.IsVisible = false;
                addBottomMenu.IsVisible = false;
                bottomList.ItemsSource = null;
            }
            else if (type == "Secondary Menu")
            {
                Variables.Variables.Layout[masterMenuSelected].BottomMenuItems.RemoveAt(bottomMenuSelected); //removes the specified object

                LoadBottomStack();
                editStack.IsVisible = false;
            }
        }

        private async void EditGuiClicked(object _, EventArgs e)
        {
            await Navigation.PushAsync(new GuiPage(masterMenuSelected, bottomMenuSelected));
        }

        private async void IconImageSelect(object _, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var mediaOptions = new Plugin.Media.Abstractions.PickMediaOptions()
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            if (selectedImageFile == null) return;

            if (type == "Main Menu")
                Variables.Variables.Layout[masterMenuSelected].IconPath = selectedImageFile.Path;
            else
                Variables.Variables.Layout[masterMenuSelected].BottomMenuItems[bottomMenuSelected].IconPath = selectedImageFile.Path;

            LoadMasterStack();
            LoadBottomStack();

        }

        private async void BackgroundImageSelect(object _, EventArgs e)
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