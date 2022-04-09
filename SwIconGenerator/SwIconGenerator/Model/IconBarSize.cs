using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwIconGenerator
{
    public class IconBarSize
    {
        public IconBarSize(int size)
        {
            Size = size;
        }

        public int Size { get; set; }

        public bool Enable { get; set; } = true;
    }
}
