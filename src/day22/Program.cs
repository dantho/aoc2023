// https://adventofcode.com/2023/day/22
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day22.txt");

// Example 1 input
lines = new string[] {
    "1,0,1~1,2,1",
    "0,0,2~2,0,2",
    "0,2,3~2,2,3",
    "0,0,4~0,2,4",
    "2,0,5~2,2,5",
    "0,1,6~2,1,6",
    "1,1,8~1,1,9",
};

// // Example 2 input
// lines = new string[] {
//     "bla bla ...",
//     };

List<(Point3D A, Point3D B)> bricks = new();

foreach (var line in lines)
{
    var arr = line.Split('~')
        .Select(s => 
            s.Split(',', StringSplitOptions.TrimEntries).ToPoint3D())
        .ToArray();
    bricks.Add((arr[0], arr[1]));
    Console.WriteLine(line);
}

// Find dimensional limits
Point3D max3D = bricks.SelectMany(tup => new Point3D[] { tup.A, tup.B })
    .Aggregate(new Point3D(int.MinValue, int.MinValue, int.MinValue),
    (maxSoFar, next) => new Point3D(
        next.X > maxSoFar.X ? next.X : maxSoFar.X, 
        next.Y > maxSoFar.Y ? next.Y : maxSoFar.Y, 
        next.Z > maxSoFar.Z ? next.Z : maxSoFar.Z));

// Create empty grid
(Point3D A, Point3D B)?[,,] grid =
    new (Point3D A, Point3D B)?[max3D.X + 1, max3D.Y + 1, max3D.Z + 1];
grid.Validate();

// Fill grid with bricks
foreach ((Point3D A, Point3D B) brick in bricks)
{
    grid.Add(brick);
    grid.Validate();
}

// Let all bricks fall
grid.InvokeGravity();
bricks = grid.Bricks();

// Now remove each brick to see if any others would fall
// Put the brick back after each test, regardless
List<(Point3D A, Point3D B)> nonStructural = new();
foreach (var b in bricks)
{
    grid.Disintegrate(b);
    bool anyWillFall = false;
    foreach (var bb in grid.Bricks())
    {
        if (b == bb) continue;
        if (grid.IsFloating(bb))
        {
            anyWillFall = true;
            break;
        }
    }
    if (!anyWillFall) nonStructural.Add(b);
    grid.Add(b);
}

Console.WriteLine();
foreach (var b in grid.Bricks()) Console.WriteLine(b);

int ansPart1 = nonStructural.Count;

// Part 2 **************************

// Test-Disolve each brick, and let all others fall if as they will.
// Count number of bricks that move (aka "fall")
// Note that disolved brick should NOT be counted among moved bricks
int brickFall = 0;
var saveGrid = ((Point3D A, Point3D B)?[,,])grid.DeepCopy();
var before = saveGrid.Bricks();
foreach (var testBrick in saveGrid.Bricks())
{
    var during = grid.Bricks();

    grid.Disintegrate(testBrick);
    // let all bricks fall
    grid.InvokeGravity();
    // Count other "before" items which are missing "after" brick removal
    // Don't count the testBrick
    var after = grid.Bricks();
    after = grid.Bricks();
    Debug.Assert(after.Count == before.Count - 1);
    brickFall += before.Where(b => !after.Contains(b)).Count() - 1;
    grid = saveGrid;
}

int ansPart2 = brickFall;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

public class Point3D : IEquatable<Point3D>
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
}

public static class MyExtensions
{
    public static Point3D ToPoint3D(this IEnumerable<int> input)
    {
        int[] inXYZ = input.ToArray();
        if (inXYZ.Length != 3)
            throw new Exception($"ToPoint3D expects exactly 3 items. Got {inXYZ.Length}.");
        return new Point3D(inXYZ[0], inXYZ[1], inXYZ[2]);
    }
    public static Point3D ToPoint3D(this IEnumerable<string> input)
    {
        int[] inXYZ = input.Select(s => int.Parse(s)).ToArray();
        if (inXYZ.Length != 3)
            throw new Exception($"ToPoint3D expects exactly 3 items. Got {inXYZ.Length}.");
        return new Point3D(inXYZ[0], inXYZ[1], inXYZ[2]);
    }
    public static List<(Point3D A, Point3D B)> Bricks(this (Point3D A, Point3D B)?[,,] grid)
    {
        HashSet<(Point3D A, Point3D B)> bricksInGrid = new();
        foreach (var b in grid)
        {
            if (b is not null)
                bricksInGrid.Add(((Point3D A, Point3D B))b);
        }        
        return bricksInGrid.OrderBy(b =>
            (b.A.Z, b.B.Z)).ToList();
    }

