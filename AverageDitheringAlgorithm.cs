using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class AverageDitheringAlgorithm
    {
        public byte[] pixelBuffer { get; set; }
        public int numberRedLevels { get; set; }
        public int numberGreenLevels { get; set; }
        public int numberBlueLevels { get; set; }
        public List<byte> redLevels { get; set; }
        public List<byte> greenLevels { get; set; }
        public List<byte> blueLevels { get; set; }
        public List<byte> redThreshold { get; set; }
        public List<byte> greenThreshold { get; set; }
        public List<byte> blueThreshold { get; set; }
        public AverageDitheringAlgorithm(int numRed, int numGreen, int numBlue)
        {
            numberRedLevels = numRed;
            numberGreenLevels = numGreen;
            numberBlueLevels = numBlue;
            redLevels = new List<byte>();
            greenLevels = new List<byte>();
            blueLevels = new List<byte>();
            redThreshold = new List<byte>();
            greenThreshold = new List<byte>();
            blueThreshold = new List<byte>();
        }
        public Bitmap ApplyAverageDithering(Bitmap sourceBitmap)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                            sourceBitmap.Width, sourceBitmap.Height),
                            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);
            InitialiseColorLevels();
            EvaluateThresholds();
            AssignColorLevels();

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;

        }
        public void InitialiseColorLevels()
        {
            double splitPointRed = 255 / (numberRedLevels - 1);
            double splitPointGreen = 255 / (numberGreenLevels - 1);
            double splitPointBlue = 255 / (numberBlueLevels - 1);

            redLevels.Add(0);
            for (int i = 1; i < numberRedLevels - 1; i++)
                redLevels.Add((byte)(splitPointRed * i));
            redLevels.Add(255);

            greenLevels.Add(0);
            for (int i = 1; i < numberGreenLevels - 1; i++)
                greenLevels.Add((byte)(splitPointGreen * i));
            greenLevels.Add(255);

            blueLevels.Add(0);
            for (int i = 1; i < numberBlueLevels - 1; i++)
                blueLevels.Add((byte)(splitPointBlue * i));
            blueLevels.Add(255);
        }
        public void EvaluateThresholds()
        {
            for (int i = 0; i < numberRedLevels - 1; i++)
            {
                int sumRed = 0;
                int counterRed = 0;
                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    if((pixelBuffer[k + 2] >= redLevels[i] && pixelBuffer[k + 2] < redLevels[i + 1]) 
                        || (pixelBuffer[k + 2] == redLevels[i + 1] && i == numberRedLevels - 2))
                    {
                        counterRed++;
                        sumRed += pixelBuffer[k + 2];
                    }
                }
                if (counterRed == 0)
                    redThreshold.Add(redLevels[i]);
                else
                    redThreshold.Add((byte)(sumRed / counterRed));
            }

            for (int i = 0; i < numberGreenLevels - 1; i++)
            {
                int sumGreen = 0;
                int counterGreen = 0;
                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    if ((pixelBuffer[k + 1] >= greenLevels[i] && pixelBuffer[k + 1] < greenLevels[i + 1])
                        || (pixelBuffer[k + 1] == greenLevels[i + 1] && i == numberGreenLevels - 2))
                    {
                        counterGreen++;
                        sumGreen += pixelBuffer[k + 1];
                    }
                }
                if (counterGreen == 0)
                    greenThreshold.Add(redLevels[i]);
                else
                    greenThreshold.Add((byte)(sumGreen / counterGreen));
            }

            for (int i = 0; i < numberBlueLevels - 1; i++)
            {
                int sumBlue = 0;
                int counterBlue = 0;
                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    if ((pixelBuffer[k] >= blueLevels[i] && pixelBuffer[k] < blueLevels[i + 1])
                        || (pixelBuffer[k] == blueLevels[i + 1] && i == numberBlueLevels - 2))
                    {
                        counterBlue++;
                        sumBlue += pixelBuffer[k];
                    }
                }
                if (counterBlue == 0)
                    blueThreshold.Add(blueLevels[i]);
                else
                    blueThreshold.Add((byte)(sumBlue / counterBlue));
            }

        }
        public void AssignColorLevels()
        {
            for (int k = 0; k < pixelBuffer.Length; k += 4)
            {
                for (int i = 0; i < numberRedLevels - 1; i++)
                {
                    if ((pixelBuffer[k + 2] >= redLevels[i] && pixelBuffer[k + 2] < redLevels[i + 1])
                       || (pixelBuffer[k + 2] == redLevels[i + 1] && i == numberRedLevels - 2))
                    {
                        if (pixelBuffer[k + 2] <= redThreshold[i])
                            pixelBuffer[k + 2] = redLevels[i];
                        else
                            pixelBuffer[k + 2] = redLevels[i + 1];
                    }
                }

                for (int i = 0; i < numberGreenLevels - 1; i++)
                {
                    if ((pixelBuffer[k + 1] >= greenLevels[i] && pixelBuffer[k + 1] < greenLevels[i + 1])
                       || (pixelBuffer[k + 1] == greenLevels[i + 1] && i == numberGreenLevels - 2))
                    {
                        if (pixelBuffer[k + 1] <= greenThreshold[i])
                            pixelBuffer[k + 1] = greenLevels[i];
                        else
                            pixelBuffer[k + 1] = greenLevels[i + 1];
                    }
                }

                for (int i = 0; i < numberBlueLevels - 1; i++)
                {
                    if ((pixelBuffer[k] >= blueLevels[i] && pixelBuffer[k] < blueLevels[i + 1])
                       || (pixelBuffer[k] == blueLevels[i + 1] && i == numberBlueLevels - 2))
                    {
                        if (pixelBuffer[k] <= blueThreshold[i])
                            pixelBuffer[k] = blueLevels[i];
                        else
                            pixelBuffer[k] = blueLevels[i + 1];
                    }
                }
            }
        }

    }
}
