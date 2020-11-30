using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CNN
{
    public class Neuron
    {
        public double Input { get; set; }
        public double Output { get; set; }
    }

    public enum ActivationFuncType
    {
        LeakyReLU,
        Sigmoid,
    }
    public class BLPNet
    {
        internal class BLPNetForReadJson
        {
            public ActivationFuncType ActivationFuncType { get; set; }
            public List<Neuron[]> Layers { get; set; }
            public List<double[,]> Connections { get; set; }
            public List<double[,]> DeltaConnections { get; set; }
            public double SpeedOfLearning { get; set; }
        }

        private Func<double, double> _activation { get; set; }

        private Func<double, double> _difActivation { get; set; }

        public double SpeedOfLearning { get; set; } = 0.5;

        public double Alpha { get; set; }

        public List<Neuron[]> Layers { get; }

        public List<double[,]> Connections { get; }

        public List<double[,]> DeltaConnections { get; }

        public BLPNet(ActivationFuncType funcType, int[] layers)
        {
            SetActivationFunc(funcType);
            Layers = new List<Neuron[]>(layers.Length);
            for (int i = 0; i < layers.Length; i++)
            {
                if (i != layers.Length - 1)
                {
                    Layers.Add(new Neuron[layers[i] + 1]);
                    Layers[i][^1] = new Neuron { Output = 1 };
                }
                else
                {
                    Layers.Add(new Neuron[layers[i]]);
                }
            }

            Connections = new List<double[,]>(layers.Length - 1);
            for (int i = 0; i < layers.Length - 1; i++)
            {
                Connections.Add(RandomiseArray(new double[Layers[i].Length, Layers[i + 1].Length - (i == layers.Length - 2 ? 0 : 1)]));
            }

            DeltaConnections = new List<double[,]>(layers.Length - 1);
            for (int i = 0; i < layers.Length - 1; i++)
            {
                DeltaConnections.Add(FillWithZeros(new double[Layers[i].Length, Layers[i + 1].Length - (i == layers.Length - 2 ? 0 : 1)]));
            }
        }

        public BLPNet(string path)
        {
            var input = File.ReadAllText(path);
            var obj = JsonConvert.DeserializeObject<BLPNetForReadJson>(input);

            SetActivationFunc(obj.ActivationFuncType);
            Layers = obj.Layers;
            Connections = obj.Connections;
            DeltaConnections = obj.DeltaConnections;
            SpeedOfLearning = obj.SpeedOfLearning;
        }

        public void Save(string path)
        {
            var output = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, output);
        }

        public double[] GetResult(double[] input)
        {
            if (input.Length != Layers[0].Length - 1)
            {
                throw new ArgumentException("Длина входного массива не соответствует количеству входов");
            }

            for (int neuron = 0; neuron < Layers[0].Length - 1; neuron++)
            {
                Layers[0][neuron] = new Neuron { Output = input[neuron] };
            }

            for (int l = 1; l < Layers.Count - 1; l++)
            {
                for (int neuron = 0; neuron < Layers[l].Length - 1; neuron++)
                {
                    double sum = 0;
                    int end = neuron;
                    for (int start = 0; start < Layers[l - 1].Length; start++)
                    {
                        sum += Layers[l - 1][start].Output * Connections[l - 1][start, end];
                    }

                    Layers[l][neuron] = new Neuron { Input = sum, Output = _activation(sum) };
                }
            }

            for (int neuron = 0; neuron < Layers[^1].Length; neuron++)
            {
                double sum = 0;
                int end = neuron;
                for (int start = 0; start < Layers[^2].Length; start++)
                {
                    sum += Layers[^2][start].Output * Connections[^2][start, end];
                }

                Layers[^1][neuron] = new Neuron { Input = sum, Output = _activation(sum) };
            }

            return Layers[^1].Select(n => n.Output).ToArray();
        }

        public double LossFunction(double[] ideal)
        {
            double sum = 0;
            for (int i = 0; i < Layers[^1].Length; i++)
            {
                sum += (ideal[i] - Layers[^1][i].Output) * (ideal[i] - Layers[^1][i].Output);
            }

            return sum / Layers[^1].Length;
        }

        public void BackPropagation(double[] ideal)
        {
            var deltas = GetDeltaConnections(ideal);
            UpdateConnections(deltas);
        }

        private List<double[,]> GetDeltaConnections(double[] ideal)
        {
            var deltas = new List<double[]>(Layers.Count);
            for (int l = 0; l < Layers.Count - 1; l++)
            {
                // не считать градиенты для нейронов смещения из скрытых слоёв 
                deltas.Add(new double[Layers[l].Length - 1]);
            }
            deltas.Add(new double[Layers[^1].Length]);

            // подсчитать дельты для выходов
            for (int outNeuron = 0; outNeuron < deltas[^1].Length; outNeuron++)
            {
                Neuron outN = Layers[^1][outNeuron];
                deltas[^1][outNeuron] = (ideal[outNeuron] - outN.Output) * _difActivation(outN.Input);
            }
            
            // подсчитать дельты для скрытых и входного слоёв
            for (int l = deltas.Count - 2; l >= 0; l--)
            {
                for (int start = 0; start < deltas[l].Length; start++)
                {
                    var sum = 0.0;
                    for (int end = 0; end < deltas[l + 1].Length; end++)
                    {
                        sum += SpeedOfLearning * deltas[l + 1][end] * Connections[l][start, end];
                    }

                    deltas[l][start] = _difActivation(Layers[l][start].Input) * sum;
                }
            }

            for (int l = 0; l < DeltaConnections.Count; l++)
            {
                for (int start = 0; start < DeltaConnections[l].GetLength(0); start++)
                {
                    for (int end = 0; end < DeltaConnections[l].GetLength(1); end++)
                    {
                        DeltaConnections[l][start, end] = SpeedOfLearning * deltas[l + 1][end] * Layers[l][start].Output;
                    }
                }
            }

            return DeltaConnections;
        }

        private void UpdateConnections(List<double[,]> deltas)
        {
            for (int l = 0; l < Connections.Count; l++)
            {
                for (int start = 0; start < Connections[l].GetLength(0); start++)
                {
                    for (int end = 0; end < Connections[l].GetLength(1); end++)
                    {
                        Connections[l][start, end] += deltas[l][start, end];
                    }
                }
            }
        }

        private void SetActivationFunc(ActivationFuncType type)
        {
            switch(type)
            {
                case ActivationFuncType.LeakyReLU:
                    _activation = (x) => Math.Max(0.1 * x, x);
                    _difActivation = (x) => x > 0.1 * x ? 0.1 : 1;
                    break;
                case ActivationFuncType.Sigmoid:
                    _activation = (x) => 1 / (1 - Math.Exp(-x));
                    _difActivation = (x) => _activation(x) * (1 - _activation(x));
                    break;
            }
        }

        private double[,] RandomiseArray(double[,] array)
        {
            Random random = new Random();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = random.NextDouble() % 0.5/*(random.Next() % 3 + 1) * random.NextDouble() * (random.NextDouble() > 0.5 ? 1 : -1)*/;
                }
            }

            return array;
        }

        private double[,] FillWithZeros(double[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = 0;
                }
            }

            return arr;
        }

        private double[] FillWithZeros(double[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }

            return arr;
        }
    }

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
    }
}
