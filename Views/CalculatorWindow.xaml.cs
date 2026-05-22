using MethodDkaWPFApp.ViewModels;

namespace MethodDkaWPFApp.Views
{
    public partial class CalculatorWindow : Window
    {
        public CalculatorWindow()
        {
            InitializeComponent();
            DataContext = new CalculatorViewModel();
        }
    }
}
