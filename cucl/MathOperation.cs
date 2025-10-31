using Calculator;
///Богатов

namespace Cucl
{
    public class MathOperation : IMathOperation, IMathHard
    {
        public double add(double number1, double number2) { return number1 + number2; }
        public double subtract(double number1, double number2) { return number1 - number2; }
        public double multiply(double number1, double number2) { return number1 * number2; }
        public double divide(double number1, double number2) { return number1 / number2; }
        public double SquareRoot(double number1)
        {
            if (number1 < 0) return double.NaN;
            double result = number1;
            for (int i = 0; i < 10; i++) result = (result + number1 / result) / 2;
            return result;
        }
        public double sine(double number1)
        {
            while (number1 > 3.14159) number1 -= 2 * 3.14159;
            while (number1 < -3.14159) number1 += 2 * 3.14159;
            double result = 0, term = number1;
            for (int i = 1; i <= 5; i++)
            {
                result += term;
                term = -term * number1 * number1 / ((2 * i) * (2 * i + 1));
            }
            return result;
        }
        public double cosine(double number1)
        {
            while (number1 > 3.14159) number1 -= 2 * 3.14159;
            while (number1 < -3.14159) number1 += 2 * 3.14159;
            double result = 0, term = 1;
            for (int i = 1; i <= 5; i++)
            {
                result += term;
                term = -term * number1 * number1 / ((2 * i - 1) * (2 * i));
            }
            return result;
        }
        public double Square(double number1)
        {
            return number1 * number1;
        }
    }
}
