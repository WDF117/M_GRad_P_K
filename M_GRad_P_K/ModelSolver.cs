using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_GRad_P_K
{
    public class ModelSolver
    {
        public double[] t, x1, x2, u, H;

        public void Run(
            double G, double Y, double R0, double R1,
            double GAMMA, double NEY,
            double uMin, double uMax,
            double T, int N,
            double Astep)
        {
            double dt = T / N;

            t = new double[N + 1];
            x1 = new double[N + 1];
            x2 = new double[N + 1];
            u = new double[N + 1];
            H = new double[N + 1];

            // Time
            for (int i = 0; i <= N; i++)
                t[i] = i * dt;

            // Initial conditions (можешь изменить)
            x1[0] = 1.0;
            x2[0] = 0.5;

            // Initial guess for u
            for (int i = 0; i <= N; i++)
                u[i] = (uMin + uMax) / 2.0;

            for (int iter = 0; iter < 50; iter++)
            {
                Integrator.ForwardRK4(u, x1, x2, G, Y, GAMMA, NEY, dt);

                var (lambda1, lambda2) =
                    AdjointSolver.Solve(x1, x2, u, G, Y, R0, R1, GAMMA, NEY, dt);

                ControlUpdater.UpdateControl(
                    u, x1, x2, lambda1, lambda2,
                    G, Y, Astep, uMin, uMax);
            }

            // Compute Hamiltonian
            for (int i = 0; i <= N; i++)
            {
                Integrator.Deriv(x1[i], x2[i], u[i],
                    G, Y, GAMMA, NEY,
                    out var dx1, out var dx2);

                H[i] =
                    u[i] * (Y - x2[i])
                    - (1 + GAMMA) * R0 * x1[i]
                    - R1 * x1[i]
                    + dx1 + dx2;
            }
        }
    }

}
