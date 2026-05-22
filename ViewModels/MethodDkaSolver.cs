using System;
using PeterO.Numbers;

namespace MethodDkaCore
{
    public class MethodDkaSolver
    {
        private readonly int n;
        private readonly EDecimal[] sa;   // 係数（正規化後）
        private readonly EDecimal[] sr;   // 解の実数部
        private readonly EDecimal[] si;   // 解の虚数部
        private bool bSyuusoku;

        public MethodDkaSolver(int degree, EDecimal[] coeffs)
        {
            if (coeffs == null) throw new ArgumentNullException(nameof(coeffs));
            if (coeffs.Length != degree + 1)
                throw new ArgumentException("係数配列の長さが次数+1と一致していません。");

            n = degree;
            sa = new EDecimal[n + 1];
            Array.Copy(coeffs, sa, n + 1);

            sr = new EDecimal[n + 1];
            si = new EDecimal[n + 1];
        }

        public MethodDkaSolver(Polynomial poly)
            : this(poly.Degree, poly.Coeffs)
        {
        }

        /// <summary>
        /// JS版 subroutine() の C# / EDecimal 完全移植版。
        /// 収束したら true を返し、sr/si に解が格納される。
        /// </summary>
        public bool Subroutine()
        {
            // sx, sy: 1ステップあたりの修正量
            var sx = new EDecimal[n + 1];
            var sy = new EDecimal[n + 1];

            // 収束判定用しきい値（50桁）
            EDecimal se = EDecimal.FromString("1e-50");
            int m = 100; // 最大反復回数

            // π
            EDecimal sp = EDecimal.Acos(EDecimal.FromInt32(-1));
            EDecimal sw = EDecimal.Zero;

            // 係数を sa[0] で正規化
            for (int i = 0; i <= n; i++)
                sa[i] = sa[i].Divide(sa[0]);

            // 初期半径の決定
            for (int i = 2; i <= n; i++)
            {
                var sq = EDecimal.FromInt32(n)
                    .Multiply(sa[i].Abs().Pow(
                        EDecimal.FromInt32(1).Divide(EDecimal.FromInt32(i))));

                if (sw.CompareTo(sq) < 0)
                    sw = sq;
            }

            // 初期配置用の角度
            EDecimal sb = sp.Multiply(2).Divide(n);
            EDecimal sc = sp.Divide(2 * n);

            // 初期値設定（複素平面上に等間隔配置）
            for (int j = 1; j <= n; j++)
            {
                var st = sb.Multiply(j - 1).Add(sc);
                sr[j] = sw.Multiply(EDecimal.Cos(st));
                si[j] = sw.Multiply(EDecimal.Sin(st));
            }

            // 反復開始
            for (int k = 1; k <= m; k++)
            {
                bool bContinue = false;

                for (int i = 1; i <= n; i++)
                {
                    EDecimal s1 = EDecimal.One;
                    EDecimal s2 = EDecimal.Zero;
                    EDecimal s3 = EDecimal.One;
                    EDecimal s4 = EDecimal.Zero;

                    sb = sr[i];
                    sc = si[i];

                    for (int j = 1; j <= n; j++)
                    {
                        // s1 + i s2: 多項式値の計算
                        var s5 = s1.Multiply(sb).Subtract(s2.Multiply(sc));
                        s2 = s1.Multiply(sc).Add(s2.Multiply(sb));
                        s1 = s5.Add(sa[j]);

                        // s3 + i s4: 他の根との差の積
                        if (j != i)
                        {
                            var sbDiff = sb.Subtract(sr[j]);
                            var scDiff = sc.Subtract(si[j]);

                            var t1 = s3.Multiply(sbDiff).Subtract(s4.Multiply(scDiff));
                            var t2 = s3.Multiply(scDiff).Add(s4.Multiply(sbDiff));

                            s3 = t1;
                            s4 = t2;
                        }
                    }

                    // 修正量計算
                    sw = s3.Pow(2).Add(s4.Pow(2));
                    sx[i] = s1.Multiply(s3).Add(s2.Multiply(s4)).Divide(sw);
                    sy[i] = s2.Multiply(s3).Subtract(s1.Multiply(s4)).Divide(sw);

                    // 根の更新
                    sr[i] = sr[i].Subtract(sx[i]);
                    si[i] = si[i].Subtract(sy[i]);

                    // 収束判定
                    if (sx[i].Abs().CompareTo(se) > 0 || sy[i].Abs().CompareTo(se) > 0)
                        bContinue = true;
                }

                if (!bContinue)
                {
                    bSyuusoku = true;
                    return true;
                }
            }

            bSyuusoku = false;
            return false;
        }

        /// <summary>
        /// 収束後の解を ComplexResult 配列として取得。
        /// Subroutine() が true を返した後に呼び出す想定。
        /// </summary>
        public ComplexResult[] GetRoots()
        {
            if (!bSyuusoku)
                throw new InvalidOperationException("まだ収束していません。先に Subroutine() を呼び出してください。");

            var results = new ComplexResult[n];
            for (int i = 1; i <= n; i++)
            {
                results[i - 1] = new ComplexResult
                {
                    Real = sr[i],
                    Imag = si[i]
                };
            }
            return results;
        }
    }
}
