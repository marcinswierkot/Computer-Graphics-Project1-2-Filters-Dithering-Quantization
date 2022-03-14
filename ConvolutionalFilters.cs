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

        public static Bitmap ImageWithPadding(Bitmap image, Kernel kernel)
        {
            Bitmap tmp = new Bitmap(image.Width + kernel.Width - 1, image.Height + kernel.Height - 1);
            for (int x = 0; x < tmp.Width; x++)
            {
                for (int y = 0; y < tmp.Height; y++)
                {
                    tmp.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    try
                    {
                        tmp.SetPixel(x + kernel.Anchor.X, y + kernel.Anchor.Y, image.GetPixel(x, y));
                    }
                    catch
                    {
                        MessageBox.Show($"Error occured when creating matrix with padding. Coordinates: x: {x + kernel.Anchor.X} y: {y + kernel.Anchor.Y}");
                    }
                }
            }
            return tmp;
        }

        public static Bitmap Filter(Bitmap image, Kernel kernel)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            Bitmap padded = ImageWithPadding(image, kernel);

            for (int x = 0; x < newImage.Width; x++)
            {
                for (int y = 0; y < newImage.Height; y++)
                {
                    int newR = 0;
                    int newG = 0;
                    int newB = 0;
                    for (int k = 0; k < kernel.Width; k++)
                    {
                        for (int l = 0; l < kernel.Height; l++)
                        {
                            try
                            {
                                newR += (kernel.Matrix[k, l] * padded.GetPixel(x + k, y + l).R);
                                newG += (kernel.Matrix[k, l] * padded.GetPixel(x + k, y + l).G);
                                newB += (kernel.Matrix[k, l] * padded.GetPixel(x + k, y + l).B);
                            }
                            catch
                            {
                                MessageBox.Show($"Error occured when applying convolution filter. Coordinates: x:{x + k} y:{y + l}");
                            }
                        }
                    }
                    newR = (byte)Truncate(kernel.Offset + newR / kernel.Divisor);
                    newG = (byte)Truncate(kernel.Offset + newG / kernel.Divisor);
                    newB = (byte)Truncate(kernel.Offset + newB / kernel.Divisor);
                    try
                    {
                        newImage.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                    }
                    catch
                    {
                        MessageBox.Show($"Error occured when setting new pixels after filter application. Coordinates: x:{x} y: {y}");
                    }
                }
            }

            return newImage;
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
                    resultBuffer[resultOffset] = (byte)((resultBlue > 255) ? 255 : (resultBlue < 0) ? 0 : resultBlue);
                    resultBuffer[resultOffset + 1] = (byte)((resultGreen > 255) ? 255 : (resultGreen < 0) ? 0 : resultGreen);
                    resultBuffer[resultOffset + 2] = (byte)((resultRed > 255) ? 255 : (resultRed < 0) ? 0 : resultRed);
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

        public static Bitmap Grayscale(Bitmap sourceBitmap)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new System.Drawing.Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] byteBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, byteBuffer, 0, byteBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            float rgb = 0;
            for (int k = 0; k < byteBuffer.Length; k += 4)
            {
                rgb = byteBuffer[k] * 0.11f;
                rgb += byteBuffer[k + 1] * 0.59f;
                rgb += byteBuffer[k + 2] * 0.3f;

                byteBuffer[k] = (byte)rgb;
                byteBuffer[k + 1] = byteBuffer[k];
                byteBuffer[k + 2] = byteBuffer[k];
                byteBuffer[k + 3] = 255;
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
                      resultBitmap.Width, resultBitmap.Height),
                      ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(byteBuffer, 0, resultData.Scan0, byteBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        //public static Bitmap ConvolutionFilter(Bitmap sourceBitmap, Kernel filter)
        //{
        //    BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
        //                             sourceBitmap.Width, sourceBitmap.Height),
        //                             ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
        //    byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

        //    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

        //    sourceBitmap.UnlockBits(sourceData);

        //    double blue = 0.0;
        //    double green = 0.0;
        //    double red = 0.0;

        //    int filterWidth = filter.Matrix.GetLength(1);
        //    int filterHeight = filter.Matrix.GetLength(0);

        //    int filterOffset = (filterWidth - 1) / 2;
        //    int calcOffset = 0;
        //    int byteOffset = 0;

        //    for (int offsetY = filterOffset; offsetY <
        //        sourceBitmap.Height - filterOffset; offsetY++)
        //    {
        //        for (int offsetX = filterOffset; offsetX <
        //            sourceBitmap.Width - filterOffset; offsetX++)
        //        {
        //            blue = 0;
        //            green = 0;
        //            red = 0;

        //            byteOffset = offsetY *
        //                         sourceData.Stride +
        //                         offsetX * 4;

        //            for (int filterY = -filterOffset;
        //                filterY <= filterOffset; filterY++)
        //            {
        //                for (int filterX = -filterOffset;
        //                    filterX <= filterOffset; filterX++)
        //                {

        //                    calcOffset = byteOffset +
        //                                 (filterX * 4) +
        //                                 (filterY * sourceData.Stride);

        //                    blue += (double)(pixelBuffer[calcOffset]) *
        //                            filter.Matrix[filterY + filterOffset,
        //                                                filterX + filterOffset];

        //                    green += (double)(pixelBuffer[calcOffset + 1]) *
        //                             filter.Matrix[filterY + filterOffset,
        //                                                filterX + filterOffset];

        //                    red += (double)(pixelBuffer[calcOffset + 2]) *
        //                           filter.Matrix[filterY + filterOffset,
        //                                              filterX + filterOffset];
        //                }
        //            }

        //            blue = (blue / filter.Divisor) + filter.Offset;
        //            green = (green / filter.Divisor) + filter.Offset;
        //            red = (red / filter.Divisor) + filter.Offset;

        //            if (blue > 255)
        //            { blue = 255; }
        //            else if (blue < 0)
        //            { blue = 0; }

        //            if (green > 255)
        //            { green = 255; }
        //            else if (green < 0)
        //            { green = 0; }

        //            if (red > 255)
        //            { red = 255; }
        //            else if (red < 0)
        //            { red = 0; }

        //            resultBuffer[byteOffset] = (byte)(blue);
        //            resultBuffer[byteOffset + 1] = (byte)(green);
        //            resultBuffer[byteOffset + 2] = (byte)(red);
        //            resultBuffer[byteOffset + 3] = 255;
        //        }
        //    }

        //    Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

        //    BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
        //                             resultBitmap.Width, resultBitmap.Height),
        //                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        //    Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
        //    resultBitmap.UnlockBits(resultData);

        //    return resultBitmap;
        //}

    }
}
