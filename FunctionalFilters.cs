using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PointCollection = System.Windows.Media.PointCollection;

namespace Project1
{
    public class FunctionalFilters
    {
        public static int Truncate(int value)
        {
            if (value > 255)
                return 255;
            if (value < 0)
                return 0;
            return value;
        }
        public static Bitmap Filter(Bitmap Image, Func<Bitmap, int, int, Bitmap> func)
        {
            Bitmap newImage = new Bitmap(Image);
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    newImage = func(newImage, i, j);
                }
            }
            return newImage;
        }
        public static Bitmap Invert(Bitmap Image, int i, int j)
        {
            Color oldColor = Image.GetPixel(i, j);
            byte newR = (byte)(255 - oldColor.R);
            byte newG = (byte)(255 - oldColor.G);
            byte newB = (byte)(255 - oldColor.B);
            Color newColor = Color.FromArgb(newR, newG, newB);
            Image.SetPixel(i, j, newColor);
            return Image;
        }

        public static Bitmap CorrectBrightness(Bitmap Image, int i, int j)
        {
            Color oldColor = Image.GetPixel(i, j);
            byte newR = (byte)Truncate(Constants.BrightnessCoefficient + oldColor.R);
            byte newG = (byte)Truncate(Constants.BrightnessCoefficient + oldColor.G);
            byte newB = (byte)Truncate(Constants.BrightnessCoefficient + oldColor.B);
            Color newColor = Color.FromArgb(newR, newG, newB);
            Image.SetPixel(i, j, newColor);
            return Image;
        }

        public static Bitmap EnhanceContrast(Bitmap Image, int i, int j)
        {
            double Factor = (double)(259 * (255 + Constants.ContrastCoefficient)) / (double)(255 * (259 - Constants.ContrastCoefficient));
            Color oldColor = Image.GetPixel(i, j);
            byte newR = (byte)Truncate((int)(Factor * (oldColor.R - 128)) + 128);
            byte newG = (byte)Truncate((int)(Factor * (oldColor.G - 128)) + 128);
            byte newB = (byte)Truncate((int)(Factor * (oldColor.B - 128)) + 128);
            Color newColor = Color.FromArgb(newR, newG, newB);
            Image.SetPixel(i, j, newColor);
            return Image;
        }
        public static Bitmap CorrectGamma(Bitmap Image, int i, int j)
        {
            Color oldColor = Image.GetPixel(i, j);
            byte newR = (byte)(Math.Pow((double)oldColor.R / 255, Constants.GammaCoefficient) * 255);
            byte newG = (byte)(Math.Pow((double)oldColor.G / 255, Constants.GammaCoefficient) * 255);
            byte newB = (byte)(Math.Pow((double)oldColor.B / 255, Constants.GammaCoefficient) * 255);
            Color newColor = Color.FromArgb(newR, newG, newB);
            Image.SetPixel(i, j, newColor);
            return Image;
        }
        public static Bitmap Custom1(Bitmap Image, PointCollection pointCollection)
        {
            Bitmap newImage = new Bitmap(Image);
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    Color oldColor = newImage.GetPixel(i, j);
                    byte newR = (byte)(255 - pointCollection[oldColor.R].Y);
                    byte newG = (byte)(255 - pointCollection[oldColor.G].Y);
                    byte newB = (byte)(255 - pointCollection[oldColor.B].Y);
                    Color newColor = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(i, j, newColor);
                }
            }
            return newImage;
        }
        public static Bitmap Custom2(Bitmap Image, PointCollection pointCollection)
        {
            Bitmap newImage = new Bitmap(Image);
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    Color oldColor = newImage.GetPixel(i, j);
                    byte newR = (byte)(255 - pointCollection[oldColor.R].Y);
                    byte newG = (byte)(255 - pointCollection[oldColor.G].Y);
                    byte newB = (byte)(255 - pointCollection[oldColor.B].Y);
                    Color newColor = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(i, j, newColor);
                }
            }
            return newImage;
        }
    }
}
