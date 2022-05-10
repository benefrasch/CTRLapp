using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.EntryViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValueEntry : ContentView
    {
        public event EventHandler<EventArgs> ValueChanged;

        // Label Text Property
        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(
                propertyName: "LabelText",
                returnType: typeof(string),
                declaringType: typeof(ValueEntry),
                defaultBindingMode: BindingMode.OneTime);

        // Entry Value Property
        public string EntryText
        {
            get { return (string)GetValue(EntryTextProperty); }
            set
            {
                SetValue(EntryTextProperty, value);
                ValueChanged?.Invoke(this, null);
            }
        }
        public static readonly BindableProperty EntryTextProperty = BindableProperty.Create(
                propertyName: "EntryText",
                returnType: typeof(string),
                declaringType: typeof(ValueEntry),
                defaultBindingMode: BindingMode.TwoWay);


        public ValueEntry()
        {
            InitializeComponent();
            stack.BindingContext = this;
        }

    }
}