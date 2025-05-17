using CM1Lab.Model;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Controls;
using CM1Lab.View;
using OxyPlot.Axes;

namespace CM1Lab.ViewModels
{
    internal class InterpolationFunctionViewModel : INotifyPropertyChanged
    {
        private string? size;
        private string? selectedFunction;
        private string? selectedMethod;
        private ObservableCollection<string> coefficientsX = new ObservableCollection<string>();
        private ObservableCollection<string> coefficientsY = new ObservableCollection<string>();
        private PlotModel _plotModel;

        private ObservableCollection<InterpolationResult> _interpolationResults;
        //private ApproximationResult _bestApproximation;
        private double _result;
        private double _bestApproximation;
        private double _coefDetermination;
        private double _pearsonCorrelation;

        private string _sredneKvOtklon;
        private string? xValue;

        public string XValue
        {
            get => xValue;
            set
            {
                xValue = value;
                OnPropertyChanged(nameof(XValue));
            }
        }

        public ObservableCollection<InterpolationResult> InterpolationResults
        {
            get => _interpolationResults;
            set
            {
                _interpolationResults = value;
                OnPropertyChanged(nameof(InterpolationResults));
            }
        }

        public string InterpolationResultsString
        {
            get
            {
                if (InterpolationResults == null || InterpolationResults.Count == 0)
                    return string.Empty;

                return string.Join(Environment.NewLine,
                    InterpolationResults.Select(r =>
                        $"{r.FunctionType}: {string.Join(", ", r.Coefficients.Select(c => c.ToString("F4")))}"));
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

        public InterpolationFunctionViewModel()
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

        public string SelectedMethod
        {
            get => selectedMethod;
            set
            {
                selectedMethod = value;
                OnPropertyChanged(nameof(SelectedMethod));
                //UpdateEquationFormula(); // Обновляем формулу при выборе функции
            }
        }


        public string Size
        {
            get => size;
            set { size = value; OnPropertyChanged(nameof(Size)); }
        }
        public double Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(nameof(Result)); }
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
            try
            {
                //ApproximationResults = new ObservableCollection<ApproximationResult>();
                InterpolationResults = new ObservableCollection<InterpolationResult>();
                // Проверка, выбран ли метод решения
                if (string.IsNullOrEmpty(SelectedMethod))
                {
                    MessageBox.Show("Метод не выбран.");
                    return;
                }

                // Проверка, выбрано ли уравнение для решения
                //if (string.IsNullOrEmpty(SelectedFunction))
                //{
                //    MessageBox.Show("Уравнение не выбрано1.");
                //    return;
                //}

                if (!double.TryParse(XValue, out double xValue))
                {
                    MessageBox.Show("Некорректное значение X для интерполяции");
                    return;
                }

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

                if (X.Distinct().Count() != X.Length)
                {
                    MessageBox.Show("Массив X содержит повторяющиеся значения — это недопустимо для интерполяции.");
                    return;
                }

                // Решение уравнения выбранным методом
                switch (SelectedMethod)
                {
                    case "Многочлен Лагранжа":
                        InterpolationResults.Add(LagrangeInterpolation(X, Y, xValue));
                        break;
                    case "Многочлен Ньютона с разделенными разностями":
                        InterpolationResults.Add(NewtonDividedDifferences(X, Y));
                        break;
                    case "Многочлен Ньютона с конечными разностями":
                        InterpolationResults.Add(NewtonFiniteDifferences(X, Y)); // Только если узлы равноотстоящие
                        break;
                    default:
                        MessageBox.Show("Метод не выбран.");
                        break;
                }

                Debug.WriteLine(SelectedMethod);
                Result = InterpolationResults
                    .FirstOrDefault(r => r.FunctionType == SelectedMethod)
                    ?.Function(double.Parse(XValue)) ?? 0.0;



                // Вычисляем коэффициент корреляции Пирсона для линейной зависимости
                //PearsonCorrelation = ApproximationMethods.CalculatePearsonCorrelation(X, Y);

                // Находим наилучшую аппроксимацию (с минимальным стандартным отклонением)
                //var best = InterpolationResults.OrderBy(r => r.StandardDeviation).First();
                //BestApproximation = best.StandardDeviation;
                //CoefDetermination = best.DeterminationCoefficient;

                //SredneKvOtklon = best.StandardDeviation.ToString("F4");
                //Eps = string.Join("; ", best.Errors.Select(e => e.ToString("F4")));

                OnPropertyChanged(nameof(InterpolationResultsString));

                BuildGraphic(X, Y);

                //if (SelectedMethod == "Многочлен Ньютона с конечными разностями")
                //{
                    //InterpolationFunctionWindow.PrintFiniteDifferencesToUI(Y, Application.Current.MainWindow.FindName("CoefficientGridResults") as Grid);
                //}

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка метода конечных разностей Ньютона: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Некорктные данные");
            }
        }

        public static InterpolationResult LagrangeInterpolation(double[] x, double[] y, double xValue)
        {
            int n = x.Length;
            double result = 0.0;
            double[] errors = new double[n];
            double sumSqError = 0;

            for (int i = 0; i < n; i++)
            {
                double term = y[i];
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                    {
                        term *= (xValue - x[j]) / (x[i] - x[j]);
                    }
                }
                result += term;
            }

