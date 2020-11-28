using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace CNN
{
    public class Neuron
    {
        public double Input { get; set; }
        public double Output { get; set; }
    }

    public enum ActivationFunc
    {
        LeakyReLU,
    }

    public enum LayerType
    {
        Convolution,
        Activation,
        Pool,
    }

    internal class CNNetForReadJson
    {
        public ActivationFunc ActivationFuncType { get; set; }
        public List<Neuron[]> HOLayers { get; set; }
        public List<double[,]> Connections { get; set; }
        public int HowInputs { get; set; }
        public double[,] ConnectionsBetweenIAndL { get; set; }
        public List<double[,]> Filters { get; set; }
        public int SizeOfPooling { get; set; }
    }

    public class CNNet
    {
        /// <summary>
        /// Функция активации.
        /// </summary>
        public ActivationFunc ActivationFuncType { get; set; }
        private Func<double, double> _activation;
        private Func<double, double> _differencialActivation;

        /// <summary>
        /// Скрытые и выходной слои
        /// </summary>
        public List<Neuron[]> HOLayers { get => _HOlayers; }
        private List<Neuron[]> _HOlayers;

        /// <summary>
        /// Коэффициент обучения.
        /// </summary>
        public double Epsilon { get => _eps; set  => _eps = value; }
        private double _eps = 0.7;

        /// <summary>
        /// Скорость обучения.
        /// </summary>
        public double Alpha { get => _alpha; set  => _alpha = value; }
        private double _alpha = 0.3;

        /// <summary>
        /// Связи между скрытыми и выходным слоями
        /// </summary>
        public List<double[,]> Connections { get => _connections; }
        private List<double[,]> _connections;

        /// <summary>
        /// Дельты связей;
        /// </summary>
        private List<double[,]> _deltaConnections;
        
        /// <summary>
        /// Количество входов.
        /// </summary>
        public int HowInputs { get => _howInputs; }
        private int _howInputs;

        /// <summary>
        /// Матрица связей между входным и первым скрытым слоем
        /// </summary>
        public double[,] ConnectionsBetweenIAndL { get => _connectionsBetweenInputAndLayer; }
        private double[,] _connectionsBetweenInputAndLayer; 

        /// <summary>
        /// Фильтры.
        /// </summary>
        public List<double[,]> Filters { get => _filters; }
        private List<double[,]> _filters;

        /// <summary>
        /// Размер ядра пулинга
        /// </summary>
        public int SizeOfPooling { get => _sizeOfPooling; }
        private readonly int _sizeOfPooling;

        /// <summary>
        /// Конструктор объекта свёрточной нейросети.
        /// </summary>
        /// <param name="activation">Функция активации.</param>
        /// <param name="neuronsPerHiddenLayer">Массив, где индекс - номер скрытого слоя, а значение - количество нейронов на данной слое.</param>
        /// <param name="filters">Список фильтров.</param>
        /// <param name="howInputs">Количество входящих параметров.</param>
        /// <param name="howOutput">Количество исходящих результатов.</param>
        /// <param name="sizeOfPooling">Размер ядра пуллинга.</param>
        public CNNet(ActivationFunc activation, int[] neuronsPerHiddenLayer, int howFilters, int sizeOfFilters, int howInputs, int howOutput, int sizeOfPooling)
        {
            ActivationFuncType = activation;
            switch (activation)
            {
                case ActivationFunc.LeakyReLU:
                {
                    _activation = (x) => Math.Max(0.01 * x, x);
                    _differencialActivation = (x) => x > 0.01 * x ? 1 : 0.01;
                    break;
                }
            }


            _filters = new List<double[,]>(howFilters);
            for (int i = 0; i < howFilters; i++)
            {
                var temp = new double[sizeOfFilters,sizeOfFilters];
                RandomiseArray(ref temp);
                _filters.Add(temp);
            }

            _sizeOfPooling = sizeOfPooling;
            _howInputs = howInputs;

            _connectionsBetweenInputAndLayer = new double[howInputs, neuronsPerHiddenLayer[0]];
            RandomiseArray(ref _connectionsBetweenInputAndLayer);

            _HOlayers = new List<Neuron[]>(neuronsPerHiddenLayer.Length);
            _connections = new List<double[,]>(neuronsPerHiddenLayer.Length);
            _deltaConnections = new List<double[,]>(neuronsPerHiddenLayer.Length);
            // TODO доделать
            _HOlayers.Add(new Neuron[neuronsPerHiddenLayer[0]]);
            for (int i = 1; i < neuronsPerHiddenLayer.Length; i++)
            {
                var temp = new double[neuronsPerHiddenLayer[i -1], neuronsPerHiddenLayer[i]];
                RandomiseArray(ref temp);
                _connections.Add(temp);

                var tempDeltas = new double[neuronsPerHiddenLayer[i -1], neuronsPerHiddenLayer[i]];
                FillWithZeros(ref tempDeltas);
                _deltaConnections.Add(tempDeltas);

                _HOlayers.Add(new Neuron[neuronsPerHiddenLayer[i]]);
            }
            
            var lastConnections = new double[neuronsPerHiddenLayer[^1], howOutput];
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
            var obj = JsonConvert.DeserializeObject<CNNetForReadJson>(input);
            ActivationFuncType = obj.ActivationFuncType;

            switch (ActivationFuncType)
            {
                case ActivationFunc.LeakyReLU:
                {
                    _activation = (x) => Math.Max(0.01 * x, x);
                    _differencialActivation = (x) => x > 0.01 * x ? 1 : 0.01;
                    break;
                }
            }

            _HOlayers = obj.HOLayers;
            _connections = obj.Connections;
            _deltaConnections = new List<double[,]> (_connections.Count);
            for (int i = 0; i < _deltaConnections.Count; i++)
            {
                var temp = new double[]
                _deltaConnections
            }

            _howInputs = obj.HowInputs;
            _connectionsBetweenInputAndLayer = obj.ConnectionsBetweenIAndL;
            _filters = obj.Filters;
            _sizeOfPooling = obj.SizeOfPooling;
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
        public double[,] Convolute(double[,] init, double[,] filter)
        {
            var res = new double[init.GetLength(0) - filter.GetLength(0) / 2 - filter.GetLength(0) % 2, init.GetLength(1) - filter.GetLength(1) / 2 - filter.GetLength(1) % 2];
            
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
        public double[,] Activate(double[,] init)
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
        public double[,] Pool(double[,] init)
        {
            var odd0 = init.GetLength(0) % _sizeOfPooling;
            var odd1 = init.GetLength(1) % _sizeOfPooling;
            var res = new double[init.GetLength(0) / _sizeOfPooling + (odd0 != 0 ? 1 : 0), init.GetLength(1) / _sizeOfPooling + (odd1 != 0 ? 1 : 0)];
            for(int i = 0; i < init.GetLength(0) - odd0; i += _sizeOfPooling)
            {
                for (int j = 0; j < init.GetLength(1) - odd1 ; j += _sizeOfPooling)
                {
                    res[i / _sizeOfPooling, j / _sizeOfPooling] = MaxInSquare(init, i, j);
                }

                if (odd1 != 0)
                {
                    int j = init.GetLength(1) - _sizeOfPooling;
                    res[i / _sizeOfPooling, j / _sizeOfPooling + 1] = MaxInSquare(init, i, j);
                }
            }

            if (odd0 != 0)
            {
                int i = init.GetLength(0) - _sizeOfPooling;
                for (int j = 0; j < init.GetLength(1) - odd1 ; j += _sizeOfPooling)
                {
                    res[res.GetLength(0) - 1, j / _sizeOfPooling] = MaxInSquare(init, i, j);
                }

                if (odd1 != 0)
                {
                    int j = init.GetLength(1) - _sizeOfPooling;
                    res[res.GetLength(0) - 1, j / _sizeOfPooling + 1] = MaxInSquare(init, i, j);
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
            for (int di = 0; di < _sizeOfPooling; di++)
            {
                for (int dj = 0; dj < _sizeOfPooling; dj++)
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
        public double LossFunction(double[] ideal)
        {
            if (ideal.Length != _HOlayers[^1].Length)
            {
                throw new ArgumentException("ideal.Length != _HOlayers[^1].Length");
            }

            var res = .0;
            for (int i = 0; i < ideal.Length; i++)
            {
                res += (ideal[i] - _HOlayers[^1][i].Output) * (ideal[i] - _HOlayers[^1][i].Output);
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

        /// <summary>
        /// Высчитывает количество входов на перцептроне в зависимости от архитектуры
        /// </summary>
        /// <param name="init">Входной массив(КВАДРАТНЫЙ)</param>
        /// <param name="numberOfFilters">Количество фильтров.</param>
        /// <param name="sizeOfFilter">Размер ядра фильтра</param>
        /// <param name="types">Архитектура сети.</param>
        /// <returns>Количество входов на перцептроне.</returns>
        public static int GetNumberOfInputsOnFullyConnected(double[,] init, int numberOfFilters, int sizeOfFilter, int sizeOfPooling, List<LayerType> types)
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
                            sizeOfOutputs = sizeOfOutputs - sizeOfFilter / 2 - sizeOfFilter % 2;
                            break;
                        }
                    case LayerType.Pool:
                        {
                            sizeOfOutputs = sizeOfOutputs / sizeOfPooling + (sizeOfOutputs % sizeOfPooling != 0 ? 1 : 0);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return sizeOfOutputs * sizeOfOutputs * outputs;
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
                for (int x = 0; x < inputs.GetLength(0); x++)
                {
                    temp += inputs[x] * _connectionsBetweenInputAndLayer[x, n];
                }

                _HOlayers[0][n] = new Neuron { Input = temp, Output = _activation(temp) };
            }

            for (int l = 1; l < _HOlayers.Count; l++)
            {
                for (int n = 0; n < _HOlayers[l].Length; n++)
                {
                    var temp = .0;
                    for (int x = 0; x < _HOlayers[l-1].GetLength(0); x++)
                    {
                        temp += _HOlayers[l-1][x].Output * _connections[l - 1][x, n];
                    }

                    _HOlayers[l][n] = new Neuron { Input = temp, Output = _activation(temp) };
                }
            }

            var output = _HOlayers[_HOlayers.Count - 1].Select(n => n.Output).ToArray();

            return output;            
        }

        private List<double[,]> AfterOneCRPLayer(List<double[,]> inits)
        {
            var res = new List<double[,]>(_filters.Count);
            foreach (var init in inits)
            {
                foreach (var filter in _filters)
                {
                    res.Add(Pool(Activate(Convolute(init, filter))));
                }
            }

            return res;
        }


        public List<double[]> GetDeltas(double[] ideal)
        {
            // создадим список дельт, где каждый массив содержит дельты для нейронов для определённых слоёв.
            List<double[]> deltas = new List<double[]>(_HOlayers.Count);
            foreach (var layer in _HOlayers)
            {
                var temp = new double[layer.Length];
                FillWithZeros(ref temp);
                deltas.Add(temp);
            }

            // высчитаем дельты для выходных параметров
            for (int o = 0; o < deltas[^1].Length; o++)
            {
                deltas[^1][o] = (ideal[o] - _HOlayers[^1][o].Output) * _differencialActivation(_HOlayers[^1][o].Input);
            }

            // вычислим дельты для скрытых слоёв
            // для каждого слоя
            for (int l = deltas.Count - 2; l >= 0; l--)
            {
                // для каждого нейрона на слое
                for (int n = 0; n < deltas[l].Length; n++)
                {
                    double sum = 0;
                    for (int i = 0; i < deltas[l + 1].Length; i++)
                    {
                        sum += _connections[l][n, i] * deltas[l + 1][i];
                    }

                    deltas[l][n] = _differencialActivation(_HOlayers[l][n].Input) * sum;
                }
            }

            return deltas;
        }

        private void SetDeltaConnections(List<double[]> deltas)
        {
            for (int start = 0; start < _connectionsBetweenInputAndLayer.GetLength(0); start++)
            {
                for (int end = 0; end < _connectionsBetweenInputAndLayer.GetLength(1); end++)
                {
                    _deltaConnections[0][start, end] = _eps * deltas[0][end] * _HOlayers[0][start].Output + _alpha * _deltaConnections[0][start, end];
                }
            }

            for (int l = 0; l < _HOlayers.Count; l++)
            {
                for (int start = 0; start < _connectionsBetweenInputAndLayer.GetLength(0); start++)
                {
                    for (int end = 0; end < _connectionsBetweenInputAndLayer.GetLength(1); end++)
                    {
                        _deltaConnections[l][start, end] = _eps * deltas[l][end] * _HOlayers[l][start].Output + _alpha * _deltaConnections[l][start, end];
                    }
                }
            }
        }

        private void SumDeltaConnectionsWithConnection()
        {
            for (int start = 0; start < _connectionsBetweenInputAndLayer.GetLength(0); start++)
            {
                for (int end = 0; end < _connectionsBetweenInputAndLayer.GetLength(1); end++)
                {
                    _connections[0][start,end] += _deltaConnections[0][start, end];
                }
            }

            for (int l = 0; l < _HOlayers.Count; l++)
            {
                for (int start = 0; start < _connectionsBetweenInputAndLayer.GetLength(0); start++)
                {
                    for (int end = 0; end < _connectionsBetweenInputAndLayer.GetLength(1); end++)
                    {
                        _connections[l][start,end] += _deltaConnections[l][start, end];;
                    }
                }
            }
        }
        
        public void BackPropagation(double[] idealOutputs)
        {
            var deltas = GetDeltas(idealOutputs);
            SetDeltaConnections(deltas);
            SumDeltaConnectionsWithConnection();
        }

        private double[] CrpToInputs(List<double[,]> inits)
        {
            double[] res = new double[inits.Count * inits[0].GetLength(0) * inits[0].GetLength(1)];
            for (int init = 0; init < inits.Count; init++)
            {
                for (int i = 0; i < inits[init].GetLength(0); i++)
                {
                    for (int j = 0; j < inits[init].GetLength(1); j++)
                    {
                        int index = init * inits[init].GetLength(0) * inits[0].GetLength(1) + i * inits[init].GetLength(1) + j;
                        res[index] = inits[init][i, j];
                    }
                }
            }

            return res;
        }

        public double[] GetResults(double[,] init)
        {
            if (init.Length != 64 * 64)
            {
                throw new ArgumentException("Init should be 64 * 64");
            }

            var _1CRP = AfterOneCRPLayer(new List<double[,]> { init });

            var _2CRP = AfterOneCRPLayer(_1CRP);

            var _3CRP = AfterOneCRPLayer(_2CRP);

            var inputsToPerceptron = CrpToInputs(_3CRP);

            var outputs = ProceessOnPerceptron(inputsToPerceptron);

            return outputs;
        }

        private void FillWithZeros(ref double[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = 0;
                }
            }
        }

        private void FillWithZeros(ref double[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }
    }
}
