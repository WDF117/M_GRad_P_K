namespace M_GRad_P_K
{
    public static class Integrator
    {
        public static void ForwardEuler(
            double[] y, double[] z,
            double a, double c,
            double dt)
        {
            for (int i = 0; i < y.Length - 1; i++)
            {
                y[i + 1] = y[i] + dt * (a * y[i] + c - z[i]);
            }
        }
    }
}


