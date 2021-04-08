using CTRLapp.Objects;
using CTRLapp.Variables;
using CTRLapp.Views.Settings_pages.GUI;
using Newtonsoft.Json;
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
        private int master_menu, bottom_menu, current_index;

        public Gui_page(int master_menu, int bottom_menu)
        {
            InitializeComponent();

            this.master_menu = master_menu;
            this.bottom_menu = bottom_menu;
        }
        protected override void OnAppearing()
        {
            Load_Objects();
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
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




        private void Add_Object(Objects.Object temp)
        {
            int index = 0;
            if (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects != null) index = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.Count;
            Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.Add(temp);
            Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects;
            Load_Object(index);
        }      //adds the object to the list and to the screen
        //------------------------------------

        private void Load_Objects()
        {
            if (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects == null) return;
            Main_Layout.Children.Clear();
            //foreach (Objects.Object o in Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects)
            for (int index = 0; index < Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.Count; index++)
            {
                Load_Object(index);
            }
        }
        private void Load_Object(int index)
        {

            //visible controls but deactivated
            var obj = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[index];
            var grid = Object_view.View(master_menu, bottom_menu, index);
            grid.IsEnabled = false;
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
                current_index = index;
                // Set_Labels();
                Navigation.PushModalAsync(new Object_page(master_menu, bottom_menu, index));
            };
            invisible_grid.GestureRecognizers.Add(tapGestureRecognizer);
            // add pan gesture
            var panGestureRecognizer = new PanGestureRecognizer();
            Point first_translation = new Point();
            panGestureRecognizer.PanUpdated += (s, e) =>
            {
                current_index = index;
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        invisible_grid.Rotation = 0;
                        if (Device.RuntimePlatform == Device.UWP)
                        {
                            //Windows is a bitch and doesnt recognize a pan outside the View, 
                            //so I have to make the inv-grid the entire screen
                            Debug.WriteLine("it ist a windows !!!!!"); 
                            first_translation.X = invisible_grid.TranslationX;
                            first_translation.Y = invisible_grid.TranslationY;
                            invisible_grid.HeightRequest = Main_Layout.Height;
                            invisible_grid.WidthRequest = Main_Layout.Width;
                            invisible_grid.TranslateTo(0, 0, 1);
                        }
                        break;
                    case GestureStatus.Running:
                        grid.TranslateTo(e.TotalX + invisible_grid.TranslationX + first_translation.X, e.TotalY + invisible_grid.TranslationY + first_translation.Y, 20);
                        break;
                    case GestureStatus.Completed:
                        invisible_grid.Rotation = obj.Rotation;
                        invisible_grid.TranslateTo(grid.TranslationX, grid.TranslationY, 1);
                        Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[index].X = (int)grid.TranslationX;
                        Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[index].Y = (int)grid.TranslationY;
                        invisible_grid.HeightRequest = obj.Height;
                        invisible_grid.WidthRequest = obj.Width;
                        break;
                }
            };
            invisible_grid.GestureRecognizers.Add(panGestureRecognizer);

            Main_Layout.Children.Add(invisible_grid);
        }


        async private void Delete_Object(object sender, EventArgs e)
        {
            if (!await App.Current.MainPage.DisplayAlert("Attention", "do you really want to delete this object", "yes", "no")) return;
            Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.RemoveAt(current_index);
            Load_Objects();
        }

    }
}