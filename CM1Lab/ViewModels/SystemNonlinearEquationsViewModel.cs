using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
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
    class SystemNonlinearEquationsViewModel : INotifyPropertyChanged
    {
        private string? accuracy;
        private string? selectedSystem;
        private string? x0;
        private string? y0;
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

        public SystemNonlinearEquationsViewModel()
        {
            PlotModel = new PlotModel { Title = "График функции" };
        }

        public string X0
        {
            get => x0;
            set { x0 = value; OnPropertyChanged(nameof(X0)); }
        }


        public string Y0
        {
            get => y0;
            set { y0 = value; OnPropertyChanged(nameof(Y0)); }
        }


        public string Accuracy
        {
            get => accuracy;
            set { accuracy = value; OnPropertyChanged(nameof(Accuracy)); }
        }

        public string SelectedSystem
        {
            get => selectedSystem;
            set { selectedSystem = value; OnPropertyChanged(nameof(SelectedSystem)); }
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
                // Проверка, выбрана ли система уравнений
                if (string.IsNullOrEmpty(SelectedSystem))
                {
                    MessageBox.Show("Система не выбрана.1");
                    return;
                }

                // Проверка корректности начальных значений x0 и y0
                if (!double.TryParse(X0, out double a) || !double.TryParse(Y0, out double b))
                {
                    MessageBox.Show("Некорректные значения интервалов x0 или y0.");
                    return;
                }

                // Проверка корректности значения точности
                if (!double.TryParse(Accuracy, out double accuracy))
                {
                    MessageBox.Show("Некорректное значение точности.");
                    return;
                }

                // Определение функций для решения системы уравнений
                Func<double, double, double> function1, function2;

                // Использование заготовленной системы уравнений
                switch (SelectedSystem.ToString())
                {
                    case "sin(x+y)-1.4x=0; x²+y²=1":
                        // Определение функций для первой системы
                        function1 = (x, y) => Math.Sin(x + y) - 1.4 * x;
                        function2 = (x, y) => Math.Sqrt(1 - x * x); // y = sqrt(1 - x^2)
                        break;
                    case "y-cos(x)=2; x+cos(y-1)=0.8":
                        // Определение функций для второй системы
                        function1 = (x, y) => Math.Cos(x) + 2;
                        function2 = (x, y) => 0.8 - Math.Cos(x - 1);
                        break;
                    default:
                        // Если система не выбрана, выводим сообщение
                        MessageBox.Show("Система не выбрана.2");
                        return;
                }

                // Вызов метода Ньютона для решения системы
                NewtonMethodForSystem(a, b, accuracy);
            }
            catch (Exception ex)
            {
                // Обработка исключений и вывод сообщения об ошибке
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод Ньютона для решения системы уравнений
        public static double[] NewtonMethodForSystem(double x0, double y0, double epsilon)
        {
            double x = x0;
            double y = y0;
            int maxIterations = 1000;
            int iteration = 0;

            // Вывод начальных приближений
            Debug.WriteLine($"Начальные приближения: x0 = {x0}, y0 = {y0}");

            // Итерационный процесс
            while (iteration < maxIterations)
            {
                // Вычисление значений функций
                double f1 = Math.Sin(x + y) - 1.4 * x;
                double f2 = x * x + y * y - 1;

                // Вычисление частных производных (матрица Якоби)
                double df1_dx = Math.Cos(x + y) - 1.4;
                double df1_dy = Math.Cos(x + y);
                double df2_dx = 2 * x;
                double df2_dy = 2 * y;

                // Вычисление определителя матрицы Якоби
                double determinant = df1_dx * df2_dy - df1_dy * df2_dx;
                if (Math.Abs(determinant) < 1e-10)
                {
                    // Если определитель близок к нулю, метод не сходится
                    Debug.WriteLine("Метод Ньютона не сошелся: определитель Якобиана близок к нулю.");
                    throw new Exception("Метод Ньютона не сошелся.");
                }

                // Решение системы для нахождения поправок Δx и Δy
                double deltaX = (-f1 * df2_dy + f2 * df1_dy) / determinant;
                double deltaY = (-f2 * df1_dx + f1 * df2_dx) / determinant;

                // Вывод отладочной информации
                Debug.WriteLine($"Итерация {iteration + 1}:");
                Debug.WriteLine($"  x = {x}, y = {y}");
                Debug.WriteLine($"  f1 = {f1}, f2 = {f2}");
                Debug.WriteLine($"  Δx = {deltaX}, Δy = {deltaY}");
                Debug.WriteLine($"  Вектор погрешностей: (|Δx| = {Math.Abs(deltaX)}, |Δy| = {Math.Abs(deltaY)})");

                // Обновление приближений
                x += deltaX;
                y += deltaY;

                // Проверка критерия завершения
                if (Math.Abs(deltaX) < epsilon && Math.Abs(deltaY) < epsilon)
                {
                    // Вывод результата и количества итераций
                    Debug.WriteLine($"Решение найдено за {iteration + 1} итераций: x = {x}, y = {y}");
                    return new double[] { x, y };
                }

                iteration++;
            }

            // Если метод не сошелся за максимальное число итераций
            Debug.WriteLine("Метод Ньютона не сошелся за максимальное число итераций.");
            throw new Exception("Метод Ньютона не сошелся за заданное количество итераций.");
        }

        // Метод для вывода результатов
        public void PrintResults(double[] root, int iterations, double[] errors)
        {
            // Вывод решения, количества итераций и вектора погрешностей
            Debug.WriteLine($"Решение: x = {root[0]}, y = {root[1]}");
            Debug.WriteLine($"Количество итераций: {iterations}");
            Debug.WriteLine($"Вектор погрешностей: [{errors[0]}, {errors[1]}]");
        }

        // Метод для проверки сходимости
        public bool CheckConvergence(double x0, double y0)
        {
            // Проверка условия сходимости для метода простой итерации
            double df1_dx = Math.Cos(x0 + y0) - 1.4;
            double df1_dy = Math.Cos(x0 + y0);
            double df2_dx = 2 * x0;
            double df2_dy = 2 * y0;

            // Вычисление нормы матрицы Якоби
            double norm = Math.Max(Math.Abs(df1_dx) + Math.Abs(df1_dy), Math.Abs(df2_dx) + Math.Abs(df2_dy));
            return norm < 1;
        }

        // Метод для построения графика
        public void BuildGraphic()
        {
            // Очистка предыдущих данных
            PlotModel.Series.Clear();
            PlotModel.Axes.Clear();

            // Проверка корректности начальных значений x0 и y0
            if (!double.TryParse(X0, out double a) || !double.TryParse(Y0, out double b))
            {
                MessageBox.Show("Некорректные значения интервалов x0 или y0.");
                return;
            }

            // Определение функций для построения графика
            Func<double, double, double> function1, function2;
            switch (SelectedSystem.ToString())
            {
                case "sin(x+y)-1.4x=0; x²+y²=1":
                    // Определение функций для первой системы
                    function1 = (x, y) => Math.Sin(x + y) - 1.4 * x;
                    function2 = (x, y) => x * x + y * y - 1;
                    break;
                case "y-cos(x)=2; x+cos(y-1)=0.8":
                    // Определение функций для второй системы
                    function1 = (x, y) => Math.Cos(x) + 2;
                    function2 = (x, y) => 0.8 - Math.Cos(y - 1);
                    break;
                default:
                    // Если система не выбрана, выводим сообщение
                    MessageBox.Show("Система не выбрана.");
                    return;
            }

            // Вывод выбранной системы для отладки
            Debug.WriteLine(SelectedSystem.ToString());

            // Создание серий для графиков
            var series1 = new LineSeries { Title = "f1(x, y)", Color = OxyColors.Blue };
            var series2 = new LineSeries { Title = "f2(x, y)", Color = OxyColors.Red };

            // Заполнение серий точками
            double step = (b - a) / 100; // 100 точек на интервале
            for (double x = a; x <= b; x += step)
            {
                series1.Points.Add(new DataPoint(x, function1(x, 0))); // передаем y = 0
                series2.Points.Add(new DataPoint(x, function2(x, 0)));
            }

            // Добавление серий в модель
            PlotModel.Series.Add(series1);
            PlotModel.Series.Add(series2);

            // Настройка осей
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "x" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "y" });

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
                            Accuracy = value; // Вызывает OnPropertyChanged
                            break;
                        case "x0":
                            X0 = value; // Вызывает OnPropertyChanged
                            break;
                        case "y0":
                            Y0 = value; // Вызывает OnPropertyChanged
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
