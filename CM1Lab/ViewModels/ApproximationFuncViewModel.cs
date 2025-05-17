using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CM1Lab.Model;
using System.Globalization;
using System.Reflection;

namespace CM1Lab.ViewModels
{
    internal class ApproximationFuncViewModel : INotifyPropertyChanged
    {
        private string? size;
        private string? selectedFunction;
        private ObservableCollection<string> coefficientsX = new ObservableCollection<string>();
        private ObservableCollection<string> coefficientsY = new ObservableCollection<string>();
        private PlotModel _plotModel;

        private ObservableCollection<ApproximationResult> _approximationResults;
        //private ApproximationResult _bestApproximation;
        private double _bestApproximation;
        private double _coefDetermination;
        private double _pearsonCorrelation;

        private string _sredneKvOtklon;
        public string SredneKvOtklon
        {
            get => _sredneKvOtklon;
            set
            {
                _sredneKvOtklon = value;
                OnPropertyChanged(nameof(SredneKvOtklon));
            }
        }

        private string _eps;
        public string Eps
        {
            get => _eps;
            set
            {
                _eps = value;
                OnPropertyChanged(nameof(Eps));
            }
        }

        public ObservableCollection<ApproximationResult> ApproximationResults
        {
            get => _approximationResults;
            set
            {
                _approximationResults = value;
                OnPropertyChanged(nameof(ApproximationResults));
            }
        }

        public string ApproximationResultsString
        {
            get
            {
                if (ApproximationResults == null || ApproximationResults.Count == 0)
                    return string.Empty;

                return string.Join(Environment.NewLine,
                    ApproximationResults.Select(r =>
                        $"{r.FunctionType}: {string.Join(", ", r.Coefficients.Select(c => c.ToString("F4")))}"));
            }
        }




        public double BestApproximation
        {
            get => _bestApproximation;
            set
            {
                _bestApproximation = value;
                OnPropertyChanged(nameof(BestApproximation));
            }
        }

        public double PearsonCorrelation
        {
            get => _pearsonCorrelation;
            set
            {
                _pearsonCorrelation = value;
                OnPropertyChanged(nameof(PearsonCorrelation));
            }
        }

