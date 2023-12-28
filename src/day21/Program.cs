// https://adventofcode.com/2023/day/21
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day21.txt");
int stepMax = 65 + 131*3;
// if stepMax == 26501365, which is 202300 * 131 + 65
// 65 + 131 * 0 => 3791
// 65 + 131 * 1 => 33646 (delta: 29,855)
// 65 + 131 * 2 => 93223 (delta: 59,577)
// 65 + 131 * 3 => 182522 (delta: 89,299)
// Example 1 input
//stepMax = 6;
//lines = new string[] {
//    "...........",
//    ".....###.#.",
//    ".###.##..#.",
//    "..#.#...#..",
//    "....#.#....",
//    ".##..S####.",
//    ".##..#...#.",
//    ".......##..",
//    ".##.#.####.",
//    ".##..##.##.",
//    "...........",
//};

// // Example 2 input
// lines = new string[] {
//     "bla bla ...",
//     };

Map map = new(lines);
Console.WriteLine(map);
int prior = 0;
for (int N = 10; N < 65+131*2+2; N++)
{
    Distances dist = new(map, N);
    int canReachInExactlyNSteps = dist.Reachable.Values.Cast<IEnumerable<int>>()
        .Where(ienum => ienum.Contains(N)).Count();
    Console.WriteLine($"{N} Steps {canReachInExactlyNSteps} possibles. Delta is {canReachInExactlyNSteps-prior}");
    prior = canReachInExactlyNSteps;
}

int ansPart1 = 0;
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

public class Distances
{
    public Point Start { get; init; }
    public int StepsMax { get; init; }
    public Hashtable Reachable;
    Map map;
    public int MaxX { get; init; }
    public int MaxY { get; init; }
    public Distances(Map map, int stepsMax)
    {
        this.map = map;
        Start = map.Start;
        MaxX = map.MaxX;
        MaxY = map.MaxY;
        StepsMax = stepsMax;
        Reachable = new Hashtable();
        RandomWalk();
    }
    void RandomWalk()
    {
        for (int step = 1; step <= StepsMax; step++)
        {
            List<Point> priorPositions = new();
            int priorStep = step - 1;
            if (priorStep == 0)
            {
                priorPositions.Add(Start);
            }
            else
            {
                foreach (System.Collections.DictionaryEntry kv in Reachable)
                    if (((HashSet<int>)kv.Value).Contains(priorStep))
                        priorPositions.Add((Point)kv.Key);
            }
            foreach (Point prior in priorPositions)
            {
                var newPoints = prior.Cardinal()
                    .Where(pp => map.Grid[pp.X.Modulo(MaxX + 1), pp.Y.Modulo(MaxY + 1)] == '.');
                foreach (Point p in newPoints)
                {
                    var set = (HashSet<int>)Reachable[p];
                    set ??= new HashSet<int>();
                    set.Add(step);
                    Reachable[p] = set;
                }
            }
        }
    }
}
public class Map
{
    public Point Start { get; init; }
    public char[,] Grid { get; set; }
    public int MaxX { get; init; }
    public int MaxY { get; init; }
    public Map(string[] lines)
    {
        MaxX = lines[0].Length - 1;
        MaxY = lines.Length - 1;
        Grid = new char[MaxX + 1, MaxY + 1];
        for (int y = 0; y <= MaxY; y++)
        {
            for (int x = 0; x <= MaxX; x++)
            {
                char c = lines[y][x];
                if (c == 'S')
                {
                    Start = new Point(x, y);
                    c = '.';
                }
                Grid[x, y] = c;
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int y = 0; y <= MaxY; y++)
        {
            if (y > 0) sb.Append('\n');
            for (int x = 0; x <= MaxX; x++)
            {
                sb.Append(Grid[x, y]);
            }

        }
        return sb.ToString();
    }
}

public static class MyExtensions
{
    public static int Modulo(this int input, int mod)
    {
        return (input % mod + mod) % mod;
    }
    public static List<Point> Cardinal(this Point here, int maxX, int maxY)
    {
        List<Point> cardinal = new();
        if (here.X > 0)
            cardinal.Add(new Point(here.X - 1, here.Y));
        if (here.Y > 0)
            cardinal.Add(new Point(here.X, here.Y - 1));
        if (here.X < maxX)
            cardinal.Add(new Point(here.X + 1, here.Y));
        if (here.Y < maxY)
            cardinal.Add(new Point(here.X, here.Y + 1));
        return cardinal;
    }
    public static List<Point> Cardinal(this Point here)
    {
        List<Point> cardinal = new();
        cardinal.Add(new Point(here.X - 1, here.Y));
        cardinal.Add(new Point(here.X, here.Y - 1));
        cardinal.Add(new Point(here.X + 1, here.Y));
        cardinal.Add(new Point(here.X, here.Y + 1));
        return cardinal;
    }
}