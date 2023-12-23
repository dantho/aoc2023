// https://adventofcode.com/2023/day/18
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Linq;

int aocPart = 2;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day18.txt");

//// Example 1 input
//lines = new string[] {
//    "R 6 (#70c710)",
//    "D 5 (#0dc571)",
//    "L 2 (#5713f0)",
//    "D 2 (#d2c081)",
//    "R 2 (#59c680)",
//    "D 2 (#411b91)",
//    "L 5 (#8ceee2)",
//    "U 2 (#caa173)",
//    "L 1 (#1b58a2)",
//    "U 2 (#caa171)",
//    "R 2 (#7807d2)",
//    "U 3 (#a77fa3)",
//    "L 2 (#015232)",
//    "U 2 (#7a21e3)",
//};

List<Instruction> instructions = new();
foreach (var line in lines)
{
    string[] split = line.Split(' ');
    char relChar = split[0][0];
    int steps = int.Parse(split[1]);
    string color = split[2][2..^1];
    instructions.Add(new(relChar, steps, color));
}

if (aocPart == 2)
    instructions = instructions.Select(i => i.ToPart2()).ToList();

    // Convert relative to absolute, starting at (0,0) absolute
    // And calculating each corner
    Point current = new(0,0);
List<Point> nodes = new() { current };

foreach (Instruction instr in instructions)
{
    current = current.Move(instr.RelDir, instr.Steps);
    nodes.Add(current);
}
Debug.Assert(nodes[0] == nodes[^1]);

// Find bounds of square region
int minX = int.MaxValue;
int maxX = int.MinValue;
int minY = int.MaxValue;
int maxY = int.MinValue;
foreach (Point p in nodes)
{
    minX = p.X < minX ? p.X : minX;
    minY = p.Y < minY ? p.Y : minY;
    maxX = p.X > maxX ? p.X : maxX;
    maxY = p.Y > maxY ? p.Y : maxY;
}

// Normalize to positive indices only
nodes = nodes.Select(n => new Point(n.X - minX, n.Y - minY)).ToList();

// Find bounds of square region (again)
minX = int.MaxValue;
maxX = int.MinValue;
minY = int.MaxValue;
maxY = int.MinValue;
foreach (Point p in nodes)
{
    minX = p.X < minX ? p.X : minX;
    minY = p.Y < minY ? p.Y : minY;
    maxX = p.X > maxX ? p.X : maxX;
    maxY = p.Y > maxY ? p.Y : maxY;
}
Debug.Assert(minX == 0 && minY == 0);

foreach (var line in lines)
    Console.WriteLine(line);

Console.WriteLine(
    $"Grid spans x from {minX} to {maxX}, y from {minY} to {maxY}. "
  + $"Size is {maxX + 1} x {maxY + 1}");

// Need smart algo:
// For each rectangle defined by a pair of sides and either the left or right edge of the map,
// depending on direction of vertical side formation, find overlap of each rectangle with any
// others facing in the opposite direction. Bisect both left-facing and right-facing rectangles
// into as many as 4 rectangles.  Each rectangle must remember it's own "overlap count" with
// rectangles facing the other direction.
// "Inside" area will have an even count of overlap with an opposite facing rectangle.
// "Outside area will have an odd overlap.

// Rectangle definition is subtle -- edge-inclusive or not depends on rectangles above/below.
// Do initial rectangles formation with overlapping edges, then post process?
// Overlapping horizontal edges are "retained" by the rectangle with the longer edge.
// The shorter-edged rectangle becomes vertically shorter by 1.  (Or two.)

// Collect endpoint pairs for every vertical border segment (inclusive on both ends)
int verticalSelectorOddEven = nodes[0].X == nodes[1].X ? 0 : 1;
var pairs = nodes.Zip(nodes.Skip(1)).Where((_, i) => i % 2 == verticalSelectorOddEven);
foreach ((Point,Point) pair in pairs)
{
    Debug.Assert(pair.Item1.X == pair.Item2.X);
}
var vSegments = pairs.Select(pair => (pair.Item1.X, pair.Item1.Y, pair.Item2.Y));

