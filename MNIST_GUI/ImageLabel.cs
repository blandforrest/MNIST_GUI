using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace MNIST_GUI
{
    class ImageLabelByte
    {
        public ImageLabelByte(short m_label, byte [] m_image)
        {
            label = m_label;
            image = m_image;
        }

        public short label;
        public byte[] image;
    }

    class ImageLabeluInt
    {
        public ImageLabeluInt(short m_label, uint [][] m_image)
        {
            label = m_label;
            image = m_image;
        }

        public short label;
        public uint[][] image;
    }

}
