using static System.Math;

const double tau11 = 4;
const double tau12 = 1;

const double t11 = tau11 / 2;
const double t12 = 7;

const double x11 = t11 - tau11 / 2;
const double x12 = t12 + tau12 / 2;

/* ---------------------------------------- */

const double dt = 0.1;


const double tau21 = 1;
const double tau22 = 4;
const double t21   = tau11 / 2;
const double t22   = 7;

const double x21 = t21 - tau21 / 2;
const double x22 = t22 + tau22 / 2;

var (x0, v0) = (0d, 0d);
var X = new[] { x0, v0 };



Console.WriteLine("End.");

static double Pow2(double x) => x * x;

static double Gauss(double x, double sgm) => Exp(-Pow2(x / sgm)) / (Sqrt(PI) * sgm);

static double F1(double x) => Gauss(x - t11, 1 / 6d * tau11) - Gauss(x - t12, 1 / 6d * tau12);
static double F2(double x) => Gauss(x - t21, 1 / 6d * tau21) - Gauss(x - t22, 1 / 6d * tau22);

static double[] dXa(double t, double[] X, double a) => new[]
{
    X[1] + a * dt / 2,
    a
};

static double[] dX(double t, double[] X) => dXa(t, X, F1(t));

static double[] Add(double[] X, double[] Y)
{
    var Z = new double[X.Length];
    for(var i = 0; i < X.Length; i++)
        Z[i] = X[i] + Y[i];
    return Z;
}

static double[] Add4(double[] X1, double[] X2, double[] X3, double[] X4)
{
    var Y = new double[X1.Length];
    for (var i = 0; i < X1.Length; i++)
        Y[i] = X1[i] + X2[i] + X3[i] + X4[i];
    return Y;
}

static double[] Mul(double[] X, double y)
{
    var Z = new double[X.Length];
    for(var i = 0; i < X.Length; i++)
        Z[i] = X[i] + y;
    return Z;
}

static double[][] RungeKutta4(Func<double, double[], double[]> f, double t1, double t2, double dt, double[] X0)
{
    var Y  = X0;
    var YY = new List<double[]>((int)((t2 - t1) / dt) + 1);
    for (var t = t1; t <= t2; t += dt)
    {
        var k1 = f(t, Y);
        var k2 = f(t + dt / 2, Add(Y, Mul(k1, dt / 2)));
        var k3 = f(t + dt / 2, Add(Y, Mul(k2, dt / 2)));
        var k4 = f(t + dt, Add(Y, Mul(k3, dt)));

        var Y1 = Add4(k1, Mul(k2, 2), Mul(k3, 2), k4);
        Y = Add(Y, Mul(Y1, dt / 6));

        YY.Add(Y);
    }

    return YY.ToArray();
}