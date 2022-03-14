using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class Constants
    {
        public static int BrightnessCoefficient = 30;

        public static int ContrastCoefficient = 100;

        public static double GammaCoefficient = 0.25;

        public static int[,] BlurKernel = new int[3, 3]
        {
            {1, 1, 1},
            {1, 1, 1},
            {1, 1, 1}
        };

        public static int[,] GaussianBlurKernel = new int[3, 3]
        {
            {0, 1, 0},
            {1, 4, 1},
            {0, 1, 0}
        };

        public static int[,] SharpenKernel = new int[3, 3]
        {
            {0, -1, 0},
            {-1, 5, -1},
            {0, -1, 0}
        };

        public static int[,] EdgeDetectionKernel = new int[3, 3]
        {
            {-1, -1, -1},
            {-1, 8, -1},
            {-1, -1, -1}
        };

        public static int[,] EmbossKernel = new int[3, 3]
        {
            {-1, 0, 1},
            {-1, 1, 1},
            {-1, 0, 1}
        };

        public static int[,] Identity = new int[3, 3]
        {
            {0, 0, 0 },
            {0, 1, 0 },
            {0, 0, 0 }
        };
    }
}
