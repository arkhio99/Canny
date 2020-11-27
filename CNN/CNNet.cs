using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace CNN
{
    internal class Neuron
    {
        public double Input { get; set; }
        public double Output { get; set; }
    }

    public enum ActivationFunc
    {
        ReLU,
    }

    public enum LayerType
    {
        Convolution,
        Activation,
        Pool,
    }

    public class CNNet
    {
        /// <summary>
        /// Функция активации.
        /// </summary>
        public ActivationFunc ActivationFunc { get; set; }
        private Func<double, double> _activation;
        private Func<double, double> _differencialActivation;
        private List<Neuron[]> _HOlayers;
        private List<double[,]> _connections;
        private int _howInputs;
        private double[,] _connectionsBetweenInputAndLayer; 
        private List<double[,]> _filters;
        private int _sizeOfPoolling;

        /// <summary>
        /// Конструктор объекта свёрточной нейросети.
        /// </summary>
        /// <param name="activation">Функция активации.</param>
        /// <param name="neuronsPerHiddenLayer">Массив, где индекс - номер скрытого слоя, а значение - количество нейронов на данной слое.</param>
        /// <param name="filters">Список фильтров.</param>
        /// <param name="howInputs">Количество входящих параметров.</param>
        /// <param name="howOutput">Количество исходящих результатов.</param>
        /// <param name="sizeOfPooling">Размер ядра пуллинга.</param>
        public CNNet(ActivationFunc activation, int[] neuronsPerHiddenLayer, List<double[,]> filters, int howInputs, int howOutput, int sizeOfPooling)
        {
            ActivationFunc = activation;
            switch (activation)
            {
                case ActivationFunc.ReLU:
                {
                    _activation = (x) => Math.Max(0.01 * x, x);
                    _differencialActivation = (x) => x > 0.01 * x ? 1 : 0.01;
                    break;
                }
            }

            _HOlayers = new List<Neuron[]>(neuronsPerHiddenLayer.Length);
            _connections = new List<double[,]>(neuronsPerHiddenLayer.Length);
            _filters = filters;
            _sizeOfPoolling = sizeOfPooling;
            _howInputs = howInputs;
            _connectionsBetweenInputAndLayer = new double[howInputs, neuronsPerHiddenLayer[0]];
            RandomiseArray(ref _connectionsBetweenInputAndLayer);
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

        /// <summary>
        /// Конструктор объектов по JSON-файлу.
        /// </summary>
        /// <param name="path">Путь к JSON-файлу.</param>
        public CNNet (string path)
        {
            var input = File.ReadAllText(path);
            var obj = JsonConvert.DeserializeObject<CNNet>(input);
            ActivationFunc = obj.ActivationFunc;

            switch (ActivationFunc)
            {
                case ActivationFunc.ReLU:
                {
                    _activation = (x) => Math.Max(0.01 * x, x);
                    _differencialActivation = (x) => x > 0.01 * x ? 1 : 0.01;
                    break;
                }
            }

            _HOlayers = obj._HOlayers;
            _connections = obj._connections;
            _howInputs = obj._howInputs;
            _connectionsBetweenInputAndLayer = obj._connectionsBetweenInputAndLayer;
            _filters = obj._filters;
            _sizeOfPoolling = obj._sizeOfPoolling;
        }

        /// <summary>
        /// Количество входов.
        /// </summary>
        public int CountOfInputs
        {
            get => _howInputs;
        }

        /// <summary>
        /// Количество нейронов на каждом слое.
        /// </summary>
        public int[] CountOfNeuronsPerLayer
        {
            get
            {
                var res = new int[_HOlayers.Count + 1];
                res[0] = _howInputs;
                for (int i = 0; i < _HOlayers.Count; i++)
                {
                    res[i + 1] = _HOlayers[i].Length;
                }
                 
                return res;
            }
        }

        /// <summary>
        /// Сохраняет свёрточную нейросеть в JSON-файл.
        /// </summary>
        /// <param name="path">Путь сохранения.</param>
        public void Save(string path)
        {
            var output = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, output);
        }

        /// <summary>
        /// Сворачивает исходный массив.
        /// </summary>
        /// <param name="init">Исходный массив.</param>
        /// <param name="filter">Фильтр.</param>
        /// <returns>Свёрнутое ихображение.</returns>
        private double[,] Convolute(double[,] init, double[,] filter)
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

        /// <summary>
        /// Применяет функцию активации к массиву.
        /// </summary>
        /// <param name="init">Исходный массив.</param>
        /// <returns>Массив, к которому применили функцию активации.</returns>
        private double[,] ReLU(double[,] init)
        {
            for (int i = 0; i < init.GetLength(0); i++)
            {
                for (int j = 0; j < init.GetLength(1); j++)
                {
                    init[i, j] = _activation(init[i, j]);
                }
            }

            return init;
        }

        /// <summary>
        /// Выполняет макс-пуллинг массива.
        /// </summary>
        /// <param name="init">Исходный массив.</param>
        /// <returns>Обработанный массив.</returns>
        private double[,] Pool(double[,] init)
        {
            var odd0 = init.GetLength(0) % _sizeOfPoolling;
            var odd1 = init.GetLength(1) % _sizeOfPoolling;
            var res = new double[init.GetLength(0) / _sizeOfPoolling + (odd0 != 0 ? 1 : 0), init.GetLength(1) / _sizeOfPoolling + (odd1 != 0 ? 1 : 0)];
            for(int i = 0; i < init.GetLength(0) - odd0; i += _sizeOfPoolling)
            {
                for (int j = 0; j < init.GetLength(1) - odd1 ; j += _sizeOfPoolling)
                {
                    res[i / _sizeOfPoolling, j / _sizeOfPoolling] = MaxInSquare(init, i, j);
                }

                if (odd1 != 0)
                {
                    int j = init.GetLength(1) - _sizeOfPoolling;
                    res[i / _sizeOfPoolling, j / _sizeOfPoolling + 1] = MaxInSquare(init, i, j);
                }
            }

            if (odd0 == 0)
            {
                int i = init.GetLength(0) - _sizeOfPoolling;
                for (int j = 0; j < init.GetLength(1) - odd1 ; j += _sizeOfPoolling)
                {
                    res[i / _sizeOfPoolling, j / _sizeOfPoolling] = MaxInSquare(init, i, j);
                }

                if (odd1 != 0)
                {
                    int j = init.GetLength(1) - _sizeOfPoolling;
                    res[i / _sizeOfPoolling, j / _sizeOfPoolling + 1] = MaxInSquare(init, i, j);
                }
            }

            return res;
        }

        /// <summary>
        /// Выбирает максимум в некотором квадрате массива, начинающемся в (i,j) с размером sizeOfPoolling.
        /// </summary>
        /// <param name="init">Исходный массив.</param>
        /// <param name="i">Строка, с которой начинается квадрат.</param>
        /// <param name="j">Столбец, с которого начинается квадрат.</param>
        /// <returns></returns>
        private double MaxInSquare(double[,] init, int i, int j)
        {
            var temp = double.MinValue;
            for (int di = 0; di < _sizeOfPoolling; di++)
            {
                for (int dj = 0; dj < _sizeOfPoolling; dj++)
                {
                    temp = init[i+di, j+dj] > temp ? init[i+di, j+dj] : temp;
                }
            }

            return temp;
        }

        /// <summary>
        /// Возвращает функцию ошибки.
        /// </summary>
        /// <param name="ideal">Ожидаемые значения.</param>
        /// <param name="actual">Полученный значения.</param>
        /// <returns>Функция ошибки.</returns>
        public double LossFunction(double[] ideal, double[] actual)
        {
            if (ideal.Length != actual.Length)
            {
                throw new ArgumentException("ideal.Length != actual.Length");
            }

            var res = .0;
            for (int i = 0; i < ideal.Length; i++)
            {
                res += (ideal[i] - actual[i]) * (ideal[i] - actual[i]);
            }

            return res / ideal.Length;
        }

        /// <summary>
        /// Рандомит массив.
        /// </summary>
        /// <param name="array">Массив.</param>
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

        // TODO доделать
        public static int GetNumberOfInputsOnFullyConnected(double[,] init, int numberOfFilters, int sizeOfFilter, int i, List<LayerType> types)
        {
            int outputs = 1;
            int sizeOfOutputs = init.GetLength(0);
            foreach (var type in types)
            {
                switch (type)
                {
                    case LayerType.Convolution:
                        {
                            outputs *= numberOfFilters;
                            sizeOfOutputs = init.GetLength(0) - sizeOfFilter / 2 - sizeOfFilter % 2;
                            break;
                        }
    

                }
            }
        }


        public double[] ProceessOnPerceptron(double[] inputs)
        {
            if (inputs.Length != _howInputs)
            {
                throw new ArgumentException($"Длина {nameof(inputs)} не совпадает с длиной, заданной в сети.");
            }

            for (int n = 0; n < _HOlayers[0].Length; n++)
            {
                var temp = .0;
                for (int input = 0; input < inputs.GetLength(0); input++)
                {
                    temp += inputs[input] * _connectionsBetweenInputAndLayer[input, n];
                }

                _HOlayers[0][n].Output = _activation(_HOlayers[0][n].Input);
            }

            for (int l = 1; l < _HOlayers.Count; l++)
            {
                for (int n = 0; n < _HOlayers[l].Length; n++)
                {
                    var temp = .0;
                    for (int input = 0; input < _HOlayers[l-1].GetLength(0); input++)
                    {
                        temp += _HOlayers[l-1][input].Output * _connectionsBetweenInputAndLayer[input, n];
                    }

                    _HOlayers[l][n].Output = _activation(_HOlayers[l][n].Input);
                }
            }

            var output = _HOlayers[_HOlayers.Count - 1].Select(n => n.Output).ToArray();

            return output;            
        }

        private double[] GetResults(double[,] init)
        {

        }
    }
}
