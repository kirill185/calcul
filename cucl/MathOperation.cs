using Calculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cucl
{
    public class MathOperation:IMathOperation
    {
        public  double add(double number1, double number2) { return number1 + number2; }
        public  double subtract(double number1, double number2) { return number1 - number2; }
        public  double multiply(double number1, double number2) { return number1 * number2; }
        public  double divide(double number1, double number2) { return number1 / number2; }
    }
}
