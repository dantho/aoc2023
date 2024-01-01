// https://adventofcode.com/2023/day/24
// Creating structs as value types for comparison an processing speed:
// https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
// Operator Overloading:
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading

using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day24.txt");

//// Example 1 input
//lines = new string[]
//{
//    "19, 13, 30 @ -2,  1, -2",
//    "18, 19, 22 @ -1, -1, -2",
//    "20, 25, 34 @ -2, -2, -4",
//    "12, 31, 28 @ -1, -2, -1",
//    "20, 19, 15 @  1, -5, -3",
//};

bool usingExample = lines.Length < 25;
float lowerLimit = usingExample ? 7.0f : 200000000000000.0f;
float upperLimit = usingExample ? 27.0f : 400000000000000.0f;

var halves = lines.SelectMany(l => l.Split('@', StringSplitOptions.TrimEntries));
(Point3D Pos, Velocity3D Speed)[] data = halves
    .PairUp()
    .Select(pair =>
        (new Point3D(pair.First.Split(',', StringSplitOptions.TrimEntries)),
         new Velocity3D(pair.Second.Split(',', StringSplitOptions.TrimEntries))))
    .ToArray();

List<Point3D<float>> xy = new();

foreach (var A in data[..^1])
{
    bool passedA = false;
    foreach (var B in data)
    {
        if (A.Equals(B))
        {
            passedA = true;
            continue;
        }
        if (!passedA) continue;
        var maybe_xy = XYIntercept(A, B);
        if (maybe_xy.X != float.NaN) xy.Add(maybe_xy);
    }
}
int ansPart1 = xy.Where(p =>
    p.X >= lowerLimit && p.X <= upperLimit &&
    p.Y >= lowerLimit && p.Y <= upperLimit
    ).Count();
var debug = xy.Where(p =>
    p.X >= lowerLimit && p.X <= upperLimit &&
    p.Y >= lowerLimit && p.Y <= upperLimit
    ).ToArray();

foreach (var line in lines)
{
    Console.WriteLine(line);
}

long ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

static Point3D MagicThrowInitialPosition(IEnumerable<(Point3D, Velocity3D)> data)
{
    // Let's assume it's 1D instead of 3D...
    // x(t) = dx*t + x
    // mx(t) = mdx * t + mx (where 'm' means magic!)
    // at some t1, mx(t1) = x(t1)
    // at another t2, mx(t2) = x'(t2)
    // at t3, mx(t3) = x''(t3)
    // etc.
    // The magic equation in 1D has 2 unknowns plus t1, t2, t3, etc.
    // dx1*t1+x1 = mdx*t1+mx
    // dx2*t2+x2 = mdx*t2+mx
    // dx3*t3+x3 = mdx*t3+mx
    
    return new();
}

static Point3D<float> XYIntercept((Point3D Pos, Velocity3D Velocity) a, (Point3D Pos, Velocity3D Velocity) b, bool excludeNegTime = true)
{
    // Considering only X and Y,
    // Equations for X(t) and Y(t) in form y = mx + y0 for both a and b...
    // ax = adx*timeA + ax0 and
    // bx = bdx*timeB + bx0 
    // ay = ady*timeA + ay0 and
    // by = bdy*timeB + by0
    (float ax0, float ay0, float _) = new Point3D<float>(a.Pos.X, a.Pos.Y, a.Pos.Z);
    (float adx, float ady, float _) = new Point3D<float>(a.Velocity.dX, a.Velocity.dY, a.Velocity.dZ);
    (float bx0, float by0, float _) = new Point3D<float>(b.Pos.X, b.Pos.Y, b.Pos.Z);
    (float bdx, float bdy, float _) = new Point3D<float>(b.Velocity.dX, b.Velocity.dY, b.Velocity.dZ);
    // -TimeA = (ax0 - ax) / adx = (ay0 - ay) / ady
    // -TimeB = (bx0 - bx) / bdx = (by0 - by) / bdy
    // Solve for ax == bx (we'll just call it "x") and ay == by ("y"),
    // regardless of timeA and timeB, to find cross point of a and b in x and y.
    // (ax0 - x) / adx = (ay0 - y) / ady
    // (bx0 - x) / bdx = (by0 - y) / bdy
    // substituting x into equation for y...
    // ax0 - x = (ay0 - y) * adx / ady
    // x = ax0 + (y - ay0) * adx / ady (1)
    // y = by0 + (x - bx0) * bdy / bdx (2)
    // y = by0 + (ax0 + (y - ay0)*adx / ady - bx0) * bdy / bdx
    // y = by0 + ax0 * bdy/bdx + y*adx*bdy/ady/bdx - ay0*adx*bdy/(ady*bdx) - bx0*bdy/bdx
    // y*(1-adx*bdy/ady/bdx)  = by0 + ax0 * bdy/bdx - ay0*adx*bdy/(ady*bdx) - bx0*bdy/bdx
    // y = (by0 + ax0 * bdy/bdx - ay0*adx*bdy/(ady*bdx) - bx0*bdy/bdx) / (1-adx*bdy/(ady*bdx))
    float y = (by0 + ax0 * bdy / (float)bdx - ay0 * adx * bdy / (ady * bdx) - bx0 * bdy / bdx)
                / (1 - adx * bdy / (ady * bdx));
    float x = ax0 + (y - ay0) * adx / ady;
    if (excludeNegTime && x != float.NaN && y != float.NaN)
    {
        // Exclude negative time
        float timeA = (x - ax0) / adx;
        float timeB = (x - bx0) / bdx;
        if (timeA < 0 || timeB < 0)
        {
            x = float.NaN;
            y = float.NaN;
        }
    }
    return new(x, y, 0);
}

