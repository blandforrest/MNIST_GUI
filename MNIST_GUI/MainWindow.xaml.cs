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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int index = 0;
        uint numImages_i = 0, numRows_i = 0, numCols_i = 0;

        ImageLabelByte[] imageListByte;
        ImageLabeluInt[] ImageListuInt;
        Bitmap[] bitmapArr;

        public MainWindow()
        {
            InitializeComponent();
            storeImageByteClass();
            classToBitmap();
            loadImage(index);
        }


        #region WPF Form
        //Load the first image into the imageBox
        void loadImage(int index)
        {
            if (index >= 0)
                labelBox.Content = imageListByte[index].label;
            imageBox1.Source = Convert(bitmapArr[index]);
        }

        //Go to previous image
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (index > 0)
                loadImage(--index);
        }

        //Go to next image
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            loadImage(++index);
        }
        #endregion

        #region Image & Label

        //Converts a pixel byte array to a bitmap image
        void storeImageByteClass()
        {
            byte[] byteImageArr = System.IO.File.ReadAllBytes("../../train-images.idx3-ubyte");
            byte[] byteLabelArr = System.IO.File.ReadAllBytes("../../train-labels.idx1-ubyte");

            //Read header from file
            numImages_i = BitConverter.ToUInt32((revArr(byteImageArr.Skip(4).Take(4).ToArray())), 0);
            numRows_i = BitConverter.ToUInt32((revArr(byteImageArr.Skip(8).Take(4).ToArray())), 0);
            numCols_i = BitConverter.ToUInt32((revArr(byteImageArr.Skip(12).Take(4).ToArray())), 0);

            uint byteImageIndex = 16;
            uint byteLabelIndex = 8;

            imageListByte = new ImageLabelByte[numImages_i];

            byte[][] byteGrid = new byte[numImages_i][];
            for (int i = 0; i < numImages_i; i++)
            {
                byte[] image = new byte[numRows_i * numCols_i];
                for (int j = 0; j < (numRows_i * numCols_i); j++)
                {
                    image[j] = byteImageArr[byteImageIndex++];
                }
                imageListByte[i] = new ImageLabelByte(byteLabelArr[byteLabelIndex++], image);
            }
            Console.WriteLine("Store Byte Operation Completed...");
            // Save to file --> bmp.Save(@"Images\img_" + i + ".bmp", ImageFormat.Bmp); // Debug.WriteLine($""); <-- useful too! //Parallel.For()
        }

        //Reads the image into a 3D array of images
        void storeImageUintClass()
        {
            byte[] byteArr = System.IO.File.ReadAllBytes("../../train-images.idx3-ubyte");
            byte[] byteLabelArr = System.IO.File.ReadAllBytes("../../train-labels.idx1-ubyte");

            //Read header from file
            numImages_i = BitConverter.ToUInt32((revArr(byteArr.Skip(4).Take(4).ToArray())), 0);
            numRows_i = BitConverter.ToUInt32((revArr(byteArr.Skip(8).Take(4).ToArray())), 0);
            numCols_i = BitConverter.ToUInt32((revArr(byteArr.Skip(12).Take(4).ToArray())), 0);

            //uint[][][] imageArr = new uint[numImages_i][][];
            uint byteImageIndex = 16;
            uint byteLabelIndex = 8;

            ImageListuInt = new ImageLabeluInt[numImages_i];

            //For each numRows x numCols image, store into 3D array
            for (int i = 0; i < numImages_i; i++)
            {
                uint[][] image = new uint[numRows_i][];

                //Fill (numRows * numCols) array and store in imageArr
                for (int j = 0; j < numRows_i; j++)
                {
                    uint[] row = new uint[numCols_i];

                    for (int k = 0; k < numCols_i; k++)
                    {
                        row[k] = byteArr[byteImageIndex++];
                    }
                    image[j] = row;
                }
                ImageListuInt[i] = new ImageLabeluInt(byteLabelArr[byteLabelIndex++], image);
            }
        }

        //Stores the images of the byte array as a bitmap
        void classToBitmap()
        {
            bitmapArr = new Bitmap[numImages_i];

            for (int i = 0; i < numImages_i; i++)
            {
                bitmapArr[i] = pixelArrToBitmap((int)numRows_i, (int)numCols_i, imageListByte[i].image);
            }
        }

        //Converts a pixel array to a bitmap format
        Bitmap pixelArrToBitmap(int width, int height, byte[] byteArr)
        {
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            ColorPalette ncp = bmp.Palette;

            for (int i = 0; i < 256; i++)
            {
                ncp.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
            }
            bmp.Palette = ncp;

            var boundRect = new System.Drawing.Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(boundRect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * bmp.Height;

            Marshal.Copy(byteArr, 0, ptr, bytes);

            bmp.UnlockBits(bmpData);


            return bmp;
        }

        //Convert a bitmap to an image source for imageBox 
        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        //Reverse the order of a byte array
        byte[] revArr(byte[] x)
        {
            Array.Reverse(x);
            return x;
        }
        #endregion

















    }
}
