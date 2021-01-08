using CTRLapp.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRLapp.Variables
{
    class Json_string
    {
        private static readonly string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");

        public static string Config
        {
            get
            {
                if (!File.Exists(fileName)) return "";
                return File.ReadAllText(fileName);
            }
            set { File.WriteAllText(fileName, value); }
        }

        public static List<Master_Menu_Item> Array
        {
            get
            {
                System.Diagnostics.Debug.WriteLine("Array got read");
                System.Diagnostics.Debug.WriteLine(Config);
                return JsonConvert.DeserializeObject<List<Master_Menu_Item>>(Config);
            }
            set
            {
                Config = JsonConvert.SerializeObject(value);
                System.Diagnostics.Debug.WriteLine("Array got written");
            }
        }

    }
}
