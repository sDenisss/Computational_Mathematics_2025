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
        private string? intervalA;
        private string? intervalB;
        private ObservableCollection<string> coefficientsX = new ObservableCollection<string>();
        private ObservableCollection<string> coefficientsY = new ObservableCollection<string>();
        private PlotModel _plotModel;

        private ObservableCollection<InterpolationResult> _interpolationResults;
        //private ApproximationResult _bestApproximation;
        private double _result;
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

        public string IntervalA
        {
            get => intervalA;
            set
            {
                intervalA = value;
                OnPropertyChanged(nameof(IntervalA));
            }
        }
        public string IntervalB
        {
            get => intervalB;
            set
            {
                intervalB = value;
                OnPropertyChanged(nameof(IntervalB));
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

        // Метод интерполяции Лагранжа
        public static InterpolationResult LagrangeInterpolation(double[] x, double[] y, double xValue)
        {
            int n = x.Length; // Количество точек
            double result = 0.0; // Результат интерполяции
            double[] errors = new double[n]; // Ошибки в узлах
            double sumSqError = 0; // Сумма квадратов ошибок

            // Вычисление значения интерполяционного многочлена в xValue
            for (int i = 0; i < n; i++)
            {
                double term = y[i]; // Начинаем с значения функции
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                    {
                        term *= (xValue - x[j]) / (x[i] - x[j]); // Умножаем на дробь Лагранжа
                    }
                }
                result += term; // Прибавляем слагаемое к сумме
            }

            // Вычисление ошибок интерполяции в узлах
            for (int i = 0; i < n; i++)
            {
                double yiInterpolated = 0.0; // Интерполированное значение в x[i]
                for (int j = 0; j < n; j++)
                {
                    double lj = 1.0; // Член Лагранжа
                    for (int k = 0; k < n; k++)
                    {
                        if (k != j)
                        {
                            lj *= (x[i] - x[k]) / (x[j] - x[k]); // Строим полином
                        }
                    }
                    yiInterpolated += y[j] * lj; // Прибавляем вклад очередного члена
                }

                double error = yiInterpolated - y[i]; // Разница между интерполяцией и точным значением
                errors[i] = error; // Сохраняем ошибку
                sumSqError += error * error; // Прибавляем квадрат ошибки
            }

            // Возвращаем объект InterpolationResult
            return new InterpolationResult
            {
                FunctionType = "Многочлен Лагранжа", // Тип функции
                Coefficients = new double[0], // Нет явных коэффициентов
                Function = (double arg) => // Функция для построения графика
                {
                    double sum = 0.0;
                    for (int i = 0; i < n; i++)
                    {
                        double li = 1.0;
                        for (int j = 0; j < n; j++)
                        {
                            if (j != i)
                                li *= (arg - x[j]) / (x[i] - x[j]); // Формула Лагранжа
                        }
                        sum += y[i] * li;
                    }
                    return sum; // Возвращаем значение функции
                },
                Errors = errors, // Ошибки по точкам
                StandardDeviation = Math.Sqrt(sumSqError / n), // Среднеквадратичное отклонение
                DeterminationCoefficient = 1 // Можно дополнительно оценивать
            };
        }

        // Метод Ньютона с разделёнными разностями
        public static InterpolationResult NewtonDividedDifferences(double[] x, double[] y)
        {
            int n = x.Length; // Количество точек
            double[,] divided = new double[n, n]; // Таблица разностей

            // Инициализация нулевого порядка (f[xi])
            for (int i = 0; i < n; i++)
                divided[i, 0] = y[i];

            // Заполнение таблицы разделённых разностей
            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < n - j; i++)
                {
                    divided[i, j] = (divided[i + 1, j - 1] - divided[i, j - 1]) / (x[i + j] - x[i]); // Формула разделённой разности
                }
            }

            double[] coefficients = new double[n]; // Коэффициенты многочлена
            for (int i = 0; i < n; i++)
                coefficients[i] = divided[0, i]; // Забираем верхнюю строку

            // Функция для построения многочлена
            Func<double, double> function = (double value) =>
            {
                double result = coefficients[0]; // Начинаем с первого коэффициента
                double product = 1.0; // Произведение членов (x - x0)(x - x1)...
                for (int i = 1; i < n; i++)
                {
                    product *= (value - x[i - 1]); // Умножаем на (x - xi)
                    result += coefficients[i] * product; // Прибавляем очередной член
                }
                return result;
            };

            double[] errors = new double[n]; // Ошибки
            double sumSq = 0; // Сумма квадратов ошибок
            for (int i = 0; i < n; i++)
            {
                double approx = function(x[i]); // Значение функции
                errors[i] = approx - y[i]; // Ошибка
                sumSq += errors[i] * errors[i]; // Квадрат ошибки
            }

            return new InterpolationResult
            {
                FunctionType = "Многочлен Ньютона с разделенными разностями",
                Coefficients = coefficients, // Коэффициенты
                Function = function, // Сама функция
                Errors = errors, // Ошибки
                StandardDeviation = Math.Sqrt(sumSq / n), // СКО
                DeterminationCoefficient = 1 // Для простоты
            };
        }

        // Метод Ньютона с конечными разностями (для равноотстоящих узлов)
        public static InterpolationResult NewtonFiniteDifferences(double[] x, double[] y)
        {
            int n = x.Length;
            double h = x[1] - x[0]; // Шаг между узлами

            // Проверка, что все узлы равноотстоящие
            for (int i = 1; i < n - 1; i++)
            {
                if (Math.Abs((x[i + 1] - x[i]) - h) > 1e-8)
                    throw new ArgumentException("Узлы не равноотстоящие — используйте метод разделённых разностей");
            }

            double[,] delta = new double[n, n]; // Таблица конечных разностей
            for (int i = 0; i < n; i++)
                delta[i, 0] = y[i]; // Первая колонка = значения функции

            // Заполнение таблицы конечных разностей
            for (int j = 1; j < n; j++)
                for (int i = 0; i < n - j; i++)
                    delta[i, j] = delta[i + 1, j - 1] - delta[i, j - 1]; // Разность предыдущих разностей

            // Построение функции Ньютона по конечным разностям
            Func<double, double> function = (double value) =>
            {
                double t = (value - x[0]) / h; // Переменная t
                double result = y[0]; // Начальное значение
                double tProduct = 1.0; // Произведение t(t-1)... для каждого i
                for (int i = 1; i < n; i++)
                {
                    tProduct *= (t - (i - 1)); // t(t-1)...(t-i+1)
                    result += (tProduct / Factorial(i)) * delta[0, i]; // Следующий член ряда
                }
                return result;
            };

            double[] errors = new double[n]; // Ошибки
            double sumSq = 0;
            for (int i = 0; i < n; i++)
            {
                double approx = function(x[i]); // Приближение
                errors[i] = approx - y[i]; // Ошибка
                sumSq += errors[i] * errors[i]; // Сумма квадратов
            }

            return new InterpolationResult
            {
                FunctionType = "Многочлен Ньютона с конечными разностями",
                Coefficients = Enumerable.Range(0, n).Select(i => delta[0, i]).ToArray(), // Коэф. — первая строка
                Function = function, // Сама функция
                Errors = errors, // Ошибки
                StandardDeviation = Math.Sqrt(sumSq / n), // СКО
                DeterminationCoefficient = 1 // Можно уточнить при желании
            };
        }


        private static long Factorial(int n)
        {
            long result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        public void GenerateFunctionData()
        {
            if (!double.TryParse(IntervalA, out double a) || !double.TryParse(IntervalB, out double b) || !int.TryParse(Size, out int n))
            {
                MessageBox.Show("Некорректные входные данные для интервала или размерности");
                return;
            }

            if (n < 2)
            {
                MessageBox.Show("Количество точек должно быть не меньше 2");
                return;
            }

            if (a >= b)
            {
                MessageBox.Show("Левая граница интервала должна быть меньше правой");
                return;
            }

            CoefficientsX.Clear();
            CoefficientsY.Clear();

            double h = (b - a) / (n - 1);
            for (int i = 0; i < n; i++)
            {
                double x = a + i * h;
                double y = CalculateFunction(x);

                CoefficientsX.Add(x.ToString("F4"));
                CoefficientsY.Add(y.ToString("F4"));
            }

            OnPropertyChanged(nameof(CoefficientsX));
            OnPropertyChanged(nameof(CoefficientsY));
        }

        private double CalculateFunction(double x)
        {
            switch (SelectedFunction)
            {
                case "sin(x)": return Math.Sin(x);
                case "x²": return x * x;
                case "ln(x)": return x > 0 ? Math.Log(x) : 0;
                default: return 0;
            }
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
