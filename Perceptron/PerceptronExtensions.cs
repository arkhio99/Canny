using System.IO;

namespace NeuralNetworks
{
    public static class PerceptronExtensions
    {
        public static void SaveAsFile(this Perceptron p, string path)
        {
            File.WriteAllText(path, p.Save());
        }

        public static Perceptron FromFile(string path)
        {
            return Perceptron.FromJson(File.ReadAllText(path));
        }
    }
}
