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
    public class Grayscale
    {
        public static Bitmap ApplyGrayscale(Bitmap sourceBitmap)
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
    }
}
