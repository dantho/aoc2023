// https://adventofcode.com/2023/day/18
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Runtime.CompilerServices;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day18.txt");

// Example 1 input
lines = new string[] {
    "R 6 (#70c710)",
    "D 5 (#0dc571)",
    "L 2 (#5713f0)",
    "D 2 (#d2c081)",
    "R 2 (#59c680)",
    "D 2 (#411b91)",
    "L 5 (#8ceee2)",
    "U 2 (#caa173)",
    "L 1 (#1b58a2)",
    "U 2 (#caa171)",
    "R 2 (#7807d2)",
    "U 3 (#a77fa3)",
    "L 2 (#015232)",
    "U 2 (#7a21e3)",
};


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
int minX = int.MaxValue;
int maxX = int.MinValue;
int minY = int.MaxValue;
int maxY = int.MinValue;
// (int, int) XYmax = 
foreach (Point p in nodes)
{
    minX = p.X < minX ? p.X : minX;
    minY = p.Y < minY ? p.Y : minY;
    maxX = p.X > maxX ? p.X : maxX;
    maxY = p.Y > maxY ? p.Y : maxY;
}

Console.WriteLine(
    $"Grid spans x from {minX} to {maxX}, y from {minY} to {maxY}. "
  + $"Size is {maxX - minX + 1} x {maxY - minY + 1}");

foreach (var line in lines)
{
    Console.WriteLine(line);
}

int ansPart1 = 0;
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

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