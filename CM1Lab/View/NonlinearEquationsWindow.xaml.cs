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
    /// Interaction logic for NonlinearEquationsWindow.xaml
    /// </summary>
    public partial class NonlinearEquationsWindow : Window
    {
        private NonlinearEquationsViewModel vm;

        public NonlinearEquationsWindow()
        {
            InitializeComponent();
            vm = new NonlinearEquationsViewModel();
            DataContext = vm; // Устанавливаем ViewModel в качестве DataContext
            this.WindowState = WindowState.Maximized;

            var methods = new List<Method>
            {
                new Method { MethodName = "Метод половинного деления" },
                new Method { MethodName = "Метод секущих" },
                new Method { MethodName = "Метод простой итерации" }
            };

            methodsComboBox.ItemsSource = methods;
            methodsComboBox.DisplayMemberPath = "MethodName";
            methodsComboBox.SelectedValuePath = "MethodName";

            var functions = new List<FunctionForChoice>
            {
                new FunctionForChoice { FunctionName = "Впиши в поле" },
                new FunctionForChoice { FunctionName = "f(x)=x²-4" },
                new FunctionForChoice { FunctionName = "f(x)=ln(x)" },
                new FunctionForChoice { FunctionName = "f(x)=3x²-6x+2" },
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
            vm.NonLinearEquationsSolve();
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
