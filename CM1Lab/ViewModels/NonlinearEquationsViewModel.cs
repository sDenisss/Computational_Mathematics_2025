using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Linq.Expressions;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;

namespace CM1Lab.ViewModels
{
    class NonlinearEquationsViewModel : INotifyPropertyChanged
    {
        private string? accuracy;
        private string? selectedMethod;
        private string? equationFormula;
        private string? selectedFunction;

        private double? bisectionRoot;
        private double? secantRoot;
        private double? simpleIterationRoot;

        private string? aInterval;
        private string? bInterval;
        private string? x0;
        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public NonlinearEquationsViewModel()
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

        public string EquationFormula
        {
            get => equationFormula;
            set { equationFormula = value; OnPropertyChanged(nameof(EquationFormula)); }
        }
        public string Ainterval
        {
            get => aInterval;
            set { aInterval = value; OnPropertyChanged(nameof(Ainterval)); }
        }

        public string Binterval
        {
            get => bInterval;
            set { bInterval = value; OnPropertyChanged(nameof(Binterval)); }
        }

        public string X0
        {
            get => x0;
            set { x0 = value; OnPropertyChanged(nameof(X0)); }
        }

        // Свойство для корня метода половинного деления
        public double? BisectionRoot
        {
            get => bisectionRoot;
            set
            {
                bisectionRoot = value;
                OnPropertyChanged(nameof(BisectionRoot));
            }
        }

        // Свойство для корня метода секущих
        public double? SecantRoot
        {
            get => secantRoot;
            private set
            {
                secantRoot = value;
                OnPropertyChanged(nameof(SecantRoot));
            }
        }

        // Свойство для корня метода простой итерации
        public double? SimpleIterationRoot
        {
            get => simpleIterationRoot;
            private set
            {
                simpleIterationRoot = value;
                OnPropertyChanged(nameof(SimpleIterationRoot));
            }
        }

