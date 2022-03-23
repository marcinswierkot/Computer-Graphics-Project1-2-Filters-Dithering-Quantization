using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project1
{
    public class MedianCutAlgorithm
    {
        public List<System.Drawing.Color> colorPalette { get; set; }
        public byte[] pixelBuffer { get; set; }
        public int numberOfColors { get; set; }
        public int numberOfLevels { get; set; }

        public MedianCutAlgorithm(int numColors)
        {
            colorPalette = new List<System.Drawing.Color>();
            numberOfColors = numColors;
            numberOfLevels = (int)Math.Ceiling(Math.Log(numColors, 2));
        }
        public Bitmap ApplyMedianCut(Bitmap sourceBitmap)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                        sourceBitmap.Width, sourceBitmap.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);
            List<MedianCut> imageColors = new List<MedianCut>();

            for (int k = 0; k < pixelBuffer.Length; k += 4)
            {
                imageColors.Add(new MedianCut(pixelBuffer[k + 2], pixelBuffer[k + 1], pixelBuffer[k]));
            }

            SplitIntoBuckets(imageColors, numberOfLevels);
            if (Math.Pow(2, numberOfLevels) > numberOfColors)
                MergeColors();
            FindRepresentativeColor();

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
        public void FindRepresentativeColor()
        {
            for (int k = 0; k < pixelBuffer.Length; k += 4)
            {
                int index = 0;
                double closest = distanceFromRGB(pixelBuffer[k + 2], pixelBuffer[k + 1], pixelBuffer[k], colorPalette[0]);
                for (int i = 1; i < colorPalette.Count; i++)
                {
                    if (closest > distanceFromRGB(pixelBuffer[k + 2], pixelBuffer[k + 1], pixelBuffer[k], colorPalette[i]))
                    {
                        closest = distanceFromRGB(pixelBuffer[k + 2], pixelBuffer[k + 1], pixelBuffer[k], colorPalette[i]);
                        index = i;
                    }
                }
                pixelBuffer[k] = colorPalette[index].B;
                pixelBuffer[k + 1] = colorPalette[index].G;
                pixelBuffer[k + 2] = colorPalette[index].R;
            }
        }

        public double distanceFromRGB(byte red, byte green, byte blue, System.Drawing.Color color)
        {
            double redDiff = Math.Abs(red - color.R);
            double greenDiff = Math.Abs(green - color.G);
            double blueDiff = Math.Abs(blue - color.B);
            return ((redDiff + greenDiff + blueDiff) / 3);
        }
        public void SplitIntoBuckets(List<MedianCut> c, int size)
        {
            if (size == 0)
            {
                CalculateColor(c);
                return;
            }

            byte redRange = (byte)(c.Max(col => col.R) - c.Min(col => col.R));
            byte greenRange = (byte)(c.Max(col => col.G) - c.Min(col => col.G));
            byte blueRange = (byte)(c.Max(col => col.B) - c.Min(col => col.B));

            if (redRange >= greenRange && redRange >= blueRange)
                c.Sort((x, y) => x.R.CompareTo(y.R));
            else if (blueRange >= redRange && blueRange >= greenRange)
                c.Sort((x, y) => x.B.CompareTo(y.B));
            else if (greenRange >= redRange && greenRange >= blueRange)
                c.Sort((x, y) => x.G.CompareTo(y.G));

            int median = c.Count / 2;
            int z = c.Count - c.GetRange(0, median).Count;
            SplitIntoBuckets(c.GetRange(0, median), size - 1);
            SplitIntoBuckets(c.GetRange(median, z), size - 1);
        }
        public void CalculateColor(List<MedianCut> c)
        {
            int redAverage = c.Sum(x => x.R) / c.Count;
            int greenAverage = c.Sum(x => x.G) / c.Count;
            int blueAverage = c.Sum(x => x.B) / c.Count;
            System.Drawing.Color col = System.Drawing.Color.FromArgb(255, redAverage, greenAverage, blueAverage);
            colorPalette.Add(col);
        }
        public void MergeColors()
        {
            int diff = (int)Math.Pow(2, numberOfLevels) - numberOfColors;
            List<System.Drawing.Color> temporaryColors = new List<System.Drawing.Color>();

            for (int i = 0; i < diff; i++)
            {
                System.Drawing.Color firstColor = colorPalette[colorPalette.Count - 2];
                System.Drawing.Color secondColor = colorPalette[colorPalette.Count - 1];
                colorPalette.RemoveRange(colorPalette.Count - 2, 2);
                System.Drawing.Color resultingColor = System.Drawing.Color
                    .FromArgb(255, (firstColor.R + secondColor.R) / 2, (firstColor.G + secondColor.G) / 2, (firstColor.B + secondColor.B) / 2);
                temporaryColors.Add(resultingColor);
            }

            colorPalette.AddRange(temporaryColors);
        }
    }
}
