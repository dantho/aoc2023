// https://adventofcode.com/2023/day/22
using System.Data.Common;
using System.Diagnostics;

namespace AoCDay22
{
    public class Brick : IEquatable<Brick>, IComparable<Brick>
    {
        public Point3D A { get; init; }
        public Point3D B { get; init; }
        public Brick(Point3D start, Point3D end)
        {
            A = start;
            B = end;
        }
        public Brick Copy()
        {
            return new Brick(this.A, this.B);
        }
        public List<Point3D> Enumerate()
        {
            List<Point3D> points = new();
            ((int fromX, int fromY, int fromZ),
             (int toX, int toY, int toZ)) = this;
            Debug.Assert(fromX <= toX && fromY <= toY && fromZ <= toZ);
            for (int x = fromX; x <= toX; x++)
                for (int y = fromY; y <= toY; y++)
                    for (int z = fromZ; z <= toZ; z++)
                        points.Add(new Point3D(x, y, z));
            return points;
        }

        private void Deconstruct(out (int fromX, int fromY, int fromZ) Item1, out (int toX, int toY, int toZ) Item2)
        {
            Item1 = (this.A.X, this.A.Y, this.A.Z);
            Item2 = (this.B.X, this.B.Y, this.B.Z);
        }
        public override string ToString()
        {
            return $"({this.A.X},{this.A.Y},{this.A.Z}),({this.B.X},{this.B.Y},{this.B.Z})";
        }

        public bool EqualValues(Brick? other)
        {
            if (other is null) return false;
            return
                this.A.Equals(other.A) &&
                this.B.Equals(other.B);
        }

        public override int GetHashCode()
        {
            return this.A.GetHashCode() * 23 + this.B.GetHashCode();
        }

        public bool Equals(Brick? other)
        {
            if (other is null) return false;
            return this.A.Equals(other.A) && this.B.Equals(other.B);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Brick);
        }

        public int CompareTo(Brick? other)
        {
            if (other is null) return -2;
            return this.A.CompareTo(other.A) != 0
                ? this.A.CompareTo(other.A)
                : this.B.CompareTo(other.B);
        }
    }
}
