using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CM1Lab.ViewModels
{
    class NumericalIntegrationViewModel : INotifyPropertyChanged
    {

        private string? accuracy;
        private string? selectedMethod;
        private string? equationFormula;
        private string? selectedFunction;

        private double? leftRectangle;
        private double? rightRectangle;
        private double? middleRectangle;
        private double? trapezoid;
        private double? simpson;

        private string? aInterval;
        private string? bInterval;
        private string? nParts;
        private string? breakPoint;
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

        public NumericalIntegrationViewModel()
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

        public string NParts
        {
            get => nParts;
            set { nParts = value; OnPropertyChanged(nameof(NParts)); }
        }

        public string BreakPoint
        {
            get => breakPoint;
            set { breakPoint = value; OnPropertyChanged(nameof(BreakPoint)); }
        }

        // Свойство для корня метода половинного деления
        public double? LeftRectangle
        {
            get => leftRectangle;
            set
            {
                leftRectangle = value;
                OnPropertyChanged(nameof(LeftRectangle));
            }
        }

        // Свойство для корня метода секущих
        public double? RightRectangle
        {
            get => rightRectangle;
            private set
            {
                rightRectangle = value;
                OnPropertyChanged(nameof(RightRectangle));
            }
        }

        // Свойство для корня метода простой итерации
        public double? MiddleRectangle
        {
            get => middleRectangle;
            private set
            {
                middleRectangle = value;
                OnPropertyChanged(nameof(MiddleRectangle));
            }
        }

        public double? Trapezoid
        {
            get => trapezoid;
            private set
            {
                trapezoid = value;
                OnPropertyChanged(nameof(Trapezoid));
            }
        }

        public double? Simpson
        {
            get => simpson;
            private set
            {
                simpson = value;
                OnPropertyChanged(nameof(Simpson));
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

        public void NumericalIntegrationSolve()
        {
            try
            {
                // Инициализация переменных для хранения корней, найденных разными методами
                LeftRectangle = null;
                RightRectangle = null;
                MiddleRectangle = null;
                Trapezoid = null;
                Simpson = null;

                // Проверка, выбран ли метод решения
                if (string.IsNullOrEmpty(SelectedMethod))
                {
                    MessageBox.Show("Метод не выбран.");
                    return;
                }

                // Проверка, выбрано ли уравнение для решения
                if (string.IsNullOrEmpty(SelectedFunction))
                {
                    MessageBox.Show("Уравнение не выбрано1.");
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

                if (!double.TryParse(NParts, out double nParts))
                {
                    MessageBox.Show("Некорректное значение частей.");
                    return;
                }

                if (!double.TryParse(BreakPoint, out double breakPoint))
                {
                    MessageBox.Show("Некорректное значение разрыва точки.");
                    return;
                }

                // Определение функции для решения
                Func<double, double> function;

                // Использование заготовленного уравнения
                switch (SelectedFunction)
                {
                    case "∫(3x³-4x²+5x-16)dx":
                        function = x => 3 * Math.Pow(x, 3) - 4 * Math.Pow(x, 2) + 5 * x - 16;
                        break;
                    case "∫x²dx":
                        function = x => Math.Pow(x, 2);
                        break;
                    case "∫sin(x)dx":
                        function = x => Math.Sin(x);
                        break;
                    case "∫(2x³-3x²+4x-22)dx":
                        function = x => 2 * Math.Pow(x, 3) - 3 * Math.Pow(x, 2) + 4 * x - 22;
                        break;
                    case "∫(1/√x)dx":
                        function = x => 1 / Math.Sqrt(x);
                        break;
                    case "∫(1/|x-0,5|)dx":
                        function = x => 1 / Math.Abs(x-0.5);
                        break;
                    default:
                        MessageBox.Show("Уравнение не выбрано2.");
                        return;
                }
                //}

                // Решение уравнения выбранным методом
                // Точка разрыва (например, a, b или точка внутри интервала)
                //breakPoint = a; // Пример: разрыв в точке a

                double? result = null;
                // Решение уравнения выбранным методом
                switch (SelectedMethod)
                {
                    case "Метод левого прямоугольника":
                        LeftRectangle = ImproperIntegral(function, a, b, breakPoint, accuracy, LeftRectangles, 2);
                        result = LeftRectangle;
                        break;
                    case "Метод правого прямоугольника":
                        RightRectangle = ImproperIntegral(function, a, b, breakPoint, accuracy, RightRectangles, 2);
                        result = RightRectangle;
                        break;
                    case "Метод среднего прямоугольника":
                        MiddleRectangle = ImproperIntegral(function, a, b, breakPoint, accuracy, MidRectangles, 2);
                        result = MiddleRectangle;
                        break;
                    case "Метод трапеций":
                        Trapezoid = ImproperIntegral(function, a, b, breakPoint, accuracy, Trapezoidal, 2);
                        result = Trapezoid;
                        break;
                    case "Метод Симпсона":
                        Simpson = ImproperIntegral(function, a, b, breakPoint, accuracy, SimpsonMethod, 4);
                        result = Simpson;
                        break;
                    default:
                        MessageBox.Show("Метод не выбран.");
                        break;
                }
                // Обработка результата
                if (double.IsNaN(result ?? double.NaN) || double.IsInfinity(result ?? double.NaN))
                {
                    MessageBox.Show("Интеграл расходится и не имеет решения.");
                }
                else
                {
                    // Вывод результата
                    Debug.WriteLine($"Результат: {result}");
                }
                //switch (SelectedMethod)
                //{
                //    case "Метод левого прямоугольника":
                //        LeftRectangle = LeftRectangles(function, a, b, nParts);
                //        Debug.WriteLine("Метод левых прямоугольников:");
                //        double? leftRectResult = RungeRule(function, a, b, accuracy, LeftRectangles, 2);
                //        break;
                //    case "Метод правого прямоугольника":
                //        RightRectangle = RightRectangles(function, a, b, nParts);
                //        Debug.WriteLine("Метод правых прямоугольников:");
                //        double? rightRectResult = RungeRule(function, a, b, accuracy, RightRectangles, 2);
                //        break;
                //    case "Метод среднего прямоугольника":
                //        MiddleRectangle = MidRectangles(function, a, b, nParts);
                //        Debug.WriteLine("Метод средних прямоугольников:");
                //        double? midRectResult = RungeRule(function, a, b, accuracy, MidRectangles, 2);
                //        break;
                //    case "Метод трапеций":
                //        Trapezoid = Trapezoidal(function, a, b, nParts);
                //        Debug.WriteLine("Метод трапеций:");
                //        double? trapezoidalResult = RungeRule(function, a, b, accuracy, Trapezoidal, 2);
                //        break;
                //    case "Метод Симпсона":
                //        Simpson = SimpsonMethod(function, a, b, nParts);
                //        Debug.WriteLine("Метод Симпсона:");
                //        double? simpsonResult = RungeRule(function, a, b, accuracy, SimpsonMethod, 4);
                //        break;
                //    default:
                //        MessageBox.Show("Метод не выбран.");
                //        break;
                //}
            }
            catch (Exception ex)
            {
                // Обработка исключений и вывод сообщения об ошибке
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод левых прямоугольников для численного интегрирования
        public static double? LeftRectangles(Func<double, double> f, double a, double b, double n)
        {
            // Вычисляем шаг разбиения (ширина каждого прямоугольника)
            double h = (b - a) / n;

            // Инициализируем переменную для накопления суммы значений функции
            double sum = 0;

            // Цикл по всем разбиениям
            for (int i = 0; i < n; i++)
            {
                // Добавляем значение функции в левой точке каждого отрезка
                sum += f(a + i * h);
            }

            // Возвращаем результат: произведение шага на сумму значений
            return h * sum;
        }

        // Метод правых прямоугольников для численного интегрирования
        public static double? RightRectangles(Func<double, double> f, double a, double b, double n)
        {
            // Вычисляем шаг разбиения (ширина каждого прямоугольника)
            double h = (b - a) / n;

            // Инициализируем переменную для накопления суммы значений функции
            double sum = 0;

            // Цикл по всем разбиениям
            for (int i = 1; i <= n; i++)
            {
                // Добавляем значение функции в правой точке каждого отрезка
                sum += f(a + i * h);
            }

            // Возвращаем результат: произведение шага на сумму значений
            return h * sum;
        }

        // Метод средних прямоугольников для численного интегрирования
        public static double? MidRectangles(Func<double, double> f, double a, double b, double n)
        {
            // Вычисляем шаг разбиения (ширина каждого прямоугольника)
            double h = (b - a) / n;

            // Инициализируем переменную для накопления суммы значений функции
            double sum = 0;

            // Цикл по всем разбиениям
            for (int i = 0; i < n; i++)
            {
                // Добавляем значение функции в средней точке каждого отрезка
                sum += f(a + (i + 0.5) * h);
            }

            // Возвращаем результат: произведение шага на сумму значений
            return h * sum;
        }

        // Метод трапеций для численного интегрирования
        public static double? Trapezoidal(Func<double, double> f, double a, double b, double n)
        {
            // Вычисляем шаг разбиения (ширина каждой трапеции)
            double h = (b - a) / n;

            // Инициализируем сумму как среднее значение функции на концах интервала
            double sum = (f(a) + f(b)) / 2;

            // Цикл по всем внутренним точкам разбиения
            for (int i = 1; i < n; i++)
            {
                // Добавляем значение функции в каждой внутренней точке
                sum += f(a + i * h);
            }

            // Возвращаем результат: произведение шага на сумму значений
            return h * sum;
        }

        // Метод Симпсона для численного интегрирования
        public static double? SimpsonMethod(Func<double, double> f, double a, double b, double n)
        {
            // Проверяем, что количество разбиений четное (метод Симпсона требует четного n)
            if (n % 2 != 0) throw new ArgumentException("n должно быть четным");

            // Вычисляем шаг разбиения (ширина каждого отрезка)
            double h = (b - a) / n;

            // Инициализируем сумму значениями функции на концах интервала
            double sum = f(a) + f(b);

            // Цикл по всем внутренним точкам разбиения
            for (int i = 1; i < n; i++)
            {
                // Для нечетных точек добавляем значение функции, умноженное на 4
                // Для четных точек добавляем значение функции, умноженное на 2
                sum += (i % 2 == 0) ? 2 * f(a + i * h) : 4 * f(a + i * h);
            }

            // Возвращаем результат: произведение шага на сумму значений, деленное на 3
            return h / 3 * sum;
        }

        // Метод для оценки погрешности по правилу Рунге
        public static double? RungeRule(Func<double, double> f, double a, double b, double precision, Func<Func<double, double>, double, double, double, double?> method, int k)
        {
            // Начальное количество разбиений
            int n = 4;

            // Вычисляем результат с текущим количеством разбиений n
            double? result1 = method(f, a, b, n);

            // Вычисляем результат с удвоенным количеством разбиений (2n)
            double? result2 = method(f, a, b, 2 * n);

            // Оцениваем погрешность по правилу Рунге: error = |result2 - result1| / (2^k - 1)
            double error = Math.Abs((double)(result2 - result1)) / (Math.Pow(2, k) - 1);

            // Выводим текущие значения n, результат и погрешность в консоль отладки
            Debug.WriteLine($"n = {n}, Результат = {result1}, Погрешность = {error}");

            // Пока погрешность больше требуемой точности, продолжаем уточнять результат
            while (error > precision)
            {
                // Удваиваем количество разбиений
                n *= 2;

                // Обновляем result1 (предыдущий результат)
                result1 = result2;

                // Вычисляем новый результат с удвоенным количеством разбиений
                result2 = method(f, a, b, 2 * n);

                // Пересчитываем погрешность
                error = Math.Abs((double)(result2 - result1)) / (Math.Pow(2, k) - 1);

                // Выводим текущие значения n, результат и погрешность в консоль отладки
                Debug.WriteLine($"n = {n}, Результат = {result2}, Погрешность = {error}");
            }

            // Выводим финальный результат и количество разбиений в консоль отладки
            Debug.WriteLine($"Финальный результат: {result2}, n = {n}");

            // Возвращаем финальный результат
            return result2;
        }

        public static bool CheckConvergence(Func<double, double> f, double a, double b, double breakPoint, double epsilon = 1e-3)
        {
            if (f(breakPoint) == double.NaN || f(breakPoint) == double.PositiveInfinity || f(breakPoint) == double.NegativeInfinity)
            {
                return false;
            }
            //// Если разрыв в точке a
            //if (Math.Abs(breakPoint - a) < epsilon)
            //{
            //    // Для 1/|x-a| интеграл ∫(a to b) 1/|x-a| dx расходится
            //    return false;
            //}

            //// Если разрыв в точке b
            //if (Math.Abs(breakPoint - b) < epsilon)
            //{
            //    // Для 1/|x-b| интеграл ∫(a to b) 1/|x-b| dx расходится
            //    return false;
            //}

            //// Если разрыв внутри интервала
            //if (breakPoint > a && breakPoint < b)
            //{
            //    // Для 1/|x-c| интеграл расходится в точке c
            //    return false;
            //}

            // Если разрыв вне интервала, интеграл сходится
            return true;
            //// Если разрыв в точке a
            //if (Math.Abs(breakPoint - a) < epsilon)
            //{
            //    double limit = Limit(f, a, epsilon);
            //    return !double.IsInfinity(limit) && !double.IsNaN(limit);
            //}

            //// Если разрыв в точке b
            //if (Math.Abs(breakPoint - b) < epsilon)
            //{
            //    double limit = Limit(f, b, epsilon);
            //    return !double.IsInfinity(limit) && !double.IsNaN(limit);
            //}

            //// Если разрыв внутри интервала
            //if (breakPoint > a && breakPoint < b)
            //{
            //    double limitLeft = Limit(f, breakPoint, epsilon, fromLeft: true);
            //    double limitRight = Limit(f, breakPoint, epsilon, fromLeft: false);
            //    return !double.IsInfinity(limitLeft) && !double.IsNaN(limitLeft) &&
            //           !double.IsInfinity(limitRight) && !double.IsNaN(limitRight);
            //}

            //// Если разрыв вне интервала, интеграл сходится
            //return true;
        }

        // Метод для вычисления предела функции в точке
        private static double Limit(Func<double, double> f, double point, double epsilon, bool fromLeft = true)
        {
            double h = epsilon;
            double x = fromLeft ? point + h : point - h;
            return f(x);
        }

        public static double? ImproperIntegral(Func<double, double> f, double a, double b, double breakPoint, double accuracy, Func<Func<double, double>, double, double, double, double?> method, int k)
        {
            // Проверка сходимости
            if (!CheckConvergence(f, a, b, breakPoint))
            {
                MessageBox.Show("Интеграл не существует (расходится).");
                return double.NaN; // Возвращаем NaN для расходящихся интегралов
            }

            // Если разрыв в точке a
            if (Math.Abs(breakPoint - a) < 1e-6)
            {
                double newA = a + accuracy; // Сдвигаем нижний предел на малую величину
                return RungeRule(f, newA, b, accuracy, method, k);
            }

            // Если разрыв в точке b
            if (Math.Abs(breakPoint - b) < 1e-6)
            {
                double newB = b - accuracy; // Сдвигаем верхний предел на малую величину
                return RungeRule(f, a, newB, accuracy, method, k);
            }

            // Если разрыв внутри интервала
            if (breakPoint > a && breakPoint < b)
            {
                // Вычисляем интеграл от a до breakPoint
                double? integral1 = RungeRule(f, a, breakPoint - accuracy, accuracy, method, k);
                // Вычисляем интеграл от breakPoint до b
                double? integral2 = RungeRule(f, breakPoint + accuracy, b, accuracy, method, k);
                return integral1 + integral2;
            }

            // Если разрыв вне интервала, вычисляем обычный интеграл
            return RungeRule(f, a, b, accuracy, method, k);
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

            if (!double.TryParse(NParts, out double nParts))
            {
                MessageBox.Show("Некорректные значения количества частей n.");
                return;
            }

            double h = (b-a) / nParts;

            // Определение функции для построения графика
            Func<double, double> function;
            //if (SelectedFunction == "Впиши в поле" && !string.IsNullOrEmpty(EquationFormula))
            //{
            //    // Парсинг пользовательского уравнения
            //    function = ParseFormula(EquationFormula);
            //}
            
            switch (SelectedFunction)
            {
                case "∫(3x³-4x²+5x-16)dx":
                    function = x => 3 * Math.Pow(x, 3) - 4 * Math.Pow(x, 2) + 5 * x - 16;
                    break;
                case "∫x²dx":
                    function = x => Math.Pow(x, 2);
                    break;
                case "∫sin(x)dx":
                    function = x => Math.Sin(x);
                    break;
                case "∫(2x³-3x²+4x-22)dx":
                    function = x => 2 * Math.Pow(x, 3) - 3 * Math.Pow(x, 2) + 4 * x - 22;
                    break;
                case "∫(1/√x)dx":
                    function = x => 1 / Math.Sqrt(x);
                    break;
                case "∫(1/|x-0,5|)dx":
                    function = x => 1 / Math.Abs(x - 0.5);
                    break;
                default:
                    MessageBox.Show("Уравнение не выбрано3.");
                    return;
            }
            

            // Создание серии для графика функции
            var lineSeries = new LineSeries
            {
                Title = "f(x)",
                Color = OxyColors.Blue
            };

            // Создание серии для точек (чёрные точки)
            var scatterSeries = new ScatterSeries
            {
                Title = "Точки",
                MarkerType = MarkerType.Circle, // Тип маркера (круг)
                MarkerSize = 3, // Размер маркера
                MarkerFill = OxyColors.Black // Цвет маркера (чёрный)
            };



            switch (SelectedMethod)
            {
                case "Метод левого прямоугольника":
                    // Заполнение серий точками
                    double step = (b - a) / 100; // 100 точек на интервале
                    b += step;

                    for (double x = a; x <= b; x += step)
                    {
                        double y = function(x);
                        // Добавляем точку в линию (график функции)
                        lineSeries.Points.Add(new OxyPlot.DataPoint(x, y));
                    }

                    b -= step;

                    for (double x = a; x < b; x += h)
                    {
                        double y = function(x);
                        // Добавляем точку в scatterSeries (чёрные точки)
                        scatterSeries.Points.Add(new ScatterPoint(x, y));//??
                    }
                    break;
                case "Метод правого прямоугольника":
                    // Заполнение серий точками
                    step = (b - a) / 100; // 100 точек на интервале
                    b += step;

                    for (double x = a; x <= b; x += step)
                    {
                        double y = function(x);
                        // Добавляем точку в линию (график функции)
                        lineSeries.Points.Add(new OxyPlot.DataPoint(x, y));
                    }

                    b -= step;

                    for (double x = b; x > a; x -= h)
                    {
                        double y = function(x);
                        // Добавляем точку в scatterSeries (чёрные точки)
                        scatterSeries.Points.Add(new ScatterPoint(x, y));//??
                    }
                    break;
                case "Метод среднего прямоугольника":
                    step = (b - a) / 100; // 100 точек на интервале
                    b += step;

                    for (double x = a; x <= b; x += step)
                    {
                        double y = function(x);
                        // Добавляем точку в линию (график функции)
                        lineSeries.Points.Add(new OxyPlot.DataPoint(x, y));
                    }

                    b -= step;

                    for (double x = a + (h / 2); x <= b; x += h)
                    {
                        double y = function(x);
                        // Добавляем точку в scatterSeries (чёрные точки)
                        scatterSeries.Points.Add(new ScatterPoint(x, y));//??
                    }
                    break;
                case "Метод трапеций":
                    step = (b - a) / 100; // 100 точек на интервале
                    b += step;

                    for (double x = a; x <= b; x += step)
                    {
                        double y = function(x);
                        // Добавляем точку в линию (график функции)
                        lineSeries.Points.Add(new OxyPlot.DataPoint(x, y));
                    }
                    b -= step;
                    for (double x = a; x <= b; x += h)
                    {
                        double y = function(x);
                        // Добавляем точку в scatterSeries (чёрные точки)
                        scatterSeries.Points.Add(new ScatterPoint(x, y));//??
                    }
                    break;
                case "Метод Симпсона":
                    step = (b - a) / 100; // 100 точек на интервале
                    b += step;
                    for (double x = a; x <= b; x += step)
                    {
                        double y = function(x);
                        // Добавляем точку в линию (график функции)
                        lineSeries.Points.Add(new OxyPlot.DataPoint(x, y));
                    }
                    b -= step;
                    for (double x = a; x <= b; x += h)
                    {
                        double y = function(x);
                        // Добавляем точку в scatterSeries (чёрные точки)
                        scatterSeries.Points.Add(new ScatterPoint(x, y));//??
                    }
                    break;
                default:
                    MessageBox.Show("Метод не выбран.");
                    break;
            }

            // Добавление серий в модель
            PlotModel.Series.Add(lineSeries);
            PlotModel.Series.Add(scatterSeries);

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
                        //case "Уравнение":
                        //    EquationFormula = value;
                        //    break;
                        case "Части":
                            NParts = value;
                            break;
                        case "Разрыв":
                            BreakPoint = value;
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
