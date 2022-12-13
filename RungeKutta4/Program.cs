using System.Diagnostics;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Legends;
using OxyPlot.Series;

var timer = Stopwatch.StartNew();

var (x0, v0) = (0d, 0d);

var YY = RungeKutta.Solve4(SystemModel.dState, 0, 10, SystemModel.dt, new State(x0, v0), 1e-2);

timer.Stop();

Console.WriteLine("Computed {0} points at {1:0.##} c", YY.Length, timer.Elapsed.TotalSeconds);

timer.Restart();

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
            ItemsSource = YY.Select(y => new { X = y.Time, Y = SystemModel.F(y.Time) }),
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

timer.Stop();

Console.WriteLine("Plot at {0:0.##} c", timer.Elapsed.TotalSeconds);

Console.WriteLine("End.");


