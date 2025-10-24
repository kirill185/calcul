using System;
using System.Drawing;

class Program
{
    static void Main()
    {
        IMathOperation[] operations = {
            new SquareRoot(),
            new Sine(),
            new Cosine(),
            new Tangent(),
            new Square()
        };

        Console.WriteLine("КАЛЬКУЛЯТОР");

        while (true)
        {
            Console.Write("\nЧисло: ");
            string input = Console.ReadLine();

            if (input == "q") break;

            if (double.TryParse(input, out double number))
            {
                Console.WriteLine($"--- {number} ---");

                foreach (var operation in operations)
                {
                    double result = operation.Calculate(number);
                    string output = double.IsNaN(result) ? "ошибка" : $"{result:0.####}";
                    Console.WriteLine($"{operation.GetName()}: {output}");
                }
            }
            else
            {
                Console.WriteLine("Ошибка ввода");
            }
        }
    }
}