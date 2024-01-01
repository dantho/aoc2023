// https://adventofcode.com/2023/day/24
// Creating structs as value types for comparison an processing speed:
// https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
// Operator Overloading:
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading

public struct Point3D : IEquatable<Point3D>
{
    public long X;
    public long Y;
    public long Z;
    public Point3D(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public Point3D(IEnumerable<long> vals)
    {
        long[] a = vals.ToArray();
        if (a.Length != 3) throw new Exception($"'IEnumerable<long> vals' contains {a.Length} items. Expecting exactly 3 (for x,y and z).");
        X = a[0];
        Y = a[1];
        Z = a[2];
    }
    public Point3D(IEnumerable<string> vals)
    {
        string[] a = vals.ToArray();
        if (a.Length != 3) throw new Exception($"'IEnumerable<string> vals' contains {a.Length} items. Expecting exactly 3 (for x,y and z).");
        X = long.Parse(a[0]);
        Y = long.Parse(a[1]);
        Z = long.Parse(a[2]);
    }
    public void Deconstruct(out long x, out long y, out long z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public bool Equals(Point3D other)
    {
        return  this.X == other.X &&
                this.Y == other.Y &&
                this.Z == other.Z;
    }

    public override string ToString() => $"{X},{Y},{Z}";

    public static Point3D operator +(Point3D a, Point3D b) =>
        new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Point3D operator -(Point3D a, Point3D b) =>
        new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Point3D operator +(Point3D a) => a;
    public static Point3D operator -(Point3D a) =>
        new Point3D(-a.X, -a.Y, -a.Z);
}

public struct Point3D<T>
{
    public T X;
    public T Y;
    public T Z;
    public Point3D(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public Point3D(IEnumerable<T> vals)
    {
        T[] a = vals.ToArray();
        if (a.Length != 3) throw new Exception($"'IEnumerable<T> vals' contains {a.Length} items. Expecting exactly 3 (for x,y and z).");
        X = a[0];
        Y = a[1];
        Z = a[2];
    }
    public void Deconstruct(out T x, out T y, out T z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public override string ToString() => $"{X},{Y},{Z}";
}
