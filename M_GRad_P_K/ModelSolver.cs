using System;

namespace M_GRad_P_K
{
    public class ModelSolver
    {
        public double[] t, y, u, H;

        // Однофазовая модель
        public void Run(
            double a, double c, double M,
            double y0, double zStar, double yStar,
            double T, int N)
        {
            double dt = T / N;

            t = new double[N + 1];
            y = new double[N + 1];
            u = new double[N + 1];
            H = new double[N + 1];

            // Time grid
            for (int i = 0; i <= N; i++)
                t[i] = i * dt;

            // Initial condition
            y[0] = y0;

            // Initial guess for u(t)
            for (int i = 0; i <= N; i++)
                u[i] = Math.Min(M, zStar * t[i]); // z*(t) = 0.001t или t — твоё решение

            // Parameters for demand
            double A = 0;       // постоянная часть
            double B = 0.001;   // скорость роста

            // прямая интеграция
            Integrator.ForwardRK4(u, y, A, B, dt);

            // Вычисление H(t)
            for (int i = 0; i <= N; i++)
            {
                double d = A + B * t[i];
                double L = a * Math.Pow(u[i] - zStar, 2) +
                           c * Math.Pow(y[i] - yStar, 2);

                H[i] = L; // Однофазовая модель — H = L
            }
        }
    }
}

