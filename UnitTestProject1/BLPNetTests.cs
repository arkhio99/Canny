using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNet;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System;
using BitmapLibrary;

namespace UnitTestProject1
{
    //[TestClass]
    //public class CNNetTests
    //{
    //    private NNetData JpegToData(string path)
    //    {
    //        var splited = path.Split('\\');
    //        return new NNetData { ideal = new double[] { double.Parse(splited[^2]) }, picture = new Bitmap(Image.FromFile(path)) };
    //    }

    //    private List<NNetData> DirectoryToData(string path)
    //    {
    //        var dir = new DirectoryInfo(path);
    //        var files = dir.GetFiles();
    //        var res = new List<NNetData>(files.Length);
    //        foreach (var file in files)
    //        {
    //            res.Add(JpegToData(file.FullName));
    //        }

    //        return res;
    //    }

    //    [TestMethod]
    //    public void HowInputsTest()
    //    {
    //        Assert.AreEqual(CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
    //            new List<LayerType>
    //            {
    //                LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,
    //                LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,
    //            }), 1323);
    //    }

    //    [TestMethod]
    //    public void Test()
    //    {

    //        var cnn = new CNNet(ActivationFuncType.LeakyReLU,
    //            new int[] { 10, 10, 10 },
    //            1,
    //            1,
    //            32 * 32,
    //            1,
    //            1);


    //        cnn.Save("test.json");
    //    }

    //    [TestMethod]
    //    public void ConvTest()
    //    {
    //        var cnn = new CNNet(ActivationFuncType.LeakyReLU,
    //            new int[] { 70, 70 },
    //            3,
    //            3,
    //            CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
    //            new List<LayerType>
    //            {
    //                LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,
    //                LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,
    //            }),
    //            1,
    //            2);
    //        double[,] inp = new double[,]
    //        {
    //            {1, 2, 3, 4},
    //            {5, 6, 7, 8},
    //            {9, 10, 11, 12},
    //            {13, 14, 15, 16},
    //        };
    //        double[,] filter = new double[,]
    //        {
    //            {1, 0, 1},
    //            { 0, 1, 0},
    //            {1, 0, 1},
    //        };
    //        double[,] expected = new double[,]
    //        {
    //            { 30, 35 },
    //            { 50, 55 },
    //        };

    //        var actual = cnn.Convolute(inp, filter);

    //        for (int i = 0; i < 2; i++)
    //        {
    //            for (int j = 0; j < 2; j++)
    //            {
    //                Assert.AreEqual(expected[i,j], actual[i,j], 0.1);
    //            }
    //        }
    //    }

    //    [TestMethod]
    //    public void PoolTest()
    //    {
    //        var cnn = new CNNet(ActivationFuncType.LeakyReLU,
    //            new int[] { 70, 70 },
    //            3,
    //            3,
    //            CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
    //            new List<LayerType>
    //            {
    //                LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,
    //                LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,LayerType.Convolution,
    //                LayerType.Activation,
    //                LayerType.Pool,
    //            }),
    //            1,
    //            2);
    //        double[,] inp = new double[,]
    //        {
    //            {1, 2, 3, 4, 5},
    //            {5, 6, 7, 8, 9},
    //            {9, 10, 11, 12, 13},
    //            {13, 14, 15, 16, 17},
    //            {16, 17, 18, 19, 20 },
    //        };

    //        var expected = new double[,]
    //        {
    //            { 6, 8, 9},
    //            { 14, 16, 17},
    //            { 17, 19, 20},
    //        };

    //        var actual = cnn.Pool(inp);

    //        for (int i = 0; i < 3; i++)
    //        {
    //            for (int j = 0; j < 3; j++)
    //            {
    //                Assert.AreEqual(expected[i,j], actual[i,j], 0.1);
    //            }
    //        }
    //    }

    //    [TestMethod]
    //    public void PercpTest()
    //    {
    //        var cnn1 = new CNNet(ActivationFuncType.LeakyReLU,
    //            new int[] { 2, 2 },
    //            1,
    //            1,
    //            2,
    //            1,
    //            1);
    //        cnn1.Save("savecnn1.json");
    //        double[] inp = new double[] { 1, 2 };
    //        var expected = cnn1.ProceessOnPerceptron(inp);
    //        var cnn2 = new CNNet("savecnn1.json");
    //        var actual = cnn2.ProceessOnPerceptron(inp);
    //        foreach (var val in actual)
    //        {
    //            System.Console.WriteLine(val);
    //        }
    //        for (int i = 0; i < expected.Length; i++)
    //        {
    //            Assert.AreEqual(expected[i], actual[i], 0.1);
    //        }

