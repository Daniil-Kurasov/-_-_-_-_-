using System;
using System.Linq;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        double[,] A = new double[0,0]; double[] B = Array.Empty<double>(); double e = 0;
        if (ReadFromFile(ref A, ref B, ref e))
        {
            Console.WriteLine("Исходная матрица\n");
            Output_A(A);
            Output_B(B, e);
            Console.WriteLine();
            A = DiagToZero(A, B, out double[] b_diag);
            Console.WriteLine("\nГлавная диагональ матрицы на 0\n");
            Output_A(A);
            Console.WriteLine("\nНулевое приближение\n");
            Output(b_diag);
            Console.WriteLine();
            Algorithm(A, b_diag, e);
            Console.WriteLine();
        }
        Console.ReadKey();
    }

    private static bool ReadFromFile(ref double[,] A, ref double[] B, ref double e)
    {
        try
        {
            string[] lines = File.ReadAllLines("input.txt").ToArray();
            int exception = lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray().Length - 1;
            if (exception != lines.Length - 1)
                throw new Exception();
            if (exception + lines.Length - 1 <= 4)
                throw new Exception();
            e = double.Parse(lines.LastOrDefault());
            A = new double[lines.Length - 1, lines.Length - 1];
            B = new double[lines.Length - 1];
            for (int i = 0; i < lines.Length - 1; i++)
            {
                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();
                for (int j = 0; j < lines.Length - 1; j++)
                {
                    if (i == j && row[j] is 0)
                        throw new Exception();
                    A[i, j] = row[j];
                }
                B[i] = row[lines.Length - 1];
            }
            return true;
        }
        catch
        {
            Console.WriteLine("Неверный ввод данных в текстовый файл");
        }
        return false;
    }

    private static double[,] DiagToZero(double[,] A, double[] B, out double[] b_diag)
    {
        double[,] result = new double[A.GetLength(0), A.GetLength(1)];
        b_diag = new double[B.GetLength(0)];
        for (int i = 0; i < A.GetLength(0); i++)
        {
            b_diag[i] = B[i] / A[i, i];
            for (int j = 0; j < A.GetLength(1); j++)
            {
                if (i != j)
                    result[i, j] = -(A[i, j] / A[i, i]);
                else
                    result[i, j] = 0;
            }
        }
        return result;
    }
    private static void Algorithm(double[,] A, double[] b_diag, double e)
    {
        int count;
        double[] x = new double[b_diag.GetLength(0)], x0 = new double[b_diag.GetLength(0)];
        double sum = 0;
        for (int root = 0; root < b_diag.GetLength(0); root++)
        {
            count = 0;
            b_diag.CopyTo(x0, 0);
            //Первое приближение
            Console.WriteLine($"Последующие приближения для X{root+1}:\n");
            x = Approximation(A, x, x0, b_diag, sum);
            Console.WriteLine($"{++count}) {x[root]}");
            while (Math.Abs(x[root] - x0[root]) >= e && count < 100)
            {
                x.CopyTo(x0, 0);
                //Последующие приближения
                x = Approximation(A, x, x0, b_diag, sum);
                Console.WriteLine($"{++count}) {x[root]}");
            }
            Console.WriteLine($"\n---------\nX{root+1} = {x[root]}\nКоличество = {count}\n---------\n");
        }
    }

    private static double[] Approximation(double[,] A, double[] x, double[] x0, double[] b_diag, double sum)
    {
        for (int i = 0; i < A.GetLength(0); i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sum = A[i, j] * x0[j] + sum;
            }
            x[i] = b_diag[i] + sum;
            sum = 0;
        }
        return x;
    }

    private static void Output_A(double[,] A)
    {
        Console.WriteLine("\t----- A -----");
        for (int i = 0; i < A.GetLength(0); i++)
        {
            for (int j = 0; j < A.GetLength(1); j++)
            {
                Console.Write("\t");
                Console.Write(A[i, j]);
            }
            Console.WriteLine();
        }
    }
    private static void Output_B(double[] B, double e)
    {
        Console.WriteLine("\n\t--- B ---");
        for (int i = 0; i < B.GetLength(0); i++)
        {
            Console.Write("\t");
            Console.WriteLine(B[i]);
        }

        Console.WriteLine($"\nТочность eps = {e}");
    }

    private static void Output(double[] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            Console.WriteLine($"X{i+1} = {matrix[i]}");
    }
}