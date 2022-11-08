using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Legends;
using OxyPlot.Series;
using RungeKutta4;
using static System.Math;

const double dt = 0.001;

const double tau1 = 1;
const double tau2 = 4;
const double t1   = tau1 / 2;
const double t2   = 6;

var (x0, v0) = (0d, 0d);
var X0 = new[] { x0, v0 };

var YY = RungeKutta4(dX, 0, 10, dt, X0);

var model = new PlotModel
{
    Title = "System dynamics",
    Background              = OxyColors.White,
    PlotAreaBorderThickness = new(0, 1, 0, 0),
    PlotMargins = new OxyThickness(double.NaN).WithTop(-20),
    Axes =
    {
        new LinearAxis
        {
            Title              = "t",
            AxislineThickness  = 1,
            AxislineStyle      = LineStyle.Solid,
            Position           = AxisPosition.Bottom,
            MinorGridlineStyle = LineStyle.Dash,
            MajorGridlineStyle = LineStyle.Solid,
            Unit               = "s",
        },
        new LinearAxis
        {
            Key                = "a",
            Title              = "a(t)",
            PositionTier       = 0,
            AxislineColor      = OxyColors.Red,
            AxislineThickness  = 1,
            AxislineStyle      = LineStyle.Solid,
            Position           = AxisPosition.Left,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColors.Red.Opacity(0.4),
            TicklineColor      = OxyColors.Red,
            MinorTicklineColor = OxyColors.Red,
            TextColor          = OxyColors.Red,
            TitleColor         = OxyColors.Red,
            Unit               = "m/s^2",
        },
        new LinearAxis
        {
            Key                = "V",
            Title              = "v(t)",
            PositionTier       = 1,
            AxislineColor      = OxyColors.Blue,
            AxislineThickness  = 1,
            AxislineStyle      = LineStyle.Solid,
            Position           = AxisPosition.Left,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColors.Blue.Opacity(0.4),
            TicklineColor      = OxyColors.Blue,
            MinorTicklineColor = OxyColors.Blue,
            TextColor          = OxyColors.Blue,
            TitleColor         = OxyColors.Blue,
            Unit               = "m/s",
        },
        new LinearAxis
        {
            Key                = "X",
            Title              = "x(t)",
            PositionTier       = 1,
            AxislineColor      = OxyColors.Black,
            AxislineThickness  = 1,
            AxislineStyle      = LineStyle.Solid,
            Position           = AxisPosition.Right,
            MinorGridlineStyle = LineStyle.Dash,
            MajorGridlineStyle = LineStyle.Solid,
            Unit               = "m",
        },
    },
    Series =
    {
        new LineSeries
        {
            Title = "Ускорение",
            ItemsSource = YY.Select((y, i) => new { X = i * dt, Y = F(i * dt) }),
            DataFieldX  = "X",
            DataFieldY  = "Y",
            Color       = OxyColors.Red,
            YAxisKey    = "a"
        },
        new LineSeries
        {
            Title = "Скорость",
            ItemsSource = YY.Select((y, i) => new { X = i * dt, Y = y[1] }),
            DataFieldX  = "X",
            DataFieldY  = "Y",
            Color       = OxyColors.Blue,
            YAxisKey    = "V"
        },
        new LineSeries
        {
            Title       = "Перемещение",
            ItemsSource = YY.Select((y, i) => new { X = i * dt, Y = y[0] }),
            DataFieldX  = "X",
            DataFieldY  = "Y",
            Color       = OxyColors.Black,
            YAxisKey    = "X"
        },
    },
    Legends =
    {
        new Legend
        {
            LegendPosition = LegendPosition.RightTop,
            LegendBackground = OxyColors.White.Opacity(0.6),
            LegendBorder = OxyColors.Blue,
            LegendBorderThickness = 2
        }
    },
};
var png = new PngExporter(800, 600);

using (var file = File.Create("graph.png"))
    png.Export(model, file);

Console.WriteLine("End.");

static double Pow2(double x) => x * x;

static double Gauss(double x, double sgm) => Exp(-Pow2(x / sgm) / 2) / (sgm * Sqrt(2 * PI));

static double F(double x) => Gauss(x - t1, tau1 / 6d) - Gauss(x - t2, tau2 / 6d);

static double[] dXa(double[] X, double a) => new[]
{
    X[1] + a * dt,
    a
};

static double[] dX(double t, double[] X) => dXa(X, F(t));

static double[] Add(double[] X, double[] Y)
{
    var Z = new double[X.Length];
    for (var i = 0; i < X.Length; i++)
        Z[i] = X[i] + Y[i];
    return Z;
}

static double[] Add4(double[] X1, double[] X2, double[] X3, double[] X4)
{
    var Y = new double[X1.Length];
    for (var i = 0; i < X1.Length; i++)
        Y[i] = X1[i] + 2 * X2[i] + 2 * X3[i] + X4[i];
    return Y;
}

static double[] Mul(double[] X, double y)
{
    var Z = new double[X.Length];
    for (var i = 0; i < X.Length; i++)
        Z[i] = X[i] * y;
    return Z;
}

static double[][] RungeKutta4(Func<double, double[], double[]> f, double t1, double t2, double dt, double[] X0)
{
    var Y  = X0;
    var YY = new List<double[]>((int)((t2 - t1) / dt) + 1);
    for (var t = t1; t <= t2; t += dt)
    {
        var k1 = f(t + 0.0 * dt, Y);
        var k2 = f(t + 0.5 * dt, Add(Y, Mul(k1, dt / 2)));
        var k3 = f(t + 0.5 * dt, Add(Y, Mul(k2, dt / 2)));
        var k4 = f(t + 1.0 * dt, Add(Y, Mul(k3, dt)));

        Y = Add(Y, Mul(Add4(k1, k2, k3, k4), dt / 6));

        YY.Add(Y);
    }

    return YY.ToArray();
}
