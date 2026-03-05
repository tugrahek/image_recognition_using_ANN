using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proje
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            double learningRate = 0.001;
            int epochs = 40;
            NeuralNetwork network = new NeuralNetwork(learningRate, rnd);

            List<int[,]> listOnes = VariateMatrix(GenerateOneMatrix(),rnd);
            List<int[,]> listTwos = VariateMatrix(GenerateTwoMatrix(),rnd);
            Console.WriteLine("1:");
            PrintMatrices(listOnes);
            Console.WriteLine("2:");
            PrintMatrices(listTwos);

            List<(int[], double, double)> matricesList = new List<(int[], double, double)>();

            foreach (var matrix in listOnes)
            {
                matricesList.Add((ConvertMatrix(matrix), 1, 0));
            }

            foreach (var matrix in listTwos)
            {
                matricesList.Add((ConvertMatrix(matrix), 0, 1));
            }

            matricesList = matricesList.OrderBy(x => rnd.Next()).ToList();

            int splitIndex = (int)(matricesList.Count * 0.7);

            var trainSet = matricesList.Take(splitIndex).ToList();
            var testSet = matricesList.Skip(splitIndex).ToList();

            network.Train(trainSet, epochs);

            double accuracy = network.Test(testSet);
            Console.WriteLine("Accuracy: " + accuracy + "%");
        }
        static int[] ConvertMatrix(int[,] matrix)
        {
            int[] array = new int[matrix.GetLength(0) * matrix.GetLength(1)];
            int index = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    array[index++] = matrix[i, j];
                }
            }
            return array;
        }
        static int[,] GenerateOneMatrix()
        {
            int[,] matrix = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                matrix[i, 2] = 1;
                if (i == 1)
                {
                    matrix[i, 1] = 1;
                }
            }
            return matrix;

        }
        static int[,] GenerateTwoMatrix()
        {
            int[,] matrix = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                if (i == 4)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        matrix[i, j] = 1;
                    }
                }
                else if (i == 0)
                {
                    matrix[i, 1] = 1;
                    matrix[i, 2] = 1;
                    matrix[i, 3] = 1;
                }
                else if (i == 1)
                {
                    matrix[i, 0] = 1;
                    matrix[i, 4] = 1;
                }
                else if (i == 2)
                {
                    matrix[i, 3] = 1;
                }
                else if (i == 3)
                {
                    matrix[i, 2] = 1;
                }
            }
            return matrix;
        }
        static List<int[,]> VariateMatrix(int[,] baseMatrix,Random rnd)
        {
            List<int[,]> variedMatrices = new List<int[,]>();
            for (int i = 0; i < 10; i++)
            {
                int[,] variation = (int[,])baseMatrix.Clone();
                for (int j = 0; j < 5; j++)
                {
                    int row = rnd.Next(5);
                    int col = rnd.Next(5);
                    variation[row, col] = (variation[row, col] == 0) ? 1 : 0;
                }
                variedMatrices.Add(variation);
            }
            return variedMatrices;
        }
        static void PrintMatrices(List<int[,]> variedMatrices)
        {
            foreach (int[,] variation in variedMatrices)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Console.Write(variation[i, j] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

        }
        public class Neuron
        {
            private double[] weights;

            public Neuron(int inputNumber, Random rnd)
            {
                weights = new double[inputNumber];
                for (int i = 0; i < inputNumber; i++)
                {
                    weights[i] = rnd.NextDouble() * 2 - 1;//[-1,1] 
                }
            }
            public double Compute(int[] inputs)
            {
                double sum = 0;
                for (int i = 0; i < inputs.Length; i++)
                {
                    sum += inputs[i] * weights[i];
                }
                return sum;
            }


            public void UpdateWeights(int[] inputs, double learningRate, double targetValue, double currentValue)
            {

                double error = targetValue - currentValue;
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] += learningRate * inputs[i] * error;
                }
            }
        }
        public class NeuralNetwork
        {
            private Neuron neuron1;
            private Neuron neuron2;
            private double learningRate;
            public NeuralNetwork(double learningRate, Random rnd)
            {
                neuron1 = new Neuron(25, rnd);
                neuron2 = new Neuron(25, rnd);
                this.learningRate = learningRate;
            }
            public (double, double) Predict(int[] inputs)
            {
                double output1 = neuron1.Compute(inputs);
                double output2 = neuron2.Compute(inputs);
                return (output1, output2);
            }
            public void Train(List<(int[], double, double)> matricesList, int epochs)
            {
                for (int epoch = 0; epoch < epochs; epoch++)
                {
                    foreach (var (inputs, target1, target2) in matricesList)
                    {
                        var (output1, output2) = Predict(inputs);
                        neuron1.UpdateWeights(inputs, learningRate, target1, output1);
                        neuron2.UpdateWeights(inputs, learningRate, target2, output2);
                    }
                }
            }
            public double Test(List<(int[], double, double)> matricesList)
            {
                int correct = 0;

                foreach (var (inputs, target1, target2) in matricesList)
                {
                    var (output1, output2) = Predict(inputs);

                    Console.WriteLine("Target 1: " + target1 + " Target 2: " + target2);
                    Console.WriteLine("Output 1: " + output1 + " Output 2: " + output2);

                    bool isCorrect = (output1 > output2 && target1 == 1) || (output2 > output1 && target2 == 1);
                    Console.WriteLine("Prediction : " + isCorrect);

                    if (isCorrect)
                    {
                        correct++;
                    }

                    Console.WriteLine();
                }

                double accuracy = (double)correct / matricesList.Count * 100;
                return accuracy;
            }


        }

    }

}


