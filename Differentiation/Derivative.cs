using System;
using System.Collections.Generic;
using System.Windows;

namespace Differentiation
{
    public enum TypeDifference 
    {
        Left,
        Right,
        Center
    }

    public class Derivative
    {
        private List<Point> points;

        public Derivative(List<Point> points) 
        {
            if (points.Count < 2) throw new Exception("Количество точек не может быть меньше двух");
            this.points = points;
        }

        public int Factorial(int num) 
        {
            if (num <= 0) throw new Exception("Для расчета факториала число должно быть больше нуля");
            
            int res = 1;
            for (int i = 1; i <= num; i++)
            {
                res *= i;
            }

            return res;
        }

        //Проверка на h = const
        private bool CheckIncrement() 
        {
            bool isConstant = true;
            double h = points[1].X - points[0].X;
            
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X - points[i - 1].X != h) 
                {
                    isConstant = false;
                    break;
                }
            }

            return isConstant;
        }

        public double GetDeltaY(int degree, int numPoint)
        {
            double result;

            if (degree == 1) result = points[numPoint + 1].Y - points[numPoint].Y;
            else result =  GetDeltaY(degree - 1, numPoint + 1) - GetDeltaY(degree - 1, numPoint);

            return result;
        }

        public Dictionary<TypeDifference, double> FiniteDifferenceMethod(Point point) 
        {
            if (!CheckIncrement()) throw new Exception("Приращение аргумента непостоянно");
            if (!points.Contains(point)) throw new Exception("Заданная точка не принадлежит дискретно заданной функции");

            Dictionary<TypeDifference, double> result = new Dictionary<TypeDifference, double>();
            double h = points[1].X - points[0].X;
            if (points.Count == points.IndexOf(point) + 1)
            {
                result.Add(TypeDifference.Left, (point.Y - points[points.IndexOf(point) - 1].Y) / h);
                result.Add(TypeDifference.Right, double.NaN);
                result.Add(TypeDifference.Center, double.NaN);
            }
            else if (points.IndexOf(point) == 0)
            {
                result.Add(TypeDifference.Right, (points[points.IndexOf(point) + 1].Y - point.Y) / h);
                result.Add(TypeDifference.Left, double.NaN);
                result.Add(TypeDifference.Center, double.NaN);
            }
            else 
            {
                result.Add(TypeDifference.Left, (point.Y - points[points.IndexOf(point) - 1].Y) / h);
                result.Add(TypeDifference.Right, (points[points.IndexOf(point) + 1].Y - point.Y) / h);
                result.Add(TypeDifference.Center, (points[points.IndexOf(point) + 1].Y - points[points.IndexOf(point) - 1].Y) / 2 * h);
            }
            
            return result;
        }

        public List<Point> QuadraticInterpolation(int degree)
        {
            List<Point> result = new List<Point>();
            double h = points[1].X - points[0].X;
            double q;

            for (int i = 0; i < degree; i++)
            {
                for (int j = 0; j < points.Count - 2; j++)
                {
                    q = (points[j].X - points[j + 1].X) / h;
                    result.Add(new Point(points[j].X, (GetDeltaY(1, j) + (2 * q - 1) / 2 * GetDeltaY(2, j)) / h));
                }
            }

            return result;
        }

        public List<Point> CubicInterpolationMethod(int degree)
        {
            List<Point> result = new List<Point>();
            double h = points[1].X - points[0].X;
            double q;
            for (int i = 0; i < degree; i++)
            {
                for (int j = 0; j < points.Count - 3; j++)
                {
                    q = (points[j].X - points[j + 1].X) / h;
                    result.Add(new Point(points[j].X, (GetDeltaY(1, j) + (2 * q - 1) / 2 * GetDeltaY(2, j) +
                            (3 * Math.Pow(q, 2) - 6 * q + 2) / 6 * GetDeltaY(3, j)) / h));
                }
            }

            return result;
        }

        public List<Point> NewtonPolynomialMethod(int n)
        {
            if (n < 0) throw new Exception("Порядок конечной разности не может быть отрицательным числом.");

            List<Point> resultList = new List<Point>();
            double h = points[1].X - points[0].X;
            double q;
            double result;

            for (int k = 0; k < points.Count; k++)
            {
                if (k == points.Count - n) break;
                result = points[k].Y;
                for (int i = 1; i < n; i++)
                {
                    
                    q = (points[i].X - points[i + 1].X) / h;
                    double mult = 1;
                    for (int j = 0; j <= i; j++)
                    {
                        mult *= q - j + 1;
                    }
                    double res = mult / Factorial(i);
                    res *= GetDeltaY(i, k);
                    result += res;
                }
                result /= h;
                resultList.Add(new Point(points[k].X, result / 10));
            }
            return resultList;
        }
    }
}
