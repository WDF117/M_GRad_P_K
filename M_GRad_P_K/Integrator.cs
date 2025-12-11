using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_GRad_P_K
{
    public static class Integrator
    {
        public static void Deriv(
            double x1, double x2, double u,
            double G, double Y, double GAMMA, double NEY,
            out double dx1, out double dx2)
        {
            dx1 = -G * u * (Y - x2) * x1 + GAMMA;
            dx2 = G * u * (Y - x2) * x1 - NEY * x2;
        }

        public static void ForwardRK4(
            double[] u, double[] x1, double[] x2,
            double G, double Y, double GAMMA, double NEY,
            double dt)
        {
            int N = u.Length - 1;

            for (int i = 0; i < N; i++)
            {
                Deriv(x1[i], x2[i], u[i], G, Y, GAMMA, NEY, out double k1x1, out double k1x2);

                Deriv(x1[i] + dt / 2 * k1x1, x2[i] + dt / 2 * k1x2, u[i], G, Y, GAMMA, NEY,
                    out double k2x1, out double k2x2);

                Deriv(x1[i] + dt / 2 * k2x1, x2[i] + dt / 2 * k2x2, u[i], G, Y, GAMMA, NEY,
                    out double k3x1, out double k3x2);

                Deriv(x1[i] + dt * k3x1, x2[i] + dt * k3x2, u[i], G, Y, GAMMA, NEY,
                    out double k4x1, out double k4x2);

                x1[i + 1] = x1[i] + dt / 6 * (k1x1 + 2 * k2x1 + 2 * k3x1 + k4x1);
                x2[i + 1] = x2[i] + dt / 6 * (k1x2 + 2 * k2x2 + 2 * k3x2 + k4x2);
            }
        }
    }


}
