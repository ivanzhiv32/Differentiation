using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MathNet.Symbolics;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace Differentiation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        SeriesCollection seriesCollection;
        LineSeries functionLine, derivativeLine;
        public MainWindow()
        {
            InitializeComponent();
            CbMethods.SelectedIndex = 0;
            CbTypesDifference.SelectedIndex = 0;
            seriesCollection = new SeriesCollection();


            functionLine = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0, Title = "Исходная функция"};
            derivativeLine = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0, Title = "Производная функция"};

            Chart.Series = seriesCollection;
            Chart.AxisX.Add(new Axis() { Title = "X" });
            Chart.AxisY.Add(new Axis() { Title = "Y" });
        }

        private void BtFindDerivative_Click(object sender, RoutedEventArgs e)
        {
            if (TbFunction.Text == string.Empty) 
            {
                MessageBox.Show("Для нахождения производнной необходимо ввести функциюю", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                List<Point> points = GetPoints((int)UdNumberPoints.Value, (decimal)UdStep.Value, TbFunction.Text);
                Derivative der = new Derivative(points);
                DifferentiationResult? result = null;
                
                switch (CbMethods.SelectedIndex) 
                {
                    case 0:
                        result = der.FiniteDifferenceMethod((TypeDifference)CbTypesDifference.SelectedIndex, (int)UdDegree.Value);
                        break;
                    case 1:
                        result = der.QuadraticInterpolation((int)UdDegree.Value);
                        break;
                    case 2:
                        result = der.CubicInterpolationMethod((int)UdDegree.Value);
                        break;
                    case 3:
                        result = der.MethodUndefinedCoefficients((int)UdDegreeMNK.Value);
                        break;
                    case 4:
                        result = der.NewtonPolynomialMethod((int)UdDegreeNewton.Value, (int)UdDegree.Value);
                        break;
                    case 5:
                        result = der.RungeMethod((int)UdDegreeRunge.Value, (int)UdDegree.Value);
                        break;
                }

                LbAbsoluteDeviation.Content = result?.AbsoluteDeviation;
                LbStandartDeviation.Content = result?.StandartDeviation;

                seriesCollection.Clear();
                functionLine.Values.Clear();
                derivativeLine.Values.Clear();

                for (int i = 0; i < result?.CountPoints; i++) functionLine.Values.Add(new ObservablePoint(points[i].X, points[i].Y));
                for (int i = 0; i < result?.CountPoints; i++) derivativeLine.Values.Add(new ObservablePoint(result.Value[i].X, result.Value[i].Y));

                seriesCollection.Add(functionLine);
                seriesCollection.Add(derivativeLine);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CbMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideElements(FiniteDifferencePanel, NewtonPolynomialPanel, RungePanel, UndefinedCoefficientsPanel);
            switch (CbMethods.SelectedIndex) 
            {
                //Выбран метод конечных разностей
                case 0:
                    FiniteDifferencePanel.Visibility = Visibility.Visible;
                    break;
                //Выбран метод неопределенных коэффициентов
                case 3:
                    UndefinedCoefficientsPanel.Visibility = Visibility.Visible;
                    break;
                //Выбран метод Ньютона
                case 4:
                    NewtonPolynomialPanel.Visibility = Visibility.Visible;
                    break;
                //Выбран метод Рунге
                case 5:
                    RungePanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void HideElements(params FrameworkElement[] elements) 
        {
            foreach (FrameworkElement element in elements)
                element.Visibility = Visibility.Hidden;
        }

        private List<Point> GetPoints(int numberPoints, decimal step, string function) 
        {
            SymbolicExpression expression = SymbolicExpression.Parse(function);
            Dictionary<string, FloatingPoint> variable = new Dictionary<string, FloatingPoint>();
            variable.Add("x", 0);

            List<Point> points = new List<Point>();
            decimal s = 0;
            for (int i = 0; i < numberPoints; i++)
            {
                variable["x"] = (double)s;
                points.Add(new Point((double)s, expression.Evaluate(variable).RealValue));
                s += step;
            }

            return points;
        }
    }
}
