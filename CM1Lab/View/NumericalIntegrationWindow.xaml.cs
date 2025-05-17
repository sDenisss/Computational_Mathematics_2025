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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CM1Lab.View
{
    /// <summary>
    /// Interaction logic for NumericalIntegrationWindow.xaml
    /// </summary>
    public partial class NumericalIntegrationWindow : Window
    {
        NumericalIntegrationViewModel vm;
        public NumericalIntegrationWindow()
        {
            InitializeComponent();
            vm = new NumericalIntegrationViewModel();
            DataContext = vm; // Устанавливаем ViewModel в качестве DataContext
            this.WindowState = WindowState.Maximized;

            var methods = new List<Method>
            {
                new Method { MethodName = "Метод левого прямоугольника" },
                new Method { MethodName = "Метод правого прямоугольника" },
                new Method { MethodName = "Метод среднего прямоугольника" },
                new Method { MethodName = "Метод трапеций" },
                new Method { MethodName = "Метод Симпсона" }
            };

            methodsComboBox.ItemsSource = methods;
            methodsComboBox.DisplayMemberPath = "MethodName";
            methodsComboBox.SelectedValuePath = "MethodName";

            var functions = new List<FunctionForChoice>
            {
                //new FunctionForChoice { FunctionName = "Впиши в поле" },
                new FunctionForChoice { FunctionName = "∫(3x³-4x²+5x-16)dx" },
                new FunctionForChoice { FunctionName = "∫x²dx" },
                new FunctionForChoice { FunctionName = "∫sin(x)dx" },
                new FunctionForChoice { FunctionName = "∫(2x³-3x²+4x-22)dx" },
                new FunctionForChoice { FunctionName = "∫(1/√x)dx" },
                new FunctionForChoice { FunctionName = "∫(1/|x-0,5|)dx" }
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



        public void CountResults(object sender, EventArgs e)
        {

            vm.BuildGraphic(); // Теперь вызываем метод из ViewModel
            vm.NumericalIntegrationSolve();
            //vm.UpdateEquationFormula();
        }


        // Обработчик события выбора элемента в ComboBox
        private void methods_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (methodsComboBox.SelectedItem is Method selectedMethod)
            {
                vm.SelectedMethod = selectedMethod.MethodName.ToString();
            }
        }

        // Обработчик события выбора элемента в ComboBox
        private void functions_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (functionsComboBox.SelectedItem is FunctionForChoice selectedFunction)
            {
                vm.SelectedFunction = selectedFunction.FunctionName.ToString();
            }
        }
        private void ToHomeClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
