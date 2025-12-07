using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace M_GRad_P_K
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BuildAllPlots();
        }

        private void BuildAllPlots()
        {
            int N = 1000;
            double T = 250;
            double dt = T / N;

            double[] t = new double[N + 1];
            double[] x1 = new double[N + 1];
            double[] x2 = new double[N + 1];
            double[] u = new double[N + 1];
            double[] H = new double[N + 1];

            // ---- Заглушки (сюда вставишь настоящую динамику) ----
            for (int i = 0; i <= N; i++)
            {
                t[i] = i * dt;

                x1[i] = 1 + Math.Sin(0.02 * t[i]) + 0.05 * t[i] / T;
                x2[i] = 0.5 + Math.Cos(0.02 * t[i]) * 0.1 + 0.003 * t[i];
                u[i] = 3.0 + Math.Sin(0.01 * t[i]);
                H[i] = Math.Exp(-0.01 * t[i]);
            }

            BuildPlot(PlotX1, "x1(t)", t, x1);
            BuildPlot(PlotX2, "x2(t)", t, x2);
            BuildPlot(PlotU, "u(t)", t, u);
            BuildPlot(PlotH, "H(t)", t, H);
        }

        private void BuildPlot(OxyPlot.Wpf.PlotView plotView, string title, double[] t, double[] y)
        {
            var model = new PlotModel { Title = title };

            model.Axes.Add(new LinearAxis
            {
                Title = "t",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            model.Axes.Add(new LinearAxis
            {
                Title = "Значение",
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            var series = new LineSeries
            {
                StrokeThickness = 2,
                MarkerSize = 0
            };

            for (int i = 0; i < t.Length; i++)
                series.Points.Add(new DataPoint(t[i], y[i]));

            model.Series.Add(series);
            plotView.Model = model;
        }
    }
}