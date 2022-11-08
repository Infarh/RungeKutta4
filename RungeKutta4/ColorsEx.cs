using OxyPlot;

namespace RungeKutta4;

internal static class ColorsEx
{
    public static OxyColor Opacity(this OxyColor color, double Ratio) => OxyColor.FromAColor((byte)(byte.MaxValue * Ratio), color);
}
