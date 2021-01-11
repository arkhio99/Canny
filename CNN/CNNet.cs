using System;
using System.Collections.Generic;
using System.Linq;
using NeuralNetworks;
using Newtonsoft.Json;

namespace NeuralNet
{
    public class ConvolutionLayer
    {
        public double[,,] Inputs { get; set; }
        public List<double[,,]> Filters { get; set; }

        public double Epsilon { get; set; } = 1;

        public double[,,] GetResult(double[,,] filter)
        {
            return Convolution(filter);
        }

        public double[,,] Convolution(double[,,] filter)
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
                init.GetLength(0) + 2 * filter.GetLength(0) - 2,
                init.GetLength(1) + 2 * filter.GetLength(1) - 2,
                init.GetLength(2) + 2 * filter.GetLength(2) - 2];

            for (int l = 0; l < init.GetLength(0); l++)
            {
                for (int y = 0; y < init.GetLength(1); y++)
                {
                    for (int x = 0; x < init.GetLength(2); x++)
                    {
                        newInit[l + filter.GetLength(0) - 1,
                            y + filter.GetLength(1) - 1,
                            x + filter.GetLength(2) - 1] = 
                            init[l, y, x];
                    }
                }
            }

            init = newInit;

            var res = new double[
                Inputs.GetLength(0),
                Inputs.GetLength(1),
                Inputs.GetLength(2)];

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
                                    double initValue = init[l + k, y + i, x + j];
                                    double filterValue = filter[filter.GetLength(0) - f1, filter.GetLength(1) - f2, filter.GetLength(2) - f3];
                                    res[l, y, x] += initValue * filterValue;
                                        
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        public double[,,] BackPropagation(double[,,] dy)
        {
            var dx = new double[Inputs.GetLength(0), Inputs.GetLength(1), Inputs.GetLength(2)];
            for (int f = 0; f < Filters.Count; f++)
            {
                var dyForLayerF = dy.GetLayer(f).To3DArray();                
                var dw = Convolution(dyForLayerF);
                var dxTemp = ReverseConvolution(dy, Filters[f]);

                Filters[f] = Filters[f].Plus( dw, -Epsilon);
                dx = dx.Plus(dxTemp);
            }

            return dx;
        }
    }

    public class PoolingLayer
    {
        /// <summary>
        /// Позиции максимумов
        /// </summary>
        public int[,,] MaxPositions { get; set; }

        /// <summary>
        /// Размер слоя пулинга
        /// </summary>
        public int PoolSize { get; set; }

        /// <summary>
        /// Количество слоёв для пулинга
        /// </summary>
        public int HowLayers { get; set; }

        public PoolingLayer(int howLayers, int poolSize, int[,,] maxPositions = null)
        {
            HowLayers = howLayers;
            PoolSize = poolSize;
            MaxPositions = maxPositions;
        }

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

        /// <summary>
        /// Производит повышение разрешения данных
        /// </summary>
        /// <param name="init">Исходные данные</param>
        /// <returns>Данные с повышенным разрешением</returns>
        public double[,,] UpSample(double[,,] init)
        {
            var res = new double[MaxPositions.GetLength(0), MaxPositions.GetLength(1), MaxPositions.GetLength(2)];
            for (int l = 0; l < init.GetLength(0); l++)
            {
                for (int i = 0; i < init.GetLength(1); i++)
                {
                    for (int j = 0; j < init.GetLength(2); j++)
                    {
                        // начинает пулинг
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

        public double[,,] BackPropagation(double[,,] init)
        {
            return UpSample(init);
        }

        /// <summary>
        /// Находит максимум в некотором кубе
        /// </summary>
        /// <param name="init">Исходные данные</param>
        /// <param name="z">Номер слоя(среза), где надо искать</param>
        /// <param name="y">Координата Y, где надо искать</param>
        /// <param name="x">Координата X, где надо искать</param>
        /// <param name="zOfMax">Номер слоя, где хранится максимум в некотором кубе</param>
        /// <param name="yOfMax">Координата Y, где хранится максимум в некотором кубе</param>
        /// <param name="xOfMax">Координата X, где хранится максимум в некотором кубе</param>
        /// <returns></returns>
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
        Perceptron perceptron;

        /// <summary>
        /// Тип функции активации
        /// </summary>
        public ActivationFunctionType ActivationType { get; private set; }

        /// <summary>
        /// Первый слой свёртки
        /// </summary>
        public ConvolutionLayer cl1 { get; private set; }

        /// <summary>
        /// Первый слой пулинга
        /// </summary>
        public PoolingLayer pl1 { get; private set; }
        int poolSize = 3;

        /// <summary>
        /// Второй слой свёртки
        /// </summary>
        public ConvolutionLayer cl2 { get; private set; }

        public PoolingLayer pl2 { get; private set; }

        public ConvolutionLayer cl3 { get; private set; }

        public PoolingLayer pl3 { get; private set; }

        private Func<double, double> _activation { get; set; }
        private Func<double, double> _difActivation { get; set; }

        /// <summary>
        /// Конструктор свёрточной нейросети
        /// </summary>
        /// <param name="type">Тип функции активации</param>
        /// <param name="sizeOfPic">Размер картинки</param>
        /// <param name="howFilters1">Количество фильтров на первом слое свёртки</param>
        /// <param name="howLayersOnFilter1">Количество слоёв на фильтрах первого слоя свёртки</param>
        /// <param name="sizeOfFilter1">Размер слоя фильтра первого слоя свёртки</param>
        /// <param name="howFilters2">Количество фильтров на втором слое свёртки</param>
        /// <param name="howLayersOnFilter2">Количество слоёв на фильтрах второго слоя свёртки</param>
        /// <param name="sizeOfFilter2">Размер слоя фильтра второго слоя свёртки</param>
        /// <param name="howOnHiddenLayer1">Количество нейронов на скрытом слое</param>
        public CNNet(ActivationFunctionType type = ActivationFunctionType.Sigmoid,
            int sizeOfPic = 128,
            int howFilters1 = 3,
            int howLayersOnFilter1 = 1,
            int sizeOfFilter1 = 5,
            int sizeOfPool1 = 2,
            int howFilters2 = 5,
            int howLayersOnFilter2 = 3,
            int sizeOfFilter2 = 5,
            int sizeOfPool2 = 2,
            int howFilters3 = 3,
            int howLayersOnFilter3 = 5,
            int sizeOfFilter3 = 5,
            int sizeOfPool3 = 2,
            int[] neuronsPerHiddenLayer = null,
            int howOutputs = 1)
        {
            ActivationType = type;
            SetActivationFunc(type);
            var eps = 0.7;

            var size_inp = ((((sizeOfPic - sizeOfFilter1 + 1) / sizeOfPool1 - sizeOfFilter2 + 1) / sizeOfPool2) - sizeOfFilter3 + 1) / sizeOfPool3;
            var layers_inp = howFilters1 / howLayersOnFilter1 * howFilters2 / howLayersOnFilter2 * howFilters3 / howLayersOnFilter3;

            if (neuronsPerHiddenLayer == null)
            {
                neuronsPerHiddenLayer = new int[] { 10, 10 };
            }

            var neuronsPerLayer = neuronsPerHiddenLayer.ToList();
            neuronsPerLayer.Insert(0, size_inp * size_inp * layers_inp);
            neuronsPerLayer.Add(howOutputs);
            perceptron = new Perceptron(type, neuronsPerLayer.ToArray(), false, eps , 0.3);

            cl1 = new ConvolutionLayer();
            cl1.Epsilon = eps;
            cl1.Filters = new List<double[,,]>(howFilters1);
            for (int i = 0; i < howFilters1; i++)
            {
                cl1.Filters.Add(RandomiseFilter(howLayersOnFilter1, sizeOfFilter1));
            }

            pl1 = new PoolingLayer(1, sizeOfPool1);

            cl2 = new ConvolutionLayer();
            cl2.Epsilon = eps;
            cl2.Filters = new List<double[,,]>(howFilters2);
            for (int i = 0; i < howFilters2; i++)
            {
                cl2.Filters.Add(RandomiseFilter(howLayersOnFilter2, sizeOfFilter2));
            }

            pl2 = new PoolingLayer(1, sizeOfPool2);

            cl3 = new ConvolutionLayer();
            cl3.Epsilon = eps;
            cl3.Filters = new List<double[,,]>(howFilters3);
            for (int i = 0; i < howFilters3; i++)
            {
                cl3.Filters.Add(RandomiseFilter(howLayersOnFilter3, sizeOfFilter3));
            }

            pl3 = new PoolingLayer(1, sizeOfPool3);
        }

        /// <summary>
        /// Конструирует свёрточную нейросеть по JSON-строке
        /// </summary>
        /// <param name="input">JSON-строка</param>
        public CNNet(string input)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(input);
            cl1 = JsonConvert.DeserializeObject<ConvolutionLayer>(dict["ConvolutionLayer1"]);
            pl1 = JsonConvert.DeserializeObject<PoolingLayer>(dict["PoolingLayer1"]);
            cl2 = JsonConvert.DeserializeObject<ConvolutionLayer>(dict["ConvolutionLayer2"]);
            pl2 = JsonConvert.DeserializeObject<PoolingLayer>(dict["PoolingLayer2"]);
            cl3 = JsonConvert.DeserializeObject<ConvolutionLayer>(dict["ConvolutionLayer3"]);
            pl3 = JsonConvert.DeserializeObject<PoolingLayer>(dict["PoolingLayer3"]);
            perceptron = Perceptron.FromJson(dict["perceptron"]);
            ActivationType = (ActivationFunctionType)Enum.Parse(ActivationType.GetType(), dict["ActivationType"]);
            SetActivationFunc(ActivationType);
            poolSize = int.Parse(dict["poolSize"]);
        }

        /// <summary>
        /// Сохраняет свёрточную нейросеть в JSON-строку
        /// </summary>
        /// <returns>JSON-строка</returns>
        public string Save()
        {
            return JsonConvert.SerializeObject(new Dictionary<string, string> 
            {
                { "ConvolutionLayer1", JsonConvert.SerializeObject(cl1) },
                { "PoolingLayer1", JsonConvert.SerializeObject(pl1) },
                { "ConvolutionLayer2", JsonConvert.SerializeObject(cl2) },
                { "PoolingLayer2", JsonConvert.SerializeObject(pl2) },
                { "ConvolutionLayer3", JsonConvert.SerializeObject(cl3) },
                { "PoolingLayer3", JsonConvert.SerializeObject(pl3) },
                { "perceptron", perceptron.Save()},
                { "ActivationType", ActivationType.ToString() },
                { "poolSize", poolSize.ToString() },
            });
        }

        /// <summary>
        /// Обрабатывает входные данные
        /// </summary>
        /// <param name="input">Входные данные</param>
        /// <returns>Выходные данные</returns>
        public double[] GetResult(double[,,] input)
        {
            cl1.Inputs = input;
            List<double[,,]> inputOnPl1 = new List<double[,,]>();
            for (int f = 0; f < cl1.Filters.Count; f++)
            {
                inputOnPl1.Add(Activate(cl1.GetResult(cl1.Filters[f])));
            }

            var outOnPl1 = pl1.GetResult(ListToArray(inputOnPl1));
            var activeFromPl1 = Activate(outOnPl1);

            cl2.Inputs = activeFromPl1;            
            List<double[,,]> inputOnPl2 = new List<double[,,]>();
            for (int f = 0; f < cl2.Filters.Count; f++)
            {
                inputOnPl2.Add(Activate(cl2.GetResult(cl2.Filters[f])));
            }


            var outOnPl2 = pl2.GetResult(ListToArray(inputOnPl2));
            var activeFromPl2 = Activate(outOnPl2);

            cl3.Inputs = activeFromPl2;
            List<double[,,]> inputOnPl3 = new List<double[,,]>();
            for (int f = 0; f < cl3.Filters.Count; f++)
            {
                inputOnPl3.Add(Activate(cl3.GetResult(cl3.Filters[f])));
            }

            var outOnPl3 = pl3.GetResult(ListToArray(inputOnPl3));
            var activeFromPl3 = Activate(outOnPl3);

            return perceptron.GetResult(activeFromPl3.ToVector(), outOnPl3.ToVector());            
        }

        /// <summary>
        /// Возвращает функцию ошибки от эталонных значений
        /// </summary>
        /// <param name="ideal">Эталонные значения</param>
        /// <returns>Функция ошибки</returns>
        public double LossFunction(double[] ideal)
        {
            return perceptron.LossFunction(ideal);
        }

        /// <summary>
        /// Обучает нейросеть
        /// </summary>
        /// <param name="ideal">Эталонные значения</param>
        /// <returns>Ошибки на первом слое</returns>
        public double[,,] BackPropagation(double[] ideal)
        {
            var deltasOnPercInp = perceptron.BackPropagation(ideal);
            var deltaOnPl3Inp = pl3.BackPropagation(
                deltasOnPercInp.ToArray3(
                    pl3.MaxPositions.GetLength(0) / pl3.HowLayers,
                    pl3.MaxPositions.GetLength(1) / pl3.PoolSize,
                    pl3.MaxPositions.GetLength(2) / pl3.PoolSize));
            var deltaOnCl3Inp = cl3.BackPropagation(deltaOnPl3Inp);
            var deltaOnPl2Inp = pl2.BackPropagation(deltaOnCl3Inp);
            var deltaOnCl2Inp = cl2.BackPropagation(deltaOnPl2Inp);
            var deltaOnPl1Inp = pl1.BackPropagation(deltaOnCl2Inp);
            var deltaOnCl1Inp = cl1.BackPropagation(deltaOnPl1Inp);

            return deltaOnCl1Inp;
        }
        
        /// <summary>
        /// Применяет функцию активации к входным данным
        /// </summary>
        /// <param name="input">Входные данные</param>
        /// <returns>Активированные данные</returns>
        private double[,,] Activate(double[,,] input)
        {
            for (int l = 0; l < input.GetLength(0); l++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    for (int x = 0; x < input.GetLength(2); x++)
                    {
                        input[l, y, x] = _activation(input[l, y, x]);
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// Устанавливает другую функцию активации
        /// </summary>
        /// <param name="type">Тип функции активации</param>
        private void SetActivationFunc(ActivationFunctionType type)
        {
            switch (type)
            {
                case ActivationFunctionType.LeakyReLU:
                    _activation = (x) => Math.Max(0.1 * x, x);
                    _difActivation = (x) => x > 0.1 * x ? 1 : 0.1;
                    break;
                case ActivationFunctionType.Sigmoid:
                    _activation = (x) => 1 / (1 + Math.Exp(-x));
                    _difActivation = (x) => _activation(x) * (1 - _activation(x));
                    break;
            }
        }

        /// <summary>
        /// Делает из списка массив
        /// </summary>
        /// <param name="init">Список</param>
        /// <returns>Массив</returns>
        private double[,,] ListToArray(List<double[,,]> init)
        {
            var res = new double[init.Count, init[0].GetLength(1), init[0].GetLength(2)];
            for (int l = 0; l < init.Count; l++)
            {
                for (int y = 0; y < init[0].GetLength(1); y++)
                {
                    for (int x = 0; x < init[0].GetLength(2); x++)
                    {
                        res[l, y, x] = init[l][0,y, x];
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Рандомит фильтр
        /// </summary>
        /// <param name="howLayers">Количество слоёв на фильтре</param>
        /// <param name="sizeOfLayer">Размер слоя фильтра</param>
        /// <returns>Фильтр</returns>
        private double[,,] RandomiseFilter(int howLayers, int sizeOfLayer)
        {
            var res = new double[howLayers, sizeOfLayer, sizeOfLayer];
            Random rand = new Random();
            for (int l = 0; l < res.GetLength(0); l++)
            {
                for (int i = 0; i < res.GetLength(1); i++)
                {
                    for (int j = 0; j < res.GetLength(2); j++)
                    {
                        res[l, i, j] = rand.NextDouble() - 0.5;
                    }
                }
            }

            return res;
        }
    }
}
