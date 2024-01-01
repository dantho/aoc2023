// https://adventofcode.com/2023/day/23
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day23.txt");

// Example 1 input
lines = new string[]
{
    "#.#####################",
    "#.......#########...###",
    "#######.#########.#.###",
    "###.....#.>.>.###.#.###",
    "###v#####.#v#.###.#.###",
    "###.>...#.#.#.....#...#",
    "###v###.#.#.#########.#",
    "###...#.#.#.......#...#",
    "#####.#.#.#######.#.###",
    "#.....#.#.#.......#...#",
    "#.#####.#.#.#########v#",
    "#.#...#...#...###...>.#",
    "#.#.#v#######v###.###v#",
    "#...#.>.#...>.>.#.###.#",
    "#####v#.#.###v#.#.###.#",
    "#.....#...#...#.#.#...#",
    "#.#########.###.#.#.###",
    "#...###...#...#...#.###",
    "###.###.#.###v#####v###",
    "#...#...#.#.>.>.#.>.###",
    "#.###.###.#.###.#.#v###",
    "#.....###...###...#...#",
    "#####################.#",
};

char[,] map = new char[lines[0].Length, lines.Length];
for (int y = 0; y < lines.Length; y++)
    for (int x = 0; x < lines[0].Length; x++)
        map[x, y] = lines[y][x];

foreach (var line in lines)
{
    Console.WriteLine(line);
}

int startingX = lines[0].IndexOf('.');
int endingX = lines[lines.Length - 1].IndexOf('.');

int ansPart1 = map.Dykstra(startingX,endingX).Max();
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

public static class MyExtensions
{
    public static List<Point> Cardinal(this char[,] map, Point p)
    {
        List<Point> points = new();
        if (p.X > 0) points.Add(new Point(p.X - 1, p.Y));
        if (p.X < map.GetLength(0) - 1) points.Add(new Point(p.X + 1, p.Y));
        if (p.Y > 0) points.Add(new Point(p.X, p.Y - 1));
        if (p.Y < map.GetLength(1) - 1) points.Add(new Point(p.X, p.Y + 1));
        return points;
    }
    public static List<int> Dykstra(this char[,] map, int startingX, int endingX)
    {
        int[,] distance = new int[map.GetLength(0), map.GetLength(1)];

        var searchResults = DystraSearch(distance, map, new Point(startingX,0));
        if (searchResults.Count == 0) searchResults.Add(-1);
        return searchResults;
    }
    static List<int> DystraSearch(int[,] distance, char[,] map, Point xy)
    {
        List<int> uniquePathStepCounts = new();
        int stepCount = distance[xy.X, xy.Y];
        do
        {
            List<Point> nextSteps = new();
            char terrain = map[xy.X, xy.Y];
            switch (terrain)
            {
                case '.':
                    nextSteps = map.Cardinal(xy).Where(p =>
                        map[p.X, p.Y] == '.' ||
                        (p.X > xy.X && map[p.X, p.Y] == '>') ||
                        (p.X < xy.X && map[p.X, p.Y] == '<') ||
                        (p.Y > xy.Y && map[p.X, p.Y] == 'v') ||
                        (p.Y < xy.Y && map[p.X, p.Y] == '^')
                        ).ToList();
                    break;
                case '>':
                    nextSteps = map.Cardinal(xy).Where(p =>
                        (p.X > xy.X &&
                        (map[p.X, p.Y] == '.' || map[p.X, p.Y] == '>'))
                        ).ToList();
                    break;
                case '<':
                    nextSteps = map.Cardinal(xy).Where(p =>
                        (p.X < xy.X &&
                        (map[p.X, p.Y] == '.' || map[p.X, p.Y] == '<'))
                        ).ToList();
                    break;
                case 'v':
                    nextSteps = map.Cardinal(xy).Where(p =>
                        (p.Y > xy.Y &&
                        (map[p.X, p.Y] == '.' || map[p.X, p.Y] == 'v'))
                        ).ToList();
                    break;
                case '^':
                    nextSteps = map.Cardinal(xy).Where(p =>
                        (p.Y < xy.Y &&
                        (map[p.X, p.Y] == '.' || map[p.X, p.Y] == '^'))
                        ).ToList();
                    break;
                default:
                    throw new Exception($"Shouldn't find char '{terrain}'");
            }
            nextSteps = nextSteps.Where(p => distance[p.X, p.Y] == 0).ToList();
            if (nextSteps.Count() == 0) return uniquePathStepCounts;
            if (nextSteps.Count() > 1)
            {
                foreach (Point next in nextSteps.Skip(1))
                {
                    int[,] distanceCopy = (int[,])distance.Clone();
                    distanceCopy[next.X, next.Y] = stepCount + 1;
                    uniquePathStepCounts.Concat(DystraSearch(distanceCopy, map, next));
                }
            }
            xy = nextSteps.First();
            stepCount++;
            distance[xy.X, xy.Y] = stepCount;
        } while (xy.Y < map.Length - 1);
        uniquePathStepCounts.Add(stepCount);
        return uniquePathStepCounts;
    }
}
// End
// End
// End
