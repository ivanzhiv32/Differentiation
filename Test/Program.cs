using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(3, 1));
            points.Add(new Point(4, 3));
            points.Add(new Point(6, 0));
            points.Add(new Point(9, 6));

            Console.WriteLine(FiniteDifferenceMethod(points, 3));

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
