using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.GUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuiPage : ContentPage
    {
        private readonly int masterMenu, bottomMenu;



        public GuiPage(int masterMenu, int bottomMenu)
        {
            InitializeComponent();

            this.masterMenu = masterMenu;
            this.bottomMenu = bottomMenu;
        }
        protected override void OnAppearing()
        {
            InitializeObjects();
            BackgroundImageSource = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].BackgroundImageSource;
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            Debug.WriteLine(JsonConvert.SerializeObject(Variables.Variables.Layout));
            File.WriteAllText(Variables.Variables.configLocation, JsonConvert.SerializeObject(Variables.Variables.Layout));

        }

        private async void ExitButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }


        // \/ \/ add objects here \/ \/


        private void AddLabel(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 100,
                Height = 30,
                Type = "Label"
            };
            temp.Arguments = new string[4];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.Transparent.ToHex();
            temp.Arguments[2] = "Label";
            temp.Arguments[3] = "21";

            AddObject(temp);
        }
        private void AddButton(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 80,
                Height = 40,
                Type = "Button",
            };
            temp.Arguments = new string[5];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.LightGray.ToHex();
            temp.Arguments[2] = "";
            temp.Arguments[3] = "";
            temp.Arguments[4] = "";


            AddObject(temp);
        }
        private void AddSwitch(object sender, EventArgs e)
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

            AddObject(temp);
        }
        private void AddSlider(object sender, EventArgs e)
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

            AddObject(temp);

        }
        private void AddJoystick(object sender, EventArgs e)
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

            AddObject(temp);

        }
        private void AddMatrix(object sender, EventArgs e)
        {
            Objects.Object temp = new Objects.Object
            {
                Width = 200,
                Height = 200,
                Type = "Matrix"
            };
            temp.Arguments = new string[8];
            temp.Arguments[0] = Color.Black.ToHex();
            temp.Arguments[1] = Color.Gray.ToHex();
            temp.Arguments[2] = "";
            temp.Arguments[3] = "";
            temp.Arguments[4] = "0";
            temp.Arguments[5] = "255";
            temp.Arguments[6] = "0";
            temp.Arguments[7] = "255";

            AddObject(temp);
        }


        private void AddObject(Objects.Object temp) //adds and loads new object
        {
            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects == null) Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects = new List<Objects.Object>();
            int obj_index = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Count;
            Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Add(temp);
            LoadObject(obj_index);
        }      //adds the object to the list and to the screen
        //------------------------------------

        private void InitializeObjects() //loads existing objects
        {
            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects == null) return;
            MainLayout.Children.Clear();
            //foreach (Objects.Object o in Variables.Variables.Layout[master_menu].BottomMenuItems[bottom_menu].Objects)
            for (int obj_index = 0; obj_index < Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Count; obj_index++)
            {
                LoadObject(obj_index);
            }
        }



        private void LoadObject(int obj_index) //object with pan and tap for config
        {
            //visible objects, but deactivated
            var obj = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[obj_index];
            var grid = new ObjectView(masterMenu, bottomMenu, obj_index)
            {
                IsEnabled = false,
                TranslationX = obj.X,
                TranslationY = obj.Y,
                Rotation = obj.Rotation,
            };
            MainLayout.Children.Add(grid);


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
                Navigation.PushModalAsync(new ObjectPage(masterMenu, bottomMenu, obj_index));
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
                            invisible_grid.HeightRequest = MainLayout.Height;
                            invisible_grid.WidthRequest = MainLayout.Width;
                            invisible_grid.RaiseChild(MainLayout); //so it doesnt interfear with other objects
                            invisible_grid.TranslateTo(0, 0, 1);
                        }
                        break;
                    case GestureStatus.Running:
                        grid.TranslationX = ((int)(Math.Round(Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[obj_index].X + e.TotalX) / (double)rounding_value)) * rounding_value;
                        grid.TranslationY = ((int)(Math.Round(Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[obj_index].Y + e.TotalY) / (double)rounding_value)) * rounding_value;
                        break;
                    case GestureStatus.Completed:
                        invisible_grid.Rotation = obj.Rotation; //reset rotation to before
                        invisible_grid.TranslationX = grid.TranslationX;
                        invisible_grid.TranslationY = grid.TranslationY;
                        Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[obj_index].X = (int)grid.TranslationX;
                        Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[obj_index].Y = (int)grid.TranslationY;
                        invisible_grid.HeightRequest = obj.Height;
                        invisible_grid.WidthRequest = obj.Width;
                        break;
                }
            };
            invisible_grid.GestureRecognizers.Add(panGestureRecognizer);

            MainLayout.Children.Add(invisible_grid);
        }


    }
}