using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NeuralNetworks.Tests
{
    [TestClass]
    public class PerceptronTests
    {
        string path = @"Networks";

        [TestMethod]
        public void Perceptron_IsCorrectSave_Tests()
        {
            Perceptron p = new Perceptron(
                ActivationFunctionType.Sigmoid,
                new int[] {2, 2, 2},
                false,
                0.7,
                0.3);

            double[] input = new double[] { 1, 2 };

            double[] expected = p.GetResult(input);

            p.SaveAsFile(path + @"\Net.json");

            var newP = PerceptronExtensions.FromFile(path + @"\Net.json");

            double[] actual = newP.GetResult(input);

            Assert.That.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Perceptron_CanBackProp_Test()
        {
            int howInputs = 60;
            int howOnHidden = 60;

            Perceptron p = new Perceptron(
                ActivationFunctionType.Sigmoid,
                new int[] {howInputs, howOnHidden, 1},
                false,
                0.7,
                0.3);

            double[] input = Enumerable.Range(1, howInputs).Select(i => (double)i).ToArray();
            double[] ideal = new double[] { 0.6 };

            double[] first = p.GetResult(input);

            System.Console.WriteLine(p.LossFunction(ideal));

            p.BackPropagation(ideal);

            double[] second = p.GetResult(input);

            System.Console.WriteLine(p.LossFunction(ideal));

            p.BackPropagation(ideal);

            double[] third = p.GetResult(input);

            System.Console.WriteLine(p.LossFunction(ideal));
        }
    }
}
