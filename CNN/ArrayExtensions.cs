using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Преообразует двумерный массив в одномерный
        /// </summary>
        /// <typeparam name="T">Тип массива</typeparam>
        /// <param name="arr">Двумерный массив</param>
        /// <returns>Одномерный массив</returns>
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

        /// <summary>
        /// Преобразует трёхмерный массив в одномерный</summary>
        /// <typeparam name="T">Тип массива</typeparam>
        /// <param name="arr">Исходный трёхмерный массив</param>
        /// <returns>Результирующий одномерный массив</returns>
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

        /// <summary>
        /// Преобразует одномерный массив в трёхмерный с заданными размерами
        /// </summary>
        /// <typeparam name="T">Тип массива</typeparam>
        /// <param name="init">Исходный одномерный массив</param>
        /// <param name="howZ">Количество слоёв массива</param>
        /// <param name="howY">Высота слоя массива</param>
        /// <param name="howX">Ширина слоя массива</param>
        /// <returns>Результирующий трёхмерный массив</returns>
        public static T[,,] ToArray3<T>(this T[] init, int howZ, int howY, int howX)
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

        public static T[,,] To3DArray<T>(this T[,] ar)
        {
            T[,,] res = new T[1, ar.GetLength(0), ar.GetLength(1)];
            for (int i = 0; i < res.GetLength(1); i++)
            {
                for (int j = 0; j < res.GetLength(2); j++)
                {
                    res[0, i, j] = ar[i, j];
                }
            }

            return res;
        }

        /// <summary>
        /// Суммирует трёхмерные массивы
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">Массив, который надо прибавить</param>
        /// <returns>Сумма массивов</returns>
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

        public static double[,] GetLayer(this double[,,] ar, int l)
        {
            double[,] res = new double[ar.GetLength(1), ar.GetLength(2)];
            for (int i = 0; i < res.GetLength(1); i++)
            {
                for (int j = 0; j < res.GetLength(2); j++)
                {
                    res[i, j] = ar[l, i, j];
                }
            }

            return res;
        }
    }
}
