using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Differentiation;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(3, 1));
            points.Add(new Point(4, 3));
            points.Add(new Point(5, 0));
            points.Add(new Point(6, 6));

            Derivative derivative = new Derivative(points);
            Dictionary<TypeDifference, double> res = derivative.FiniteDifferenceMethod(points[1]);

            foreach (TypeDifference t in res.Keys)
            {
                Console.WriteLine(res[t]);
            }

            double qua = derivative.QuadraticInterpolationMethod(points[1]);
            double cub = derivative.CubicInterpolationMethod(points[1]);
            double newton = derivative.NewtonPolynomialMethod(points[1], 3);

            Console.WriteLine("Квадратик = {0} и Кубик = {1} и еще Ньютон = {2}", qua, cub, newton);

            int fuckyou = derivative.Factorial(3);

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
