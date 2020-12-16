using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet
{
    public static class ListExtensions
    {       
        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = list[j];
                list[j] = list[i];
                list[i] = tmp;
            }

            return list;
        }

        public static List<T> OneByOne<T>(List<T> a, List<T> b)
        {
            if (a.Count != b.Count)
            {
                throw new ArgumentException("Длины массивов не совпадают");
            }

            var res = new List<T>(a.Count * 2);
            for (int i = 0; i < a.Count; i++)
            {
                res.Add(a[i]);
                res.Add(b[i]);
            }

            return res;
        }
    }
}
