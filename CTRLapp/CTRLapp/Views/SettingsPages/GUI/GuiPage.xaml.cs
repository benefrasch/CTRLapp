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

        private async void ExitButtonClicked(object _, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }


        // \/ \/ add objects here \/ \/


        private void AddLabel(object _, EventArgs e)
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
        private void AddButton(object _, EventArgs e)
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
        private void AddSwitch(object _, EventArgs e)
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
        private void AddSlider(object _, EventArgs e)
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
        private void AddJoystick(object _, EventArgs e)
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
        private void AddMatrix(object _, EventArgs e)
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
            int objIndex = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Count;
            Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Add(temp);
            LoadObject(objIndex);
        }      //adds the object to the list and to the screen
        //------------------------------------

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
            Grid invisibleGrid = new Grid
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
                Navigation.PushModalAsync(new ObjectPage(masterMenu, bottomMenu, objIndex));
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