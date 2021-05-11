using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.GUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ObjectPage : ContentPage
    {
        private readonly int masterMenu, bottomMenu, objIndex;

        public ObjectPage(int masterMenu, int bottomMenu, int objIndex)
        {
            this.masterMenu = masterMenu; this.bottomMenu = bottomMenu; this.objIndex = objIndex;
            InitializeComponent();

        }


        protected override void OnAppearing()
        {
            grid.BindingContext = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex];


            //just ignore this piece of shit code, it works so don't touch it! (except, if it doesn't)
            switch (Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex].Type)
            {
                case "Label":
                    sizeStack.IsVisible = false;

                    var edit5 = new EditLayouts.Label();
                    edit5.UpdateEvent += (_, e) =>
                    {
                        UpdatePreview();
                    };
                    editStack.Children.Add(edit5);
                    break;
                case "Button":
                    var edit1 = new EditLayouts.Button();
                    edit1.UpdateEvent += (_, e) =>
                    {
                        UpdatePreview();
                    };
                    editStack.Children.Add(edit1);
                    break;
                case "Switch":
                    var edit2 = new EditLayouts.Switch();
                    edit2.UpdateEvent += (_, e) =>
                    {
                        UpdatePreview();
                    };
                    editStack.Children.Add(edit2);
                    break;
                case "Slider":
                    var edit3 = new EditLayouts.Slider();
                    edit3.UpdateEvent += (_, e) =>
                    {
                        UpdatePreview();
                    };
                    editStack.Children.Add(edit3);
                    break;
                case "Joystick":
                    var edit4 = new EditLayouts.Joystick();
                    edit4.UpdateEvent += (_, e) =>
                    {
                        UpdatePreview();
                    };
                    editStack.Children.Add(edit4);
                    break;
                case "Matrix":
                    var edit6 = new EditLayouts.Matrix();
                    edit6.UpdateEvent += (_, e) =>
                    {
                        UpdatePreview();
                    };
                    editStack.Children.Add(edit6);
                    break;
            }



            UpdatePreview();

            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            Debug.WriteLine(JsonConvert.SerializeObject(Variables.Variables.Layout));
            File.WriteAllText(Variables.Variables.configLocation, JsonConvert.SerializeObject(Variables.Variables.Layout));

        }

        private Frame view; // so we can easily delete it, when updating preview
        private void UpdatePreview(object _ = null, TextChangedEventArgs e = null) //preview in middle of right side
        {
            if (view != null) grid.Children.Remove(view);
            var obj = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex];
            view = new Frame()
            {
                BorderColor = Color.LightGray,
                CornerRadius = 0,
                HasShadow = false,
                Padding = 0,
                Rotation = obj.Rotation,
                BackgroundColor = Color.Transparent,
            }; //to outline the object, i.e. for visualizing size
            view.Content = new ObjectView(masterMenu, bottomMenu, objIndex); //make the view without translation
            grid.Children.Add(view, 2, 0);
            view.HorizontalOptions = LayoutOptions.Center;
            view.VerticalOptions = LayoutOptions.Center;
        }

        private void SaveButtonPressed(object _, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private async void DeleteButtonPressed(object _, EventArgs e)
        {
            bool accept = await DisplayAlert("Delete?", "do you really want to delete this?", "Yes", "No");
            if (accept)
                Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects.RemoveAt(objIndex);
            _ = Navigation.PopModalAsync();
        }
    }


}