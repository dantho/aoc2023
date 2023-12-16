// https://adventofcode.com/2023/day/16
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Collections;
using System.Globalization;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day16.txt");

//// Example input
//lines = new string[]
//{
//    @".|...\....",
//    @"|.-.\.....",
//    @".....|-...",
//    @"........|.",
//    @"..........",
//    @".........\",
//    @"..../.\\..",
//    @".-.-/..|..",
//    @".|....-|.\",
//    @"..//.|....",
//};

foreach (var line in lines)
{
    Console.WriteLine(line);
}

var map = new Map(lines);
map.Illuminate((0, 0), Dir.East);
int ansPart1 = map.EnergizedCount();

int ansPart2 = 0;
for (int y = 0; y <= map.MaxY; y += map.MaxY)
{
    Dir dir = y == 0 ? Dir.South : Dir.North;
    for (int x = 0; x <= map.MaxX; x++)
    {
        map.Clear();
        map.Illuminate((x, y), dir);
        ansPart2 = Math.Max(ansPart2, map.EnergizedCount());
    }
}
for (int x = 0; x <= map.MaxX; x += map.MaxX)
{
    Dir dir = x == 0 ? Dir.East : Dir.West;
    for (int y = 0; y <= map.MaxY; y++)
    {
        map.Clear();
        map.Illuminate((x, y), dir);
        ansPart2 = Math.Max(ansPart2, map.EnergizedCount());
    }
}
Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End


class Map
{
    public int MaxX { get; init; }
    public int MaxY { get; init; }
    readonly char[,] grid;
    Dictionary<(int,int), List<Dir>> energized;
    public Map(string[] lines)
    {
        MaxY = lines.Length - 1;
        MaxX = lines[0].Length - 1;
        grid = new char[MaxX + 1, MaxY + 1];
        for (int y = 0; y <= MaxY; y++)
        {
            string line = lines[y];
            for (int x = 0; x <= MaxX; x++)
                grid[x,y] = line[x];
        }
        energized = new();
    }
    public void Clear()
    {
        energized = new();
    }
    public int EnergizedCount()
    {
        return energized.Count;
    }
    public void Illuminate((int,int) xy, Dir dir)
    {
        do
        {
            var (x, y) = xy;
            // Light might escape...
            if (x < 0 || y < 0 || x > MaxX || y > MaxY) return;

            bool success = Energize(xy, dir);
            if (!success) return;
            char terrain = grid[x, y];
            var (nextDir, extraDir) = dir.Next(terrain);
            if (extraDir is not null)
            {
                var xy2 = xy.Move((Dir)extraDir);
                // Recurse for extra
                Illuminate(xy2, (Dir)extraDir);
            }
            dir = nextDir;
            xy = xy.Move(dir);
        } while (true);
    }
    bool Energize((int,int) xy, Dir dir)
    {
        // Memorize your route in all directions through every node
        // Quit if you ever find yourself on the same path (same xy and direction)
        if (energized.ContainsKey(xy))
        {
            // Been here, done that?  If so, no sense continuing.
            if (energized[xy].Contains(dir))
                return false;
            else
                energized[xy].Add(dir);
        }
        else
        {
            energized.Add(xy, new() { dir });
        }
        return true;
    }
}

public enum Dir { North = 1, East = 2, South = 4, West = 8 };


public static class MyExtensions
{
    public static (int, int) Move(this (int, int) xy, Dir dir)
    {
        var (x, y) = xy;
        switch (dir)
        {
            case Dir.North:
                return (x, y - 1);
            case Dir.East:
                return (x + 1, y);
            case Dir.South:
                return (x, y + 1);
            case Dir.West:
                return (x - 1, y);
            default:
                throw new Exception("Illegal Dir!");
        }
    }
    public static (Dir, Dir?) Next(this Dir dir, char terrain)
    {
        switch (terrain)
        {
            case '.':
                return (dir, null);
            case '/':
                switch (dir)
                {
                    case Dir.North:
                        return (Dir.East, null);
                    case Dir.East:
                        return (Dir.North, null);
                    case Dir.South:
                        return (Dir.West, null);
                    case Dir.West:
                        return (Dir.South, null);
                    default:
                        throw new Exception("Illegal Dir!");
                }
            case '\\':
                switch (dir)
                {
                    case Dir.North:
                        return (Dir.West, null);
                    case Dir.East:
                        return (Dir.South, null);
                    case Dir.South:
                        return (Dir.East, null);
                    case Dir.West:
                        return (Dir.North, null);
                    default:
                        throw new Exception("Illegal Dir!");
                }
            case '|':
                switch (dir)
                {
                    case Dir.North:
                    case Dir.South:
                        return (dir, null);
                    case Dir.East:
                    case Dir.West:
                        return (Dir.North, Dir.South);
                    default:
                        throw new Exception("Illegal Dir!");
                }
            case '-':
                switch (dir)
                {
                    case Dir.East:
                    case Dir.West:
                        return (dir, null);
                    case Dir.North:
                    case Dir.South:
                        return (Dir.East, Dir.West);
                    default:
                        throw new Exception("Illegal Dir!");
                }
            default:
                throw new Exception($"Unknown terrain '{terrain}'");
        }
    }
}