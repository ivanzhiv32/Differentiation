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
            points.Add(new Point(5, 3));
            points.Add(new Point(7, 0));
            points.Add(new Point(9, 6));
            points.Add(new Point(11, 3));
            points.Add(new Point(13, 9));
            points.Add(new Point(15, 4));
            points.Add(new Point(17, 10));
            points.Add(new Point(19, 1));
            points.Add(new Point(21, 5));
            points.Add(new Point(23, 0));
            points.Add(new Point(25, 7));

            Derivative derivative = new Derivative(points);
            Dictionary<TypeDifference, double> res = derivative.FiniteDifferenceMethod(points[4]);

            foreach (TypeDifference t in res.Keys)
            {
                Console.WriteLine(res[t]);
            }

            double qua = derivative.QuadraticInterpolationMethod(points[3]);
            double cub = derivative.CubicInterpolationMethod(points[3]);
            double newton = derivative.NewtonPolynomialMethod(points[3], 7);

            Console.WriteLine("Квадратик = {0} и Кубик = {1}", qua, cub);
            Console.WriteLine("Ньютон = {0}", newton);

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
