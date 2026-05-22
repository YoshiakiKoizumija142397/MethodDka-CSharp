using PeterO.Numbers;

namespace MethodDkaCore
{
    public static class Polynomial
    {
        /// <summary>
        /// 多項式 P(x) を計算する
        /// </summary>
        /// <param name="coeffs">係数配列（a0, a1, ..., an）</param>
        /// <param name="x">複素数 x</param>
        public static ComplexResult Evaluate(EDecimal[] coeffs, ComplexResult x)
        {
            int n = coeffs.Length - 1;

            var real = EDecimal.Zero;
            var imag = EDecimal.Zero;

            for (int i = 0; i <= n; i++)
            {
                var pow = ComplexPow(x, n - i);
                real += coeffs[i] * pow.Real;
                imag += coeffs[i] * pow.Imag;
            }

            return new ComplexResult(real, imag);
        }

        /// <summary>
        /// 多項式 P'(x) を計算する
        /// </summary>
        public static ComplexResult EvaluateDerivative(EDecimal[] coeffs, ComplexResult x)
        {
            int n = coeffs.Length - 1;

            var real = EDecimal.Zero;
            var imag = EDecimal.Zero;

            for (int i = 0; i < n; i++)
            {
                var pow = ComplexPow(x, n - i - 1);
                var coef = coeffs[i] * EDecimal.FromInt32(n - i);

                real += coef * pow.Real;
                imag += coef * pow.Imag;
            }

            return new ComplexResult(real, imag);
        }

        /// <summary>
        /// 複素数のべき乗 x^k
        /// </summary>
        public static ComplexResult ComplexPow(ComplexResult x, int k)
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

        /// <summary>
        /// 複素数の乗算
        /// </summary>
        public static ComplexResult ComplexMul(ComplexResult a, ComplexResult b)
        {
            return new ComplexResult(
                a.Real * b.Real - a.Imag * b.Imag,
                a.Real * b.Imag + a.Imag * b.Real
            );
        }
    }
}