            // Вычисляем значения функции в узлах для оценки погрешности
            for (int i = 0; i < n; i++)
            {
                double yiInterpolated = 0.0;
                for (int j = 0; j < n; j++)
                {
                    double lj = 1.0;
                    for (int k = 0; k < n; k++)
                    {
                        if (k != j)
                        {
                            lj *= (x[i] - x[k]) / (x[j] - x[k]);
                        }
                    }
                    yiInterpolated += y[j] * lj;
                }

                double error = yiInterpolated - y[i];
                errors[i] = error;
                sumSqError += error * error;
            }

            return new InterpolationResult
            {
                FunctionType = "Многочлен Лагранжа",
                Coefficients = new double[0], // коэффициенты неявные
                Function = (double arg) =>
                {
                    double sum = 0.0;
                    for (int i = 0; i < n; i++)
                    {
                        double li = 1.0;
                        for (int j = 0; j < n; j++)
                        {
                            if (j != i)
                                li *= (arg - x[j]) / (x[i] - x[j]);
                        }
                        sum += y[i] * li;
                    }
                    return sum;
                },
                Errors = errors,
                StandardDeviation = Math.Sqrt(sumSqError / n),
                DeterminationCoefficient = 1 // можно оценить при необходимости
            };
        }

        public static InterpolationResult NewtonDividedDifferences(double[] x, double[] y)
        {
            int n = x.Length;
            double[,] divided = new double[n, n];

            // Инициализация нулевого порядка
            for (int i = 0; i < n; i++)
                divided[i, 0] = y[i];

            // Построение таблицы разделённых разностей
            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < n - j; i++)
                {
                    divided[i, j] = (divided[i + 1, j - 1] - divided[i, j - 1]) / (x[i + j] - x[i]);
                }
            }

            // Построение функции на основе коэффициентов
            double[] coefficients = new double[n];
            for (int i = 0; i < n; i++)
                coefficients[i] = divided[0, i];

            Func<double, double> function = (double value) =>
            {
                double result = coefficients[0];
                double product = 1.0;
                for (int i = 1; i < n; i++)
                {
                    product *= (value - x[i - 1]);
                    result += coefficients[i] * product;
                }
                return result;
            };

            // Ошибки в узлах
            double[] errors = new double[n];
            double sumSq = 0;
            for (int i = 0; i < n; i++)
            {
                double approx = function(x[i]);
                errors[i] = approx - y[i];
                sumSq += errors[i] * errors[i];
            }

            return new InterpolationResult
            {
                FunctionType = "Многочлен Ньютона с разделенными разностями",
                Coefficients = coefficients,
                Function = function,
                Errors = errors,
                StandardDeviation = Math.Sqrt(sumSq / n),
                DeterminationCoefficient = 1 // не рассчитываем здесь
            };
        }

        public static InterpolationResult NewtonFiniteDifferences(double[] x, double[] y)
        {
            int n = x.Length;
            double h = x[1] - x[0];

            // Проверка на равноотстоящие узлы
            for (int i = 1; i < n - 1; i++)
            {
                if (Math.Abs((x[i + 1] - x[i]) - h) > 1e-8)
                    throw new ArgumentException("Узлы не равноотстоящие — используйте метод разделённых разностей");
            }

            // Построение таблицы конечных разностей
            double[,] delta = new double[n, n];
            for (int i = 0; i < n; i++)
                delta[i, 0] = y[i];

            for (int j = 1; j < n; j++)
                for (int i = 0; i < n - j; i++)
                    delta[i, j] = delta[i + 1, j - 1] - delta[i, j - 1];

            Func<double, double> function = (double value) =>
            {
                double t = (value - x[0]) / h;
                double result = y[0];
                double tProduct = 1.0;
                for (int i = 1; i < n; i++)
                {
                    tProduct *= (t - (i - 1));
                    result += (tProduct / Factorial(i)) * delta[0, i];
                }
                return result;
            };

            // Ошибки в узлах
            double[] errors = new double[n];
            double sumSq = 0;
            for (int i = 0; i < n; i++)
            {
                double approx = function(x[i]);
                errors[i] = approx - y[i];
                sumSq += errors[i] * errors[i];
            }

            return new InterpolationResult
            {
                FunctionType = "Многочлен Ньютона с конечными разностями",
                Coefficients = Enumerable.Range(0, n).Select(i => delta[0, i]).ToArray(),
                Function = function,
                Errors = errors,
                StandardDeviation = Math.Sqrt(sumSq / n),
                DeterminationCoefficient = 1
            };
        }

        private static long Factorial(int n)
        {
            long result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }




        // Метод для построения графика
        public void BuildGraphic(double[] x, double[] y)
        {
            PlotModel.Series.Clear();
            PlotModel.Axes.Clear();

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "X",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Y",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });


            // Исходные точки
            var scatterSeries = new ScatterSeries
            {
                Title = "Исходные данные",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerFill = OxyColors.Blue
            };

            for (int i = 0; i < x.Length; i++)
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));

            PlotModel.Series.Add(scatterSeries);

            // Аппроксимирующие функции
            foreach (var result in InterpolationResults)
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
                    lineSeries.Points.Add(new DataPoint(xi, result.Function(xi)));

                PlotModel.Series.Add(lineSeries);
            }

            // Добавление точки интерполяции (красной)
            double.TryParse(XValue, out double xInterp);
            
            var pointSeries = new ScatterSeries
            {
                Title = "Интерполяционная точка",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerFill = OxyColors.Red
            };

            pointSeries.Points.Add(new ScatterPoint(xInterp, Result));
            Debug.WriteLine(xInterp + " || " + Result);
            PlotModel.Series.Add(pointSeries);
            

            PlotModel.InvalidatePlot(true);
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
                        case "Интерполирующая_точка":
                            XValue = value;
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
