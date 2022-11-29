namespace RungeKutta4;

public interface IComputable<T> where T : IComputable<T>
{
    static abstract T operator +(T a, T b);
    static abstract T operator -(T? a, T? b);

    static abstract T operator *(T a, double b);
    
    static abstract T operator *(double a, T b);

    static abstract bool operator ==(T? a, T? b);
    static abstract bool operator !=(T? a, T? b);

    static abstract bool operator <(T a, double b);

    static abstract bool operator >(T a, double b);
}
