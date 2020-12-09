using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NeuralNet
{
    internal class ConvolutionLayer
    {
        public double[,,] Inputs { get; set; }
        public List<double[,,]> Filters { get; set; }

        public double[,,] GetResult(double[,,] filter)
        {
            // по умолчанию массив заполняется нулями 
            var res = new double[
                Inputs.GetLength(0) - filter.GetLength(0) + 1,
                Inputs.GetLength(1) - filter.GetLength(1) + 1,
                Inputs.GetLength(2) - filter.GetLength(2) + 1];

            for (int l = 0; l < res.GetLength(0); l++)
            {
                for (int y = 0; y < res.GetLength(1); y++)
                {
                    for (int x = 0; x < res.GetLength(2); x++)
                    {

                        // Начать свёртку
                        for (int k = 0; k < filter.GetLength(0); k++)
                        {
                            for (int i = 0; i < filter.GetLength(1); i++)
                            {
                                for (int j = 0; j < filter.GetLength(2); j++)
                                {
                                    res[l, y, x] += Inputs[l + k, y + i, x + j] * filter[k, i, j];
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        public double[,,] ReverseConvolution(double[,,] init, double[,,] filter)
        {
            // по умолчанию массив заполняется нулями 
            var newInit = new double[
                init.GetLength(0) + filter.GetLength(0) - 1,
                init.GetLength(1) + filter.GetLength(1) - 1,
                init.GetLength(2) + filter.GetLength(2) - 1];

            for (int l = 0; l < init.GetLength(0); l++)
            {
                for (int y = 0; y < init.GetLength(1); y++)
                {
                    for (int x = 0; x < init.GetLength(2); x++)
                    {
                        newInit[l + filter.GetLength(0) / 2 * 2,
                            y + filter.GetLength(1) / 2 * 2,
                            x + filter.GetLength(2) / 2 * 2] = init[l, y, x];
                    }
                }
            }

            init = newInit;

            var res = new double[
                init.GetLength(0) - filter.GetLength(0) + 1,
                init.GetLength(1) - filter.GetLength(1) + 1,
                init.GetLength(2) - filter.GetLength(2) + 1];

            for (int l = 0; l < res.GetLength(0); l++)
            {
                for (int y = 0; y < res.GetLength(1); y++)
                {
                    for (int x = 0; x < res.GetLength(2); x++)
                    {

                        // Начать свёртку
                        for (int k = 0; k < filter.GetLength(0); k++)
                        {
                            for (int i = 0; i < filter.GetLength(1); i++)
                            {
                                for (int j = 0; j < filter.GetLength(2); j++)
                                {
                                    int f1 = k + 1;
                                    int f2 = i + 1;
                                    int f3 = j + 1;
                                    res[l, y, x] += init[l + k, y + i, x + j] *
                                        filter[filter.GetLength(0) - f1,
                                        filter.GetLength(1) - f2,
                                        filter.GetLength(2) - f3];
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }
    }

    internal class PoolingLayer
    {
        public int[,,] MaxPositions { get; set; }
        public int PoolSize { get; set; }
        public int HowLayers { get; set; }

        public double[,,] GetResult(double[,,] init)
        {
            var res = new double[init.GetLength(0) / HowLayers, init.GetLength(1) / PoolSize, init.GetLength(2) / PoolSize];
            MaxPositions = new int[init.GetLength(0), init.GetLength(1), init.GetLength(2)];

            for (int l = 0; l < res.GetLength(0); l++)
            {
                for (int y = 0; y < res.GetLength(1); y++)
                {
                    for (int x = 0; x < res.GetLength(2); x++)
                    {
                        res[l, y, x] = MaxInSquare(init, l * HowLayers, y * PoolSize, x * PoolSize,
                            out int zOfMax, out int yOfMax, out int xOfMax);

                        MaxPositions[zOfMax, yOfMax, xOfMax] = 1;
                    }
                }
            }

            return res;
        }

        public double[,,] UpSample(double[,,] init)
        {
            var res = new double[init.GetLength(0) * HowLayers, init.GetLength(1) * PoolSize, init.GetLength(2) * PoolSize];
            for (int l = 0; l < init.GetLength(0); l++)
            {
                for (int i = 0; i < init.GetLength(1); i++)
                {
                    for (int j = 0; j < init.GetLength(2); j++)
                    {
                        for (int poolL = 0; poolL < HowLayers; poolL++)
                        {
                            for (int poolI = 0; poolI < PoolSize; poolI++)
                            {
                                for (int poolJ = 0; poolJ < PoolSize; poolJ++)
                                {
                                    res[l * HowLayers + poolL, i * PoolSize + poolI, j * PoolSize + poolJ] =
                                        MaxPositions[l * HowLayers + poolL, i * PoolSize + poolI, j * PoolSize + poolJ] * init[l, i, j];
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        private double MaxInSquare(double[,,] init, int z, int y, int x, out int zOfMax, out int yOfMax, out int xOfMax)
        {
            zOfMax = z;
            yOfMax = y;
            xOfMax = x;
            for (int l = 0; l < HowLayers; l++)
            {
                for (int i = 0; i < PoolSize; i++)
                {
                    for (int j = 0; j < PoolSize; j++)
                    {
                        if (init[z + l, y + i, x + j] > init[zOfMax, yOfMax, xOfMax])
                        {
                            zOfMax = z + l;
                            yOfMax = y + i;
                            xOfMax = x + j;
                        }
                    }
                }
            }

            return init[zOfMax, yOfMax, xOfMax];
        }
    }

    public class CNNet
    {
        BNPNet perceptron;
        ConvolutionLayer cl1, cl2;
        PoolingLayer pl1;
        int poolSize = 3;

        public CNNet(ActivationFuncType type = ActivationFuncType.Sigmoid,
            int howFilters1 = 3,
            int howLayersOnFilter1 = 1,
            int sizeOfFilter1 = 5,
            int howFilters2 = 5,
            int howLayersOnFilter2 = 5,
            int sizeOfFilter2 = 5)
        {

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

        public CNNet(string input)
        {
            string[] percAndOther = input.Split("\nConvolutionLayer1:\n");
            perceptron = new BNPNet(percAndOther[0]);
            string[] cl1AndOther = percAndOther[1].Split("\nConvolutionLayer2:\n");
            cl1 = JsonConvert.DeserializeObject<ConvolutionLayer>(cl1AndOther[0]);
            string[] cl2AndOther = cl1AndOther[1].Split("\nPoolLayer1:\n");
            cl2 = JsonConvert.DeserializeObject<ConvolutionLayer>(cl2AndOther[0]);
            string[] pl1AndOther = cl2AndOther[1].Split("\npoolSize:\n");
            pl1 = JsonConvert.DeserializeObject<PoolingLayer>(pl1AndOther[0]);
            poolSize = int.Parse(pl1AndOther[1].Split("\npoolSize:\n")[1]);
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

        public string Save()
        {
            string perceptronString = perceptron.Save();
            string cl1String = JsonConvert.SerializeObject(cl1);
            string cl2String = JsonConvert.SerializeObject(cl2);
            string pl1String = JsonConvert.SerializeObject(pl1);
            return perceptronString + 
                "\nConvolutionLayer1:\n" + cl1String + 
                "\nConvolutionLayer2:\n" + cl2String + 
                "\nPoolLayer:\n" + pl1String + 
                "\npoolSize:\n" + poolSize;
        }
    }
}
