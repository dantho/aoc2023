// https://adventofcode.com/2023/day/18
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
using System.Text;

int aocPart = 1;
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


// // Example 2 input
// lines = new string[] {
//     "bla bla ...",
//     };
List<Instruction> instructions = new();
foreach (var line in lines)
{
    string[] split = line.Split(' ');
    char relChar = split[0][0];
    int steps = int.Parse(split[1]);
    string color = split[2][2..^1];
    instructions.Add(new(relChar,steps,color));
}

// Convert relative to absolute, starting at (0,0) absolute
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
// Process the vertices to be non-overlapping before we do the rectangles formation.
// Overlapping horizontal edges are "retained" by the rectangle with the longer edge.
// The shorter-edged rectangle becomes vertically shorter by 1.  (Or two.)

// Create blank map of sufficient size
char[,] map = new char[maxX + 1, maxY + 1];
for (int x = 0; x <= maxX; x++)
    for (int y = 0; y <= maxY; y++)
        map[x, y] = '.';

// Now "Draw" outline on map
// One segment at a time
List<Point> nodes2 = nodes.Skip(1).ToList();
foreach ((Point, Point) segmentEndpoints in nodes.Zip(nodes2))
{
    var (p1, p2) = segmentEndpoints;
    bool isPosX = p2.X >= p1.X;
    bool isPosY = p2.Y >= p1.Y;
    for (int x = p1.X; isPosX ? x <= p2.X : x >= p2.X; x += isPosX ? 1 : -1)
        for (int y = p1.Y; isPosY ? y <= p2.Y : y >= p2.Y; y += isPosY ? 1 : -1)
            map[x, y] = '#';
}

// Create printable map
StringBuilder sb = new();
List<string> mapStrings = new();
for (int y = minY; y <= maxY; y++)
{
    for (int x = 0; x <= maxX; x++)
        sb.Append(map[x, y]);
    mapStrings.Add(sb.ToString());
    sb.Clear();
}

// Fill inside (by guessing that center pos is inside)
Queue<Point> spots2search = new();
spots2search.Enqueue(new Point(maxX / 2, maxY / 2));
Debug.Assert(map[spots2search.Peek().X, spots2search.Peek().Y] != '#');

while (spots2search.Count > 0)
{
    Point s = spots2search.Dequeue();
    if (map[s.X, s.Y] == '#') continue;
    map[s.X, s.Y] = '#';
    foreach (var delta in new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
    {
        int x = s.X + delta.Item1;
        int y = s.Y + delta.Item2;
        if (x < 0 || x > maxX || y < 0 || y > maxY) continue;
        spots2search.Enqueue(new Point(x, y));
    }

}

// Create printable map
sb.Clear();
mapStrings = new();
for (int y = minY; y <= maxY; y++)
{
    for (int x = 0; x <= maxX; x++)
        sb.Append(map[x, y]);
    mapStrings.Add(sb.ToString());
    sb.Clear();
}

foreach (var row in mapStrings)
    Console.WriteLine(row);


int ansPart1 = mapStrings.Select(s => s.Where(c => c == '#').Count()).Sum();
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

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