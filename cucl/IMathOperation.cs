using System;


namespace Calculator
{
    public interface IMathOperation
    {
        public  double add(double number1, double number2);
        public  double subtract(double number1, double number2);
        public  double multiply(double number1, double number2);
        public  double divide(double number1, double number2);
        public double mod(double number1, double number2);  
    }
}