using System;

namespace M_GRad_P_K
{
    public static class Integrator
    {
        // Правая часть dy/dt = z(t) – d(t)
        public static double Deriv(double y, double u, double demand)
        {
            return u - demand;
        }

        // Однофазовый Рунге–Кутта 4-го порядка
        public static void ForwardRK4(
            double[] u,
            double[] y,
            double A, double B,  // параметры спроса
            double dt)
        {
            int N = u.Length - 1;

            for (int i = 0; i < N; i++)
            {
                double t = i * dt;
                double d = A + B * t;

                double k1 = Deriv(y[i], u[i], d);

                double k2 = Deriv(
                    y[i] + dt * 0.5 * k1,
                    u[i],
                    A + B * (t + dt * 0.5)
                );

                double k3 = Deriv(
                    y[i] + dt * 0.5 * k2,
                    u[i],
                    A + B * (t + dt * 0.5)
                );

                double k4 = Deriv(
                    y[i] + dt * k3,
                    u[i],
                    A + B * (t + dt)
                );

                y[i + 1] = y[i] + dt * (k1 + 2 * k2 + 2 * k3 + k4) / 6.0;

                // Ограничение на нижнюю границу
                if (y[i + 1] < 0)
                    y[i + 1] = 0;
            }
        }
    }
}

