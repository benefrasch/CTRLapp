﻿using CTRLapp.Views.Settings_pages.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gui_page : ContentPage
    {
        private int master_menu, bottom_menu;

        

        public Gui_page(int master_menu, int bottom_menu)
        {
            InitializeComponent();

            this.master_menu = master_menu;
            this.bottom_menu = bottom_menu;
        }
        protected override void OnAppearing()
        {
            Initialize_Objects();
            BackgroundImageSource = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].BackgroundImageSource;
            base.OnAppearing();
        }

        private async void Exit_button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        // \/ \/ add objects here \/ \/


        private void Add_Rectangle_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 120,
                Height = 240,
            };
            temp.Arguments = new string[1];
            temp.Arguments[0] = Color.Black.ToHex();

            Add_Object(temp);
        }
        private void Add_Label_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 100,
                Height = 60,
                Type = "Label"
            };
            temp.Arguments = new string[4];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.Transparent.ToHex();
            temp.Arguments[2] = "Label";
            temp.Arguments[3] = "21";

            Add_Object(temp);
        }
        private void Add_Button_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 80,
                Height = 40,
                Type = "Button",
            };
            temp.Arguments = new string[5];
            temp.Arguments[0] = (Color.Black.ToHex());
            temp.Arguments[1] = Color.LightGray.ToHex();
            temp.Arguments[2] = "";
            temp.Arguments[3] = "";
            temp.Arguments[4] = "";


            Add_Object(temp);
        }
        private void Add_Switch_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 80,
                Height = 40,
                Type = "Switch",
            };
            temp.Arguments = new string[5];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.Orange.ToHex();
            temp.Arguments[2] = "";
            temp.Arguments[3] = "0";
            temp.Arguments[4] = "255";

            Add_Object(temp);
        }
        private void Add_Slider_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 200,
                Height = 40,
                Type = "Slider",
                Rotation = 0,
            };
            temp.Arguments = new string[6];
            temp.Arguments[0] = Color.Gray.ToHex();
            temp.Arguments[1] = Color.Black.ToHex();
            temp.Arguments[2] = Color.Orange.ToHex();
            temp.Arguments[3] = "";
            temp.Arguments[4] = "0";
            temp.Arguments[5] = "255";

            Add_Object(temp);

        }
        private void Add_Joystick_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 200,
                Height = 200,
                Type = "Joystick"
            };
            temp.Arguments = new string[10];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.Gray.ToHex();
            temp.Arguments[2] = "";
            temp.Arguments[3] = "";
            temp.Arguments[4] = "0";
            temp.Arguments[5] = "255";
            temp.Arguments[6] = "1";
            temp.Arguments[7] = "0";
            temp.Arguments[8] = "255";
            temp.Arguments[9] = "1";

            Add_Object(temp);

        }
        private void Add_Matrix_Pressed(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 200,
                Height = 200,
                Type = "Matrix"
            };
            temp.Arguments = new string[8];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.LightGray.ToHex();
            temp.Arguments[2] = "";
            temp.Arguments[3] = "";
            temp.Arguments[4] = "0";
            temp.Arguments[5] = "255";
            temp.Arguments[6] = "0";
            temp.Arguments[7] = "255";

            Add_Object(temp);
        }


        private void Add_Object(Objects.Object temp) //adds and loads new object
        {
            if (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects == null) Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects = new List<Objects.Object>();
            int obj_index = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.Count;
            Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.Add(temp);
            Load_Object(obj_index);
        }      //adds the object to the list and to the screen
        //------------------------------------

        private void Initialize_Objects() //loads existing objects
        {
            if (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects == null) return;
            Main_Layout.Children.Clear();
            //foreach (Objects.Object o in Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects)
            for (int obj_index = 0; obj_index < Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.Count; obj_index++)
            {
                Load_Object(obj_index);
            }
        }

        

        private void Load_Object(int obj_index) //object with pan and tap for config
        {

            //visible controls but deactivated
            var obj = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];
            var grid = new Object_view(master_menu, bottom_menu, obj_index)
            {
                IsEnabled = false,
                TranslationX = obj.X,
                TranslationY = obj.Y,
                Rotation = obj.Rotation,
            };
            Main_Layout.Children.Add(grid);


            //invisible Grid with controls
            Grid invisible_grid = new Grid
            {
                WidthRequest = obj.Width,
                HeightRequest = obj.Height,
                Rotation = obj.Rotation,
            };
            invisible_grid.TranslateTo(obj.X, obj.Y, 1);
            // add tap gesture
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Navigation.PushModalAsync(new Object_page(master_menu, bottom_menu, obj_index));
            };
            invisible_grid.GestureRecognizers.Add(tapGestureRecognizer);
            // add pan gesture
            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += (s, e) =>
            {
                byte rounding_value = 1;
                if (snapBox.IsChecked) rounding_value = 20;  //checks if snap is enabled
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        invisible_grid.Rotation = 0; //rotation to 0 for correct movement
                        if (Device.RuntimePlatform == Device.UWP)
                        {
                            //Windows is a bitch
                            Debug.WriteLine("it ist a windows !!!!!");
                            invisible_grid.HeightRequest = Main_Layout.Height;
                            invisible_grid.WidthRequest = Main_Layout.Width;
                            invisible_grid.RaiseChild(Main_Layout); //so it doesnt interfear with other objects
                            invisible_grid.TranslateTo(0, 0, 1);
                        }
                        break;
                    case GestureStatus.Running:
                        grid.TranslationX = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index].X + (((int)Math.Round(e.TotalX / (double)rounding_value)) * rounding_value);
                        grid.TranslationY = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index].Y + (((int)Math.Round(e.TotalY / (double)rounding_value)) * rounding_value);
                        break;
                    case GestureStatus.Completed:
                        invisible_grid.Rotation = obj.Rotation; //reset rotation to before
                        invisible_grid.TranslationX = grid.TranslationX;
                        invisible_grid.TranslationY = grid.TranslationY;
                        Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index].X = (int)grid.TranslationX;
                        Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index].Y = (int)grid.TranslationY;
                        invisible_grid.HeightRequest = obj.Height;
                        invisible_grid.WidthRequest = obj.Width;
                        break;
                }
            };
            invisible_grid.GestureRecognizers.Add(panGestureRecognizer);

            Main_Layout.Children.Add(invisible_grid);
        }


    }
}