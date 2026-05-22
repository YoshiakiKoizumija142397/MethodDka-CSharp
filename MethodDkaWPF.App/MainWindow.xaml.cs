using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MethodDkaCore;

namespace MethodDkaWPFApp
{
    public partial class MainWindow : Window
    {
        private List<TextBox> coefficientBoxes = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetDegree_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(DegreeBox.Text, out int degree) || degree < 1)
            {
                MessageBox.Show("次数を正しく入力してください。");
                return;
            }

            CoefficientPanel.Children.Clear();
            coefficientBoxes.Clear();

            for (int i = degree; i >= 0; i--)
            {
                var tb = new TextBox { Width = 100, Margin = new Thickness(0, 5, 0, 5) };
                coefficientBoxes.Add(tb);

                CoefficientPanel.Children.Add(new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new TextBlock { Text = $"{i}次の係数:", Width = 120 },
                        tb
                    }
                });
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double[] coeffs = new double[coefficientBoxes.Count];

                for (int i = 0; i < coeffs.Length; i++)
                {
                    if (!double.TryParse(coefficientBoxes[i].Text, out coeffs[i]))
                    {
                        MessageBox.Show("係数を正しく入力してください。");
                        return;
                    }
                }

                var (success, real, imag) = MethodDkaSolver.Solve(coeffs);

                if (!success)
                {
                    MessageBox.Show("計算に失敗しました。係数を確認してください。", "エラー");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("収束しました。\n");

                for (int i = 0; i < real.Length; i++)
                    sb.AppendLine($"解{i + 1}: 実数部 {real[i]}, 虚数部 {imag[i]}");

                ResultBlock.Text = sb.ToString();
            }
            catch
            {
                MessageBox.Show("予期せぬエラーが発生しました。", "エラー");
            }
        }
    }
}