        public double CoefDetermination
        {
            get => _coefDetermination;
            set
            {
                _coefDetermination = value;
                OnPropertyChanged(nameof(CoefDetermination));
            }
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public ApproximationFuncViewModel()
        {
            // Инициализация модели графика
            PlotModel = new PlotModel { Title = "График функции" };
        }

        public string SelectedFunction
        {
            get => selectedFunction;
            set
            {
                selectedFunction = value;
                OnPropertyChanged(nameof(SelectedFunction));
                //UpdateEquationFormula(); // Обновляем формулу при выборе функции
            }
        }

        public string Size
        {
            get => size;
            set { size = value; OnPropertyChanged(nameof(Size)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public ObservableCollection<string> CoefficientsX
        {
            get => coefficientsX;
            set { coefficientsX = value; OnPropertyChanged(nameof(CoefficientsX)); }
        }
        public ObservableCollection<string> CoefficientsY
        {
            get => coefficientsY;
            set { coefficientsY = value; OnPropertyChanged(nameof(CoefficientsY)); }
        }

        private double[] X;
        private double[] Y;
        public void ApproximationSolve()
        {

            ApproximationResults = new ObservableCollection<ApproximationResult>();

            if (CoefficientsX.Count == 0 || CoefficientsY.Count == 0)
            {
                MessageBox.Show("Введите данные в таблицу!");
                return;
            }

            int n = int.Parse(Size);
            X = new double[n];
            Y = new double[n];

            for (int i = 0; i < n; i++)
            {
                if (i >= CoefficientsX.Count || i >= CoefficientsY.Count)
                {
                    MessageBox.Show($"Не хватает данных в строке {i + 1}!");
                    return;
                }

                if (!double.TryParse(CoefficientsX[i], out X[i]) || !double.TryParse(CoefficientsY[i], out Y[i]))
                {
                    MessageBox.Show($"Некорректные данные в строке {i + 1}!");
                    return;
                }
            }

            Debug.WriteLine("--- Коэффициенты X ---");
            for (int i = 0; i < X.Length; i++)
            {
                Debug.WriteLine($"X[{i}] = {X[i]}");
            }

            Debug.WriteLine("--- Коэффициенты Y ---");
            for (int i = 0; i < Y.Length; i++)
            {
                Debug.WriteLine($"Y[{i}] = {Y[i]}");
            }

            // Вычисляем все виды аппроксимации
            var linear = ApproximationMethods.LinearApproximation(X, Y);
            ApproximationResults.Add(linear);

            var quadratic = ApproximationMethods.QuadraticApproximation(X, Y);
            ApproximationResults.Add(quadratic);

            var cubic = ApproximationMethods.CubicApproximation(X, Y);
            ApproximationResults.Add(cubic);

            var exp = ApproximationMethods.ExponentialApproximation(X, Y);
            ApproximationResults.Add(exp);

            var log = ApproximationMethods.LogarithmicApproximation(X, Y);
            ApproximationResults.Add(log);

            var pow = ApproximationMethods.PowerApproximation(X, Y);
            ApproximationResults.Add(pow);

            // Вычисляем коэффициент корреляции Пирсона для линейной зависимости
            PearsonCorrelation = ApproximationMethods.CalculatePearsonCorrelation(X, Y);

            // Находим наилучшую аппроксимацию (с минимальным стандартным отклонением)
            var best = ApproximationResults.OrderBy(r => r.StandardDeviation).First();
            BestApproximation = best.StandardDeviation;
            CoefDetermination = best.DeterminationCoefficient;

            SredneKvOtklon = best.StandardDeviation.ToString("F4");
            Eps = string.Join("; ", best.Errors.Select(e => e.ToString("F4")));

            OnPropertyChanged(nameof(ApproximationResultsString));

            BuildGraphic(X, Y);
        }

        // Метод для построения графика
        public void BuildGraphic(double[] x, double[] y)
        {
            PlotModel.Series.Clear();
            PlotModel.Axes.Clear();

            // Добавляем исходные точки
            var scatterSeries = new ScatterSeries
            {
                Title = "Исходные данные",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerFill = OxyColors.Blue
            };

            for (int i = 0; i < x.Length; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));
            }

            PlotModel.Series.Add(scatterSeries);

            // Добавляем графики аппроксимирующих функций
            foreach (var result in ApproximationResults)
            {
                var lineSeries = new LineSeries
                {
                    Title = result.FunctionType,
                    StrokeThickness = 2
                };

                double minX = x.Min();
                double maxX = x.Max();
                double step = (maxX - minX) / 100;

                for (double xi = minX - 1; xi <= maxX + 1; xi += step)
                {
                    lineSeries.Points.Add(new DataPoint(xi, result.Function(xi)));
                }

                PlotModel.Series.Add(lineSeries);
            }

            PlotModel.InvalidatePlot(true);
        }

        public void WriteInFile()
        {
            try
            {
                string filePath = Configs.pathToFileApproximation;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файла не существует");
                    return;
                }

                var lines = File.ReadAllLines(filePath).ToList();

                void ReplaceLine(string key, string newValue)
                {
                    string prefix = key.TrimEnd(':') + ":";
                    int index = lines.FindIndex(l => l.Trim().StartsWith(prefix));
                    if (index != -1)
                    {
                        lines[index] = $"{prefix} {newValue}";
                    }
                    else
                    {
                        lines.Add($"{prefix} {newValue}");
                    }
                }


                ReplaceLine("Коэфы апроксимирующих функций:", ApproximationResultsString);
                ReplaceLine("Среднеквадрат. отклонение:", SredneKvOtklon);
                ReplaceLine("Эпсилон:", Eps);
                ReplaceLine("Коэф корреляции:", PearsonCorrelation.ToString("F4"));
                ReplaceLine("Коэф Детерминации:", CoefDetermination.ToString("F4"));
                ReplaceLine("Наилучшая апроксимирующая функция:", BestApproximation.ToString());

                File.WriteAllLines(filePath, lines);

                MessageBox.Show("Результаты успешно записаны в файл!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи данных в файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadDataFromText(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Файл не найден.");
                }

                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ' ' }, 2);
                    if (parts.Length < 2)
                        continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "Размерность":
                            Size = value;
                            break;
                        case "X":
                            CoefficientsX.Clear();
                            var coeffs = value.Split(' ').Select(double.Parse).ToList();
                            foreach (var coeff in coeffs)
                            {
                                CoefficientsX.Add(coeff.ToString());
                            }
                            break;
                        case "Y":
                            CoefficientsY.Clear();
                            var coeffsY = value.Split(' ').Select(double.Parse).ToList();
                            foreach (var coeff in coeffsY)
                            {
                                CoefficientsY.Add(coeff.ToString());
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