    //    }

    //    [TestMethod]
    //    public void Deltas()
    //    {
    //        int howInp = 2;
    //        double cur;
    //        double last;

    //        while (true)
    //        {
    //            cur = 0;
    //            last = 0;
    //            var input1 = Enumerable.Range(1, howInp).Select((i) => (double)i).ToArray();
    //            var ideal1 = new double[] { 1.5 };
    //            //var cnn = new CNNet("badresult.json");
    //            var cnn1 = new CNNet(ActivationFuncType.LeakyReLU,
    //                new int[] { 1 },
    //                1,
    //                1,
    //                howInp,
    //                1,
    //                1);
    //            cnn1.Save("badresult.json");
    //            var output1 = cnn1.ProceessOnPerceptron(input1);
    //            var loss1 = cnn1.LossFunction(ideal1);
    //            last = loss1;
    //            cur = loss1;
    //            cnn1.BackPropagation(input1, ideal1);
    //            output1 = cnn1.ProceessOnPerceptron(input1);
    //            loss1 = cnn1.LossFunction(ideal1);
    //            last = cur;
    //            cur = loss1;
    //            if (cur > last)
    //            {
    //                break;
    //            }
    //            //System.Console.WriteLine(loss1);
    //            cnn1.BackPropagation(input1, ideal1);
    //            output1 = cnn1.ProceessOnPerceptron(input1);
    //            loss1 = cnn1.LossFunction(ideal1);
    //            last = cur;
    //            cur = loss1;
    //            if (cur > last)
    //            {
    //                break;
    //            }
    //            //System.Console.WriteLine(loss1);
    //        }

    //        var input = Enumerable.Range(1, howInp).Select((i) => (double)i).ToArray();
    //        var ideal = new double[] { 1.5 };
    //        var cnn = new CNNet("badresult.json");
    //        var output = cnn.ProceessOnPerceptron(input);
    //        var loss = cnn.LossFunction(ideal);
    //        System.Console.WriteLine(loss);
    //        cnn.BackPropagation(input, ideal);
    //        output = cnn.ProceessOnPerceptron(input);
    //        loss = cnn.LossFunction(ideal);
    //        System.Console.WriteLine(loss);
    //        cnn.BackPropagation(input, ideal);
    //        output = cnn.ProceessOnPerceptron(input);
    //        loss = cnn.LossFunction(ideal);
    //        System.Console.WriteLine(loss);
    //    }

    //    [TestMethod]
    //    public void TrainNet()
    //    {
    //        var cnn = new CNNet("test.json");
    //        string path = @"C:\Users\vladb\Desktop\somaset\TrainData";
    //        var trainData1 = DirectoryToData(path + "\\1");
    //        var losses = cnn.Train(trainData1, BitmapLibrary.SmoothMatrixType.Simple, 3, 10);
    //        System.Console.WriteLine($"first = {losses[0]}\t, last = {losses[^1]}");
    //        var trainData0 = DirectoryToData(path + "\\0");
    //        losses = cnn.Train(trainData1, BitmapLibrary.SmoothMatrixType.Simple, 3, 10);
    //        System.Console.WriteLine($"first = {losses[0]}\t, last = {losses[^1]}");
    //    }
    //}

    [TestClass]
    public class BLPNetTests
    {
        string pathToData =  @"C:\Users\vladb\Desktop\";
        private NNetData JpegToData(string path)
        {
            var splited = path.Split('\\');
            double ideal = double.Parse(splited[^2]);
            return new NNetData 
            { 
                ideal = new double[] 
                { 
                    Math.Abs(ideal) > 0.001 ? ideal : 0.001,
                    //Math.Abs(1 - ideal) > 0.001 ? 1 - ideal : 0.001,
                },
                picture = new Bitmap(Image.FromFile(path)),
            };
        }

