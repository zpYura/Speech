using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Example
{
    class Frame
    {
        double[] data;

        public Frame(double [] t)
        {
            data = t;
        }

        public double[] Data
        {
            get { return data; }
            set { data = value; }
    }

    }
}
