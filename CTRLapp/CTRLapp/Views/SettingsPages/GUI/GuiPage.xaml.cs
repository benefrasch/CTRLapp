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

        private struct Item
        {
            public Item(string name, Objects.Object @object)
            {
                Name = name; NewObject = @object;
            }
            public string Name { get; set; }
            public Objects.Object NewObject { get; set; }
        }

        readonly List<Item> items = new List<Item>()
        {
            new Item("Label", new Objects.Object
            {
                Width = 100,
                Height = 30,
                Type = "Label",
                Arguments = new string[4] { SecondaryColor.ToHex(), Color.Transparent.ToHex(), "Label", "21" }
            }),
            //new Item("ValueDisplay", new Objects.Object
            //{
            //    Width = 100,
            //    Height = 30,
            //    Type = "ValueDisplay",
            //    Arguments = new string[6] { SecondaryColor.ToHex(), Color.Transparent.ToHex(), "Value display","","", "21" }
            //}), //buggy, so it is deactivated until fixed
            new Item("Button", new Objects.Object
            {
                Width = 80,
                Height = 40,
                Type = "Button",
                Arguments = new string[5] { PrimaryColor.ToHex(), SecondaryColor.ToHex(), "Button", "", "" },
            }),
            new Item("SwitchButton", new Objects.Object
            {
                Width = 80,
                Height = 40,
                Type = "SwitchButton",
                Arguments = new string[7] { PrimaryColor.ToHex(), SecondaryColor.ToHex(),Color.Red.ToHex(), "Switch btn", "", "0", "255"},
            }),
            new Item("Switch", new Objects.Object
            {
                Width = 80,
                Height = 40,
                Type = "Switch",
                Arguments = new string[5] { SecondaryColor.ToHex(), Color.Red.ToHex(), "", "0", "255" },
            }),
            new Item("Slider", new Objects.Object
            {
                Width = 200,
                Height = 40,
                Type = "Slider",
                Rotation = 0,
                Arguments = new string[6] { SecondaryColor.ToHex(), Color.Red.ToHex(), Color.Gray.ToHex(), "", "0", "255" },
            }),
            new Item("Joystick", new Objects.Object
            {
                Width = 200,
                Height = 200,
                Type = "Joystick",
                Arguments = new string[10] { SecondaryColor.ToHex(), "#80404040", "", "", "0", "255", "1", "0", "255", "1" },
            }),
            new Item("Matrix", new Objects.Object
            {
                Width = 200,
                Height = 200,
                Type = "Matrix",
                Arguments = new string[8] { SecondaryColor.ToHex(), "#80404040", "", "", "0", "255","0", "255" },
            }),
        };

        static private Color PrimaryColor
        {
            get
            {
                if (Application.Current.RequestedTheme == OSAppTheme.Light) return Color.WhiteSmoke;
                else return Color.FromHex("#181818");
            }
        }
        static private Color SecondaryColor
        {
            get
            {
                if (Application.Current.RequestedTheme == OSAppTheme.Light) return Color.Black;
                else return Color.WhiteSmoke;
            }
        }


        public GuiPage(int masterMenu, int bottomMenu)
        {
            InitializeComponent();

            this.masterMenu = masterMenu;
            this.bottomMenu = bottomMenu;

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
            Debug.WriteLine(JsonConvert.SerializeObject(Variables.Variables.Layout));
            File.WriteAllText(Variables.Variables.configLocation, JsonConvert.SerializeObject(Variables.Variables.Layout));
            base.OnDisappearing();
        }

        private async void ExitButtonClicked(object _, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }



        private void NewObject(object _, ItemTappedEventArgs e)
        {
            AddObject(((Item)e.Item).NewObject);

        }




        private void AddObject(Objects.Object temp) //adds and loads new object
        {
            if (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects == null)
                Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects = new List<Objects.Object>();
            int objIndex = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Count;
            Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.Add(temp);
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