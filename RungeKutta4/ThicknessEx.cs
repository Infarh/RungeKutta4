using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace RungeKutta4;
internal static class ThicknessEx
{
    public static OxyThickness WithTop(this OxyThickness thickness, double Top) => new(thickness.Left, Top, thickness.Right, thickness.Bottom);
    public static OxyThickness Right(this OxyThickness thickness, double Right) => new(thickness.Left, thickness.Top, Right, thickness.Bottom);
    public static OxyThickness Bottom(this OxyThickness thickness, double Bottom) => new(thickness.Left, thickness.Top, thickness.Right, Bottom);
    public static OxyThickness Left(this OxyThickness thickness, double Left) => new(Left, thickness.Top, thickness.Right, thickness.Bottom);
}
