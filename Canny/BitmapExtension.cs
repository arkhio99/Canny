using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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

        /// <summary>
        /// Smoothing Black-White picture.
        /// </summary>
        /// <param name="pic">Picture.</param>
        /// <param name="type">Type of smootthing.</param>
        /// <param name="matrixSize">Size of matrix of smooth.</param>
        /// <returns>Smoothed picture.</returns>
        public static Bitmap SmoothBWPicture(this Bitmap pic, SmoothMatrixType type, int matrixSize)
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
                case SmoothMatrixType.Gauss:
                    {
                        //TODO doesn't work
                        // calculate discret gauss matrix
                        double div = 0;
                        for (int i = 0; i < matrixSize; i++)
                        {
                            for (int j = 0; j < matrixSize; j++)
                            {
                                matrix[i,j] = GaussCoeff(i, j, 1);
                                div += matrix[i, j] * matrix[i, j];
                            }
                        }

                        // norm matrix
                        for (int i = 0; i < matrixSize; i++)
                        {
                            for (int j = 0; j < matrixSize; j++)
                            {
                                matrix[i, j] /= div;
                            }
                        }

                        break;
                    }
            }

            // Smooth pitcture
            Bitmap result = SmoothingBW(pic, matrix);

            return result;
        }

        private static double GaussCoeff(int x, int y, double disp)
        {
            return (1 / (2 * Math.PI * disp * disp)) * Math.Exp(-1 * Math.Sqrt(x * x + y * y) / (2 * disp * disp));
        }

        private static Bitmap SmoothingBW(Bitmap pic, double[,] matrix)
        {
            int centerOfMatrixMinus1 = matrix.GetLength(0) / 2;
            Bitmap res = new Bitmap(pic.Width, pic.Height);
            for (int y = centerOfMatrixMinus1; y < pic.Height - centerOfMatrixMinus1; y++)
            {
                for (int x = centerOfMatrixMinus1; x < pic.Width - centerOfMatrixMinus1; x++)
                {
                    double temp = 0;
                    for (int i = -centerOfMatrixMinus1; i < matrix.GetLength(0) - centerOfMatrixMinus1; i++)
                    {
                        for (int j = -centerOfMatrixMinus1; j < matrix.GetLength(1) - centerOfMatrixMinus1; j++)
                        {
                            temp += pic.GetPixel(x + j, y + i).R * matrix[i + centerOfMatrixMinus1, j + centerOfMatrixMinus1];;
                        }
                    }

                    byte lambda = Convert.ToByte(temp);
                    res.SetPixel(x, y, Color.FromArgb(255, lambda, lambda, lambda));
                }
            }

            return res;
        }

        /// <summary>
        /// Finds gradients and angle of gradients (Sobel);
        /// </summary>
        /// <param name="pic">Picture.</param>
        /// <returns>Array of Gradients.</returns>
        public static GradientVector[,] FindGradients(this Bitmap pic)
        {
            GradientVector[,] vecs = new GradientVector[pic.Height, pic.Width];
            double[,] gYMask = new double[,]
            {
                { -1, -2, -1},
                {0, 0, 0 },
                { 1, 2, 1}
            };
            double[,] gXMask = new double[,]
            {
                {-1, 0, 1 },
                {-2, 0, 2 },
                {-1, 0, 1 }
            };
            for (int x = 1; x < pic.Width - 1; x++)
            {
                for (int y = 1; y < pic.Height - 1; y++)
                {
                    double gYRes = 0;
                    double gXRes = 0;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            byte b = pic.GetPixel(x + j, y + i).R;
                            gXRes += pic.GetPixel(x + j, y + i).R * gXMask[i + 1, j + 1];
                            gYRes += pic.GetPixel(x + j, y + i).R * gYMask[i + 1, j + 1];
                        }
                    }

                    vecs[y, x].Length = Math.Sqrt(gXRes * gXRes + gYRes * gYRes);
                    vecs[y, x].Angle = GetCorrectAngle(Math.Atan(gYRes / gXRes));
                }
            }

            return vecs;
        }

        private static double GetCorrectAngle(double a)
        {
            double res = a / Math.PI * 180;
            bool negative = res < 0;
            
            if (negative)
            {
                res *= -1;
            }

            res = Math.Round(res / 45) * 45;
            
            if (negative && Math.Abs(res - 90) > 0.1)
            {
                res *= -1;
            }

            return res;
        }

        /// <summary>
        /// Makes edges with width = widthOfEdge black.
        /// </summary>
        /// <param name="pic">Picture.</param>
        /// <param name="widthOfEdge">Width of edge.</param>
        /// <returns>Picture.</returns>
        public static Bitmap EdgeToBlack(this Bitmap pic, int widthOfEdge)
        {
            for (int y = 0; y < pic.Height; y++)
            {
                for (int x = 0; x < widthOfEdge; x++)
                {
                    pic.SetPixel(x, y, Color.Black);
                    pic.SetPixel(pic.Width - x - 1, y, Color.Black);
                }
            }

            for (int x = 0; x < pic.Width; x++)
            {
                for (int y = 0; y < widthOfEdge; y++)
                {
                    pic.SetPixel(x, y, Color.Black);
                    pic.SetPixel(x, pic.Height - 1 - y, Color.Black);
                }
            }

            return pic;
        }

        public static double[,] Integrate(this Bitmap pic)
        {
            double[,] integratedArray = new double[pic.Height, pic.Width];
            integratedArray[0, 0] = pic.GetPixel(0, 0).R;
            for (int i = 1; i < integratedArray.GetLength(0); i++)
            {
                for (int j = 1; j < integratedArray.GetLength(1); j++)
                {
                    if (i == 0 || j == 0)
                    {
                        integratedArray[i, j] = pic.GetPixel(i, j).R;
                    }
                    integratedArray[i, j] = pic.GetPixel(i, j).R - integratedArray[i - 1, j - 1] + integratedArray[i, j - 1] + integratedArray[i - 1, j];
                }
            }

            return integratedArray;
        }
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
        Gauss
    }

    
}
