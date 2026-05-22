using System;
using System.Windows;
using System.Windows.Controls;
using MethodDkaCore;

namespace MethodDkaWPF.App
{
    public partial class CalculatorWindow : Window
    {
        private TextBox[] CoefficientTextBoxes;

        public CalculatorWindow()
        {
            InitializeComponent();
        }

        private void CreateFields_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int n = int.Parse(MaxDegreeTextBox.Text);

                CoefficientPanel.Children.Clear();
                CoefficientTextBoxes = new TextBox[n + 1];

                for (int i = 0; i <= n; i++)
                {
                    var label = new TextBlock
                    {
                        Text = $"{n - i}次の係数",
                        FontSize = 16,
                        Margin = new Thickness(0, 5, 0, 0)
                    };

                    var tb = new TextBox
                    {
                        Width = 200,
                        FontSize = 16,
                        Margin = new Thickness(0, 2, 0, 10)
                    };

                    CoefficientTextBoxes[i] = tb;

                    CoefficientPanel.Children.Add(label);
                    CoefficientPanel.Children.Add(tb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラー: " + ex.Message);
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CoefficientTextBoxes == null)
                {
                    MessageBox.Show("先に『最高次数を設定』を押してください。");
                    return;
                }

                int n = CoefficientTextBoxes.Length - 1;

                double[] sa = new double[n + 1];

                for (int i = 0; i <= n; i++)
                {
                    string text = CoefficientTextBoxes[i].Text;
                    sa[i] = double.TryParse(text, out double v) ? v : 0;
                }

                var result = MethodDkaSolver.Solve(sa);

                ResultsListBox.Items.Clear();

                if (!result.success)
                {
                    ResultsListBox.Items.Add("収束しませんでした。");
                    return;
                }

                for (int i = 1; i <= n; i++)
                {
                    ResultsListBox.Items.Add(
                        $"解{i}: 実数部 {result.real[i]}, 虚数部 {result.imag[i]}"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラー: " + ex.Message);
            }
        }
    }
}
