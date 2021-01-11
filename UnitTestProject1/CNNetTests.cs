using BitmapLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNetworks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace NeuralNet.Tests
{
    [TestClass]
    public class CNNetTests
    {
        string pathToData = @"..\..\..\..\";

        [TestMethod]
        public void ConvTest_2d_1d()
        {
            double[,,] x = new double[1, 3, 3]
            { {
                {0.0, 1.0, 2.0},
                {3.0, 4.0, 5.0},
                {6.0, 7.0, 8.0},
            } };

            double[,,] f = new double[1, 2, 2]
            {
                {
                    { 1, 2 },
                    { -1, 3 },
                }
            };

            double[,,] expected = new double[1, 2, 2]
            {
                {
                    { 11, 16 },
                    { 26, 31 },
                }
            };

            var convLayer = new ConvolutionLayer
            {
                Inputs = x,
                Filters = new List<double[,,]> { f },
            };

            var actual = convLayer.GetResult(f);

            Assert.That.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvText_3D_1d()
        {
            double[,,] x = new double[2, 3, 3]
            {
                {
                    {0.0, 1.0, 2.0},
                    {3.0, 4.0, 5.0},
                    {6.0, 7.0, 8.0},
                },
                {
                    {11, 12, 13},
                    {14, 15, 16},
                    {17, 18, 19},
                }
            };

            double[,,] f = new double[2, 2, 2]
            {
                {
                    { 1, 2 },
                    { -1, 3 },
                },
                {
                    {3, -1},
                    {-2, 1},
                }
            };

            double[,,] expected = new double[1, 2, 2]
            {
                {
                    { 19, 25 },
                    { 37, 43 },
                }
            };

            var convLayer = new ConvolutionLayer
            {
                Inputs = x,
                Filters = new List<double[,,]> { f },
            };

            var actual = convLayer.GetResult(f);

            Assert.AreEqual(expected.Length, actual.Length);
            for (int l = 0; l < actual.GetLength(0); l++)
            {
                for (int i = 0; i < actual.GetLength(1); i++)
                {
                    for (int j = 0; j < actual.GetLength(2); j++)
                    {
                        Assert.AreEqual(expected[l, i, j], actual[l, i, j], 0.1);
                    }
                }
            }
        }

        [TestMethod]
        public void ConvTest_3D_2D()
        {
            double[,,] x = new double[3, 3, 3]
            {
                {
                    {0.0, 1.0, 2.0},
                    {3.0, 4.0, 5.0},
                    {6.0, 7.0, 8.0},
                },
                {
                    {11, 12, 13},
                    {14, 15, 16},
                    {17, 18, 19},
                },
                {
                    {1, 2, 3},
                    {4, 5, 6},
                    {7, 8, 9},
                }
            };

            double[,,] f = new double[2, 2, 2]
            {
                {
                    { 1, 2 },
                    { -1, 3 },
                },
                {
                    {3, -1},
                    {-2, 1},
                }
            };

            double[,,] expected = new double[2, 2, 2]
            {
                {
                    { 19, 25 },
                    { 37, 43 },
                },
                {
                    {64, 70},
                    {82, 88},
                }
            };

            var convLayer = new ConvolutionLayer
            {
                Inputs = x,
                Filters = new List<double[,,]> { f },
            };

            var actual = convLayer.GetResult(f);

            Assert.That.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpSample()
        {
            var maxPoses = new int[,,]
            { {
                {0,1,1,0 },
                {0,0,0,0 },
                { 1,0,0,0},
                { 0,0,0,1},
            } };
            var maxs = new double[,,]
            {{
                {4, 5 },
                {6, 7 },
            } };

            var expected = new double[,,]
            {{
                {0,4,5,0 },
                {0,0,0,0 },
                { 6,0,0,0},
                { 0,0,0,7},
            } };

            PoolingLayer pl = new PoolingLayer(howLayers: 1, maxPositions: maxPoses, poolSize: 2);

            var actual = pl.UpSample(maxs);

            Assert.That.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReverseConv_Test_3D_2d()
        {
            var dy = new double[,,]
            {
                {
                    { 1, 2 },
                    { 3, 4 },
                },
                {
                    { -1, 3 },
                    { 4, 2 },
                },
            };

            var fs = new double[,,]
            {
                {
                    { 1, 2 },
                    { -1, 3 },
                },
                {
                    { 3, -1 },
                    { -2, 1 },
                },
            };

            var expected = new double[,,]
            {
                {
                    { 1, 4, 4 },
                    { 2, 11, 14 },
                    { -3, 5, 12 },
                },
                {
                    { 2, 6, 4 },
                    { 12, 10, 11 },
                    { -10, 5, 10 },
                },
                {
                    { -3, 10, -3 },
                    { 14, -5, 1 },
                    {-8, 0, 2 },
                }
            };

            var cl1 = new ConvolutionLayer
            {
                Filters = new List<double[,,]> { fs },
            };

            var actual = cl1.ReverseConvolution(dy, fs);
            Assert.That.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanSaveAndLoadCorrectly()
        {
            string toFile = pathToData + @"somaset\TrainData\1\im0002.jpg";
            int sizeOfPic = 124;
            CNNet net = new CNNet(ActivationFunctionType.Sigmoid, sizeOfPic,
                5, 1, 5,
                2,
                5, 5, 5,
                2,
                3, 5, 5, 2,
                new int[] { 100, 100 },
                1);

            var pic = JpegToData(toFile).picture;
            var testVector = PrepareData(pic, 124);
            var expected = net.GetResult(testVector);

            File.WriteAllText(pathToData + @"somaset\CanSaveAndLoadCorrectly.json", net.Save());

            CNNet newnet = new CNNet(File.ReadAllText(pathToData + @"somaset\CanSaveAndLoadCorrectly.json"));
            var actual = newnet.GetResult(testVector);

            Assert.That.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanBackProp()
        {
            int sizeOfPic = 124;
            CNNet net = new CNNet(ActivationFunctionType.Sigmoid, sizeOfPic,
                5, 1, 5,
                2,
                5, 5, 5,
                2,
                3, 5, 5, 2,
                new int[] { 100, 100 },
                1);

            string toFile = pathToData + @"somaset\TrainData\1\im0002.jpg";
            var pic = JpegToData(toFile).picture;
            var testVector = PrepareData(pic, 124);
            var ideal = new double[] { 0.8 };

            var actual = net.GetResult(testVector);
            Console.WriteLine($"Loss = {net.LossFunction(ideal)}");

            net.BackPropagation(ideal);

            actual = net.GetResult(testVector);
            Console.WriteLine($"Loss = {net.LossFunction(ideal)}");
        }

        private NNetData JpegToData(string path)
        {
            var splited = path.Split('\\');
            double ideal = double.Parse(splited[^2]);
            var idealAr = new double[2];
            idealAr[0] = ideal == 1 ? 1 : 0;
            idealAr[1] = ideal == 2 ? 1 : 0;
            return new NNetData
            {
                ideal = idealAr,
                picture = new Bitmap(Image.FromFile(path)),
            };
        }

        private double[,,] PrepareData(Bitmap bm, int sizeOfPic)
        {
            int kernel = 3;
            var bitmap = new Bitmap(bm, sizeOfPic, sizeOfPic).GetBWPicture();
            var smoothedBWPicture = bitmap.SmoothBWPicture(SmoothMatrixType.Simple, kernel);
            var gradients = smoothedBWPicture.FindGradients();
            var gradientsWithSuppressedMaximums = gradients.SuppressMaximums();
            var cuttedGradients = gradientsWithSuppressedMaximums.BlackEdge(kernel / 2 + 1);

            var doubleViewOfPicture = new double[1, sizeOfPic, sizeOfPic];
            for (int y = 0; y < sizeOfPic; y++)
            {
                for (int x = 0; x < sizeOfPic; x++)
                {
                    doubleViewOfPicture[0, y, x] = cuttedGradients[y, x].Length;
                }
            }

            return doubleViewOfPicture;
        }

        private List<NNetData> DirectoryToData(string path, int epochs)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            var res = new List<NNetData>(files.Length);
            foreach (var file in files)
            {
                if (epochs < 0)
                {
                    break;
                }

                epochs--;
                res.Add(JpegToData(file.FullName));
            }

            return res;
        }

        [TestMethod]
        public void tempTest()
        {
            int sizeOfPic = 124;
            int epochs = 100;

            var network = new CNNet(ActivationFunctionType.Sigmoid, sizeOfPic, 
                5, 1, 5, 
                2,
                5, 5, 5,
                2,
                3, 5, 5, 2,
                new int[] { 800, 300, 100},
                2);
            File.WriteAllText(pathToData + @"somaset\network_untrained.json", network.Save());
            //var network = new Perceptron(File.ReadAllText(pathToData + @"somaset\network_untrained.json"));
            string path = pathToData + @"somaset\TrainData";

            var trainData1 = DirectoryToData(path + "\\1", (int)((double)epochs /10 * 8 / 2));
            trainData1.AddRange(DirectoryToData(path + "\\2", (int)((double)epochs / 2)));
            trainData1 = trainData1.Shuffle();
            //var trainData2 = DirectoryToData(path + "\\2", (int)((double)epochs / 2));
            //var trainData = ListExtensions.OneByOne(trainData1, trainData2);

            var losses = Train(ref network, trainData1, epochs, sizeOfPic);

            File.WriteAllText(pathToData + @"somaset\network.json", network.Save());

            string pathTest = pathToData + @"somaset\TestData";
            var testData1 = DirectoryToData(pathTest + "\\1", (int)((double)epochs / 80 * 20) / 2);
            var testData2 = DirectoryToData(pathTest + "\\2", (int)((double)epochs / 80 * 20) / 2);
            int all = testData1.Count + testData2.Count;
            int success = 0;
            int tp = 0, fp = 0, tn = 0, fn = 0;
            for (int i = 0; i < testData1.Count; i++)
            {
                var testVector = PrepareData(testData1[i].picture, sizeOfPic);

                var output = network.GetResult(testVector);
                Console.WriteLine($"Expected ({testData1[i].ideal[0]}, {testData1[i].ideal[1]}), Actual: ({output[0]:f6}, {output[1]:f6}) , loss = {network.LossFunction(testData1[i].ideal)}");
                success += output[0] > output[1] ? 1 : 0;
                if (output[0] > output[1])
                {
                    tp++;
                }
                else
                {
                    fn++;
                }
            }

            for (int i = 0; i < testData2.Count; i++)
            {
                var testVector = PrepareData(testData2[i].picture, sizeOfPic);

                var output = network.GetResult(testVector);

                Console.WriteLine($"Expected ({testData2[i].ideal[0]}, {testData2[i].ideal[1]}), Actual: ({output[0]:f6}, {output[1]:f6}) , loss = {network.LossFunction(testData2[i].ideal)}");
                success += output[0] < output[1] ? 1 : 0;
                if (output[0] < output[1])
                {
                    tn++;
                }
                else
                {
                    fp++;
                }
            }

            Console.WriteLine($"Процент попадания: {(double)success / all * 100}");
            Console.WriteLine($"Правильно положительные = {tp}");
            Console.WriteLine($"Неправильно положительные = {fn}");
            Console.WriteLine($"Правильно отрицательные = {tn}");
            Console.WriteLine($"Неправильно отрицательные = {fp}");

        }

        private double[] Train(ref CNNet net, List<NNetData> trainingDatas, int epochs, int sizeOfPic)
        {
            double[] loss = new double[Math.Min(trainingDatas.Count, epochs)];
            for (int i = 0; i < trainingDatas.Count && i < epochs; i++)
            {
                var trainVector = PrepareData(trainingDatas[i].picture, sizeOfPic);

                var actual = net.GetResult(trainVector);

                loss[i] = net.LossFunction(trainingDatas[i].ideal);
                
                if ((actual[0] > actual[1] && trainingDatas[i].ideal[0] > trainingDatas[i].ideal[1]) || (actual[0] < actual[1] && trainingDatas[i].ideal[0] < trainingDatas[i].ideal[1]))
                { }
                else
                {
                    net.BackPropagation(trainingDatas[i].ideal);
                }

                var actual1 = net.GetResult(trainVector);
                Console.WriteLine($"{i}. Expected ({trainingDatas[i].ideal[0]}, {trainingDatas[i].ideal[1]}), Actual: ({actual[0]:f6}, {actual[1]:f6}), newActual: ({actual1[0]:f6}, {actual1[1]:f6}) , loss = {net.LossFunction(trainingDatas[i].ideal)}");
            }

            return loss;
        }

        private List<NNetData> DirectoryToDataForCnn(string path, int epochs)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            var res = new List<NNetData>(files.Length);
            foreach (var file in files)
            {
                if (epochs < 0)
                {
                    break;
                }

                epochs--;
                res.Add(JpegToData(file.FullName));
            }

            return res;
        }
    }
}
