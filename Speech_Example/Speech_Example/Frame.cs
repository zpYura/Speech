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
        double[] mfcc;

        public Frame(double [] t)
        {
            data = t;
        }

        public Frame(double[] t, double [] mfcc)
        {
            data = t;
            this.mfcc = mfcc;
        }

        public double[] Data
        {
            get { return data; }
            set { data = value; }
    }

        public double[] MFCC
        {
            get { return mfcc; }
            set { mfcc = value; }
        }

    }
}
