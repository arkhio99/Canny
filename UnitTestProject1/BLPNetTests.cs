using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNN;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
        [TestMethod]
        public void CanSaveTest()
        {
            var network = new BLPNet(ActivationFuncType.LeakyReLU,
                new int[] { 2, 2, 1 });

            network.Save("saveblp.json");
        }

        [TestMethod]
        public void CanGetResult()
        {
            var network = new BLPNet(ActivationFuncType.LeakyReLU,
                new int[] { 2, 2, 1 });

            var input = new double[] { 1, 2 };

            network.Save("saveblp.json");
            var expected = network.GetResult(input);

            network = new BLPNet("saveblp.json");
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
            BLPNet net = new BLPNet("saveblp.json");
            var input = Enumerable.Range(1, howInput).Select(n => (double)n).ToArray();
            var ideal = new double[] { 1.5 };
            while (cur <= last)
            {
                net = new BLPNet(ActivationFuncType.Sigmoid,
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
            net.Save("saveblp.net");

            net = new BLPNet("saveblp.net");
            net.GetResult(input);
            cur = net.LossFunction(ideal);
            System.Console.WriteLine(cur);
            net.BackPropagation(ideal);
            net.GetResult(input);
            last = cur;
            cur = net.LossFunction(ideal);
            System.Console.WriteLine(net.LossFunction(ideal));
        }
    }
}
