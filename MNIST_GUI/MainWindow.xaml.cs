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
        public MainWindow()
        {
            InitializeComponent();
            storeImageByteArr();
            loadImage(index);
        }

        Bitmap [] bitmapArr;
        int index = 0;

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

        //Load the first image into the imageBox
        void loadImage(int index)
        {
            if(index >= 0)
                imageBox1.Source = Convert(bitmapArr[index]);
        }


        //Print a 2D uint Array
        static void print2DArray(uint[][] x)
        {
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    Console.Write(x[i][j] + " ");
                }
                Console.WriteLine();
            }
        }

        //Reverse the order of a byte array
        byte[] revArr(byte [] x)
        {
            Array.Reverse(x);
            return x;
        }

        void storeLabelByteArr()
        {



        }


        void storeImageByteArr()
        {
            byte [] byteImageArr = System.IO.File.ReadAllBytes("../../train-images.idx3-ubyte");
            byte[] byteLabelArr = System.IO.File.ReadAllBytes("../../train-labels.idx1-ubyte");

            //Read header from file
            uint magic_i = BitConverter.ToUInt32(revArr(byteImageArr.Take(4).ToArray()), 0);
            uint numImages_i = BitConverter.ToUInt32((revArr(byteImageArr.Skip(4).Take(4).ToArray())), 0);
            uint numRows_i = BitConverter.ToUInt32((revArr(byteImageArr.Skip(8).Take(4).ToArray())), 0);
            uint numCols_i = BitConverter.ToUInt32((revArr(byteImageArr.Skip(12).Take(4).ToArray())), 0);

            uint byteImageIndex = 16;
            uint byteLabelIndex = 8;

            bitmapArr = new Bitmap[numImages_i];

            byte[][] byteGrid = new byte[numImages_i][];
            for(int i = 0; i < numImages_i; i++)
            {
                byte[] image = new byte[numRows_i * numCols_i];
                for(int j = 0; j < (numRows_i*numCols_i); j++)
                {
                    image[j] = byteImageArr[byteImageIndex];
                    byteImageIndex++;
                }
                byteGrid[i] = image;

                bitmapArr[i] = pixelArrToBitmap((int)numCols_i, (int)numRows_i, image);
            }
            Console.WriteLine("Store Byte Operation Completed...");
        }
        // Save to file --> bmp.Save(@"Images\img_" + i + ".bmp", ImageFormat.Bmp);



        //Converts a pixel byte array to a bitmap image
        Bitmap pixelArrToBitmap(int width, int height, byte [] byteArr)
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

        //Reads the image into a 3D array of imagesd
        void newalgo()
        {
            byte[] byteArr = System.IO.File.ReadAllBytes("../../train-images.idx3-ubyte");

            //Read header from file
            uint magic_i = BitConverter.ToUInt32(revArr(byteArr.Take(4).ToArray()), 0);
            uint numImages_i = BitConverter.ToUInt32((revArr(byteArr.Skip(4).Take(4).ToArray())), 0);
            uint numRows_i = BitConverter.ToUInt32((revArr(byteArr.Skip(8).Take(4).ToArray())), 0);
            uint numCols_i = BitConverter.ToUInt32((revArr(byteArr.Skip(12).Take(4).ToArray())), 0);
            // Debug.WriteLine($""); <-- useful too!
            Console.WriteLine($"Magic Number: {magic_i}\nNumber of Images: {numImages_i}\nImage Size: {numRows_i} x {numCols_i}\n ");
            
            uint[][][] imageArr = new uint[numImages_i][][];
            uint byteIndex = 16;

            //Parallel.For()
            //For each numRows x numCols image, store into 3D array
            for (int i = 0; i < numImages_i; i++)
            {
                uint [][] image = new uint [numRows_i][];

                //Fill (numRows * numCols) array and store in imageArr
                for (int j = 0; j < numRows_i; j++)
                {
                    uint[] row = new uint[numCols_i];

                    for (int k = 0; k < numCols_i; k++)
                    {
                        row[k] = byteArr[byteIndex];
                        byteIndex++;
                    }

                    image[j] = row;
                }

                imageArr[i] = image;
                print2DArray(imageArr[i]);
            }

        }


        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if(index > 0)
                loadImage(--index);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            loadImage(++index);
        }
    }
}
