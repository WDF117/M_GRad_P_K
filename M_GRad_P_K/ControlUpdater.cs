using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_GRad_P_K
{
    public static class ControlUpdater
    {
        public static void UpdateControl(
            double[] u,
            double[] x1, double[] x2,
            double[] lambda1, double[] lambda2,
            double G, double Y,
            double Astep,
            double uMin, double uMax)
        {
            int N = u.Length - 1;

            for (int i = 0; i <= N; i++)
            {
                double grad =
                    (Y - x2[i])
                    + lambda1[i] * (-G * (Y - x2[i]) * x1[i])
                    + lambda2[i] * (G * (Y - x2[i]) * x1[i]);

                double newU = u[i] + Astep * grad;

                if (newU < uMin) newU = uMin;
                if (newU > uMax) newU = uMax;

                u[i] = newU;
            }
        }
    }

}