int? priorSecondItem2 = null;

// Circular nature of segments requires extras added (and ultimately removed)
vSegments = vSegments.Concat(vSegments.Take(2)).ToList();
var vSegsmentsShifted = vSegments.Skip(1);
// Eliminate endpoint overlap by eliminating the overlap on the "shorter" rectangle
// Also create new data format while we're at it
List<(bool IsDown, int X, int Y0, int Y1)> nonOverlapping = vSegments.Zip(vSegsmentsShifted).Select(pair =>
{
    Debug.Assert(pair.First.Item3 == pair.Second.Item2);
    if (priorSecondItem2 is not null)
        pair.First.Item2 = (int)priorSecondItem2;
    bool isValid = true;
    bool isDown = pair.First.Item3 > pair.First.Item2;
    bool secondIsDown = pair.Second.Item3 > pair.Second.Item2;
    if (secondIsDown == isDown)
    {
        if (isDown)
        {
            if (pair.First.X > pair.Second.X) // Larger X wins going down with clockwise rotation
                pair.Second.Item2 += 1; // down one
            else
                pair.First.Item3 -= 1; // up one
            // Check and flag rare complete elimination of first segment
            isValid &= !(pair.First.Item2 > pair.First.Item3);
        }
        else // moving up
        {
            if (pair.First.X < pair.Second.X) // Smaller X wins going up with clockwise rotation
                pair.Second.Item2 -= 1; // up one
            else
                pair.First.Item3 += 1; // down one
            // Check and flag rare complete elimination of first segment
            isValid &= !(pair.First.Item2 < pair.First.Item3);
        }
        priorSecondItem2 = pair.Second.Item2;
    }
    else
    {
        priorSecondItem2 = null;
    }
    return (isValid, firstIsDown:isDown, pair.First);
}).Skip(1).Where(tup => tup.isValid).Select(tup => (tup.firstIsDown, tup.First.X, tup.First.Item2, tup.First.Item3)).ToList();
// Reverse Y0/Y1 when !isDown, such that Y0 is always smaller
nonOverlapping = nonOverlapping.Select(tup =>
    tup.IsDown ? tup :
    (tup.IsDown, tup.X, tup.Y1, tup.Y0)
    ).ToList();

// Now break up all vertical segments into pieces such that any overlap is complete or zero
// So (x0,0,4) and (x1,2,5) break into (x0,0,1) (x0,2,4) and (x1,2,4) (x1,5,5)
var lowYs = nonOverlapping.Select(tup => tup.Y0).ToList();

List<(bool IsDown, int X, int Y0, int Y1)> segs = new();
foreach (var seg in nonOverlapping)
{
    var myLowSideBreaks = lowYs.Where(lo => lo > seg.Y0 && lo <= seg.Y1).ToList();
    myLowSideBreaks.Sort();
    int pos = seg.Y0;
    foreach (int newBreak in myLowSideBreaks)
    {
        segs.Add((seg.IsDown, seg.X, pos, newBreak - 1));
        pos = newBreak;
    }
    // Finish with final (or only/whole) segment:
    segs.Add((seg.IsDown, seg.X, pos, seg.Y1));
}

nonOverlapping = segs;
segs = new();
var highYs = nonOverlapping.Select(tup => tup.Y1).ToList();

foreach (var seg in nonOverlapping)
{
    var myHighSideBreaks = highYs.Where(lo => lo >= seg.Y0 && lo < seg.Y1).ToList();
    myHighSideBreaks.Sort();
    int pos = seg.Y0;
    foreach (int newBreak in myHighSideBreaks)
    {
        segs.Add((seg.IsDown, seg.X, pos, newBreak));
        pos = newBreak+1;
    }
    // Finish with final (or only/whole) segment:
    segs.Add((seg.IsDown, seg.X, pos, seg.Y1));
}

nonOverlapping = segs;


// Now vertical segments all overlap FULLY (both endpoints match) with 1 or more others, in up/down pairs.
// When sets are sorted by X, they should alternate up/down/up/down.  (Or down/up/down/up if counter-clockwise.)
// Filled area is between up/down pairs, inclusive of both endpoints.

