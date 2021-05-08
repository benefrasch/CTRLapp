using CTRLapp.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRLapp.Variables
{
    class Variables
    {
        public static List<MasterMenuItem> Layout
        {
            get; set;
        }
        public static readonly string configLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");

    }
}