        public string Accuracy
        {
            get => accuracy;
            set { accuracy = value; OnPropertyChanged(nameof(Accuracy)); }
        }
        public string SelectedMethod
        {
            get => selectedMethod;
            set { selectedMethod = value; OnPropertyChanged(nameof(SelectedMethod)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NonLinearEquationsSolve()
        {
            try
            {
                // Инициализация переменных для хранения корней, найденных разными методами
                BisectionRoot = null;
                SecantRoot = null;
                SimpleIterationRoot = null;

                // Проверка, выбран ли метод решения
                if (string.IsNullOrEmpty(SelectedMethod))
                {
                    MessageBox.Show("Метод не выбран.");
                    return;
                }

                // Проверка, выбрано ли уравнение для решения
                if (string.IsNullOrEmpty(SelectedFunction))
                {
                    MessageBox.Show("Уравнение не выбрано.");
                    return;
                }

                // Проверка корректности введенных интервалов a и b
                if (!double.TryParse(Ainterval, out double a) || !double.TryParse(Binterval, out double b))
                {
                    MessageBox.Show("Некорректные значения интервалов a или b.");
                    return;
                }

                // Проверка корректности введенной точности
                if (!double.TryParse(Accuracy, out double accuracy))
                {
                    MessageBox.Show("Некорректное значение точности.");
                    return;
                }

                // Определение функции для решения
                Func<double, double> function;
                if (SelectedFunction == "Впиши в поле")
                {
                    // Проверка, введено ли уравнение в поле
                    if (string.IsNullOrEmpty(EquationFormula))
                    {
                        MessageBox.Show("Введите уравнение в поле.");
                        return;
                    }
                    // Парсинг пользовательского уравнения
                    function = ParseFormula(EquationFormula);
                }
                else
                {
                    // Использование заготовленного уравнения
                    switch (SelectedFunction)
                    {
                        case "f(x)=x²-4":
                            function = x => x * x - 4;
                            break;
                        case "f(x)=ln(x)":
                            function = x => Math.Log(x);
                            break;
                        case "f(x)=3x²-6x+2":
                            function = x => 3 * x * x - 6 * x + 2;
                            break;
                        default:
                            MessageBox.Show("Уравнение не выбрано.");
                            return;
                    }
                }

                // Решение уравнения выбранным методом
                switch (SelectedMethod)
                {
                    case "Метод половинного деления":
                        BisectionRoot = BisectionMethod(a, b, accuracy, function);
                        break;
                    case "Метод секущих":
                        SecantRoot = SecantMethod(a, b, accuracy, function);
                        break;
                    case "Метод простой итерации":
                        SimpleIterationRoot = SimpleIterationMethod(a, b, accuracy, function);
                        break;
                    default:
                        MessageBox.Show("Метод не выбран.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и вывод сообщения об ошибке
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Функция для вычисления производной
        public static double Derivative(Func<double, double> f, double x, double h = 1e-4)
        {
            // Вывод значения производной в отладку
            Debug.WriteLine((f(x + h) - f(x - h)) / (2 * h));
            // Возврат значения производной
            return (f(x + h) - f(x - h)) / (2 * h);
        }

        // Метод для преобразования строки формулы в лямбда-функцию
        public static Func<double, double> ParseFormula(string formula)
        {
            try
            {
                // Замена всех x^n на Math.Pow(x, n)
                string pattern = @"x\^(\d+)";
                string replacement = "Math.Pow(x, $1)";
                string csharpFormula = Regex.Replace(formula, pattern, replacement);

                // Удаление ключевых слов и точек с запятой
                csharpFormula = csharpFormula.Replace("return", "").Replace(";", "").Trim();

                // Создание параметра x
                var xParam = System.Linq.Expressions.Expression.Parameter(typeof(double), "x");

                // Компиляция выражения
                var compiledExpression = DynamicExpressionParser.ParseLambda(new[] { xParam }, null, csharpFormula);

                // Преобразование в Func<double, double>
                var function = (Func<double, double>)compiledExpression.Compile();
                return function;
            }
            catch (Exception ex)
            {
                // Обработка ошибки парсинга формулы
                MessageBox.Show($"Ошибка при парсинге формулы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return x => double.NaN; // Возврат функции, возвращающей NaN при ошибке
            }
        }

        // Метод половинного деления
        public static double? BisectionMethod(double a, double b, double epsilon, Func<double, double> function)
        {
            try
            {
                // Проверка, что функция имеет разные знаки на концах интервала
                if (function(a) * function(b) >= 0)
                {
                    MessageBox.Show("Функция должна иметь разные знаки на концах интервала.");
                    throw new ArgumentException("Функция должна иметь разные знаки на концах интервала.");
                }

                double c;
                int iteration = 0;

                // Итерационный процесс поиска корня
                while ((b - a) >= epsilon)
                {
                    c = (a + b) / 2;
                    iteration++;

                    // Вывод информации о текущей итерации
                    Debug.WriteLine($"Итерация {iteration}: a = {a}, b = {b}, c = {c}, f(c) = {function(c)}");

                    if (function(c) == 0.0)
                        break;

                    if (function(c) * function(a) < 0)
                        b = c;
                    else
                        a = c;
                }
                double result = (a + b) / 2;
                // Вывод результата и количества итераций
                Debug.WriteLine($"Метод половинного деления: корень = {result}, итераций = {iteration}");
                return result;
            }
            catch (Exception ex)
            {
                // Обработка ошибки в методе половинного деления
                Debug.WriteLine($"Ошибка в методе половинного деления: {ex.Message}");
                return null;
            }
        }

        // Метод секущих
        public static double? SecantMethod(double x0, double x1, double epsilon, Func<double, double> function)
        {
            try
            {
                double x = x1;
                double xPrev = x0;
                int iteration = 0;

                // Итерационный процесс поиска корня
                while (Math.Abs(function(x)) >= epsilon)
                {
                    double xNext = x - function(x) * (x - xPrev) / (function(x) - function(xPrev));
                    xPrev = x;
                    x = xNext;
                    iteration++;

                    // Вывод информации о текущей итерации
                    Debug.WriteLine($"Итерация {iteration}: x = {x}, f(x) = {function(x)}");
                }
                // Вывод результата и количества итераций
                Debug.WriteLine($"Метод секущих: корень = {x}, итераций = {iteration}");
                return x;
            }
            catch (Exception ex)
            {
                // Обработка ошибки в методе секущих
                Debug.WriteLine($"Ошибка в методе секущих: {ex.Message}");
                return null;
            }
        }

        // Метод простой итерации
        public static double? SimpleIterationMethod(double a, double b, double accuracy, Func<double, double> function)
        {
            try
            {
                double x0 = (a - b) / 2;
                double x = x0;
                double xNext;
                int maxIterations = 1000;
                int iteration = 0;

                // Вычисление параметров для метода простой итерации
                double M = Math.Abs(Derivative(function, x));
                double lambda = 1.0 / M;

                Func<double, double> phi = x => x - lambda * function(x);
                Func<double, double> phiDerivative = x => 1 - lambda * Derivative(function, x);

                // Проверка условия сходимости
                if (Math.Abs(phiDerivative(x0)) >= 1)
                {
                    MessageBox.Show("Условие сходимости не выполнено. Выберите другую функцию phi(x).");
                    throw new InvalidOperationException("Условие сходимости не выполнено. Выберите другую функцию phi(x).");
                }

                // Итерационный процесс поиска корня
                do
                {
                    xNext = phi(x);
                    iteration++;

                    // Вывод информации о текущей итерации
                    Debug.WriteLine($"Итерация {iteration}: x = {x}, xNext = {xNext}, f(xNext) = {function(xNext)}");

                    if (Math.Abs(xNext - x) < accuracy)
                        break;

                    x = xNext;

                    if (iteration >= maxIterations)
                        throw new Exception("Метод не сошелся за максимальное число итераций.");
                } while (true);

                // Вывод результата и количества итераций
                Debug.WriteLine($"Метод простой итерации: корень = {xNext}, итераций = {iteration}");
                return xNext;
            }
            catch (Exception ex)
            {
                // Обработка ошибки в методе простой итерации
                Debug.WriteLine($"Ошибка в методе простой итерации: {ex.Message}");
                return null;
            }
        }

        // Метод для вычисления производной
        private static double Derivative(Func<double, double> function, double x)
        {
            double h = 1e-5;
            return (function(x + h) - function(x)) / h;
        }

        // Метод для построения графика
        public void BuildGraphic()
        {
            // Очистка предыдущих данных
            PlotModel.Series.Clear();
            PlotModel.Axes.Clear();

            // Проверка корректности введенных интервалов a и b
            if (!double.TryParse(Ainterval, out double a) || !double.TryParse(Binterval, out double b))
            {
                MessageBox.Show("Некорректные значения интервалов a или b.");
                return;
            }

            // Определение функции для построения графика
            Func<double, double> function;
            if (SelectedFunction == "Впиши в поле" && !string.IsNullOrEmpty(EquationFormula))
            {
                // Парсинг пользовательского уравнения
                function = ParseFormula(EquationFormula);
            }
            else
            {
                // Использование заготовленного уравнения
                switch (SelectedFunction)
                {
                    case "f(x)=x²-4":
                        function = x => x * x - 4;
                        break;
                    case "f(x)=ln(x)":
                        function = x => Math.Log(x);
                        break;
                    case "f(x)=3x²-6x+2":
                        function = x => 3 * x * x - 6 * x + 2;
                        break;
                    default:
                        MessageBox.Show("Уравнение не выбрано.");
                        return;
                }
            }

            // Создание серии для графика функции
            var series = new LineSeries
            {
                Title = "f(x)",
                Color = OxyColors.Blue
            };

            // Заполнение серии точками
            double step = (b - a) / 100; // 100 точек на интервале
            for (double x = a; x <= b; x += step)
            {
                series.Points.Add(new OxyPlot.DataPoint(x, function(x)));
            }

            // Добавление серии в модель
            PlotModel.Series.Add(series);

            // Настройка осей
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "x" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "f(x)" });

            // Обновление графика
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
                        case "Точность":
                            Accuracy = value;
                            break;
                        case "Интервал(a)":
                            Ainterval = value;
                            break;
                        case "Интервал(b)":
                            Binterval = value;
                            break;
                        case "Уравнение":
                            EquationFormula = value;
                            break;
                        case "x0":
                            X0 = value;
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
