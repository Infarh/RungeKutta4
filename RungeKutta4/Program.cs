using System.Data;
using System.Diagnostics;

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
const double t1 = tau1 / 2;
const double t2 = 6;
var tt = Stopwatch.StartNew();

var (x0, v0) = (0d, 0d);

var YY = RungeKutta4(dX, 0, 10, dt, new State(x0, v0), 1e-5);

var model = new PlotModel
{
    Title = "System dynamics",
    Background = OxyColors.White,
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
            Title       = "Ускорение",
            ItemsSource = YY.Select(y => new { X = y.Time, Y = F(y.Time) }),
            DataFieldX  = "X",
            DataFieldY  = "Y",
            Color       = OxyColors.Red,
            YAxisKey    = "a"
        },
        new LineSeries
        {
            Title       = "Скорость",
            ItemsSource = YY.Select(y => new { X = y.Time, Y = y.Value.V }),
            DataFieldX  = "X",
            DataFieldY  = "Y",
            Color       = OxyColors.Blue,
            YAxisKey    = "V"
        },
        new LineSeries
        {
            Title       = "Перемещение",
            ItemsSource = YY.Select(y => new { X = y.Time, Y = y.Value.X }),
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
            LegendPosition        = LegendPosition.RightTop,
            LegendBackground      = OxyColors.White.Opacity(0.6),
            LegendBorder          = OxyColors.Blue,
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

static State dX(double t, State X)
{
    var a = F(t);
    return new(X.V + a * dt, a);
}

static (double Time, T Value)[] RungeKutta4<T>(
    Func<double, T, T> f,
    double t1,
    double t2,
    double dt0,
    T X0,
    double eps = 1e-5)
    where T : IComputable<T>
{
    var dt = dt0;
    var Y = X0;
    var YY = new List<(double, T)>((int)((t2 - t1) / dt0) + 1);
    var t = t1;
    var dY_last = default(T);
    while (t <= t2)
    {
        var k1 = f(t, Y);

        var dY0 = RungeKutta4Step(f, t, Y, k1, dt);

        if (dY_last != default)
        {
            var dY1 = RungeKutta4Step(f, t, Y, k1, dt * 2);

            if (dY1 - dY_last < eps)
            {
                Console.WriteLine("t:{0,-20} | dt:{1,-20} ->   2dt:{2}", t, dt, dt * 2);
                dt *= 2;
                (dY0, dY1) = (dY1, RungeKutta4Step(f, t, Y, k1, dt * 2));
            }
            else
            {
                dY1 = RungeKutta4Step(f, t, Y, k1, dt / 2);
                while (dY0 - dY1 > eps)
                {
                    Console.WriteLine("t:{0,-20} | dt:{1,-20} -> 0.5dt:{2}", t, dt, dt / 2);
                    dt /= 2;
                    dY0 = RungeKutta4Step(f, t, Y, k1, dt);
                }
            }
        }

        dY_last = dY0;
        Y += dY0;
        t += dt;
        YY.Add((t, Y));
    }

    return YY.ToArray();
}

static T RungeKutta4Step<T>(Func<double, T, T> f, double t, T Y, T k1, double dt)
    where T : IComputable<T>
{
    var k2 = f(t + 0.5 * dt, Y + dt / 2 * k1);
    var k3 = f(t + 0.5 * dt, Y + dt / 2 * k2);
    var k4 = f(t + 1.0 * dt, Y + dt * k3);

    var dY = dt / 6 * (k1 + 2 * k2 + 2 * k3 + k4);

    return dY;
}
