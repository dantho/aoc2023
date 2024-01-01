// https://adventofcode.com/2023/day/24
// Creating structs as value types for comparison an processing speed:
// https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
// Operator Overloading:
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading

// End
// End
// End

public struct Velocity3D : IEquatable<Velocity3D>
{
    public int dX;
    public int dY;
    public int dZ;

    public Velocity3D(int dx, int dy, int dz)
    {
        dX = dx;
        dY = dy;
        dZ = dz;
    }
    public Velocity3D(IEnumerable<int> vals)
    {
        int[] a = vals.ToArray();
        if (a.Length != 3) throw new Exception($"'IEnumerable<int> vals' contains {a.Length} items. Expecting exactly 3 (for x,y and z).");
        dX = a[0];
        dY = a[1];
        dZ = a[2];
    }
    public Velocity3D(IEnumerable<string> vals)
    {
        string[] a = vals.ToArray();
        if (a.Length != 3) throw new Exception($"'IEnumerable<string> vals' contains {a.Length} items. Expecting exactly 3 (for x,y and z).");
        dX = int.Parse(a[0]);
        dY = int.Parse(a[1]);
        dZ = int.Parse(a[2]);
    }
    public void Deconstruct(out int dx, out int dy, out int dz)
    {
        dx = dX;
        dy = dY;
        dz = dZ;
    }
    public bool Equals(Velocity3D other)
    {
        return this.dX == other.dX &&
                this.dY == other.dY &&
                this.dZ == other.dZ;
    }

    public override string ToString() => $"{dX},{dY},{dZ}";

    public static Point3D operator *(Velocity3D vel, int time)
    {
        return new Point3D(vel.dX * time, vel.dY * time, vel.dZ * time);
    }
}
