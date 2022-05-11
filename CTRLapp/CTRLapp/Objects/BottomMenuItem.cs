using System.Collections.Generic;

namespace CTRLapp.Objects
{
    public class BottomMenuItem
    {
        public string Name { get; set; }

        public string IconPath { get; set; }

        public string BackgroundImageSource { get; set; }

        public List<CTRLapp.Objects.BaseObject> Objects { get; set; }
    }
}

