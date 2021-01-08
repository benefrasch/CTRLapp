using System.Collections.Generic;

namespace CTRLapp.Objects
{
    public class Master_Menu_Item
    {
        public string Name { get; set; }

        public string Icon_path { get; set; }
        public List<Bottom_Menu_Item> Bottom_Menu_Items { get; set; }
    }
}
