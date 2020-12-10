using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.Tests
{
    public static class AssertExtensions
    {
        public static void AreEqual(this Assert t, double[,,] expected, double[,,] actual)
        {
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

        public static void AreEqual(this Assert t, double[] expected, double[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.GetLength(0); i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.1);
            }
        }
    }
}
