using SolutionSystemEquations;
using System;
using System.Collections.Generic;
using System.Windows;
using SolutionSystemEquations;


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

        public List<Point> FiniteDifferenceMethod(TypeDifference typeDifference, int degreeDerivates)
        {
            List<Point> result = new List<Point>(points);

            double h = result[1].X - result[0].X;

            for (int i = 0; i < degreeDerivates; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 1; j < result.Count - 1; j++)
                {
                    if (typeDifference == TypeDifference.Left) tempResult.Add(new Point(result[j].X, (result[j].Y - result[j - 1].Y) / h));
                    if (typeDifference == TypeDifference.Right) tempResult.Add(new Point(result[j].X, (result[j + 1].Y - result[j].Y) / h));
                    if (typeDifference == TypeDifference.Center) tempResult.Add(new Point(result[j].X, (result[j + 1].Y - result[j - 1].Y) / (2 * h)));
                }
                result = tempResult;
            }

            return result;
        }

        public List<Point> QuadraticInterpolation(int degreeDerivates)
        {
            List<Point> result = new List<Point>(points);
            double h = result[1].X - result[0].X;
            double q;

            for (int i = 0; i < degreeDerivates; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 0; j < result.Count - 2; j++)
                {
                    q = (result[j].X - result[j + 1].X) / h;
                    tempResult.Add(new Point(result[j].X, (GetDeltaY(result, 1, j) + (2 * q - 1) / 2 * GetDeltaY(result, 2, j)) / h));
                }
                result = tempResult;
            }

            return result;
        }

        public List<Point> CubicInterpolationMethod(int degreeDerivates)
        {
            List<Point> result = new List<Point>(points);
            double h = result[1].X - result[0].X;
            double q;
            for (int i = 0; i < degreeDerivates; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 0; j < result.Count - 3; j++)
                {
                    q = (result[j].X - result[j + 1].X) / h;
                    tempResult.Add(new Point(result[j].X, (GetDeltaY(result, 1, j) + (2 * q - 1) / 2 * GetDeltaY(result, 2, j) +
                            (3 * Math.Pow(q, 2) - 6 * q + 2) / 6 * GetDeltaY(result, 3, j)) / h));
                }
                result = tempResult;
            }

            return result;
        }

        public List<Point> NewtonPolynomialMethod(int degreePolynom, int degreeDerivate)
        {
            if (degreePolynom < 0) throw new Exception("Порядок конечной разности не может быть отрицательным числом.");

            List<Point> result = new List<Point>(points);
            double h = result[1].X - result[0].X;
            double q, tempResult;
            for (int m = 0; m < degreeDerivate; m++)
            {
                List<Point> tempResultList = new List<Point>();
                for (int k = 0; k < result.Count - degreePolynom; k++)
                {
                    tempResult = result[k].Y;
                    for (int i = 1; i < degreePolynom; i++)
                    {
                        q = (result[i].X - result[i + 1].X) / h;
                        double mult = 1;
                        for (int j = 0; j <= i; j++)
                        {
                            mult *= q - j + 1;
                        }
                        double res = mult / Factorial(i);
                        res *= GetDeltaY(result, i, k);
                        tempResult += res;
                    }
                    tempResult /= h;
                    tempResultList.Add(new Point(result[k].X, tempResult));
                }
                result = tempResultList;
            }
            return result;
        }

        public List<Point> MethodUndefinedCoefficients(int degreeAccuracy, int degreeDerivate)
        {
            List<Point> result = new List<Point>(points);

            double[] coefficients = new double[degreeAccuracy - 1];
            double[,] matrixC = new double[result.Count, result.Count];
            double[,] matrixFreeTerms = new double[result.Count, 1];

            for (int i = 0; i < matrixC.GetLength(0); i++)
            {
                for (int j = 0; j < matrixC.GetLength(1); j++)
                {
                    //Заполнение матрицы коэффицентов C
                    matrixC[i, j] = Math.Pow(result[j].X - result[0].X, i);

                    //Заполнение матрицы свободных членов
                    if(j == 0)
                    {
                        if (i == 0) matrixFreeTerms[i, j] = 0;
                        else if (i == 1) matrixFreeTerms[i, j] = 1;
                        else
                        {
                            matrixFreeTerms[i, j] = i * Math.Pow(result[1].X - result[0].X, i - 1);
                        }
                    }
                }
            }
            SystemEquations equations = new SystemEquations(matrixC, matrixFreeTerms);
            double[,] res = equations.GausGordanMethod();
            List<Point> tempResult = new List<Point>(result);
            result.Clear();

            for (int i = 0; i < tempResult.Count; i++)
            {
                result.Add(new Point(tempResult[i].X, res[i, 0]));
            }

            return result;
        }

        public List<Point> RungeMethod(int degreeAccuracy, int degreeDerivate)
        {
            List<Point> result = new List<Point>(points);
            int k = 2;
            double h = result[1].X - result[0].X;
            double fx, fxh;

            for (int i = 0; i < degreeDerivate; i++)
            {
                List<Point> tempResult = new List<Point>();
                for (int j = 2; j < result.Count; j++)
                {
                    fx = (result[j].Y - result[j - 1].Y) / h;
                    fxh = (result[j].Y - result[j - 2].Y) / (2 * h);
                    tempResult.Add(new Point(result[j].X, fx + (fx - fxh) / (Math.Pow(k, degreeAccuracy) - 1)));
                }
                result = tempResult;
            }

            return result;
        }

        public double StandartDevation(List<Point> startPoints, List<Point> finalPoints)
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

        public double AbsoluteDevation(List<Point> startPoints, List<Point> finalPoints)
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
