using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.Settings_pages.GUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Object_page : ContentPage
    {
        private int master_menu, bottom_menu, obj_index;

        public Object_page(int master_menu, int bottom_menu, int obj_index)
        {
            this.master_menu = master_menu; this.bottom_menu = bottom_menu; this.obj_index = obj_index;
            InitializeComponent();



        }


        protected override void OnAppearing()
        {
            grid.BindingContext = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];


            //just ignore this piece of shit code, it works so don't touch it! (except, if it doesn't)
            switch (Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index].Type)
            {
                case "Button":
                    var edit1 = new edit_layouts.Button();
                    edit1.UpdateEvent += (s, e) =>
                    {
                        UpdatePreview();
                    };
                    edit_stack.Children.Add(edit1);
                    break;
                case "Switch":
                    var edit2 = new edit_layouts.Switch();
                    edit2.UpdateEvent += (s, e) =>
                    {
                        UpdatePreview();
                    };
                    edit_stack.Children.Add(edit2);
                    break;
                case "Slider":
                    var edit3 = new edit_layouts.Slider();
                    edit3.UpdateEvent += (s, e) =>
                    {
                        UpdatePreview();
                    };
                    edit_stack.Children.Add(edit3);
                    break;
                case "Joystick":
                    var edit4 = new edit_layouts.Joystick();
                    edit4.UpdateEvent += (s, e) =>
                    {
                        UpdatePreview();
                    };
                    edit_stack.Children.Add(edit4);
                    break;
            }



            UpdatePreview();

            base.OnAppearing();
        }


        private Frame view; // so we can easily delete it, when updating preview
        private void UpdatePreview(object sender = null, TextChangedEventArgs e = null) //preview in middle of right side
        {
            if (view != null) grid.Children.Remove(view);
            var obj = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];
            view = new Frame()
            {
                BorderColor = Color.LightGray,
                CornerRadius = 0,
                HasShadow = false,
                Padding = 0,
                Rotation = obj.Rotation,
            }; //to outline the object, i.e. for visualizing size
            view.Content = new Object_view(master_menu, bottom_menu, obj_index); //make the view without translation
            grid.Children.Add(view, 2, 0);
            view.HorizontalOptions = LayoutOptions.Center;
            view.VerticalOptions = LayoutOptions.Center;
        }

        private void Save_Button_Pressed(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private async void Delete_Button_Pressed(object sender, EventArgs e)
        {
            bool accept = await DisplayAlert("Delete?", "do you really want to delete this?", "Yes", "No");
            if (accept)
                Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects.RemoveAt(obj_index);
            _ = Navigation.PopModalAsync();
        }
    }


}