using Calculator;
using Cucl;
using System;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            IMathOperation calc1 = new MathOperation();
           
      
  

            Console.Write("Введите первое число: ");
            double number1 = Convert.ToSingle(Console.ReadLine());
            Console.Write("Введите Второе число: ");
            double number2 = Convert.ToSingle(Console.ReadLine());


            Console.WriteLine( "Сумма ваших чисел: " + calc1.add(number1, number2));
            Console.WriteLine( "Вычетание чисел: " + calc1.subtract(number1, number2));
            Console.WriteLine( "Вычетание чисел: " + calc1.multiply(number1, number2));
            Console.WriteLine( "Вычетание чисел: " + calc1.divide(number1, number2));


        }
    }
}