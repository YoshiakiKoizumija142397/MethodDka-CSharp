using MethodDkaWPFApp.Views;
using System.Windows.Input;

namespace MethodDkaWPFApp.ViewModels
{
    public class MainViewModel
    {
        public ICommand OpenCalculatorCommand { get; }

        public MainViewModel()
        {
            OpenCalculatorCommand = new RelayCommand(OpenCalculator);
        }

        private void OpenCalculator()
        {
            var win = new CalculatorWindow();
            win.Show();
        }
    }
}
