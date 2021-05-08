using System.Collections.Generic;

namespace CTRLapp.Objects
{
    public class MasterMenuItem
    {
        public string Name { get; set; }
        public string IconPath { get; set; }
        public List<BottomMenuItem> BottomMenuItems { get; set; }
    }
}
