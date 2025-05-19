using CM1Lab.Model;
using CM1Lab.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CM1Lab.View
{
    /// <summary>
    /// Interaction logic for InterpolationFunctionWindow.xaml
    /// </summary>
    public partial class InterpolationFunctionWindow : Window
    {
        InterpolationFunctionViewModel vm;
        public InterpolationFunctionWindow()
        {
            InitializeComponent();
            vm = new InterpolationFunctionViewModel();
            DataContext = vm; // Устанавливаем ViewModel в качестве DataContext
            this.WindowState = WindowState.Maximized;


            var methods = new List<Method>
            {
                new Method { MethodName = "Многочлен Лагранжа" },
                new Method { MethodName = "Многочлен Ньютона с разделенными разностями" },
                new Method { MethodName = "Многочлен Ньютона с конечными разностями" },
            };

            methodsComboBox.ItemsSource = methods;
            methodsComboBox.DisplayMemberPath = "MethodName";
            methodsComboBox.SelectedValuePath = "MethodName";

            var functions = new List<FunctionForChoice>
            {
                //new FunctionForChoice { FunctionName = "Впиши в поле" },
                new FunctionForChoice { FunctionName = "x²" },
                new FunctionForChoice { FunctionName = "sin(x)" },
                new FunctionForChoice { FunctionName = "ln(x)" },
            };

            functionsComboBox.ItemsSource = functions;
            functionsComboBox.DisplayMemberPath = "FunctionName";
            functionsComboBox.SelectedValuePath = "FunctionName";
        }

        public void ChooseWayClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog.Title = "Выберите файл с данными";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    vm.LoadDataFromText(openFileDialog.FileName);
                    //MessageBox.Show("Данные успешно загружены из файла.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных из файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void UpdateCoefficientGrid()
        {
            try
            {
                int size = int.Parse(vm.Size);
                CoefficientGrid.Children.Clear();
                CoefficientGrid.RowDefinitions.Clear();
                CoefficientGrid.ColumnDefinitions.Clear();

                double cellWidth = Math.Max(200.0 / size, 30);
                double cellHeight = 20;


                // Просто убедимся, что коллекции имеют нужный размер, но не перезаписываем данные
                while (vm.CoefficientsX.Count < size) vm.CoefficientsX.Add("0");
                while (vm.CoefficientsY.Count < size) vm.CoefficientsY.Add("0");
                while (vm.CoefficientsX.Count > size) vm.CoefficientsX.RemoveAt(vm.CoefficientsX.Count - 1);
                while (vm.CoefficientsY.Count > size) vm.CoefficientsY.RemoveAt(vm.CoefficientsY.Count - 1);

                for (int i = 0; i < 2; i++)
                {
                    CoefficientGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    for (int j = 0; j < size; j++)
                    {

                        if (size <= 2)
                        {
                            MessageBox.Show($"Минимум две точки требуются для интерполяции");
                            return;
                        }

                        if (i == 0)
                        {
                            CoefficientGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                        }

                        TextBox textBox = new TextBox
                        {
                            Width = cellWidth,
                            Height = cellHeight,
                            Margin = new Thickness(1)
                        };

                        // Привязка TextBox к CoeffA
                        Binding binding = new Binding($"Coefficients{(i == 0 ? "X" : "Y")}[{j}]")
                        {
                            Source = vm,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        };

                        textBox.SetBinding(TextBox.TextProperty, binding);

                        Grid.SetRow(textBox, i);
                        Grid.SetColumn(textBox, j);
                        CoefficientGrid.Children.Add(textBox);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Некоректные данные: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void PrintFiniteDifferencesToUI(double[] y, Grid grid)
        {
            int n = y.Length;
            double[,] delta = new double[n, n];

            // Заполнение первой колонки
            for (int i = 0; i < n; i++)
                delta[i, 0] = y[i];

            // Вычисление конечных разностей
            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < n - j; i++)
                {
                    delta[i, j] = delta[i + 1, j - 1] - delta[i, j - 1];
                }
            }

            // Очистка сетки
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            // Создание колонок
            for (int j = 0; j < n; j++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            for (int i = 0; i < n; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int j = 0; j < n - i; j++)
                {
                    var textBlock = new TextBlock
                    {
                        Text = delta[i, j].ToString("F4"),
                        Margin = new Thickness(3),
                        FontSize = 12
                    };
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);
                    grid.Children.Add(textBlock);
                }
            }
        }


        public void CountResults(object sender, EventArgs e)
        {

            //vm.BuildGraphic(); // Теперь вызываем метод из ViewModel
            vm.GenerateFunctionData();
            vm.ApproximationSolve();
            if (vm.SelectedMethod == "Многочлен Ньютона с конечными разностями")
            {
                var y = vm.CoefficientsY.Select(double.Parse).ToArray();
                PrintFiniteDifferencesToUI(y, CoefficientGridResults);
            }
            //PrintFiniteDifferencesToUI(vm.CoefficientsY.Select(double.Parse).ToArray(), CoefficientGridResults);
        }

        public void Confirm(object sender, EventArgs e)
        {
            UpdateCoefficientGrid();
        }

        private void ToHomeClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
