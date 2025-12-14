using System;

namespace M_GRad_P_K
{
    public class ModelSolver
    {
        public double[] t, y, z, H;

        public void Run(
            double a, double c, double M,
            double y0, double yStar,
            double T, int N)
        {
            double dt = T / N;

            t = new double[N + 1];
            y = new double[N + 1];
            z = new double[N + 1];
            H = new double[N + 1];

            y[0] = y0;

            for (int i = 0; i <= N; i++)
            {
                t[i] = i * dt;

                // z*(t) = 0.001 t с ограничением
                z[i] = Math.Min(M, 0.001 * t[i]);
            }

            Integrator.ForwardEuler(y, z, a, c, dt);

            for (int i = 0; i <= N; i++)
            {
                H[i] = Math.Pow(y[i] - yStar, 2);
            }
        }
    }
}


