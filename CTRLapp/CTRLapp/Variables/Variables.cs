using CTRLapp.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace CTRLapp.Variables
{
    class Variables
    {
        public static List<MasterMenuItem> Layout
        {
            get; set;
        }
        public static readonly string configLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");



        public static Color PrimaryColor
        {
            get
            {
                if (Application.Current.RequestedTheme == OSAppTheme.Light) return Color.WhiteSmoke;
                else return Color.FromHex("#181818");
            }
        }
        public static Color SecondaryColor
        {
            get
            {
                if (Application.Current.RequestedTheme == OSAppTheme.Light) return Color.Black;
                else return Color.WhiteSmoke;
            }
        }
    }
}
