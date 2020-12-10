using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet
{
    public static class ArrayExtensions
    {
        public static T[] ToVector<T>(this T[,] arr)
        {
            T[] res = new T[arr.Length];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    res[i * arr.GetLength(1) + j] = arr[i, j];
                }
            }
            return res;
        }

        public static T[] ToVector<T>(this T[,,] arr)
        {
            T[] res = new T[arr.Length];
            for (int l = 0; l < arr.GetLength(0); l++)
            {
                for (int i = 0; i < arr.GetLength(1); i++)
                {
                    for (int j = 0; j < arr.GetLength(2); j++)
                    {
                        res[l * arr.GetLength(1) * arr.GetLength(2) + i * arr.GetLength(2) + j] = arr[l, i, j];
                    }
                }
            }

            return res;
        }

        public T[,,] ToArray3<T>(this T[] init, int howZ, int howY, int howX)
        {
            var res = new T[howZ,howY,howX];
            
            for (int z = 0; z < howZ; z++)
            {
                for (int y = 0; y < howY; y++)
                {
                    for (int x = 0; x < howX; x++)
                    {
                        res[z, y, x] = init[z * howY * howX + y * howX + x];
                    }
                }
            }

            return res;
        }

        public static double[,,] Plus(this double[,,] a, double[,,] b)
        {
            for (int l = 0; l < a.GetLength(0); l++)
            {
                for (int y = 0; y < a.GetLength(1); y++)
                {
                    for (int x = 0; x < a.GetLength(2); x++)
                    {
                        a[l, y, x] += b[l, y, x];
                    }
                }
            }

            return a;
        }
    }
}
