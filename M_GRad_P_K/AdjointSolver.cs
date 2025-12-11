using System;

namespace M_GRad_P_K
{
    public static class AdjointSolver
    {
        // Решение для лямбды в дискретной обратной манере
        // Возвращает массив лямбды с длиной N+1 (совпадая с y)
        public static double[] Solve(double[] y, double yStar, double c, double dt)
        {
            int N = y.Length - 1;
            double[] lambda = new double[N + 1];
            lambda[N] = 0.0; // терминальное условие

            for (int i = N - 1; i >= 0; i--)
            {
                // lambda' = 2*c*(y - yStar)
                double dlam = 2.0 * c * (y[i] - yStar);
                // шаг назад, подобный шагу Эйлера:
                lambda[i] = lambda[i + 1] + dt * dlam;
            }
            return lambda;
        }
    }
}
