using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MethodDkaCore;
using PeterO.Numbers;

namespace MethodDkaWPFApp.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private int _degree;
        private string _resultText;

        public int Degree
        {
            get => _degree;
            set
            {
                _degree = value;
                OnPropertyChanged();
            }
        }

        public string ResultText
        {
            get => _resultText;
            set
            {
                _resultText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CoefficientField> CoefficientFields { get; }
            = new ObservableCollection<CoefficientField>();

        public ICommand CreateFieldsCommand { get; }
        public ICommand CalculateCommand { get; }

        public CalculatorViewModel()
        {
            CreateFieldsCommand = new RelayCommand(CreateFields);
            CalculateCommand = new RelayCommand(Calculate);
        }

        /// <summary>
        /// 最高次数を設定 → 係数入力フィールドを生成
        /// </summary>
        private void CreateFields()
        {
            if (Degree <= 0)
            {
                MessageBox.Show("最高次数を正しく入力してください。");
                return;
            }

            CoefficientFields.Clear();

            for (int i = 0; i <= Degree; i++)
            {
                CoefficientFields.Add(new CoefficientField
                {
                    Label = $"{Degree - i}次の係数:",
                    Value = "0"
                });
            }
        }

        /// <summary>
        /// 計算開始 → MethodDkaSolver を呼び出し
        /// </summary>
        private void Calculate()
        {
            if (CoefficientFields.Count != Degree + 1)
            {
                MessageBox.Show("先に『最高次数を設定』してください。");
                return;
            }

            try
            {
                // 係数を EDecimal に変換
                var coeffs = new EDecimal[Degree + 1];
                for (int i = 0; i <= Degree; i++)
                {
                    coeffs[i] = EDecimal.FromString(CoefficientFields[i].Value);
                }

                // ソルバー生成
                var solver = new MethodDkaSolver(Degree, coeffs);

                // 収束計算
                bool ok = solver.Subroutine();

                if (!ok)
                {
                    ResultText = "収束しませんでした。";
                    return;
                }

                // 結果取得
                var roots = solver.GetRoots();

                var sb = new StringBuilder();
                for (int i = 0; i < roots.Length; i++)
                {
                    sb.AppendLine(
                        $"解{i + 1}: 実数部 {roots[i].Real}, 虚数部 {roots[i].Imag}"
                    );
                }

                ResultText = sb.ToString();
            }
            catch (Exception ex)
            {
                ResultText = $"エラー: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
