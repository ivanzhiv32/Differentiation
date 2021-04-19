﻿using System;
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
        //private double GetDeltaY(Point point) 
        //{
        //    Dictionary<TypeDifference, double> dictionary = FiniteDifferenceMethod(point);

        //    double deltaY = double.NaN;
        //    foreach (TypeDifference type in dictionary.Keys)
        //    {
        //        if (!double.IsNaN(dictionary[type]))
        //        {
        //            deltaY = dictionary[type];
        //            break;
        //        }
        //    }

        //    return deltaY;
        //}

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

        public double QuadraticInterpolationMethod(Point point) 
        {
            double h = points[1].X - points[0].X;
            double q = (point.X - points[0].X) / h;
            double result = (GetDeltaY(1, points.IndexOf(point)) + (2 * q - 1) / 2 * GetDeltaY(2, points.IndexOf(point))) / h;

            return result;
        }

        public double CubicInterpolationMethod(Point point)
        {
            double h = points[1].X - points[0].X;
            double q = (point.X - points[0].X) / h;

            double result = (GetDeltaY(1, points.IndexOf(point)) + (2 * q - 1) / 2 * GetDeltaY(2, points.IndexOf(point)) + 
                            (3 * Math.Pow(q, 2) - 6 * q + 2) / 6 * GetDeltaY(3, points.IndexOf(point))) / h;

            return result;
        }

        public double NewtonPolynomialMethod(Point point, int n)
        {
            if (n < 0) throw new Exception("Порядок конечной разности не может быть отрицательным числом.");

            //double deltaY = GetDeltaY(point);
            double h = points[1].X - points[0].X;
            double q = (point.X - points[0].X) / h;

            double result = point.Y;
            for (int i = 1; i < n; i++)
            {
                double mult = 1;
                for (int j = 1; j <= i; j++)
                {
                    mult *= q - j + 1;
                }
                double res = mult / Factorial(i);
                res *= GetDeltaY(i, points.IndexOf(point));
                result += res;
            }
            result /= h;

            return result;
        }
    }
}