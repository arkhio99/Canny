using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NeuralNetworks
{
    public class Neuron
    {
        public double In { get; set; }
        public double Out { get; set; }
    }

    public enum ActivationFunctionType
    {
        Sigmoid,
        LeakyReLU,
    }

    public class Perceptron
    {
        private class PerceptronFromJson
        {
            public int[] neuronsPerLayer { get; set; }
            public List<double[,]> W { get; set; }
            public ActivationFunctionType ActivationType { get; set; }
            public bool WithSoftmax { get; set; }
            public double Epsilon { get; set; }
            public double Alpha { get; set; }
        }

        public int[] neuronsPerLayer { get; private set; }
        public List<double[,]> W { get; private set; }
        public ActivationFunctionType ActivationType { get; private set; }
        public bool WithSoftmax { get; private set; }
        public double Epsilon { get; private set; }
        public double Alpha { get; private set; }
        private double _sumOfExps;
        private List<Neuron[]> L { get; set; }
        private List<double[,]> DeltaW { get; set; }
        private Func<double, double> _f;
        private Func<double, double> _derF;

        public Perceptron(ActivationFunctionType type, int[] neuronsPerLayer, bool withSoftmax, double epsilon, double alpha)
        {
            this.neuronsPerLayer = neuronsPerLayer;
            WithSoftmax = withSoftmax;
            ActivationType = type;
            Epsilon = epsilon;
            Alpha = alpha;
            SetActivationFunc(ActivationType);

            L = new List<Neuron[]>(neuronsPerLayer.Length);
            for (int l = 0; l < neuronsPerLayer.Length; l++)
            {
                bool isOutputLayer = l == neuronsPerLayer.Length - 1;
                L.Add(CreateLayer(neuronsPerLayer[l], isOutputLayer));
            }

            W = new List<double[,]>(L.Count);
            DeltaW = new List<double[,]>(L.Count);
            for (int l = 0; l < L.Count - 1; l++)
            {
                bool onOutput = l + 1 == L.Count - 1;
                W.Add(CreateWeights(L[l].Length, L[l + 1].Length - (onOutput ? 0 : 1)));
                DeltaW.Add(new double[neuronsPerLayer[l], neuronsPerLayer[l + 1]]);
            }
        }

        public static Perceptron FromJson(string json)
        {
            var fromJson = JsonConvert.DeserializeObject<PerceptronFromJson>(json);

            var newP = new Perceptron(fromJson.ActivationType, fromJson.neuronsPerLayer, fromJson.WithSoftmax, fromJson.Epsilon, fromJson.Alpha);

            newP.W = fromJson.W;

            return newP;
        }

        public string Save()
        {
            return JsonConvert.SerializeObject(this);
        }

        public double[] GetResult(double[] inputs, double[] inputsFromAnotherLayer = null)
        {
            if (inputs.Length != L[0].Length - 1)
            {
                throw new ArgumentException("Длина массива входных параметров и количество нейронов не совпадают.");
            }

            for (int n = 0; n < L[0].Length - 1; n++)
            {
                L[0][n] = new Neuron 
                { 
                    In = inputsFromAnotherLayer != null ? inputsFromAnotherLayer[n] : 0,
                    Out = inputs[n],
                };
            }

            L[0][L[0].Length - 1] =new Neuron{ Out = 1 };

            for (int l = 1; l < L.Count - 1; l++)
            {
                for (int n = 0; n < L[l].Length - 1; n++)
                {
                    double sum = 0;
                    for (int i = 0; i < L[l - 1].Length; i++)
                    {
                        sum += L[l - 1][i].Out * W[l - 1][i, n];
                    }

                    L[l][n] = new Neuron
                    {
                        In = sum,
                        Out = _f(sum)
                    };
                }

                L[l][L[l].Length - 1] = new Neuron
                {
                    In = 0,
                    Out = 1,
                };
            }

            int last = L.Count - 1;
            for (int n = 0; n < L[last].Length; n++)
            {
                double sum = 0;
                for (int i = 0; i < L[last - 1].Length; i++)
                {
                    sum += L[last - 1][i].Out * W[last - 1][i, n];
                }

                L[last][n] = new Neuron
                {
                    In = sum,
                    Out = _f(sum)
                };
            }

            if (WithSoftmax)
            {
                _sumOfExps = L[last].Sum(val => Math.Exp(val.Out));
                L[last] = L[last].Select(val => new Neuron 
                {
                    In = val.In,
                    Out = val.Out / _sumOfExps
                }).ToArray();
            }

            return L[last].Select(val => val.Out).ToArray();
        }

        public double[] BackPropagation(double[] ideal)
        {
            var deltas = new List<double[]>(neuronsPerLayer.Length);
            for (int l = 0; l < L.Count; l++)
            {
                deltas.Add(new double[neuronsPerLayer[l]]);
            }

            for (int l = L.Count - 1; l >= 0; l--)
            {
                for(int n = 0; n < deltas[l].Length; n++)
                {
                    var e = GetError(l, n,
                        deltas,
                        l == L.Count - 1 ? (double?)ideal[n] : null);

                    deltas[l][n] = _derF(L[l][n].In) * e;
                }
            }

            for (int l = 0; l < DeltaW.Count; l++)
            {
                for (int i = 0; i < DeltaW[l].GetLength(0); i++)
                {
                    for (int j = 0; j < DeltaW[l].GetLength(1); j++)
                    {
                        DeltaW[l][i,j] = Epsilon * deltas[l + 1][j] * L[l][i].Out + Alpha * DeltaW[l][i, j];
                        W[l][i,j] += DeltaW[l][i,j];
                    }
                }
            }

            return deltas[0].ToArray();
        }

        public double LossFunction(double[] ideal)
        {
            if (ideal.Length != L.Last().Length)
            {
                throw new ArgumentException("Длина массива эталонных значений не совпадает в количеством выходных данных.");
            }

            double d = 0;
            for (int i = 0; i < ideal.Length; i++)
            {
                d += (ideal[i] - L.Last()[i].Out) * (ideal[i] - L.Last()[i].Out);
            }

            return d / 2;
        }

        private double GetError(int l, int n, List<double[]> deltas, double? ideal)
        {
            double error = 0;

            if (l == L.Count - 1)
            {
                error = ideal.Value - L[l][n].Out;
            }
            else
            {
                for (int neuronOnNextLayer = 0; neuronOnNextLayer < deltas[l+1].Length; neuronOnNextLayer++)
                {
                    error += deltas[l+1][neuronOnNextLayer] * W[l][n, neuronOnNextLayer];
                }
            }

            return error;
        }

        private void SetActivationFunc(ActivationFunctionType type)
        {
            switch(type)
            {
                case ActivationFunctionType.Sigmoid:
                    _f = (x) => 1 / (1 + Math.Exp(-x));
                    _derF = (x) =>
                    {
                        var e = _f(x);
                        return e * (1 - e);
                    };
                    break;
                case ActivationFunctionType.LeakyReLU:
                    _f = (x) => Math.Max(x, 0.1 * x);
                    _derF = (x) => x > 0.1 * x ? 1 : 0.1;
                    break;
            }
        }

        private double[,] RandomiseArray(int r, int c)
        {
            double[,] res = new double[r, c];
            double edge = 10;
            foreach(var layer in L)
            {
                edge /= layer.Length;
            }

            Random rand = new Random();

            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    res[i, j] = rand.NextDouble() - 0.5; //% (2 * edge) - edge;
                }
            }

            return res;
        }

        private Neuron[] CreateLayer(int numberOfNeurons, bool isOutputLayer)
        {
            return new Neuron[numberOfNeurons + (isOutputLayer ? 0 : 1)];
        }

        private double[,] CreateWeights(int numberOfNeuronsOnCurrentLayer, int numberOfNeuronsOnNextLayer)
        {
            return RandomiseArray(numberOfNeuronsOnCurrentLayer, numberOfNeuronsOnNextLayer);
        }
    }
}
