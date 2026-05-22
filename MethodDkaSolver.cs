using System;
using PeterO.Numbers;

namespace MethodDkaCore
{
    public class MethodDkaSolver
    {
        private readonly int n;
        private readonly EDecimal[] a;
        private readonly ComplexResult[] roots;

        public MethodDkaSolver(int degree, EDecimal[] coefficients)
        {
            n = degree;
            a = coefficients;
            roots = new ComplexResult[n];
        }

        public bool Subroutine()
        {
            if (n <= 0) return false;

            // 初期値：単位円上に等間隔配置
            for (int i = 0; i < n; i++)
            {
                double angle = 2.0 * Math.PI * i / n;
                roots[i] = new ComplexResult(
                    EDecimal.FromDouble(Math.Cos(angle)),
                    EDecimal.FromDouble(Math.Sin(angle))
                );
            }

            const int MAX_ITER = 200;
            const double EPS = 1e-30;

            for (int iter = 0; iter < MAX_ITER; iter++)
            {
                bool converged = true;

                for (int i = 0; i < n; i++)
                {
                    var xi = roots[i];

                    // P(xi) と P'(xi) を計算
                    var px = EvalPoly(xi);
                    var dpx = EvalPolyDerivative(xi);

                    // ニュートンステップ
                    var delta = px.Divide(dpx);

                    // 更新
                    var newXi = new ComplexResult(
                        xi.Real - delta.Real,
                        xi.Imag - delta.Imag
                    );

                    // 収束判定
                    if ((newXi.Real - xi.Real).Abs().ToDouble() > EPS ||
                        (newXi.Imag - xi.Imag).Abs().ToDouble() > EPS)
                    {
                        converged = false;
                    }

                    roots[i] = newXi;
                }

                if (converged) return true;
            }

            return false;
        }

        private ComplexResult EvalPoly(ComplexResult x)
        {
            var real = EDecimal.Zero;
            var imag = EDecimal.Zero;

            for (int i = 0; i <= n; i++)
            {
                var pow = ComplexPow(x, n - i);
                real += a[i] * pow.Real;
                imag += a[i] * pow.Imag;
            }

            return new ComplexResult(real, imag);
        }

        private ComplexResult EvalPolyDerivative(ComplexResult x)
        {
            var real = EDecimal.Zero;
            var imag = EDecimal.Zero;

            for (int i = 0; i < n; i++)
            {
                var pow = ComplexPow(x, n - i - 1);
                var coef = a[i] * EDecimal.FromInt32(n - i);

                real += coef * pow.Real;
                imag += coef * pow.Imag;
            }

            return new ComplexResult(real, imag);
        }

        private ComplexResult ComplexPow(ComplexResult x, int k)
        {
            if (k == 0) return new ComplexResult(EDecimal.One, EDecimal.Zero);
            if (k == 1) return x;

            var r = new ComplexResult(EDecimal.One, EDecimal.Zero);

            for (int i = 0; i < k; i++)
            {
                r = ComplexMul(r, x);
            }

            return r;
        }

        private ComplexResult ComplexMul(ComplexResult a, ComplexResult b)
        {
            return new ComplexResult(
                a.Real * b.Real - a.Imag * b.Imag,
                a.Real * b.Imag + a.Imag * b.Real
            );
        }

        public ComplexResult[] GetRoots() => roots;
    }
}
