using CTRLapp.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRLapp.Variables
{
    class Variables
    {
        public static List<Master_Menu_Item> Layout
        {
            get; set;
        }
        public static readonly string configLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");

        public static bool Alert_active { get; set; }
        public static bool MQTT_connecting { get; set; }
    }
}
