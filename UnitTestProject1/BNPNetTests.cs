using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNet;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System;
using BitmapLibrary;
using NeuralNetworks;

namespace NeuralNet.Tests
{
    public class NNetData
    {
        public double[] ideal;
        public Bitmap picture;
    }

    [TestClass]
    public class PerceptronTests
    {
        string pathToData = @"..\..\..\..\";
        private NNetData JpegToData(string path)
        {
            var splited = path.Split('\\');
            double ideal = double.Parse(splited[^2]);
            return new NNetData 
            { 
                ideal = new double[] 
                {
                    Math.Abs(ideal) > 0.001 ? ideal : 0.001,
                    Math.Abs(1 - ideal) > 0.001 ? 1 - ideal : 0.001,
                },
                picture = new Bitmap(Image.FromFile(path)),
            };
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
        public void CanSaveTest()
        {
            var network = new Perceptron(ActivationFunctionType.LeakyReLU,
                new int[] { 2, 3, 1 }, false, 0.8, 0.3);

            File.WriteAllText("saveblp.json", network.Save());
        }

        [TestMethod]
        public void CanGetResult()
        {
            var network = new Perceptron(ActivationFunctionType.Sigmoid,
                new int[] { 2, 3, 1 }, true, 0.8, 0.3);

            var input = new double[] { 0, 1 };

            File.WriteAllText("saveblp.json", network.Save());

            var expected = network.GetResult(input);

            network = Perceptron.FromJson(File.ReadAllText("saveblp.json"));
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
            Perceptron net = Perceptron.FromJson(File.ReadAllText("saveblp.json"));
            var input = Enumerable.Range(1, howInput).Select(n => (double)n).ToArray();
            var ideal = new double[] { 1.5 };
            while (cur <= last)
            {
                net = new Perceptron(ActivationFunctionType.Sigmoid,
                  new int[] { howInput, 50, 50, 50, 1 }, false, 0.8, 0.3);
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

            net = Perceptron.FromJson(File.ReadAllText("saveblp.json"));
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
            var network = new Perceptron(ActivationFunctionType.LeakyReLU, new int[] { sizeOfPic * sizeOfPic, 100, 100, 2 }, true, 0.8, 0.3);
            string path = pathToData + @"somaset\TrainData";
            int epochs = 400;

            var trainData = DirectoryToData(path + "\\1", epochs / 100 * 80 / 2);
            trainData.AddRange(DirectoryToData(path + "\\0", epochs / 100 * 80 / 2));
            trainData.Shuffle();
            
            var losses = Train(ref network, trainData, epochs);
            File.WriteAllText(pathToData + @"somaset\network.json", network.Save());
        }

        [TestMethod]
        public void TestNet()
        {
            int epochs = 200;
            var network = Perceptron.FromJson(File.ReadAllText(pathToData + @"somaset\Норм\network.json"));
            string pathTest = pathToData + @"somaset\TestData";
            var testData1 = DirectoryToData(pathTest + "\\1", epochs / 80 * 20 / 2).ToList();
            var testData0 = DirectoryToData(pathTest + "\\0", epochs / 80 * 20 / 2).ToList();
            
            int all = testData1.Count + testData0.Count;
            int success = 0;
            int tp = 0, fp = 0, tn = 0, fn = 0;
            for (int i = 0; i < testData1.Count; i++)
            {
                var testVector = PrepareData(testData1[i].picture);

                var output = network.GetResult(testVector);
                Console.WriteLine($"{i}.\tloss{testData1[i].ideal[0]} = " + network.LossFunction(testData1[i].ideal));
                success += Math.Abs(output[0] - testData1[i].ideal[0]) < 0.2 ? 1 : 0;
                if (Math.Abs(output[0] - 1) < 0.2)
                {
                    tp++;
                }
                else
                {
                    fn++;
                }
            }

            for (int i = 0; i < testData0.Count; i++)
            {
                var testVector = PrepareData(testData0[i].picture);

                var output = network.GetResult(testVector);

                Console.WriteLine($"{i}.\tloss{testData0[i].ideal[0]} = " + network.LossFunction(testData0[i].ideal));
                success += Math.Abs(output[0] - testData0[i].ideal[0]) < 0.2 ? 1 : 0;
                if (Math.Abs(output[0]) < 0.2)
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

        [TestMethod]
        public void tempTest()
        {
            int sizeOfPic = 64;
            var network = new Perceptron(ActivationFunctionType.Sigmoid, new int[] { sizeOfPic * sizeOfPic, 100, 100, 100, 2 }, false, 0.8, 0.3);
            File.WriteAllText(pathToData + @"somaset\network_untrained.json", network.Save());
            //var network = new Perceptron(File.ReadAllText(pathToData + @"somaset\network_untrained.json"));
            string path = pathToData + @"somaset\TrainData";
            int epochs = 100;

            var trainData = DirectoryToData(path + "\\1", epochs / 2).ToList();
            trainData.AddRange(DirectoryToData(path + "\\0", epochs / 2).ToList());
            trainData = trainData.Shuffle();

            var losses = Train(ref network, trainData, epochs);
            Console.WriteLine($"Train losses:");
            for (int i = 0; i < losses.Length; i++)
            {
                Console.WriteLine($"{i}.\t train{trainData[i].ideal[0]}\t = {losses[i]}");
            }
            Console.WriteLine();

            File.WriteAllText(pathToData + @"somaset\network.json", network.Save());

            string pathTest = pathToData + @"somaset\TestData";
            var testData1 = DirectoryToData(pathTest + "\\1", epochs / 80 * 20 / 2).ToList();
            var testData0 = DirectoryToData(pathTest + "\\0", epochs / 80 * 20 / 2).ToList();
            int all = testData1.Count + testData0.Count;
            int success = 0;
            int tp = 0, fp = 0, tn = 0, fn = 0;
            for (int i = 0; i < testData1.Count; i++)
            {
                var testVector = PrepareData(testData1[i].picture);

                var output = network.GetResult(testVector);
                Console.WriteLine("loss = " + network.LossFunction(testData1[i].ideal));
                success += Math.Abs(output[0] - testData1[i].ideal[0]) < 0.2 ? 1 : 0;
                if (Math.Abs(output[0] - testData1[i].ideal[0]) < 0.2)
                {
                    tp++;
                }
                else
                {
                    fn++;
                }
            }

            for (int i = 0; i < testData0.Count; i++)
            {
                var testVector = PrepareData(testData0[i].picture);

                var output = network.GetResult(testVector);

                Console.WriteLine("loss = " + network.LossFunction(testData0[i].ideal));
                success += Math.Abs(output[0] - testData0[i].ideal[0]) < 0.2 ? 1 : 0;
                if (Math.Abs(output[0] - testData0[i].ideal[0]) < 0.2)
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

        private double[] Train(ref Perceptron net, List<NNetData> trainingDatas, int epochs)
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
    }
}
