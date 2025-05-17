using CM1Lab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM1Lab.ViewModels // Объявление пространства имён, где лежит класс методов аппроксимации
{
    public static class ApproximationMethods // Объявление статического класса с методами аппроксимации
    {
        // Метод линейной аппроксимации: y = a * x + b
        public static ApproximationResult LinearApproximation(double[] x, double[] y) // Метод принимает массивы x и y
        {
            ValidateInput(x, y); // Проверка входных данных на корректность

            int n = x.Length; // Количество точек
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0; // Инициализация переменных для подсчёта сумм

            for (int i = 0; i < n; i++) // Проходим по всем точкам
            {
                sumX += x[i]; // Сумма всех X
                sumY += y[i]; // Сумма всех Y
                sumXY += x[i] * y[i]; // Сумма произведений X на Y
                sumX2 += x[i] * x[i]; // Сумма квадратов X
            }

            double delta = sumX2 * n - sumX * sumX; // Вычисляем определитель
            if (Math.Abs(delta) < 1e-10) // Проверяем, не вырождена ли система
                throw new InvalidOperationException("Система уравнений вырождена"); // Генерируем исключение, если да

            double a = (sumXY * n - sumX * sumY) / delta; // Вычисляем коэффициент наклона (a)
            double b = (sumX2 * sumY - sumX * sumXY) / delta; // Вычисляем свободный коэффициент (b)

            return CreateResult(x, y, "Линейная", new[] { a, b }, xi => a * xi + b); // Формируем и возвращаем результат
        }

        // Метод квадратичной аппроксимации: y = a + b*x + c*x^2
        public static ApproximationResult QuadraticApproximation(double[] x, double[] y)
        {
            ValidateInput(x, y); // Проверяем входные данные

            int n = x.Length; // Количество точек
            double sumX = 0, sumX2 = 0, sumX3 = 0, sumX4 = 0; // Суммы степеней X
            double sumY = 0, sumXY = 0, sumX2Y = 0; // Суммы для правой части

            for (int i = 0; i < n; i++) // Цикл по всем точкам
            {
                double xi = x[i]; // Текущий X
                double xi2 = xi * xi; // X^2

                sumX += xi; // Сумма X
                sumX2 += xi2; // Сумма X^2
                sumX3 += xi2 * xi; // Сумма X^3
                sumX4 += xi2 * xi2; // Сумма X^4
                sumY += y[i]; // Сумма Y
                sumXY += xi * y[i]; // Сумма X*Y
                sumX2Y += xi2 * y[i]; // Сумма X^2 * Y
            }

            var matrix = new[,] // Система линейных уравнений для коэффициентов
            {
                { n, sumX, sumX2, sumY }, // Первая строка
                { sumX, sumX2, sumX3, sumXY }, // Вторая строка
                { sumX2, sumX3, sumX4, sumX2Y } // Третья строка
            };

            var coefficients = SolveSystem(matrix); // Решаем систему
            return CreateResult(x, y, "Квадратичная", coefficients, // Возвращаем результат
                xi => coefficients[0] + coefficients[1] * xi + coefficients[2] * xi * xi); // Функция аппроксимации
        }

        // Метод кубической аппроксимации: y = a + b*x + c*x^2 + d*x^3
        public static ApproximationResult CubicApproximation(double[] x, double[] y)
        {
            ValidateInput(x, y); // Проверка входа

            int n = x.Length; // Кол-во точек
            double sumX = 0, sumX2 = 0, sumX3 = 0, sumX4 = 0, sumX5 = 0, sumX6 = 0; // Суммы X^n
            double sumY = 0, sumXY = 0, sumX2Y = 0, sumX3Y = 0; // Суммы с Y

            for (int i = 0; i < n; i++) // Обход точек
            {
                double xi = x[i]; // X
                double xi2 = xi * xi; // X^2
                double xi3 = xi2 * xi; // X^3

                sumX += xi;
                sumX2 += xi2;
                sumX3 += xi3;
                sumX4 += xi2 * xi2;
                sumX5 += xi3 * xi2;
                sumX6 += xi3 * xi3;
                sumY += y[i];
                sumXY += xi * y[i];
                sumX2Y += xi2 * y[i];
                sumX3Y += xi3 * y[i];
            }

            var matrix = new[,] // Матрица системы уравнений
            {
                { n, sumX, sumX2, sumX3, sumY },
                { sumX, sumX2, sumX3, sumX4, sumXY },
                { sumX2, sumX3, sumX4, sumX5, sumX2Y },
                { sumX3, sumX4, sumX5, sumX6, sumX3Y }
            };

            var coefficients = SolveSystem(matrix); // Решение системы
            return CreateResult(x, y, "Кубическая", coefficients, // Возврат результата
                xi => coefficients[0] + coefficients[1] * xi + coefficients[2] * xi * xi + coefficients[3] * xi * xi * xi);
        }

        // Экспоненциальная аппроксимация: y = a * e^(b * x)
        public static ApproximationResult ExponentialApproximation(double[] x, double[] y)
        {
            ValidateInput(x, y); // Проверка

            var lnY = y.Select(yi => Math.Log(yi)).ToArray(); // Логарифмируем Y
            var linearResult = LinearApproximation(x, lnY); // Решаем как линейную

            double a = Math.Exp(linearResult.Coefficients[1]); // Восстановление a
            double b = linearResult.Coefficients[0]; // b уже готов

            return CreateResult(x, y, "Экспоненциальная", new[] { a, b }, xi => a * Math.Exp(b * xi)); // Возврат
        }

        // Логарифмическая аппроксимация: y = a * ln(x) + b
        public static ApproximationResult LogarithmicApproximation(double[] x, double[] y)
        {
            ValidateInput(x, y);

            var lnX = x.Select(xi => Math.Log(xi)).ToArray(); // Логарифмируем X
            var linearResult = LinearApproximation(lnX, y); // Решаем как линейную

            double a = linearResult.Coefficients[0];
            double b = linearResult.Coefficients[1];

            return CreateResult(x, y, "Логарифмическая", new[] { a, b }, xi => a * Math.Log(xi) + b);
        }

        // Степенная аппроксимация: y = a * x^b
        public static ApproximationResult PowerApproximation(double[] x, double[] y)
        {
            ValidateInput(x, y);

            var lnX = x.Select(xi => Math.Log(xi)).ToArray(); // ln(x)
            var lnY = y.Select(yi => Math.Log(yi)).ToArray(); // ln(y)
            var linearResult = LinearApproximation(lnX, lnY); // Решаем как линейную

            double a = Math.Exp(linearResult.Coefficients[1]);
            double b = linearResult.Coefficients[0];

            return CreateResult(x, y, "Степенная", new[] { a, b }, xi => a * Math.Pow(xi, b));
        }

        // Создание структуры результата аппроксимации
        private static ApproximationResult CreateResult(double[] x, double[] y, string functionType,
            double[] coefficients, Func<double, double> function)
        {
            int n = x.Length;
            double[] predictedY = new double[n]; // Предсказанные значения
            double[] errors = new double[n]; // Ошибки
            double sumSquaredErrors = 0; // Сумма квадратов ошибок

            for (int i = 0; i < n; i++)
            {
                predictedY[i] = function(x[i]); // Предсказание
                errors[i] = y[i] - predictedY[i]; // Ошибка
                sumSquaredErrors += errors[i] * errors[i]; // Квадрат ошибки
            }

            return new ApproximationResult // Возврат полной структуры результата
            {
                FunctionType = functionType,
                Coefficients = coefficients,
                Errors = errors,
                SumSquaredErrors = sumSquaredErrors,
                StandardDeviation = Math.Sqrt(sumSquaredErrors / n),
                DeterminationCoefficient = CalculateDeterminationCoefficient(y, predictedY),
                Function = function
            };
        }

        // Проверка входных массивов на ошибки
        private static void ValidateInput(double[] x, double[] y)
        {
            if (x == null || y == null || x.Length != y.Length || x.Length == 0)
                throw new ArgumentException("Некорректные входные данные");
        }

        // Метод Гаусса для решения СЛАУ
        private static double[] SolveSystem(double[,] matrix)
        {
            int n = matrix.GetLength(0); // Число уравнений
            int m = matrix.GetLength(1); // Число столбцов

            if (m != n + 1)
                throw new ArgumentException("Некорректная размерность матрицы");

            for (int k = 0; k < n; k++) // Прямой ход
            {
                int maxRow = k;
                for (int i = k + 1; i < n; i++)
                    if (Math.Abs(matrix[i, k]) > Math.Abs(matrix[maxRow, k]))
                        maxRow = i;

                if (maxRow != k) // Меняем строки
                    for (int j = k; j < m; j++)
                        (matrix[k, j], matrix[maxRow, j]) = (matrix[maxRow, j], matrix[k, j]);

                double div = matrix[k, k]; // Ведущий элемент
                if (Math.Abs(div) < 1e-10)
                    throw new InvalidOperationException("Система уравнений вырождена");

                for (int j = k; j < m; j++)
                    matrix[k, j] /= div;

                for (int i = 0; i < n; i++)
                    if (i != k)
                    {
                        double factor = matrix[i, k];
                        for (int j = k; j < m; j++)
                            matrix[i, j] -= factor * matrix[k, j];
                    }
            }

            double[] solution = new double[n]; // Ответ
            for (int i = 0; i < n; i++)
                solution[i] = matrix[i, n];

            return solution;
        }

        // Коэффициент детерминации (R²)
        public static double CalculateDeterminationCoefficient(double[] y, double[] predictedY)
        {
            int n = y.Length;
            double yMean = y.Average(); // Среднее значение Y
            double ssTotal = 0;
            double ssResidual = 0;

            for (int i = 0; i < n; i++)
            {
                ssTotal += Math.Pow(y[i] - yMean, 2); // Общая дисперсия
                ssResidual += Math.Pow(y[i] - predictedY[i], 2); // Остаточная дисперсия
            }

            return Math.Abs(ssTotal) < 1e-10 ? 0 : 1 - (ssResidual / ssTotal); // R²
        }

        // Коэффициент корреляции Пирсона
        public static double CalculatePearsonCorrelation(double[] x, double[] y)
        {
            if (x.Length != y.Length || x.Length < 2)
                return 0;

            int n = x.Length;
            double sumX = 0, sumY = 0, sumXY = 0;
            double sumX2 = 0, sumY2 = 0;

            for (int i = 0; i < n; i++)
            {
                sumX += x[i];
                sumY += y[i];
                sumXY += x[i] * y[i];
                sumX2 += x[i] * x[i];
                sumY2 += y[i] * y[i];
            }

            double numerator = sumXY - (sumX * sumY) / n;
            double denomX = Math.Sqrt(sumX2 - sumX * sumX / n);
            double denomY = Math.Sqrt(sumY2 - sumY * sumY / n);

            return (denomX < 1e-10 || denomY < 1e-10) ? 0 : numerator / (denomX * denomY); // r
        }
    }
}
