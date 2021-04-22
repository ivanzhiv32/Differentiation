using System;
using System.Collections.Generic;
using System.Windows;
using SolutionSystemEquations;


namespace Differentiation
{
    public struct DifferentiationResult 
    {
        public double AbsoluteDeviation { get; }
        public double StandartDeviation { get; }
        public List<Point> DerivativePoints { get; } //заменить на индексатор

        public DifferentiationResult(List<Point> derivativePoints, double absoluteDeviation, double standartDeviation)
        {
            AbsoluteDeviation = absoluteDeviation;
            StandartDeviation = standartDeviation;
            DerivativePoints = derivativePoints;
        }
    }

    public enum TypeDifference 
    {
        Left,
        Right,
        Center
    }

    public class Derivative
    {
        private List<Point> points;
        private bool stepIsConstant;
        private double step;

        public Derivative(List<Point> points) 
        {
            if (points.Count < 2) throw new Exception("Количество точек не может быть меньше двух");
            this.points = points;
            stepIsConstant = CheckIncrement();
            step = points[1].X - points[0].X;
        }

        private int Factorial(int num) 
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

        private double GetDeltaY(List<Point> points, int degree, int numPoint)
        {
            double result;

            if (degree == 1) result = points[numPoint + 1].Y - points[numPoint].Y;
            else result =  GetDeltaY(points, degree - 1, numPoint + 1) - GetDeltaY(points, degree - 1, numPoint);

            return result;
        }

        public DifferentiationResult FiniteDifferenceMethod(TypeDifference typeDifference, int degreeDerivates)
        {            
            List<Point> derivativePoints = new List<Point>(points);
            for (int i = 0; i < degreeDerivates; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 1; j < derivativePoints.Count - 1; j++)
                {
                    if (typeDifference == TypeDifference.Left) tempResult.Add(new Point(derivativePoints[j].X, (derivativePoints[j].Y - derivativePoints[j - 1].Y) / step));
                    if (typeDifference == TypeDifference.Right) tempResult.Add(new Point(derivativePoints[j].X, (derivativePoints[j + 1].Y - derivativePoints[j].Y) / step));
                    if (typeDifference == TypeDifference.Center) tempResult.Add(new Point(derivativePoints[j].X, (derivativePoints[j + 1].Y - derivativePoints[j - 1].Y) / (2 * step)));
                }
                derivativePoints = tempResult;
            }

            return new DifferentiationResult(derivativePoints, AbsoluteDeviation(derivativePoints), StandartDeviation(derivativePoints));
        }

        public DifferentiationResult QuadraticInterpolation(int degreeDerivates)
        {
            List<Point> derivativePoints = new List<Point>(points);
            double h = derivativePoints[1].X - derivativePoints[0].X;
            double q;

            for (int i = 0; i < degreeDerivates; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 0; j < derivativePoints.Count - 2; j++)
                {
                    q = (derivativePoints[j].X - derivativePoints[j + 1].X) / h;
                    tempResult.Add(new Point(derivativePoints[j].X, (GetDeltaY(derivativePoints, 1, j) + (2 * q - 1) / 2 * GetDeltaY(derivativePoints, 2, j)) / h));
                }
                derivativePoints = tempResult;
            }

