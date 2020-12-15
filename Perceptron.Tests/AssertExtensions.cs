using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetworks.Tests
{
    public static class AssertExtensions
    {
        public static void AreEqual(this Assert a, double[] expected, double[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.001);
            };
        }
    }
}