        private List<NNetData> DirectoryToData(string path)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            var res = new List<NNetData>(files.Length);
            foreach (var file in files)
            {
                res.Add(JpegToData(file.FullName));
            }

            return res;
        }

        [TestMethod]
        public void CanSaveTest()
        {
            var network = new BNPNet(ActivationFuncType.LeakyReLU,
                new int[] { 2, 3, 1 });

            File.WriteAllText("saveblp.json", network.Save());
        }

        [TestMethod]
        public void CanGetResult()
        {
            var network = new BNPNet(ActivationFuncType.Sigmoid,
                new int[] { 2, 3, 1 }, true);

            var input = new double[] { 0, 1 };

            File.WriteAllText("saveblp.json", network.Save());

            var expected = network.GetResult(input);

            network = new BNPNet(File.ReadAllText("saveblp.json"));
            var actual = network.GetResult(input);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.1);
            }
        }

        [TestMethod]
        public void CanBackProp()
        {
            int howInput = 32 * 32;

            double cur = 0;
            double last = 0;
            BNPNet net = new BNPNet(File.ReadAllText("saveblp.json"));
            var input = Enumerable.Range(1, howInput).Select(n => (double)n).ToArray();
            var ideal = new double[] { 1.5 };
            while (cur <= last)
            {
                net = new BNPNet(ActivationFuncType.Sigmoid,
                  new int[] { howInput, 50, 50, 50, 1 });
                net.GetResult(input);
                cur = net.LossFunction(ideal);
                System.Console.WriteLine(cur);
                net.BackPropagation(ideal);
                net.GetResult(input);
                last = cur;
                cur = net.LossFunction(ideal);
                System.Console.WriteLine(cur);
            }
            File.WriteAllText("saveblp.net", net.Save());

            net = new BNPNet(File.ReadAllText("saveblp.json"));
            net.GetResult(input);
            cur = net.LossFunction(ideal);
            System.Console.WriteLine(cur);
            net.BackPropagation(ideal);
            net.GetResult(input);
            last = cur;
            cur = net.LossFunction(ideal);
            System.Console.WriteLine(net.LossFunction(ideal));
        }

        [TestMethod]
        public void TrainNet()
        {
            int sizeOfPic = 64;
            var network = new BNPNet(ActivationFuncType.LeakyReLU, new int[] { sizeOfPic * sizeOfPic, 100, 100, 2 }, true);
            string path = pathToData + @"somaset\TrainData";
            int epochs = 400;

            var trainData = DirectoryToData(path + "\\1");
            trainData.AddRange(DirectoryToData(path + "\\0"));
            trainData.Shuffle();
            
            var losses = Train(network, trainData, epochs);
            File.WriteAllText(pathToData + @"somaset\network.json", network.Save());
        }

        [TestMethod]
        public void TestNet()
        {
            int sizeOfPic = 64;
            var network = new BNPNet(File.ReadAllText(pathToData + @"somaset\network.json"));
            string pathTest = pathToData + @"somaset\TestData";
            var testData1 = DirectoryToData(pathTest + "\\1");
            var testData0 = DirectoryToData(pathTest + "\\0");
            int all = testData1.Count + testData0.Count;
            int success = 0;
            for (int i = 0; i < testData1.Count; i++)
            {
                var pic = new Bitmap(testData1[i].picture, sizeOfPic, sizeOfPic);
                double[,] doublePic = new double[pic.Height, pic.Width];
                for (int y = 0; y < doublePic.GetLength(0); y++)
                {
                    for (int x = 0; x < doublePic.GetLength(1); x++)
                    {
                        doublePic[y, x] = testData1[i].picture.GetPixel(x, y).R;
                    }
                }

                var output = network.GetResult(doublePic.ToVector());
                Console.WriteLine("loss = " + network.LossFunction(testData1[i].ideal));
                success += Math.Abs(output[0] - testData1[i].ideal[0]) < 0.2 ? 1 : 0;
            }

            for (int i = 0; i < testData0.Count; i++)
            {
                var pic = new Bitmap(testData0[i].picture, sizeOfPic, sizeOfPic);
                double[,] doublePic = new double[pic.Height, pic.Width];
                for (int y = 0; y < doublePic.GetLength(0); y++)
                {
                    for (int x = 0; x < doublePic.GetLength(1); x++)
                    {
                        doublePic[y, x] = testData0[i].picture.GetPixel(x, y).R;
                    }
                }

                var output = network.GetResult(doublePic.ToVector());

                Console.WriteLine("loss = " + network.LossFunction(testData0[i].ideal));
                success += Math.Abs(output[0] - testData0[i].ideal[0]) < 0.2 ? 1 : 0;
            }

            Console.WriteLine($"Процент попадания: {(double)success / all * 100}");
        }

        [TestMethod]
        public void tempTest()
        {
            int sizeOfPic = 64;
            var network = new BNPNet(ActivationFuncType.LeakyReLU, new int[] { sizeOfPic * sizeOfPic, 100, 100, 1 }, false);
            File.WriteAllText(pathToData + @"somaset\network_untrained.json", network.Save());
            //var network = new BNPNet(File.ReadAllText(pathToData + @"somaset\network_untrained.json"));
            string path = pathToData + @"somaset\TrainData";
            int epochs = 100;

            var trainData = DirectoryToData(path + "\\1").Take(epochs / 2).ToList();
            trainData.AddRange(DirectoryToData(path + "\\0").Take(epochs / 2).ToList());
            //trainData = trainData.Shuffle();

            var losses = Train(network, trainData, epochs);
            Console.WriteLine($"Train losses:");
            for (int i = 0; i < losses.Length; i++)
            {
                Console.WriteLine($"{i}.\t train{trainData[i].ideal[0]}\t\t = {losses[i]}");
            }
            Console.WriteLine();

            File.WriteAllText(pathToData + @"somaset\network.json", network.Save());

            string pathTest = pathToData + @"somaset\TestData";
            var testData1 = DirectoryToData(pathTest + "\\1");
            var testData0 = DirectoryToData(pathTest + "\\0");
            int all = testData1.Count + testData0.Count;
            int success = 0;
            for (int i = 0; i < testData1.Count; i++)
            {
                var testVector = PrepareData(testData1[i].picture);

                var output = network.GetResult(testVector);
                Console.WriteLine("loss = " + network.LossFunction(testData1[i].ideal));
                success += Math.Abs(output[0] - testData1[i].ideal[0]) < 0.2 ? 1 : 0;
            }

            for (int i = 0; i < testData0.Count; i++)
            {
                var testVector = PrepareData(testData0[i].picture);

                var output = network.GetResult(testVector);

                Console.WriteLine("loss = " + network.LossFunction(testData0[i].ideal));
                success += Math.Abs(output[0] - testData0[i].ideal[0]) < 0.2 ? 1 : 0;
            }

            Console.WriteLine($"Процент попадания: {(double)success / all * 100}");
        }

        private double[] Train(BNPNet net, List<NNetData> trainingDatas, int epochs)
        {
            double[] loss = new double[Math.Min(trainingDatas.Count, epochs)];
            for (int i = 0; i < trainingDatas.Count && i < epochs; i++)
            {
                var trainVector = PrepareData(trainingDatas[i].picture);                

                net.GetResult(trainVector);

                loss[i] = net.LossFunction(trainingDatas[i].ideal);
                if (loss[i] > 0.1)
                {
                    net.BackPropagation(trainingDatas[i].ideal);
                }
            }

            return loss;
        }

        private double[] PrepareData(Bitmap bm)
        {
            int sizeOfPic = 64;
            int kernel = 3;
            var bitmap = new Bitmap(bm, sizeOfPic, sizeOfPic).GetBWPicture();
            var smoothedBWPicture = bitmap.SmoothBWPicture(SmoothMatrixType.Simple, kernel);
            var gradients = smoothedBWPicture.FindGradients();
            var gradientsWithSuppressedMaximums = gradients.SuppressMaximums();
            var cuttedGradients = gradientsWithSuppressedMaximums.BlackEdge(kernel / 2 + 1);
            
            var doubleViewOfPicture = new double[sizeOfPic, sizeOfPic];
            for (int y = 0; y < sizeOfPic; y++)
            {
                for (int x = 0; x < sizeOfPic; x++)
                {
                    doubleViewOfPicture[y, x] = cuttedGradients[y, x].Length;
                }
            }

            return doubleViewOfPicture.ToVector();
        }
    }
}
