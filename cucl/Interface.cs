using System;

public interface IMathOperation
{
    string GetName();
    double Calculate(double x);
}

// Квадратный корень
public class SquareRoot : IMathOperation
{
    public string GetName() => "√";

    public double Calculate(double x)
    {
        if (x < 0) return double.NaN;
        double result = x;
        for (int i = 0; i < 10; i++) result = (result + x / result) / 2;
        return result;
    }
}

// Синус
public class Sine : IMathOperation
{
    public string GetName() => "sin";

    public double Calculate(double x)
    {
        while (x > 3.14159) x -= 2 * 3.14159;
        while (x < -3.14159) x += 2 * 3.14159;
        double result = 0, term = x;
        for (int i = 1; i <= 5; i++)
        {
            result += term;
            term = -term * x * x / ((2 * i) * (2 * i + 1));
        }
        return result;
    }
}

// Косинус
public class Cosine : IMathOperation
{
    public string GetName() => "cos";

    public double Calculate(double x)
    {
        while (x > 3.14159) x -= 2 * 3.14159;
        while (x < -3.14159) x += 2 * 3.14159;
        double result = 0, term = 1;
        for (int i = 1; i <= 5; i++)
        {
            result += term;
            term = -term * x * x / ((2 * i - 1) * (2 * i));
        }
        return result;
    }
}

// Тангенс
public class Tangent : IMathOperation
{
    public string GetName() => "tan";

    public double Calculate(double x)
    {
        Sine sin = new Sine();
        Cosine cos = new Cosine();
        double sinVal = sin.Calculate(x);
        double cosVal = cos.Calculate(x);
        if (Math.Abs(cosVal) < 0.0001) return double.NaN;
        return sinVal / cosVal;
    }
}

// Квадрат числа
public class Square : IMathOperation
{
    public string GetName() => "x²";

    public double Calculate(double x)
    {
        return x * x;
    }
}