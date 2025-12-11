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
using OxyPlot.Wpf;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.IO;
using System.Globalization;
using System.Printing;


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
            
        }

        private void BuildAllPlots(double[] t, double[] x1, double[] x2, double[] u, double[] H)
        {
            BuildPlot(PlotX1, "x1(t)", t, x1);
            BuildPlot(PlotU, "u(t)", t, u);
            BuildPlot(PlotH, "H(t)", t, H);
        }


        private void BuildPlot(OxyPlot.Wpf.PlotView view, string title,
                       double[] t, double[] y)
        {
            var model = new PlotModel { Title = title };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "t",
                MajorGridlineStyle = LineStyle.Solid
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Значение",
                MajorGridlineStyle = LineStyle.Solid
            });

            var series = new LineSeries { StrokeThickness = 2 };

            for (int i = 0; i < t.Length; i++)
                series.Points.Add(new DataPoint(t[i], y[i]));

            model.Series.Add(series);
            view.Model = model;
        }
        private void BuildPlots(double[] t, double[] x, double[] u, double[] H)
        {
            BuildPlot(PlotX1, "x(t)", t, x);
            BuildPlot(PlotU, "u(t)", t, u);
            BuildPlot(PlotH, "H(t)", t, H);
        }


        private void RunModel_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, что все числовые поля введены корректно
            bool ok = true;

            ok &= ParseDoubleSafe(InputG, out double G);
            ok &= ParseDoubleSafe(InputY, out double Y);
            ok &= ParseDoubleSafe(InputR0, out double R0);
            ok &= ParseDoubleSafe(InputR1, out double R1);
            ok &= ParseDoubleSafe(InputGamma, out double GAMMA);
            ok &= ParseDoubleSafe(InputNey, out double NEY);

            ok &= ParseDoubleSafe(InputUMin, out double uMin);
            ok &= ParseDoubleSafe(InputUMax, out double uMax);

            ok &= ParseDoubleSafe(InputT, out double T);
            ok &= ParseIntSafe(InputN, out int N);

            ok &= ParseDoubleSafe(InputAStep, out double Astep);

            if (!ok)
            {
                MessageBox.Show("Некоторые параметры введены неверно.\nПроверь выделенные поля.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (N < 20)
            {
                MessageBox.Show("N должно быть больше 20.", "Ошибка", MessageBoxButton.OK);
                return;
            }

            // ---- Запуск модели --------------------------------------------------

            ModelSolver solver = new ModelSolver();

            solver.Run(
                G, Y, R0, R1,
                GAMMA, NEY,
                uMin, uMax,
                T, N,
                Astep
            );

            // ---- Построение графиков --------------------------------------------

            BuildPlot(PlotX1, "x(t)", solver.t, solver.x1);
            BuildPlot(PlotU, "u(t)", solver.t, solver.u);
            BuildPlot(PlotH, "H(t)", solver.t, solver.H);

            MessageBox.Show("Расчёт завершён.", "Готово", MessageBoxButton.OK);
        }

        private void SaveCharts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SavePlotAsPng(PlotX1, "x1.png", 1200, 400);
                SavePlotAsPng(PlotU, "u.png", 1200, 400);
                SavePlotAsPng(PlotH, "H.png", 1200, 400);
                MessageBox.Show("Графики сохранены в текущей папке.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SavePlotAsPng(PlotView plotView, string filename, int width = 1200, int height = 800)
        {
            if (plotView?.Model == null) return;

            // при необходимости установить белый фон:
            plotView.Model.Background = OxyPlot.OxyColors.White;

            var exporter = new PngExporter { Width = width, Height = height };
            BitmapSource bitmap = exporter.ExportToBitmap(plotView.Model);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fs = File.OpenWrite(filename))
            {
                encoder.Save(fs);
            }
        }

        private bool ParseDoubleSafe(TextBox box, out double value)
            {
                string text = box.Text.Trim().Replace(",", ".");

                bool ok = double.TryParse(
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value);

                if (!ok)
                {
                    // выделяем неправильное поле
                    box.BorderBrush = Brushes.Red;
                    box.BorderThickness = new Thickness(2);
                }
                else
                {
                    // убираем выделение
                    box.BorderBrush = Brushes.Gray;
                    box.BorderThickness = new Thickness(1);
                }

                return ok;
            }
        private bool ParseIntSafe(TextBox box, out int value)
        {
            bool ok = int.TryParse(box.Text.Trim(), out value);

            if (!ok)
            {
                box.BorderBrush = Brushes.Red;
                box.BorderThickness = new Thickness(2);
            }
            else
            {
                box.BorderBrush = Brushes.Gray;
                box.BorderThickness = new Thickness(1);
            }

            return ok;
        }
        private void Deriv(double x, double u, double G, double Y, double GAMMA, out double dx)
        {
            dx = -G * u * (Y - x) * x + GAMMA;
        }
        private void IntegrateRK4(double[] x, double[] u, int N, double dt,
                          double G, double Y, double GAMMA)
        {
            for (int i = 0; i < N; i++)
            {
                double k1, k2, k3, k4;

                Deriv(x[i], u[i], G, Y, GAMMA, out k1);
                Deriv(x[i] + dt * 0.5 * k1, u[i], G, Y, GAMMA, out k2);
                Deriv(x[i] + dt * 0.5 * k2, u[i], G, Y, GAMMA, out k3);
                Deriv(x[i] + dt * k3, u[i], G, Y, GAMMA, out k4);

                x[i + 1] = x[i] + dt / 6.0 * (k1 + 2 * k2 + 2 * k3 + k4);
            }
        }
        private void ComputeHamiltonian(double[] H, double[] x, double[] u,
                                double[] lambda, int N, double G, double Y, double GAMMA)
        {
            for (int i = 0; i <= N; i++)
            {
                double dx;
                Deriv(x[i], u[i], G, Y, GAMMA, out dx);

                double L = u[i] * (Y - x[i]) - x[i];   // Пример твоей L (меняется при необходимости)

                H[i] = L + lambda[i] * dx;
            }
        }
        private void ComputeAdjoint(double[] lambda, double[] x, double[] u,
                            int N, double dt, double G, double Y, double GAMMA)
        {
            lambda[N] = 0; // терминальное условие

            for (int i = N - 1; i >= 0; i--)
            {
                double dH_dx =
                    -u[i]                     // ∂L/∂x
                    + lambda[i + 1] * (       // ∂(λ·dx)/∂x
                       -G * u[i] * x[i]       // от -G u (Y-x) x
                       - G * u[i] * (Y - x[i]) // ещё одно x из производной
                    );

                lambda[i] = lambda[i + 1] + dt * dH_dx;
            }
        }
        private void UpdateControl(double[] u, double[] x, double[] lambda,
                           int N, double Astep, double uMin, double uMax,
                           double G, double Y)
        {
            for (int i = 0; i <= N; i++)
            {
                double grad =
                    (Y - x[i]) +
                    lambda[i] * (-G * x[i] * (Y - x[i]));

                double u_new = u[i] + Astep * grad;

                if (u_new < uMin) u_new = uMin;
                if (u_new > uMax) u_new = uMax;

                u[i] = u_new;
            }
        }

    }
}