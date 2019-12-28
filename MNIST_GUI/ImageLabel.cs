using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIST_GUI
{
    class ImageLabelByte
    {
        short label;
        byte[] image;
    }

    class ImageLabeluInt
    {
        short label;
        uint[][] image;
    }

}