    public static (Point3D A, Point3D B)?[,,] DeepCopy(this (Point3D A, Point3D B)?[,,] grid)
    {
        var copy = ((Point3D A, Point3D B)?[,,])grid.Clone();
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
            for (int y = 0; y <= grid.GetUpperBound(1); y++)
                for (int z = 0; z <= grid.GetUpperBound(2); z++)
                {
                    if (copy[x, y, z] is null) copy[x, y, z] = null; // Unnecessary?
                    else
                    {
                        var old = copy[x, y, z] ?? throw new Exception("Can't happen");
                        copy[x, y, z] = (old.A, old.B);
                    }
                }
        return copy;
    }
    public static void Add(this (Point3D A, Point3D B)?[,,] grid, (Point3D A, Point3D B) brick)
    {
        brick.Enumerate().ForEach(p => grid.Point(p) = brick);
    }
    public static void Disintegrate(this (Point3D A, Point3D B)?[,,] grid, (Point3D A, Point3D B) brick)
    {
        int bricksBefore = grid.Bricks().Count();
        brick.Enumerate().ForEach(p =>
        {
            if (grid.Point(p) == brick)
            {
                grid.Point(p) = null;
            }
            else if (grid.Point(p) is null)
                throw new Exception($"Removing brick {brick} from already null point at {p}");
            else
                throw new Exception($"Expecting to remove {brick} at {p} but found {grid.Point(p)}");
        });
        int bricksAfter = grid.Bricks().Count();
        Debug.Assert(bricksBefore == 1 + bricksAfter, "brick removal gone wrong");
    }
    public static ref (Point3D A, Point3D B)? Point(this (Point3D A, Point3D B)?[,,] grid, Point3D point)
    {
        return ref grid[point.X, point.Y, point.Z];
    }
    public static bool IsFloating(this (Point3D A, Point3D B)?[,,] grid, (Point3D A, Point3D B) brick)
    {
        const int Floor = 1;
        return brick
            .Enumerate()
            .Aggregate(true, (isFloating, p) =>
                isFloating &&
                p.Z > Floor &&
                (grid[p.X, p.Y, p.Z - 1] is null ||
                 grid[p.X, p.Y, p.Z - 1] == brick));
    }
    public static void Drop(this (Point3D A, Point3D B)?[,,] grid, (Point3D A, Point3D B) brick)
    {
        Debug.Assert(brick.A.Z > 0);
        (Point3D A, Point3D B) brickBelow =
            (new Point3D(brick.A.X, brick.A.Y, brick.A.Z - 1),
             new Point3D(brick.B.X, brick.B.Y, brick.B.Z - 1));

        foreach (Point3D p in brick.Enumerate())
        {
            Debug.Assert(grid.Point(p) == brick);
            grid.Point(p) = null;
        }

        foreach (Point3D p in brickBelow.Enumerate())
        {
            Debug.Assert(grid.Point(p) is null);
            grid.Point(p) = brickBelow;
        }
    }
    public static void Validate(this (Point3D A, Point3D B)?[,,] grid)
    {
        var filledPoints = grid.Bricks().SelectMany(b => b.Enumerate()).ToList();
        var emptyPoints =
            (new Point3D(0, 0, 0),
             new Point3D(grid.GetUpperBound(0),
                         grid.GetUpperBound(1),
                         grid.GetUpperBound(2)))
            .Enumerate().Where(p => !filledPoints.Contains(p)).ToList();
        List<Point3D> problems = new();
        foreach (var brick in grid.Bricks())
        {
            foreach (var p in brick.Enumerate())
            {
                if (grid.Point(p) != brick)
                    problems.Add(p);
            }
        }
        // Check cells that should be null are null
        foreach (var p in emptyPoints)
        {
            if (grid.Point(p) is not null)
                problems.Add(p);
        }
        if (problems.Count > 0)
        {
            string? firstProb = grid.Point(problems[0]) is null ? "null entry" : grid.Point(problems[0]).ToString();
            throw new Exception($"Grid does NOT validate. Found {problems.Count} problems.\n"
            + $"First is: At point {problems[0]} found {firstProb}");
        }
    }
    public static void InvokeGravity(this (Point3D A, Point3D B)?[,,] grid)
    {
        bool anyBrickFell;
        do
        {
            anyBrickFell = false;
            var bricks = grid.Bricks();
            foreach (var brick in bricks)
            {
                if (grid.IsFloating(brick))
                {
                    grid.Drop(brick);
                    grid.Validate();
                    anyBrickFell = true;
                }
            }
        } while (anyBrickFell);
        grid.Validate();
    }

    public static List<Point3D> Enumerate(this (Point3D A, Point3D B) brick)
    {
        List<Point3D> points = new();
        ((int fromX, int fromY, int fromZ),
         (int toX, int toY, int toZ)) = brick;
        Debug.Assert(fromX <= toX && fromY <= toY && fromZ <= toZ);
        for (int x = fromX; x <= toX; x++)
            for (int y = fromY; y <= toY; y++)
                for (int z = fromZ; z <= toZ; z++)
                    points.Add(new Point3D(x, y, z));
        return points;
    }
}