using CTRLapp.Objects;
using System.Collections.Generic;

namespace CTRLapp.Variables
{
    class Variables
    {
        public static List<Master_Menu_Item> Layout
        {
            get; set;
        }

        public static bool Alert_active { get; set; }
        public static bool MQTT_connecting { get; set; }
    }
}
