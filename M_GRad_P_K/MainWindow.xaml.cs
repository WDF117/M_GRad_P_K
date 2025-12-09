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


        private void RunModel_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем каждый параметр
            bool ok = true;

            ok &= ParseDoubleSafe(InputD0, out double d0);
            ok &= ParseDoubleSafe(InputC, out double c);
            ok &= ParseDoubleSafe(InputZmax, out double zmax);
            ok &= ParseDoubleSafe(InputT, out double T);
            ok &= ParseIntSafe(InputN, out int N);

            if (!ok)
            {
                MessageBox.Show(
                    "Некоторые параметры введены неверно.\n" +
                    "Проверьте поля, выделенные красным.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Дополнительные логические проверки
            if (N <= 10)
            {
                MessageBox.Show("N должно быть больше 10.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (T <= 0)
            {
                MessageBox.Show("T должно быть больше 0.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (zmax <= 0)
            {
                MessageBox.Show("z_max должно быть положительным.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если всё нормально — запускаем расчёт
            double dt = T / N;

            double[] t = new double[N + 1];
            double[] x1 = new double[N + 1];
            double[] u = new double[N + 1];
            double[] H = new double[N + 1];

            for (int i = 0; i <= N; i++)
            {
                t[i] = i * dt;
                double d = d0 + c * t[i];

                x1[i] = Math.Max(0, 10 - d + 0.5 * Math.Sin(0.2 * i));
                u[i] = Math.Min(zmax, 3 + Math.Sin(0.02 * i));
                H[i] = x1[i] * u[i] - d;
            }

            BuildPlot(PlotX1, "x1(t)", t, x1);
            BuildPlot(PlotU, "u(t)", t, u);
            BuildPlot(PlotH, "H(t)", t, H);
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

    }
}