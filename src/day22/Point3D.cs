// https://adventofcode.com/2023/day/22
// End
// End
// End

public class Point3D : IEquatable<Point3D>, IComparable<Point3D>
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Z { get; init; }

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public void Deconstruct(out int x, out int y, out int z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public override string ToString()
    {
        return $"{X},{Y},{Z}";
    }

    bool IEquatable<Point3D>.Equals(Point3D? other)
    {
        if (other is null)
            return false;
        return this.X == other.X &&
               this.Y == other.Y &&
               this.Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return ((IEquatable<Point3D>)this).Equals(obj as Point3D);
    }

    public override int GetHashCode()
    {
        return (this.X * 17 + this.Y) * 17 + this.Z;
    }

    public int CompareTo(Point3D? other)
    {
        if (other is null) return -2;
        if (this.Z < other.Z) return -1;
        if (this.Z > other.Z) return 1;
        if (this.X < other.X) return -1;
        if (this.X > other.X) return 1;
        if (this.Y < other.Y) return -1;
        if (this.Y > other.Y) return 1;
        return 0;
    }
}