            return new DifferentiationResult(derivativePoints, AbsoluteDeviation(derivativePoints), StandartDeviation(derivativePoints));
        }

        public DifferentiationResult CubicInterpolationMethod(int degreeDerivates)
        {
            List<Point> derivativePoints = new List<Point>(points);
            double h = derivativePoints[1].X - derivativePoints[0].X;
            double q;
            for (int i = 0; i < degreeDerivates; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 0; j < derivativePoints.Count - 3; j++)
                {
                    q = (derivativePoints[j].X - derivativePoints[j + 1].X) / h;
                    tempResult.Add(new Point(derivativePoints[j].X, (GetDeltaY(derivativePoints, 1, j) + (2 * q - 1) / 2 * GetDeltaY(derivativePoints, 2, j) +
                            (3 * Math.Pow(q, 2) - 6 * q + 2) / 6 * GetDeltaY(derivativePoints, 3, j)) / h));
                }
                derivativePoints = tempResult;
            }

            return new DifferentiationResult(derivativePoints, AbsoluteDeviation(derivativePoints), StandartDeviation(derivativePoints));
        }

        public DifferentiationResult NewtonPolynomialMethod(int degreePolynom, int degreeDerivate)
        {
            if (degreePolynom < 0) throw new Exception("Порядок конечной разности не может быть отрицательным числом.");

            List<Point> derivativePoints = new List<Point>(points);
            double h = derivativePoints[1].X - derivativePoints[0].X;
            double q, tempResult;
            for (int m = 0; m < degreeDerivate; m++)
            {
                List<Point> tempResultList = new List<Point>();
                for (int k = 0; k < derivativePoints.Count - degreePolynom; k++)
                {
                    tempResult = derivativePoints[k].Y;
                    for (int i = 1; i < degreePolynom; i++)
                    {
                        q = (derivativePoints[i].X - derivativePoints[i + 1].X) / h;
                        double mult = 1;
                        for (int j = 0; j <= i; j++)
                        {
                            mult *= q - j + 1;
                        }
                        double res = mult / Factorial(i);
                        res *= GetDeltaY(derivativePoints, i, k);
                        tempResult += res;
                    }
                    tempResult /= h;
                    tempResultList.Add(new Point(derivativePoints[k].X, tempResult));
                }
                derivativePoints = tempResultList;
            }
            return new DifferentiationResult(derivativePoints, AbsoluteDeviation(derivativePoints), StandartDeviation(derivativePoints));
        }

        public DifferentiationResult MethodUndefinedCoefficients(int degreeAccuracy)
        {
            if(degreeAccuracy >= points.Count) throw new Exception("Порядок точности не может быть больше кол-ва точек.");

            double[,] matrixC = new double[points.Count, points.Count];
            double[,] matrixFreeTerms = new double[points.Count, 1];

            for (int i = 0; i < matrixC.GetLength(0); i++)
            {
                for (int j = 0; j < matrixC.GetLength(1); j++)
                {
                    //Заполнение матрицы коэффицентов C
                    if ((j == 0)&&(i != 0)) matrixC[i, j] = 0;
                    else if (i == 0) matrixC[i, j] = 1;
                    else matrixC[i, j] = Math.Pow(j * step, i);

                    //Заполнение матрицы свободных членов
                    if (j == 0)
                    {
                        if (i == 0) matrixFreeTerms[i, j] = i;
                        else if (i == 1) matrixFreeTerms[i, j] = i;
                        else
                        {
                            matrixFreeTerms[i, j] = Math.Pow(step, i - 1) * i;
                        }
                    }
                }
            }

            //Решение СЛАУ
            SystemEquations equations = new SystemEquations(matrixC, matrixFreeTerms);
            double[,] res = equations.GausGordanMethod();

            //Вычисление производных
            List<Point> derivativePoints = new List<Point>();
            for (int i = 1; i < res.GetLength(0) - degreeAccuracy; i++)
            {
                double derivate = 0;
                for (int j = 0; j <= degreeAccuracy; j++)
                {
                    derivate += res[j, 0] * points[i - 1 + j].Y;
                }
                derivativePoints.Add(new Point(points[i].X, derivate));
            }

            return new DifferentiationResult(derivativePoints, AbsoluteDeviation(derivativePoints), StandartDeviation(derivativePoints));
        }

        public DifferentiationResult RungeMethod(int degreeAccuracy, int degreeDerivate)
        {
            List<Point> derivativePoints = new List<Point>(points);
            int k = 2;
            double h = derivativePoints[1].X - derivativePoints[0].X;
            double fx, fxh;

            for (int i = 0; i < degreeDerivate; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 2; j < derivativePoints.Count; j++)
                {
                    fx = (derivativePoints[j].Y - derivativePoints[j - 1].Y) / h;
                    fxh = (derivativePoints[j].Y - derivativePoints[j - 2].Y) / (2 * h);
                    tempResult.Add(new Point(derivativePoints[j].X, fx + (fx - fxh) / (Math.Pow(k, degreeAccuracy) - 1)));
                }
                derivativePoints = tempResult;
            }

            return new DifferentiationResult(derivativePoints, AbsoluteDeviation(derivativePoints), StandartDeviation(derivativePoints));
        }

        private double StandartDeviation(List<Point> finalPoints)
        {
            double s = 0;
            int n = finalPoints.Count;
            int k = 0;

            for (int i = 0; i < finalPoints.Count; i++)
            {
                for (int j = k; j < points.Count; j++)
                {
                    if (points[j].X == finalPoints[i].X)
                    {
                        s += Math.Pow(Math.Abs(points[j].Y - finalPoints[i].Y), 2);
                        k = j;
                        break;
                    }
                }
            }
            double result = Math.Sqrt(s / n);

            return result;
        }

        private double AbsoluteDeviation(List<Point> finalPoints)
        {
            double max = 0;
            int k = 0;
            for (int i = 0; i < finalPoints.Count; i++)
            {
                for (int j = k; j < points.Count; j++)
                {
                    double tempMax = points[j].Y - finalPoints[i].Y;
                    if ((points[j].X == finalPoints[i].X) && ((tempMax) > max))
                    {
                        max = tempMax;
                        k = j;
                        break;
                    }
                }
            }

            return max;
        }

        public double StandartDeviation(List<Point> startPoints, List<Point> finalPoints)
        {
            double s = 0;
            int n = finalPoints.Count;
            int k = 0;

            for (int i = 0; i < finalPoints.Count; i++)
            {
                for (int j = k; j < startPoints.Count; j++)
                {
                    if (startPoints[j].X == finalPoints[i].X)
                    {
                        s += Math.Pow(Math.Abs(startPoints[j].Y - finalPoints[i].Y), 2);
                        k = j;
                        break;
                    }
                }
            }
            double result = Math.Sqrt(s / n);

            return result;
        }

        public double AbsoluteDeviation(List<Point> startPoints, List<Point> finalPoints)
        {
            double max = 0;
            int k = 0;
            for (int i = 0; i < finalPoints.Count; i++)
            {
                for (int j = k; j < startPoints.Count; j++)
                {
                    double tempMax = startPoints[j].Y - finalPoints[i].Y;
                    if ((startPoints[j].X == finalPoints[i].X)&&((tempMax) > max))
                    {
                        max = tempMax;
                        k = j;
                        break;
                    }
                }
            }

            return max;
        }
    }
}
