using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Drawing;
using BitmapLibrary;

namespace NeuralNet
{
    internal class ConvolutionLayer
    {
        public List<double[,]> Inputs { get; set; }
        public List<double[,,]> Filters { get; set; }
    }

    internal class PoolingLayer
    {
        public List<int[,]> PositionOfSquare { get; set; }
    }

    public class CNNet
    {
        BNPNet perceptron;
        ConvolutionLayer cl1, cl2;
        PoolingLayer pl1;
        int poolSize = 3;

        public CNNet(ActivationFuncType type)
        {
            int howFilters1 = 3;
            int howLayersOnFilter1 = 1;
            int sizeOfFilter1 = 5;
            int howFilters2 = 5;
            int howLayersOnFilter2 = 5;
            int sizeOfFilter2 = 5;

            perceptron = new BNPNet(type, new int[] { 8000, 70, 2 }, true);

            cl1.Filters = new List<double[,,]>(howFilters1);
            for (int i = 0; i < howFilters1; i++)
            {
                cl1.Filters.Add(RandomiseFilter(howLayersOnFilter1, sizeOfFilter1));
            }

            cl2.Filters = new List<double[,,]>(howFilters2);
            for (int i = 0; i < howFilters2; i++)
            {
                cl2.Filters.Add(RandomiseFilter(howLayersOnFilter2, sizeOfFilter2));
            }

        }

        public double[,,] RandomiseFilter(int howLayers, int sizeOfLayer)
        {
            var res = new double[howLayers, sizeOfLayer, sizeOfLayer];
            Random rand = new Random();
            for (int l = 0; l < res.GetLength(0); l++)
            {
                for (int i = 0; i < res.GetLength(1); i++)
                {
                    for (int j = 0; j < res.GetLength(2); j++)
                    {
                        res[l, i, j] = rand.NextDouble() % 0.5; 
                    }
                }
            }

            return res;
        }
    }
}
