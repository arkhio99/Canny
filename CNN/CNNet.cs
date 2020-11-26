using System;
using System.Collections.Generic;

namespace CNN
{
    internal class Neuron
    {
        public double Input { get; set; }
        public double Output { get; set; }
    }

    public class CNNet
    {
        Func<double, double> _activation;
        Func<double, double> _differencialActivation;
        List<Neuron[]> _HOlayers;
        List<double[,]> _connections;
        double[] inputs;
        double[,] connectionsBetweenInputAndLayer; 
        List<double[,]> _filters;
        int _sizeOfPooling;
        
        public CNNet(Func<double, double> activation, Func<double, double> differencialActivation, int[] neuronsPerHiddenLayer, List<double[,]> filters, int howInputs, int howOutput, int sizeOfPooling)
        {
            _activation = activation;
            _differencialActivation = differencialActivation;
            _HOlayers = new List<Neuron[]>(neuronsPerHiddenLayer.Length);
            _connections = new List<double[,]>(neuronsPerHiddenLayer.Length);
            _filters = filters;
            _sizeOfPooling = sizeOfPooling;
            inputs = new double[howInputs];
            connectionsBetweenInputAndLayer = new double[howInputs, neuronsPerHiddenLayer[0]];
            RandomiseArray(ref connectionsBetweenInputAndLayer);
            _HOlayers.Add(new Neuron[neuronsPerHiddenLayer[0]]);
            for (int i = 1; i < neuronsPerHiddenLayer.Length - 1; i++)
            {
                var temp = new double[neuronsPerHiddenLayer[i -1], neuronsPerHiddenLayer[i]];
                RandomiseArray(ref temp);
                _connections.Add(temp);
                _HOlayers.Add(new Neuron[neuronsPerHiddenLayer[i]]);
            }
            
            var lastConnections = new double[neuronsPerHiddenLayer[neuronsPerHiddenLayer.Length - 1], howOutput];
            RandomiseArray(ref lastConnections);
            _connections.Add(lastConnections);
            _HOlayers.Add(new Neuron[howOutput]);
        }

        public double[,] Convolute(double[,] init, double[,] filter)
        {
            var res = new double[init.GetLength(0) - filter.GetLength(0) - filter.GetLength(0) % 2, init.GetLength(1) - filter.GetLength(1) - filter.GetLength(1) % 2];
            
            for (int y = 0; y < res.GetLength(0); y++)
            {
                for (int x = 0; x < res.GetLength(1); x++)
                {
                    double temp = 0;
                    for (int i = 0; i < filter.GetLength(0); i++)
                    {
                        for (int j = 0; j < filter.GetLength(1); j++)
                        {
                            temp += init[y + i, x + j] * filter[i, j];
                        }
                    }

                    res[y, x] = temp;
                }
            }

            return res;
        }

        public double[,] Pool(double[,] init)
        {
            var res = new double[init.GetLength(0) / _sizeOfPooling + init.GetLength(0) % _sizeOfPooling, init.GetLength(1) / _sizeOfPooling + init.GetLength(1) % _sizeOfPooling];
            for(int i = 0; i <  )
        }

        private void RandomiseArray(ref double[,] array)
        {
            Random random = new Random();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i,j] = random.NextDouble() * (random.Next(1) == 1 ? 1 : -1);
                }
            }
        }
    }
}
