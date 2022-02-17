using System;
using System.Diagnostics;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {

            double acumulado = 1;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 1; i < 1000; i++)
            {
                acumulado += i * 0.1;
                Console.WriteLine("iteracion");
            }
            stopwatch.Stop();

            Console.WriteLine($"Milisegundos: {stopwatch.Elapsed.TotalMilliseconds}");
            Console.WriteLine($"Ticks: {stopwatch.Elapsed.Ticks}");
            Console.Read();

        }
    }
}
