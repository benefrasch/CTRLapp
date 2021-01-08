using System.Collections.Generic;

namespace CTRLapp.Objects
{
    public class Object
    {
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Rotation { get; set; }

        public string[] Arguments { get; set; }
    }
}
