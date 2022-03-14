using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Kernel
    {
        public string Name { get; set; }
        public int[,] Matrix { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Coordinates Anchor { get; set; }
        public int Offset { get; set; }
        public int Divisor { get; set; }

        public Kernel(int[,] matrix, string name, int x = 0, int y = 0)
        {
            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);
            Anchor = new Coordinates(x, y);
            Matrix = new int[Height, Width];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    Matrix[i, j] = matrix[i, j];
            ComputeDivisor();
            Offset = 0;
            Name = name;
        }

        //public Kernel(int[,] matrix, string name, Coordinates anchor, int height = 3, int width = 3)
        //{
        //    Height = height;
        //    Width = width;
        //    Anchor = anchor;
        //    Matrix = new int[3, 3];
        //    ComputeDivisor();
        //    Offset = 0;
        //    Name = name;
        //}

        public override string ToString()
        {
            return Name;
        }

        //public Kernel(Kernel original)
        //{
        //    Height = original.Height;
        //    Width = original.Width;
        //    Anchor = original.Anchor;
        //    Matrix = original.Matrix;
        //    Offset = original.Offset;
        //    Name = original.Name;
        //    ComputeDivisor();
        //}

        public void ComputeDivisor()
        {
            int sum = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    sum += Matrix[i, j];
                }
            }
            if (sum == 0) Divisor = 1;
            else Divisor = sum;
        }

    }
}
