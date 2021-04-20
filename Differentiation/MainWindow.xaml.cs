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
            seriesCollection = new SeriesCollection();

            functionLine = new LineSeries { Values = new ChartValues<ObservablePoint>() };
            derivativeLine = new LineSeries { Values = new ChartValues<ObservablePoint>() };

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
                List<Point> pointsDerivative = der.CubicInterpolationMethod(1);

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
            if (CbMethods.SelectedIndex == 4 && !LbNewtonPolynomial.IsEnabled && !UdDegreeNewton.IsEnabled)
            {
                LbNewtonPolynomial.IsEnabled = true;
                UdDegreeNewton.IsEnabled = true;
            }
            else if (LbNewtonPolynomial.IsEnabled && UdDegreeNewton.IsEnabled) 
            {
                LbNewtonPolynomial.IsEnabled = false;
                UdDegreeNewton.IsEnabled = false;
            }
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
