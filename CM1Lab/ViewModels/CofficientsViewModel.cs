using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM1Lab.ViewModels
{
    public class CoefficientsModel : INotifyPropertyChanged
    {
        private double coeffA;
       
        public double CoeffA
        {
            get => coeffA;
            set { coeffA = value; OnPropertyChanged(nameof(CoeffA)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
