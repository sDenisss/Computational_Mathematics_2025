using System;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using CM1Lab.View;

namespace CM1Lab
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Устанавливаем контекст данных
            //MainWindow mainWindow = new MainWindow();
            this.WindowState = WindowState.Maximized;
        }

        private void gauss_seidelWindow_Click(object sender, RoutedEventArgs e)
        {
            Gauss_Seidel_MethodWindow gauss_seidelWindow = new Gauss_Seidel_MethodWindow();
            gauss_seidelWindow.Show();
            this.Close();
        }

        private void nonlinearEquationsWindow_Click(object sender, RoutedEventArgs e)
        {
            NonlinearEquationsWindow nonlinearEquationsWindow = new NonlinearEquationsWindow();
            nonlinearEquationsWindow.Show();
            this.Close();
        }
        private void systemNonlinearEquationsWindow_Click(object sender, RoutedEventArgs e)
        {
            SystemNonlinearEquationsWindow systemNonlinearEquationsWindow = new SystemNonlinearEquationsWindow();
            systemNonlinearEquationsWindow.Show();
            this.Close();
        }

        private void numericalIntegrationWindow_Click(object sender, RoutedEventArgs e)
        {
            NumericalIntegrationWindow numericalIntegrationWindow = new NumericalIntegrationWindow();
            numericalIntegrationWindow.Show();
            this.Close();
        }
        private void approximationFuncWindow_Click(object sender, RoutedEventArgs e)
        {
            ApproximationFuncWindow approximationFuncWindow = new ApproximationFuncWindow();
            approximationFuncWindow.Show();
            this.Close();
        }
        private void interpolationFunctionWindow_Click(object sender, RoutedEventArgs e)
        {
            InterpolationFunctionWindow interpolationFunctionWindow = new InterpolationFunctionWindow();
            interpolationFunctionWindow.Show();
            this.Close();
        }

    }
}
