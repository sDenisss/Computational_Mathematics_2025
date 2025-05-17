using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM1Lab.Model
{
    public class ApproximationResult
    {
        public string FunctionType { get; set; } // Название типа аппроксимации (например, "Линейная", "Квадратичная")

        public double[] Coefficients { get; set; } // Коэффициенты уравнения (a, b, c, ...)

        public double[] Errors { get; set; } // Массив εᵢ = φ(xᵢ) - yᵢ для каждого значения

        public double SumSquaredErrors { get; set; } // S = Σ εᵢ² — сумма квадратов отклонений

        public double StandardDeviation { get; set; } // Среднеквадратичное отклонение: sqrt(S / n)

        public double DeterminationCoefficient { get; set; } // Коэффициент детерминации R²

        public Func<double, double> Function { get; set; } // Сама аппроксимирующая функция φ(x)

    }

}
