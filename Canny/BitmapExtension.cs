using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Canny
{
    public static class BitmapExtension
    {
        /// <summary>
        /// Calculate black-white color for color "c" .
        /// </summary>
        /// <param name="c">Color.</param>
        /// <returns>Black-white color.</returns>
        public static Color GetBWColor(Color c)
        {
            byte lambda = (byte)(((int)c.R + (int)c.G + (int)c.B) / 3);
            return Color.FromArgb(255, lambda, lambda, lambda);
        }

        /// <summary>
        /// Calculate black-white picture of "picture".
        /// </summary>
        /// <param name="picture">Picture.</param>
        /// <returns>Black-white picture.</returns>
        public static Bitmap GetBWPicture(this Bitmap picture)
        {
            Bitmap res = new Bitmap(picture.Width, picture.Height);
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    res.SetPixel(x, y, 
                        GetBWColor(picture.GetPixel(x, y)));
                }
            }

            return res;
        }

        public static Bitmap SmoothPicture(this Bitmap pic, SmoothMatrixType type, int matrixSize)
        {
            double[,] matrix = new double[matrixSize, matrixSize];
            
            // Calculate matrix of smooth
            switch (type)
            {
                case SmoothMatrixType.Simple:
                {
                    double temp = 1.0 / (matrixSize * matrixSize);
                    for (int i = 0; i < matrixSize; i++)
                    {
                        for (int j = 0; j < matrixSize; j++)
                        {
                            matrix[i,j] = temp;
                        }
                    }
                    break;
                }
            }

            // Smooth pitcture
            Bitmap result = SmoothingBW(pic, matrix);

            return result;
        }

        private static Bitmap SmoothingBW(Bitmap pic, double[,] matrix)
        {
            Bitmap res = new Bitmap(pic.Width, pic.Height);
            for (int y = 0; y < pic.Height - matrix.GetLength(0) + 1; y++)
            {
                for (int x = 0; x < pic.Width - matrix.GetLength(1) + 1; x++)
                {
                    double temp = 0;
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            temp += matrix[i,j] * pic.GetPixel(x + i, y + j).R;
                        }
                    }

                    byte lambda = Convert.ToByte(temp);
                    res.SetPixel(x, y, Color.FromArgb(255, lambda, lambda, lambda));
                }
            }

            return res;
        }

        public static GradientVector[,] FindGradients(this Bitmap pic)
        {
            GradientVector[,] vecs = new GradientVector[pic.Width, pic.Height];
            for (int x = 0; x < pic.Width - 1; x++)
            {
                for (int y = 0; y < pic.Height - 1; y++)
                {
                    double dX = pic.GetPixel(x + 1, y).R - pic.GetPixel(x, y).R;
                    double dY = pic.GetPixel(x, y + 1).R - pic.GetPixel(x, y).R;
                    vecs[x, y].Length = Math.Sqrt(dX * dX + dY * dY);
                    double arctan = Math.Atan2(dY, dX);
                }
            }

            return vecs;
        }
    }

    public struct GradientVector
    {
        public double Length;
        public double Angle;
    }

    /// <summary>
    /// Тип матрица сглаживания.
    /// </summary>
    public enum SmoothMatrixType
    {
        /// <summary>
        /// Простая единичная матрица.
        /// </summary>
        Simple,
        /// <summary>
        /// Фильтр Гаусса.
        /// </summary>
        //Gaus
    }
}
