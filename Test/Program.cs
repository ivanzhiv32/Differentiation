using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Differentiation;
using MathNet.Symbolics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string funcString = "sin(x)";
            SymbolicExpression functionExpression = SymbolicExpression.Parse(funcString);
            Dictionary<string, FloatingPoint> variable = new Dictionary<string, FloatingPoint>();
            variable["x"] = 0;

            List<Point> point = new List<Point>();
            for (double i = 0; i < 10; i += 0.1)
            {
                variable["x"] = i;
                point.Add(new Point(i, functionExpression.Evaluate(variable).RealValue));
            }

            for (int i = 0; i < point.Count; i++)
            {
                Console.WriteLine("X = {0}, Y = {1}", point[i].X, point[i].Y);
            }

            Console.WriteLine("Квадратичная интерполяция");
            Derivative der = new Derivative(point);
            List<Point> quadratik = der.QuadraticInterpolation(1);
            for (int i = 0; i < quadratik.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.000}; {2:0.000}", i, quadratik[i].X, quadratik[i].Y);
            }

            Console.WriteLine("\nКубическая интерполяция");
            List<Point> cubik = der.CubicInterpolationMethod(1);
            for (int i = 0; i < cubik.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.000}; {2:0.000}", i, cubik[i].X, cubik[i].Y);
            }

            Console.WriteLine("\nПолином Ньютона");
            List<Point> newton = der.NewtonPolynomialMethod(9);
            for (int i = 0; i < newton.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.000}; {2:0.000}", i, newton[i].X, newton[i].Y);
            }

            Console.Read();
        }

        static double FiniteDifferenceMethod(List<Point> points, int numPoint)
        {
            double result = 0;
            if (points.Count() == numPoint + 1) result = (points[numPoint].Y - points[numPoint - 1].Y) / (points[numPoint].X - points[numPoint - 1].X);
            else if(numPoint == 0) result = (points[numPoint + 1].Y - points[numPoint].Y) / (points[numPoint + 1].X - points[numPoint].X);
            else result = (points[numPoint + 1].Y - points[numPoint].Y) / (points[numPoint + 1].X - points[numPoint].X);
            return result;
        }
    }
}
