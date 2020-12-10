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
    }
}
