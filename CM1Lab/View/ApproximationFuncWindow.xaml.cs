using CM1Lab.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CM1Lab.View
{
    /// <summary>
    /// Interaction logic for ApproximationFuncWindow.xaml
    /// </summary>
    public partial class ApproximationFuncWindow : Window
    {
        ApproximationFuncViewModel vm;
        public ApproximationFuncWindow()
        {
            InitializeComponent();
            vm = new ApproximationFuncViewModel();
            DataContext = vm; // Устанавливаем ViewModel в качестве DataContext
            this.WindowState = WindowState.Maximized;

            //var functions = new List<FunctionForChoice>
            //{
            //    //new FunctionForChoice { FunctionName = "Впиши в поле" },
            //    new FunctionForChoice { FunctionName = "2x + 3" }, // Линейная функция
            //    new FunctionForChoice { FunctionName = "x² + 1" }, // Полиномиальная функция 2-й степени
            //    new FunctionForChoice { FunctionName = "x³ - x" }, // Полиномиальная функция 3-й степени
            //    new FunctionForChoice { FunctionName = "2^x" }, // Экспоненциальная функция
            //    new FunctionForChoice { FunctionName = "ln(x)" }, // Логарифмическая функция
            //    new FunctionForChoice { FunctionName = "x^3" } // Степенная функция
            //};


            //functionsComboBox.ItemsSource = functions;
            //functionsComboBox.DisplayMemberPath = "FunctionName";
            //functionsComboBox.SelectedValuePath = "FunctionName";
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

                    if(size > 12 || size < 8)
                    {
                        MessageBox.Show($"размер должен быть от 8 до 12");
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

        public void CountResults(object sender, EventArgs e)
        {
            
            //vm.BuildGraphic(); // Теперь вызываем метод из ViewModel
            vm.ApproximationSolve();
            vm.WriteInFile();
        }

        public void Confirm(object sender, EventArgs e)
        {
            UpdateCoefficientGrid();
        }

        // Обработчик события выбора элемента в ComboBox
        //private void functions_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (functionsComboBox.SelectedItem is FunctionForChoice selectedFunction)
        //    {
        //        vm.SelectedFunction = selectedFunction.FunctionName.ToString();
        //    }
        //}
        private void ToHomeClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
