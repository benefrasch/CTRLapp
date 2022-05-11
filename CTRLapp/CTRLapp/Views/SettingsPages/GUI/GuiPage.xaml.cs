﻿using CTRLapp.Objects.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.GUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuiPage : ContentPage
    {
        private readonly int masterMenu, bottomMenu;

        private struct Item
        {
            public Item(string name, string icon, Objects.BaseObject @object)
            {
                Name = name; Icon = icon; NewObject = @object;
            }
            public string Name { get; set; }
            public string Icon { get; set; }
            public Objects.BaseObject NewObject { get; set; }
        }

        private readonly List<Item> items;


        public GuiPage(int masterMenu, int bottomMenu)
        {
            InitializeComponent();

            this.masterMenu = masterMenu;
            this.bottomMenu = bottomMenu;

            string theme;
            if (Application.Current.RequestedTheme == OSAppTheme.Light)
                theme = "light";
            else
                theme = "dark";




            items = new List<Item>()
            {
                new Item("Label", "text_box_icon_"+theme+".png",
                    new LabelObject
                {
                    Width = 100,
                    Height = 30,
                    Type = "Label",
                    TextColor = Variables.Variables.SecondaryColor,
                    BackgroundColor = Color.Transparent,
                    Text = "Label",
                    FontSize = 21
                }),

                //new Item("ValueDisplay", new Objects.Object
                //{
                //    Width = 100,
                //    Height = 30,
                //    Type = "ValueDisplay",
                //    Arguments = new string[6] { SecondaryColor.ToHex(), Color.Transparent.ToHex(), "Value display","","", "21" }
                //}), //buggy, so it is deactivated until fixed

                new Item("Button", "button_icon_"+theme+".png",
                    new ButtonObject
                {
                    Width = 80,
                    Height = 40,
                    Type = "Button",
                    BackgroundColor = Variables.Variables.SecondaryColor,
                    TextColor = Variables.Variables.PrimaryColor,
                    Text = "Button",
                    Topic = "",
                    Message = ""
                }),

                new Item("Button", "button_icon_"+theme+".png",
                    new ButtonObject
                {
                    Width = 80,
                    Height = 40,
                    Type = "Button",
                    BackgroundColor = Variables.Variables.SecondaryColor,
                    TextColor = Variables.Variables.PrimaryColor,
                    Text = "Button",
                    Topic = "",
                    Message = ""

                }),

                new Item("SwitchBtn", "switch_button_icon_"+theme+".png",
                    new SwitchButtonObject
                {
                    Width = 80,
                    Height = 40,
                    Type = "SwitchButton",
                    BackgroundColor = Variables.Variables.SecondaryColor,
                    TextColor = Variables.Variables.PrimaryColor,
                    OnColor = Color.Red,
                    Text = "SwitchBtn",
                    LowMessage = "0",
                    HighMessage = "255"
                }),

                new Item("Switch", "switch_icon_"+theme+".png",
                    new SwitchObject
                {
                    Width = 80,
                    Height = 40,
                    Type = "Switch",
                    ThumbColor = Variables.Variables.SecondaryColor,
                    OnColor = Color.Red,
                    Topic = "",
                    LowMessage = "0",
                    HighMessage = "255"
                }),

                new Item("Slider", "slider_icon_"+theme+".png",
                    new SliderObject
                {
                    Width = 230,
                    Height = 40,
                    Type = "Slider",
                    ThumbColor = Variables.Variables.SecondaryColor,
                    MinimumTrackColor = Color.Red,
                    MaximumTrackColor = Color.Gray,
                    Topic = "",
                    Minimum = 0,
                    Maximum = 255,
                }),

                new Item("Joystick", "joystick_icon_"+theme+".png",
                    new JoystickObject
                {
                    Width = 200,
                    Height = 200,
                    Type = "Joystick",
                    ThumbColor = Variables.Variables.SecondaryColor,
                    BackgroundColor = Color.FromHex("#80404040"),
                    TopicX = "",
                    MinimumX = 0,
                    MaximumX = 255,
                    SensitivityX = 1,
                    TopicY = "",
                    MinimumY = 0,
                    MaximumY = 255,
                    SensitivityY = 1,
                }),

                new Item("Matrix", "matrix_icon_"+theme+".png",
                    new MatrixObject
                {
                    Width = 200,
                    Height = 200,
                    Type = "Matrix",
                     ThumbColor = Variables.Variables.SecondaryColor,
                    BackgroundColor = Color.FromHex("#80404040"),
                    TopicX = "",
                    MinimumX = 0,
                    MaximumX = 255,
                    TopicY = "",
                    MinimumY = 0,
                    MaximumY = 255,
                }),
            };

            //new object list view items
            newObjectListView.ItemsSource = items;

            BackgroundImageSource = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].BackgroundImageSource;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeObjects();
        }

        protected override void OnDisappearing()
        {
            File.WriteAllText(Variables.Variables.configLocation, JsonConvert.SerializeObject(Variables.Variables.Layout));
            base.OnDisappearing();
        }

        private async void ExitButtonClicked(object _, EventArgs e)
        {
            await Navigation.PopAsync();
        }



        private void NewObject(object _, ItemTappedEventArgs e)
        {

            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects == null)
                Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects = new List<Objects.BaseObject>();
            int objIndex = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Count;
            Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Add
                (JsonConvert.DeserializeObject<Objects.BaseObject>(JsonConvert.SerializeObject(((Item)e.Item).NewObject)));
            LoadObject(objIndex);
        }

        private void InitializeObjects() //loads existing objects
        {
            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects == null) return;
            MainLayout.Children.Clear();
            for (int objIndex = 0; objIndex < Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Count; objIndex++)
            {
                LoadObject(objIndex);
            }
        }


        private void LoadObject(int objIndex) //object with pan and tap for config
        {
            //visible objects, but deactivated
            var obj = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex];
            var grid = new ObjectView(masterMenu, bottomMenu, objIndex)
            {
                IsEnabled = false,
                TranslationX = obj.X,
                TranslationY = obj.Y,
                Rotation = obj.Rotation,
            };
            MainLayout.Children.Add(grid);


            //invisible Grid with controls
            Grid invisibleGrid = new()
            {
                WidthRequest = obj.Width,
                HeightRequest = obj.Height,
                Rotation = obj.Rotation,
            };
            invisibleGrid.TranslateTo(obj.X, obj.Y, 1);
            // add tap gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (_, e) =>
            {
                Navigation.PushAsync(new ObjectPage(masterMenu, bottomMenu, objIndex));
            };
            invisibleGrid.GestureRecognizers.Add(tapGestureRecognizer);
            // add pan gesture
            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += (_, e) =>
            {
                byte roundingValue = 1;
                if (snapBox.IsChecked) roundingValue = 20;  //checks if snap is enabled
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        invisibleGrid.Rotation = 0; //rotation to 0 for correct movement
                        if (Device.RuntimePlatform == Device.UWP)
                        {
                            //Windows is a bitch
                            Debug.WriteLine("it ist a windows !!!!!");
                            invisibleGrid.HeightRequest = MainLayout.Height;
                            invisibleGrid.WidthRequest = MainLayout.Width;
                            invisibleGrid.RaiseChild(MainLayout); //so it doesnt interfear with other objects
                            invisibleGrid.TranslateTo(0, 0, 1);
                        }
                        break;
                    case GestureStatus.Running:
                        grid.TranslationX = ((int)(Math.Round(Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex].X + e.TotalX) / (double)roundingValue)) * roundingValue;
                        grid.TranslationY = ((int)(Math.Round(Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex].Y + e.TotalY) / (double)roundingValue)) * roundingValue;
                        break;
                    case GestureStatus.Completed:
                        invisibleGrid.Rotation = obj.Rotation; //reset rotation to before
                        invisibleGrid.TranslationX = grid.TranslationX;
                        invisibleGrid.TranslationY = grid.TranslationY;
                        Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex].X = (int)grid.TranslationX;
                        Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex].Y = (int)grid.TranslationY;
                        invisibleGrid.HeightRequest = obj.Height;
                        invisibleGrid.WidthRequest = obj.Width;
                        break;
                }
            };
            invisibleGrid.GestureRecognizers.Add(panGestureRecognizer);

            MainLayout.Children.Add(invisibleGrid);
        }


    }
}