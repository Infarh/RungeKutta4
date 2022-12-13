using static System.Math;

namespace RungeKutta4;

public static class SystemModel
{
    public const double dt = 0.001;

    const double tau1 = 1;
    const double tau2 = 4;
    const double t1 = tau1 / 2;
    const double t2 = 6;

    private static double Pow2(double x) => x * x;

    private static double Gauss(double x, double sgm) => Exp(-Pow2(x / sgm) / 2) / (sgm * Sqrt(2 * PI));

    public static double F(double x) => Gauss(x - t1, tau1 / 6d) - Gauss(x - t2, tau2 / 6d);

    public static State dState(double t, State X)
    {
        var a = F(t);
        return new(X.V + a * dt, a);
    }
}
