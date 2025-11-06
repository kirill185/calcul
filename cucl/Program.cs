using Calculator;
using System;


namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            IMathOperation calc1 = new MathOperation();
            IMathHard cacl2 = new MathOperation();
            while (true)
            {
                Console.Write("Введите первое число: ");
                double number1 = Convert.ToSingle(Console.ReadLine());
                Console.Write("Введите Второе число: ");
                double number2 = Convert.ToSingle(Console.ReadLine());
                Console.Write("Выберите действие: ");
                string bounc = Console.ReadLine();
                switch (bounc) {

                    case "+":  
                        Console.WriteLine("Сумма ваших чисел: " + calc1.add(number1, number2));
                        break;
                    case "-": 
                        Console.WriteLine("Вычетание чисел: " + calc1.subtract(number1, number2));
                        break;
                    case "*":
                        Console.WriteLine("Умножение: " + calc1.multiply(number1, number2));
                        break;
                    case "/":
                        Console.WriteLine("Деление: " + calc1.divide(number1, number2));
                        break;
                    case "SquareRoot":
                        Console.WriteLine("Квадратный корень: " + cacl2.SquareRoot(number1));
                        break;
                    case "Sine":
                        Console.WriteLine("Синус: " + cacl2.sine(number1));
                        break;
                    case "Cosine":
                        Console.WriteLine("Косинус: " + cacl2.cosine(number1));
                        break;
                    case "Square":
                        Console.WriteLine("Квадрат числа: " + cacl2.Square(number1));
                        break;

                }
                Console.WriteLine("Выйти Q, Продолжить любая клавиша: ");
                string quet = Console.ReadLine();

                if (quet.Trim().ToLower() == "q" ) {
                    
                    break;
                }


            }




        }
    }
}
