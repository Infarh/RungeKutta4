using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RungeKutta4;

public static class RungeKutta
{
    public static (double Time, T Value)[] Solve4<T>(
        Func<double, T, T> f,
        double t1,
        double t2,
        double dt0,
        T X0,
        double eps = 1e-5)
        where T : IComputable<T>
    {
        var dt      = dt0;
        var Y       = X0;
        var YY      = new List<(double, T)>((int)((t2 - t1) / dt0) + 1);
        var t       = t1;
        var dY_last = default(T);
        while (t <= t2)
        {
            var k1 = f(t, Y);

            var dY0 = Step4(f, t, Y, k1, dt);

            if (dY_last != default)
            {
                var dY1 = Step4(f, t, Y, k1, dt * 2);

                if (dY1 - dY_last < eps)
                {
                    Console.WriteLine("t:{0,-20} | dt:{1,-20} ->   2dt:{2}", t, dt, dt * 2);
                    dt         *= 2;
                    (dY0, dY1) =  (dY1, Step4(f, t, Y, k1, dt * 2));
                }
                else
                {
                    dY1 = Step4(f, t, Y, k1, dt / 2);
                    while (dY0 - dY1 > eps)
                    {
                        Console.WriteLine("t:{0,-20} | dt:{1,-20} -> 0.5dt:{2}", t, dt, dt / 2);
                        dt  /= 2;
                        dY0 =  Step4(f, t, Y, k1, dt);
                    }
                }
            }

            dY_last =  dY0;
            Y       += dY0;
            t       += dt;
            YY.Add((t, Y));
        }

        return YY.ToArray();
    }

    private static T Step4<T>(Func<double, T, T> f, double t, T Y, T k1, double dt)
        where T : IComputable<T>
    {
        var k2 = f(t + 0.5 * dt, Y + dt / 2 * k1);
        var k3 = f(t + 0.5 * dt, Y + dt / 2 * k2);
        var k4 = f(t + 1.0 * dt, Y + dt * k3);

        var dY = dt / 6 * (k1 + 2 * k2 + 2 * k3 + k4);

        return dY;
    }
}
