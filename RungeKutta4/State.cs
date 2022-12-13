using static System.Math;

namespace RungeKutta4;

public readonly struct State : IComputable<State>
{
    public double X { get; }

    public double V { get; }

    public State(double X, double V) => (this.X, this.V) = (X, V);

    public override string ToString() => $"X:{X} V:{V}";

    public bool Equals(State other) => X.Equals(other.X) && V.Equals(other.V);

    public override bool Equals(object? obj) => obj is State other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, V);

    public static State operator +(State a, State b) => new(a.X + b.X, a.V + b.V);
    public static State operator -(State a, State b) => new(a.X - b.X, a.V - b.V);

    public static State operator *(State a, double b) => new(a.X * b, a.V * b);

    public static State operator *(double b, State a) => new(a.X * b, a.V * b);

    public static bool operator ==(State a, State b) => (a.X, a.V) == (b.X, b.V);

    public static bool operator !=(State a, State b) => !(a == b);

    public static bool operator <(State a, double b) => Abs(a.X) < b && Abs(a.V) < b;

    public static bool operator >(State a, double b) => Abs(a.X) > b && Abs(a.V) > b;
}
