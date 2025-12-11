using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_GRad_P_K
{
    public static class AdjointSolver
    {
        public static (double[] lambda1, double[] lambda2) Solve(
            double[] x1, double[] x2, double[] u,
            double G, double Y, double R0, double R1,
            double GAMMA, double NEY,
            double dt)
        {
            int N = u.Length - 1;

            double[] lam1 = new double[N + 1];
            double[] lam2 = new double[N + 1];

            lam1[N] = 0.0;
            lam2[N] = 0.0;

            for (int i = N - 1; i >= 0; i--)
            {
                // dH/dx1
                double dHdx1 =
                    -(1 + GAMMA) * R0 - R1
                    + lam1[i + 1] * (-G * u[i] * (Y - x2[i]))
                    + lam2[i + 1] * (G * u[i] * (Y - x2[i]));

                // dH/dx2
                double dHdx2 =
                    -u[i]
                    + lam1[i + 1] * (-G * u[i] * x1[i])
                    + lam2[i + 1] * (-G * u[i] * x1[i] - NEY);

                lam1[i] = lam1[i + 1] + dt * (-dHdx1);
                lam2[i] = lam2[i + 1] + dt * (-dHdx2);
            }

            return (lam1, lam2);
        }
    }

}
