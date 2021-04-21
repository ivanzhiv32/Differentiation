using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Symbolics;
using LiveCharts.Charts;
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

            functionLine = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0};
            derivativeLine = new LineSeries { Values = new ChartValues<ObservablePoint>(), PointGeometrySize = 0 };

            Chart.Series = seriesCollection;
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
                List<Point> points = GetPoints();
                Derivative der = new Derivative(points);
                List<Point> pointsDerivative = new List<Point>();

                switch (CbMethods.SelectedIndex) 
                {
                    case 0:
                        pointsDerivative = der.FiniteDifferenceMethod((TypeDifference)CbTypesDifference.SelectedIndex, (int)UdDegree.Value);
                        break;
                    case 1:
                        pointsDerivative = der.QuadraticInterpolation((int)UdDegree.Value);
                        break;
                    case 2:
                        pointsDerivative = der.CubicInterpolationMethod((int)UdDegree.Value);
                        break;
                    case 3:
                        pointsDerivative = der.MethodUndefinedCoefficients(7, (int)UdDegree.Value);
                        break;
                    case 4:
                        pointsDerivative = der.NewtonPolynomialMethod((int)UdDegreeNewton.Value,(int)UdDegree.Value);
                        break;
                    case 5:
                        pointsDerivative = der.RungeMethod((int)UdDegreeRunge.Value, (int)UdDegree.Value);
                        break;
                }

                seriesCollection.Clear();
                functionLine.Values.Clear();
                derivativeLine.Values.Clear();

                for (int i = 0; i < points.Count; i++) functionLine.Values.Add(new ObservablePoint(points[i].X, points[i].Y));
                for (int i = 0; i < pointsDerivative.Count; i++) derivativeLine.Values.Add(new ObservablePoint(pointsDerivative[i].X, pointsDerivative[i].Y));


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
            if (CbMethods.SelectedIndex == 4)
            {
                LbNewtonPolynomial.Visibility = UdDegreeNewton.Visibility = Visibility.Visible;
                LbTypesDifference.Visibility = CbTypesDifference.Visibility = Visibility.Hidden;
                LbRunge.Visibility = UdDegreeRunge.Visibility = Visibility.Hidden;
            }
            else if (CbMethods.SelectedIndex == 0) 
            {
                LbNewtonPolynomial.Visibility = UdDegreeNewton.Visibility = Visibility.Hidden;
                LbTypesDifference.Visibility = CbTypesDifference.Visibility = Visibility.Visible;
                LbRunge.Visibility = UdDegreeRunge.Visibility = Visibility.Hidden;
            }
            else if (CbMethods.SelectedIndex == 5)
            {
                LbRunge.Visibility = UdDegreeRunge.Visibility = Visibility.Visible;
                LbNewtonPolynomial.Visibility = UdDegreeNewton.Visibility = Visibility.Hidden;
                LbTypesDifference.Visibility = CbTypesDifference.Visibility = Visibility.Hidden;
            }
            else LbNewtonPolynomial.Visibility = UdDegreeNewton.Visibility = LbTypesDifference.Visibility = CbTypesDifference.Visibility = Visibility.Hidden;
        }

        private List<Point> GetPoints() 
        {
            SymbolicExpression expression = SymbolicExpression.Parse(TbFunction.Text);
            Dictionary<string, FloatingPoint> variable = new Dictionary<string, FloatingPoint>();
            variable.Add("x", 0);

            List<Point> points = new List<Point>();
            double step = 0;
            for (int i = 0; i < UdNumberPoints.Value; i++)
            {
                variable["x"] = step;
                points.Add(new Point(step, expression.Evaluate(variable).RealValue));
                step += (double)UdStep.Value;
            }

            return points;
        }
    }
}
