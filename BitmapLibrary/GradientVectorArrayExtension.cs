using System;
using System.Drawing;

namespace BitmapLibrary
{
    public static class GradientVectorArrayExtension
    {
        /// <summary>
        /// Converts to Bitmap.
        /// </summary>
        /// <param name="resGrad">Array of gradients.</param>
        /// <returns>Bitmap.</returns>
        public static Bitmap ToBitmap(this GradientVector[,] resGrad)
        {
            Bitmap result = new Bitmap(resGrad.GetLength(1), resGrad.GetLength(0));
            for (int i = 0; i < resGrad.GetLength(0); i++)
            {
                for (int j = 0; j < resGrad.GetLength(1); j++)
                {
                    result.SetPixel(j, i, resGrad[i, j].GradientToColor());
                }
            }

            return result;
        }

        public static GradientVector[,] SuppressMaximums(this GradientVector[,] vecs)
        {
            GradientVector[,] res = new GradientVector[vecs.GetLength(0), vecs.GetLength(1)];
            for (int y = 1; y < vecs.GetLength(0) - 1; y++)
            {
                for (int x = 1; x < vecs.GetLength(1) - 1; x++)
                {
                    res[y, x].Angle = vecs[y, x].Angle;
                    switch (vecs[y, x].Angle)
                    {
                        case 0:
                            {
                                res[y, x].Length = vecs[y, x].Length >= vecs[y, x + 1].Length && vecs[y, x].Length >= vecs[y, x - 1].Length ? vecs[y, x].Length : 0;
                                break;
                            }
                        case 90:
                            {
                                res[y, x].Length = vecs[y, x].Length >= vecs[y + 1, x].Length && vecs[y, x].Length >= vecs[y - 1, x].Length ? vecs[y, x].Length : 0;
                                break;
                            }
                        case 45:
                            {
                                res[y, x].Length = vecs[y, x].Length >= vecs[y - 1, x - 1].Length && vecs[y, x].Length >= vecs[y + 1, x + 1].Length ? vecs[y, x].Length : 0;
                                break;
                            }
                        case -45:
                            {
                                res[y, x].Length = vecs[y, x].Length >= vecs[y + 1, x - 1].Length && vecs[y, x].Length >= vecs[y - 1, x + 1].Length ? vecs[y, x].Length : 0;
                                break;
                            }
                    }
                }
            }

            return res;
        }

        public static double FindMaximum(this GradientVector[,] vecs)
        {
            double result = 0;
            for (int y = 0; y < vecs.GetLength(0); y++)
            {
                for (int x = 0; x < vecs.GetLength(1); x++)
                {
                    result = vecs[y, x].Length > result ? vecs[y, x].Length : result;
                }
            }
            return result;
        }

        public static double FindMinimum(this GradientVector[,] vecs)
        {
            double result = double.MaxValue;
            for (int y = 0; y < vecs.GetLength(0); y++)
            {
                for (int x = 0; x < vecs.GetLength(1); x++)
                {
                    result = vecs[y, x].Length < result ? vecs[y, x].Length : result;
                }
            }
            return result;
        } 

        public static GradientVector[,] Filtering(this GradientVector[,] vecs)
        {
            double average = GetChosenAverage(vecs);
            for (int y = 0; y < vecs.GetLength(0); y++)
            {
                for (int x = 0; x < vecs.GetLength(1); x++)
                {
                    vecs[y, x].Length = vecs[y, x].Length > average ? 255 : 0;
                }
            }
            
            return vecs;
        }

        public static GradientVector[,] BlackEdge(this GradientVector[,] pic, int widthOfEdge)
        {
            for (int y = 0; y < pic.GetLength(0); y++)
            {
                for (int x = 0; x < widthOfEdge; x++)
                {
                    pic[y, x].Length = 0;
                    pic[y, pic.GetLength(1) - x - 1].Length = 0;
                }
            }

            for (int x = 0; x < pic.GetLength(1); x++)
            {
                for (int y = 0; y < widthOfEdge; y++)
                {
                    pic[y, x].Length = 0;
                    pic[pic.GetLength(0) - y - 1, x].Length = 0;
                }
            }

            return pic;
        }

        public static double GetChosenAverage(this GradientVector[,] vecs)
        {
            double maxInVecs = vecs.FindMaximum();
            double minInVecs = vecs.FindMinimum();

            int howIntervals = 1 + (int)(3.32 * Math.Log10(vecs.Length));
            double intervalH = (maxInVecs - minInVecs) / howIntervals;
            double[] intervals = new double[howIntervals];
            double[] rightEdgeOfInterval = new double[howIntervals];
            for (int i = 0; i < howIntervals; i++)
            {
                intervals[i] = 0;
                rightEdgeOfInterval[i] = minInVecs + (i + 1) * intervalH;
            }

            // Find frequencies
            for (int y = 0; y < vecs.GetLength(0); y++)
            {
                for (int x = 0; x < vecs.GetLength(1); x++)
                {
                    int index = 0;
                    for (int i = 0; i < howIntervals; i++)
                    {
                        if (vecs[y, x].Length < rightEdgeOfInterval[i])
                        {
                            index = i;
                            break;
                        }
                    }

                    intervals[index]++;
                }
            }

            // Find Chosen average
            double sum = 0;
            double res = 0;
            for (int i = howIntervals - 1; i >= 0 ; i--)
            {
                if (sum + intervals[i] < vecs.Length / (howIntervals) )
                {
                    sum += intervals[i];
                    res = rightEdgeOfInterval[i];
                }
                else
                {
                    break;
                }
            }

            return res;
        }

        public static double[,] LengthsToArray(this GradientVector[,] vecs)
        {
            var res = new double[vecs.GetLength(0), vecs.GetLength(1)];
            for (int y = 0; y < vecs.GetLength(0); y++)
            {
                for (int x = 0; x < vecs.GetLength(1); x++)
                {
                    res[y, x] = vecs[y, x].Length;
                }
            }

            return res;
        }
    }
}
