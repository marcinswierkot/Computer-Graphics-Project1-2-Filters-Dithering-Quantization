using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class MedianCut
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public MedianCut(byte red, byte green, byte blue)
        {
            R = red;
            G = green;
            B = blue;
        }
    }
}
