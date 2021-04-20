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

            //for (int i = 0; i < point.Count; i++)
            //{
            //    Console.WriteLine("X = {0}, Y = {1}", point[i].X, point[i].Y);
            //}

            Derivative der = new Derivative(point);

            Console.WriteLine("Конечная разность");
            List<Point> finalDiff = der.FiniteDifferenceMethod(TypeDifference.Center, 5);
            for (int i = 0; i < finalDiff.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.00}; {2:0.000}", i, finalDiff[i].X, finalDiff[i].Y);
            }

            Console.WriteLine("Квадратичная интерполяция");
            List<Point> quadratik = der.QuadraticInterpolation(6);
            for (int i = 0; i < quadratik.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.00}; {2:0.000}", i, quadratik[i].X, quadratik[i].Y);
            }

            Console.WriteLine("\nКубическая интерполяция");
            List<Point> cubik = der.CubicInterpolationMethod(6);
            for (int i = 0; i < cubik.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.00}; {2:0.000}", i, cubik[i].X, cubik[i].Y);
            }

            Console.WriteLine("\nПолином Ньютона");
            List<Point> newton = der.NewtonPolynomialMethod(6, 2);
            for (int i = 0; i < newton.Count; i++)
            {
                Console.WriteLine("Точка №{0} = {1:0.00}; {2:0.000}", i, newton[i].X, newton[i].Y);
            }

            Console.Read();
        }
    }
}
