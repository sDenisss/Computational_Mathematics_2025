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
    /// Interaction logic for SystemNonlinearEquationsWindow.xaml
    /// </summary>
    public partial class SystemNonlinearEquationsWindow : Window
    {
        private SystemNonlinearEquationsViewModel vm;

        public SystemNonlinearEquationsWindow()
        {
            InitializeComponent();
            vm = new SystemNonlinearEquationsViewModel();
            DataContext = vm; // Устанавливаем ViewModel в качестве DataContext
            this.WindowState = WindowState.Maximized;

            var systems = new List<SystemForChoice>
            {
                new SystemForChoice { SystemNonlinearEquationName = "Впиши в поле" },
                new SystemForChoice { SystemNonlinearEquationName = "sin(x+y)-1.4x=0; x²+y²=1" }, // Убрали \r\n
                new SystemForChoice { SystemNonlinearEquationName = "y-cos(x)=2; x+cos(y-1)=0.8" }, // Убрали \r\n
            };

            systemComboBox.ItemsSource = systems;
            systemComboBox.DisplayMemberPath = "SystemNonlinearEquationName";
            systemComboBox.SelectedValuePath = "SystemNonlinearEquationName";
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
        private void system_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (systemComboBox.SelectedItem is SystemForChoice selectedSystem)
            {
                vm.SelectedSystem = selectedSystem.SystemNonlinearEquationName.ToString();
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

