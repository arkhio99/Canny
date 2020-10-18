using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
                case SmoothMatrixType.Gauss:
                    {
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

        /// <summary>
        /// disp = 5
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
            res = (Math.Round(res / 45) % 4) * 45;
            return res;
        }

        public static GradientVector[,] SuppressMaximums(this GradientVector[,] vecs)
        {
            GradientVector[,] res = new GradientVector[vecs.GetLength(0), vecs.GetLength(1)];
            for(int i = 1; i < vecs.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < vecs.GetLength(1) - 1; j++)
                {
                    res[i, j].Angle = vecs[i, j].Angle;
                    switch (vecs[i, j].Angle)
                    {
                        case 0:
                            {
                                res[i, j].Length = vecs[i, j].Length > vecs[i, j + 1].Length && vecs[i, j].Length > vecs[i, j - 1].Length ? 255 : 0;
                                break;
                            }
                        case 90:
                            {
                                res[i, j].Length = vecs[i, j].Length > vecs[i + 1, j].Length && vecs[i, j].Length > vecs[i - 1, j].Length ? 255 : 0;
                                break;
                            }
                        case 135:
                            {
                                res[i, j].Length = vecs[i, j].Length > vecs[i - 1, j - 1].Length && vecs[i, j].Length > vecs[i + 1, j + 1].Length ? 255 : 0;
                                break;
                            }
                        case 45:
                            {
                                res[i, j].Length = vecs[i, j].Length > vecs[i + 1, j - 1].Length && vecs[i, j].Length > vecs[i - 1, j + 1].Length ? 255 : 0;
                                break;
                            }
                    }
                }
            }

            return res;
            //for (int i = 0; i < vecs.GetLength(0); i++)
            //{
            //    double maxRow = FindMaximum(vecs, i) > 255 ? 255 : 0;
            //    for (int j = 0; j < vecs.GetLength(1); j++)
            //    {
            //        vecs[i, j].Length = Math.Abs((vecs[i,j].Length > 255 ? 255 : 0) - maxRow) < 10 ? 255 : 0;
            //    }
            //}

            //return vecs;
        }

        private static double FindMaximum(GradientVector[,] vecs, int row)
        {
            double result = 0;
            for (int i = 0; i < vecs.GetLength(1); i++)
            {
                result = vecs[row, i].Length > result ? vecs[row, i].Length : result;
            }

            return result;
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
        Gauss
    }
}