// Reorder the tuple so a default sort will do what I want
// Because I don't know how to change the default sort
var newTuple = nonOverlapping.Select(tup => (tup.Y0, tup.Y1, tup.X, tup.IsDown)).ToList();
newTuple.Sort();

int ansPart1 = 0;

for (int i = 0; i < newTuple.Count-1; i+=2)
{
    var tup1 = newTuple[i];
    var tup2 = newTuple[i + 1];
    //Debug.Assert(tup1.IsDown != tup2.IsDown);
    //Debug.Assert(tup1.X < tup2.X);
    Debug.Assert(tup1.Y0 == tup2.Y0);
    Debug.Assert(tup1.Y1 == tup2.Y1);
    ansPart1 += (tup1.Y1 - tup1.Y0 + 1) * (tup2.X - tup1.X + 1);
}

//// Create blank map of sufficient size
//char[,] map = new char[maxX + 1, maxY + 1];
//for (int x = 0; x <= maxX; x++)
//    for (int y = 0; y <= maxY; y++)
//        map[x, y] = '.';
//// print map
//string printable = PrintableMap(map);
//Console.WriteLine(printable);

if (ansPart1 > 1000)
    Debug.Assert(ansPart1 == 67891);

int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

string PrintableMap(char[,] map)
{
    StringBuilder sb = new();
    for (int y = minY; y <= maxY; y++)
    {
        for (int x = 0; x <= maxX; x++)
            sb.Append(map[x, y]);
        sb.AppendLine();
    }
    return sb.ToString();
}

public class Map
{
    public int[,] Grid;
    public int MaxX;
    public int MaxY;
}

public class Instruction {
    public RelDir RelDir;
    public int Steps;
    public string Color;
    public Instruction(char charUpDownLeftRight, int steps, string color)
    {
        switch (char.ToUpper(charUpDownLeftRight)) {
            case 'U':
                RelDir = RelDir.Up;
                break;
            case 'D':
                RelDir = RelDir.Down;
                break;
            case 'L':
                RelDir = RelDir.Left;
                break;
            case 'R':
                RelDir = RelDir.Right;
                break;
            default:
                throw new Exception("Input parsing error");
        }
        Steps = steps;
        Color = color;
    }
}

public enum RelDir {
    Up,
    Down,
    Left,
    Right,
}

public static class MyExtensions
{
    public static int Hex2Dec(this string hex)
    {
        int value = 0;
        foreach (char c in hex)
        {
            value *= 16;
            switch (char.ToUpper(c))
            {
                case 'F':
                    value += 15;
                    break;
                case 'E':
                    value += 15;
                    break;
                case 'D':
                    value += 15;
                    break;
                case 'C':
                    value += 15;
                    break;
                case 'B':
                    value += 15;
                    break;
                case 'A':
                    value += 15;
                    break;
                case >= '0' and <= '9':
                    value += c - '0';
                    break;
                default:
                    throw new Exception($"Illegal Hex char '{c}'");
            }
        }
        return value;
    }
    public static Instruction ToPart2(this Instruction instr)
    {
        switch (char.ToUpper(instr.Color[5]))
        {
            case '0':
                instr.RelDir = RelDir.Right;
                break;
            case '1':
                instr.RelDir = RelDir.Down;
                break;
            case '2':
                instr.RelDir = RelDir.Left;
                break;
            case '3':
                instr.RelDir = RelDir.Up;
                break;
            default:
                throw new Exception($"Illegal color final char {instr.Color[5]}");
        }
        instr.Steps = instr.Color.Substring(0, 5).Hex2Dec();
        return instr;
    }
    public static Point Move(this Point p, RelDir relDir, int steps)
    {
        switch (relDir)
        {
            case RelDir.Up:
                return new Point(p.X, p.Y - steps);
            case RelDir.Down:
                return new Point(p.X, p.Y + steps);
            case RelDir.Left:
                return new Point(p.X - steps, p.Y);
            case RelDir.Right:
                return new Point(p.X + steps, p.Y);
            default:
                throw new Exception("Can't get here");
        }
    }
}