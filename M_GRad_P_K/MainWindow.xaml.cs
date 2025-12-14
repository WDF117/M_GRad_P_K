using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace M_GRad_P_K
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Запуск модели по параметрам из UI
        private void RunModel_Click(object sender, RoutedEventArgs e)
        {
            bool ok = true;

            ok &= ParseDoubleSafe(InputA, out double a);
            ok &= ParseDoubleSafe(InputC, out double c);
            ok &= ParseDoubleSafe(InputM, out double M);
            ok &= ParseDoubleSafe(InputY0, out double y0);
            ok &= ParseDoubleSafe(InputT, out double T);
            ok &= ParseIntSafe(InputN, out int N);

            if (!ok)
            {
                MessageBox.Show("Проверьте корректность ввода данных");
                return;
            }

            double yStar = 2.0;

            var solver = new ModelSolver();
            solver.Run(a, c, M, y0, yStar, T, N);

            BuildPlot(PlotY, "y(t)", solver.t, solver.y);
            BuildPlot(PlotU, "z(t)", solver.t, solver.z);
            BuildPlot(PlotH, "H(t)", solver.t, solver.H);
        }


        // Экспорт каждого графика в PNG
        private void SaveCharts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SavePlotAsPng(PlotY, "y.png", 1200, 360);
                SavePlotAsPng(PlotU, "u.png", 1200, 360);
                SavePlotAsPng(PlotH, "H.png", 1200, 360);
                MessageBox.Show("Графики сохранены (папка с .exe).", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuildPlot(PlotView view, string title, double[] t, double[] y)
        {
            var model = new PlotModel { Title = title };
            model.Background = OxyColors.White;

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

            var series = new LineSeries { StrokeThickness = 1.5 };

            for (int i = 0; i < t.Length; i++)
                series.Points.Add(new DataPoint(t[i], y[i]));

            model.Series.Add(series);
            view.Model = model;
        }

        private void SavePlotAsPng(PlotView plotView, string filename, int width = 1200, int height = 800)
        {
            if (plotView?.Model == null) return;

            plotView.Model.Background = OxyColors.White;

            var exporter = new PngExporter { Width = width, Height = height };
            BitmapSource bitmap = exporter.ExportToBitmap(plotView.Model);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fs = File.OpenWrite(filename))
            {
                encoder.Save(fs);
            }
        }

        // Безопасное парсирование double (принимает и запятую, и точку)
        private bool ParseDoubleSafe(TextBox box, out double value)
        {
            string text = box.Text.Trim().Replace(",", ".");
            bool ok = double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

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
