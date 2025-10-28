using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calculator
{
    public  interface IMathOperation
    {
        public  double add(double number1, double number2);
        public  double subtract(double number1, double number2);
        public  double multiply(double number1, double number2);
        public  double divide(double number1, double number2);
    }
}