using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project1
{
    public class ConvolutionalFilters
    {
        private static int Truncate(int value)
        {
            if (value > 255)
                return 255;
            if (value < 0)
                return 0;
            return value;
        }
        public static Kernel FindKernelByName(List<Kernel> kernels, string name)
        {
            foreach (var kernel in kernels)
            {
                if (kernel.Name == name)
                    return kernel;
            }
            return null;
        }
        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap, Kernel filter)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                        sourceBitmap.Width, sourceBitmap.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filtersizeY = filter.Height;
            int filtersizeX = filter.Width;
            int filterOffsetX = (filtersizeX - 1) / 2;
            int filterOffsetY = (filtersizeY - 1) / 2;
            int byteOffset = 0;
            int resultOffset = 0;

            for (int offsetY = 0; offsetY < sourceBitmap.Height; offsetY++)
            {
                for (int offsetX = 0; offsetX < sourceBitmap.Width; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;
                    for (int filterY = -filterOffsetY + filter.Anchor.Y; filterY <= filterOffsetY + filter.Anchor.Y; filterY++)
                    {
                        for (int filterX = -filterOffsetX + filter.Anchor.X; filterX <= filterOffsetX + filter.Anchor.X; filterX++)
                        {

                            int vertPos = offsetY + filterY;
                            int horPos = 4 * (offsetX + filterX);

                            if (vertPos > sourceData.Height - 1)
                            {
                                vertPos = sourceData.Height - 1;
                            }
                            else if (vertPos < 0)
                            {
                                vertPos = 0;
                            }
                            if (horPos >= sourceData.Stride)
                            {
                                horPos = sourceData.Stride - 4;
                            }
                            else if (horPos < 0)
                            {
                                horPos = 0;
                            }

                            byteOffset = ((vertPos * sourceData.Stride) + horPos);

                            blue += (double)(pixelBuffer[byteOffset]) *
                                     filter.Matrix[filterY - filter.Anchor.Y + filterOffsetY,
                                     filterX - filter.Anchor.X + filterOffsetX];


                            green += (double)(pixelBuffer[byteOffset + 1]) *
                                      filter.Matrix[filterY - filter.Anchor.Y + filterOffsetY,
                                      filterX - filter.Anchor.X + filterOffsetX];


                            red += (double)(pixelBuffer[byteOffset + 2]) *
                                    filter.Matrix[filterY - filter.Anchor.Y + filterOffsetY,
                                    filterX - filter.Anchor.X + filterOffsetX];
                        }
                    }
                    resultOffset = (offsetX * 4) + (offsetY * sourceData.Stride);
                    double resultBlue = (blue / filter.Divisor) + filter.Offset;
                    double resultGreen = (green / filter.Divisor) + filter.Offset;
                    double resultRed = (red / filter.Divisor) + filter.Offset;
                    resultBuffer[resultOffset] = (byte)Truncate((int)resultBlue);
                    resultBuffer[resultOffset + 1] = (byte)Truncate((int)resultGreen);
                    resultBuffer[resultOffset + 2] = (byte)Truncate((int)resultRed);
                    resultBuffer[resultOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }
    }
}
