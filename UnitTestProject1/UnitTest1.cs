using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNN;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void HowInputsTest()
        {
            Assert.AreEqual(CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
                new List<LayerType>
                {
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                }), 1323);
        }

        [TestMethod]
        public void Test()
        {

            var cnn = new CNNet(ActivationFunc.LeakyReLU,
                new int[] { 70, 70 },
                3,
                3,
                CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
                new List<LayerType>
                {
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                }),
                1,
                2);
            

            cnn.Save("savecnn1.json");
        }

        [TestMethod]
        public void ConvTest()
        {
            var cnn = new CNNet(ActivationFunc.LeakyReLU,
                new int[] { 70, 70 },
                3,
                3,
                CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
                new List<LayerType>
                {
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                }),
                1,
                2);
            double[,] inp = new double[,]
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, 16},
            };
            double[,] filter = new double[,]
            {
                {1, 0, 1},
                { 0, 1, 0},
                {1, 0, 1},
            };
            double[,] expected = new double[,]
            {
                { 30, 35 },
                { 50, 55 },
            };

            var actual = cnn.Convolute(inp, filter);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Assert.AreEqual(expected[i,j], actual[i,j], 0.1);
                }
            }
        }

        [TestMethod]
        public void PoolTest()
        {
            var cnn = new CNNet(ActivationFunc.LeakyReLU,
                new int[] { 70, 70 },
                3,
                3,
                CNNet.GetNumberOfInputsOnFullyConnected(new double[64, 64], 3, 3, 2,
                new List<LayerType>
                {
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                    LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,LayerType.Convolution,
                    LayerType.Activation,
                    LayerType.Pool,
                }),
                1,
                2);
            double[,] inp = new double[,]
            {
                {1, 2, 3, 4, 5},
                {5, 6, 7, 8, 9},
                {9, 10, 11, 12, 13},
                {13, 14, 15, 16, 17},
                {16, 17, 18, 19, 20 },
            };

            var expected = new double[,]
            {
                { 6, 8, 9},
                { 14, 16, 17},
                { 17, 19, 20},
            };

            var actual = cnn.Pool(inp);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(expected[i,j], actual[i,j], 0.1);
                }
            }
        }

        [TestMethod]
        public void PercpTest()
        {
            var cnn1 = new CNNet(ActivationFunc.LeakyReLU,
                new int[] { 2, 2 },
                1,
                1,
                2,
                1,
                1);
            cnn1.Save("savecnn1.json");
            double[] inp = new double[] { 1, 2 };
            var expected = cnn1.ProceessOnPerceptron(inp);
            var cnn2 = new CNNet("savecnn1.json");
            var actual = cnn2.ProceessOnPerceptron(inp);
            foreach (var val in actual)
            {
                System.Console.WriteLine(val);
            }
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.1);
            }

        }

        [TestMethod]
        public void Deltas()
        {
            var input = new double[] { 0, 1 };
            var ideal = new double[] { 1.5 };
            //var cnn = new CNNet("savecnn.json");
            var cnn = new CNNet(ActivationFunc.LeakyReLU,
                new int[] { 2, 2 },
                1,
                1,
                2,
                1,
                1);
            var output = cnn.ProceessOnPerceptron(input);
            var loss = cnn.LossFunction(ideal);
            System.Console.WriteLine(loss);
            cnn.BackPropagation(input, ideal);
            output = cnn.ProceessOnPerceptron(input);
            loss = cnn.LossFunction(ideal);
            System.Console.WriteLine(loss);
            cnn.BackPropagation(input, ideal);
            output = cnn.ProceessOnPerceptron(input);
            loss = cnn.LossFunction(ideal);
            System.Console.WriteLine(loss);
        }
    }
}
