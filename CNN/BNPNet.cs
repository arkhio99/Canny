using BitmapLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace NeuralNet
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

    public class NNetData
    {
        public double[] ideal;
        public Bitmap picture;
    }

    public class BNPNet
    {
        internal class BLPNetForJson
        {
            public ActivationFuncType ActivationFuncType { get; set; }
            public int[] Layers { get; set; }
            public List<double[,]> Connections { get; set; }
            public List<double[,]> DeltaConnections { get; set; }
            public double SpeedOfLearning { get; set; }
            public bool WithSoftmax { get ; set; }
        }

        private ActivationFuncType ActivationFuncType { get ;set;}

        private Func<double, double> _activation { get; set; }

        private Func<double, double> _difActivation { get; set; }
        
        private double _sumOfExpsOnOutput;

        /// <summary>
        /// Применяется ли SoftMax
        /// </summary>
        public bool WithSoftmax { get; private set; } = false;

        /// <summary>
        /// Скорость обучения
        /// </summary>
        public double SpeedOfLearning { get; set; } = 0.5;

        /// <summary>
        /// Скорость прироста
        /// </summary>
        public double Alpha { get; set; } = 0.3;

        /// <summary>
        /// Слои нейронов
        /// </summary>
        public List<Neuron[]> Layers { get; private set; }

        /// <summary>
        /// Список массивов связей между нейронами
        /// [0] - между входным и первым скрытым слоем
        /// </summary>
        public List<double[,]> Connections { get; private set; }

        /// <summary>
        /// Список массивов дельт связей
        /// </summary>
        public List<double[,]> DeltaConnections { get; private set; }

        /// <summary>
        /// Конструктор перцептрона
        /// </summary>
        /// <param name="funcType">Тип функции активации</param>
        /// <param name="layers">Массив, где каждый элемент - количество нейронов на слое</param>
        /// <param name="WithSoftmax">Использовать ли SoftMax</param>
        public BNPNet(ActivationFuncType funcType, int[] layers, bool WithSoftmax = false)
        {
            ActivationFuncType = funcType;
            this.WithSoftmax = WithSoftmax;
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

        /// <summary>
        /// Конструктор по JSON-строке
        /// </summary>
        /// <param name="input">JSON-строка</param>
        public BNPNet(string input)
        {
            var obj = JsonConvert.DeserializeObject<BLPNetForJson>(input);

            SetActivationFunc(obj.ActivationFuncType);
            Layers = new List<Neuron[]>(obj.Layers.Length);
            for (int i = 0; i < obj.Layers.Length; i++)
            {
                if (i != obj.Layers.Length - 1)
                {
                    Layers.Add(new Neuron[obj.Layers[i] + 1]);
                    Layers[i][^1] = new Neuron { Output = 1 };
                }
                else
                {
                    Layers.Add(new Neuron[obj.Layers[i]]);
                }
            }
            Connections = obj.Connections;
            DeltaConnections = obj.DeltaConnections;
            SpeedOfLearning = obj.SpeedOfLearning;
            WithSoftmax = obj.WithSoftmax;
        }

        /// <summary>
        /// Сохраняет в JSON-строку
        /// </summary>
        /// <returns>JSON-строка</returns>
        public string Save()
        {
            var toWrite = new BLPNetForJson 
            {
                ActivationFuncType = ActivationFuncType,
                Connections = Connections,
                DeltaConnections = DeltaConnections,
                SpeedOfLearning = SpeedOfLearning,
                WithSoftmax = WithSoftmax,
            };
            toWrite.Layers = new int[Layers.Count];
            for (int i = 0; i < Layers.Count - 1; i++)
            {
                toWrite.Layers[i] = Layers[i].Length - 1;
            }
            toWrite.Layers[^1] = Layers[^1].Length;

            var output = JsonConvert.SerializeObject(toWrite);
            return output;
        }

        /// <summary>
        /// Обработать входные данные
        /// </summary>
        /// <param name="input">Входные данные</param>
        /// <returns>Результат обработки</returns>
        public double[] GetResult(double[] input)
        {
            if (input.Length != Layers[0].Length - 1)
            {
                throw new ArgumentException("Длина входного массива не соответствует количеству входов");
            }

            double MaxOfInputs = input.Max();

            for (int neuron = 0; neuron < Layers[0].Length - 1; neuron++)
            {
                Layers[0][neuron] = new Neuron { Output = input[neuron] / MaxOfInputs };
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

            // Нормализовать вывод
            if (WithSoftmax)
            {
                _sumOfExpsOnOutput = Layers[^1].Sum(o => Math.Exp(o.Output));
                Layers[^1] = Layers[^1].Select(o => new Neuron 
                {
                    Input = o.Input,
                    Output = Math.Exp(o.Output) / _sumOfExpsOnOutput,
                }).ToArray();
            }

            return Layers[^1].Select(n => n.Output).ToArray();
        }

        /// <summary>
        /// Функция ошибки.
        /// </summary>
        /// <param name="ideal">Эталонные значения</param>
        /// <returns>Значение ошибки</returns>
        public double LossFunction(double[] ideal)
        {
            double sum = 0;
            for (int i = 0; i < Layers[^1].Length; i++)
            {
                double actual = Layers[^1][i].Output;
                sum += (ideal[i] - actual) * (ideal[i] - actual);
            }

            return sum / Layers[^1].Length;
        }

        /// <summary>
        /// Обратное распространение ошибки
        /// </summary>
        /// <param name="ideal">Эталонные значения</param>
        /// <returns>Массив ошибок на входном слое</returns>
        public double[] BackPropagation(double[] ideal)
        {
            var deltas = GetDeltaConnections(ideal, out double[] omegasOnFirstLayer);
            UpdateConnections(deltas);
            return omegasOnFirstLayer;
        }

        private double SoftMax(double o)
        {
            return Math.Exp(o) / _sumOfExpsOnOutput;
        }

        private List<double[,]> GetDeltaConnections(double[] ideal, out double[] firstLayerOmegas)
        {
            var omegas = new List<double[]>(Layers.Count);
            for (int l = 0; l < Layers.Count - 1; l++)
            {
                // не считать градиенты для нейронов смещения из скрытых слоёв 
                omegas.Add(new double[Layers[l].Length - 1]);
            }
            omegas.Add(new double[Layers[^1].Length]);

            // подсчитать дельты для выходов
            for (int outNeuron = 0; outNeuron < omegas[^1].Length; outNeuron++)
            {
                Neuron outN = Layers[^1][outNeuron];
                double actual = WithSoftmax ? Math.Log(outN.Output) * _sumOfExpsOnOutput : outN.Output;
                double expected = WithSoftmax ? Math.Log(ideal[outNeuron]) * _sumOfExpsOnOutput : ideal[outNeuron];
                omegas[^1][outNeuron] = (expected -  actual) * _difActivation(outN.Input);
            }
            
            // подсчитать дельты для скрытых и входного слоёв
            for (int l = omegas.Count - 2; l >= 0; l--)
            {
                for (int start = 0; start < omegas[l].Length; start++)
                {
                    var sum = 0.0;
                    for (int end = 0; end < omegas[l + 1].Length; end++)
                    {
                        sum += omegas[l + 1][end] * Connections[l][start, end];
                    }
                    
                    omegas[l][start] = SpeedOfLearning * _difActivation(Layers[l][start].Input) * sum;
                }
            }

            firstLayerOmegas = omegas[0];

            for (int l = 0; l < DeltaConnections.Count; l++)
            {
                for (int start = 0; start < DeltaConnections[l].GetLength(0); start++)
                {
                    for (int end = 0; end < DeltaConnections[l].GetLength(1); end++)
                    {
                        //                                                                         TODO Output -> Input
                        DeltaConnections[l][start, end] = SpeedOfLearning * omegas[l + 1][end] * Layers[l][start].Input + Alpha * DeltaConnections[l][start,end];
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
                    _difActivation = (x) => x > 0.1 * x ? 1 : 0.1;
                    break;
                case ActivationFuncType.Sigmoid:
                    _activation = (x) => 1 / (1 + Math.Exp(-x));
                    _difActivation = (x) => _activation(x) * (1 - _activation(x));
                    break;
            }
        }

        private double[,] RandomiseArray(double[,] array)
        {
            var edge = 1.0;
            for (int l = 0; l < Layers.Count; l++)
            {
                edge *= Layers[l].Length;
            }
            edge = 1 / edge;
            Random random = new Random();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = random.NextDouble() % (2 * edge) - edge/*(random.Next() % 3 + 1) * random.NextDouble() * (random.NextDouble() > 0.5 ? 1 : -1)*/;
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
