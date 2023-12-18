// https://adventofcode.com/2023/day/17
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.IO;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day17.txt");

//// Example 1 input
//lines = new string[]
//{
//    "2413432311323",
//    "3215453535623",
//    "3255245654254",
//    "3446585845452",
//    "4546657867536",
//    "1438598798454",
//    "4457876987766",
//    "3637877979653",
//    "4654967986887",
//    "4564679986453",
//    "1224686865563",
//    "2546548887735",
//    "4322674655533",
//};

Map map = new Map(lines);
int maxX = map.MaxX;
int maxY = map.MaxY;
PathExplorer.Init(maxX, maxY);
Map[,] minHeatloss = new Map[3,4];
for (int i = 0; i < 3; i++)
    for (int j = 0; j < 4; j++)
        minHeatloss[i,j] = new Map(int.MaxValue, maxX, maxY);
// Starting in upper left (0,0), try walking East or South
// On every step, walk in every possible direction that results in lower heatloss
Stack<PathExplorer> pathsToExplore = new();
pathsToExplore.Push(new(new(1, 0), Dir.E, 0, 1));
pathsToExplore.Push(new(new(0, 1), Dir.S, 0, 1));

// Algo: Choose the first available of paths to explore
//       (now, or previously?) Eliminate any of the options that are:
//          out-of-bounds or
//          would be 4th in a row in the same direction
//       Add position's heat loss to cumulative heat-loss for the path
//          if new heatloss > minHeatLoss (so far), eliminate option
//          else modify minHeatLoss at the new position and keep going another step down this path
//       If new position is end target, eliminate path (continue to next option) since we're done with that route
//       Find (validated) new positions to step to -- add them to the list of paths to explore
//       Loop until all paths terminate. (By reaching the end or, more frequently,
//          by crossing lower heatloss paths.
// 
// Final minHeatLoss for bottom right position is absolute min of all paths

while (pathsToExplore.Count > 0)
{
    PathExplorer path = pathsToExplore.Pop();
    path.CummulativeHeatLoss += map.Grid[path.Pos.X, path.Pos.Y];
    int prelimMinHeatLoss = Math.Min(minHeatloss[2,(int)Dir.S].Grid[maxX, maxY],minHeatloss[2,(int)Dir.E].Grid[maxX, maxY]);
    if (path.CummulativeHeatLoss >= prelimMinHeatLoss)
        // Don't bother, dude.
        continue;
    if (path.CummulativeHeatLoss < minHeatloss[path.FacingStepsInaRow - 1, (int)path.Facing].Grid[path.Pos.X, path.Pos.Y])
    {
        // We're the best!  (...so far.)
        for (int j = path.FacingStepsInaRow - 1; j < 3; j++)
            if (path.CummulativeHeatLoss < minHeatloss[j, (int)path.Facing].Grid[path.Pos.X, path.Pos.Y])
                minHeatloss[j, (int)path.Facing].Grid[path.Pos.X, path.Pos.Y] = path.CummulativeHeatLoss;
    }
    else
        // Someone faster/better has been here (...it might have been us!)
        continue;
    if (path.Pos.X == maxX && path.Pos.Y == maxY)
    {
        Console.WriteLine($"Reached end with {path.CummulativeHeatLoss} Heat lost. {pathsToExplore.Count} more paths to explore.");
        // We're done! (...with this path)
        continue;
    }
    List<PathExplorer> maybePaths = path.NextSteps();
    foreach (PathExplorer p in maybePaths)
        pathsToExplore.Push(p);
}

foreach (var line in lines)
{
    Console.WriteLine(line);
}
int ansPart1 = Math.Min(minHeatloss[2, (int)Dir.S].Grid[map.MaxX, map.MaxY], minHeatloss[2, (int)Dir.E].Grid[map.MaxX, map.MaxY]);
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

class PathExplorer
{
    public static int MaxX { get; set; }
    public static int MaxY { get; set; }

    const int MaxStepsInTheSameDir = 3;
    readonly Dir[] NESW = { Dir.N, Dir.E, Dir.S, Dir.W };

    public Position Pos;
    public Dir Facing;
    public int FacingStepsInaRow;
    public int CummulativeHeatLoss;

    public static void Init(int maxX, int maxY)
    {
        MaxX = maxX;
        MaxY = maxY;
    }
    public PathExplorer(Position pos, Dir facing, int cummulativeHeatLossSoFar) :
        this(pos, facing, cummulativeHeatLossSoFar, 1) { }
    public PathExplorer(Position pos, Dir facing, int cummulativeHeatLossSoFar, int facingStepInARowToHere)
    {
        Pos = pos;
        if (MaxX == 0 && MaxY == 0)
            throw new Exception("Must call PathExplorer.Init() before creating first path");
        if (!IsValid(pos))
            throw new Exception("Invalid position initialization");
        Facing = facing;
        FacingStepsInaRow = facingStepInARowToHere;
        CummulativeHeatLoss = cummulativeHeatLossSoFar;
    }
    public List<PathExplorer> NextSteps()
    {
        List<PathExplorer> validPaths = new();
        foreach (Dir dir in NESW.Where(d => d != Facing.Opposite()))
        {
            Position newPos = Pos.Next(dir);
            if (IsValid(newPos))
                if (dir == Facing)
                {
                    if (FacingStepsInaRow + 1 <= MaxStepsInTheSameDir)
                        validPaths.Add(new PathExplorer(newPos, Facing, CummulativeHeatLoss, FacingStepsInaRow + 1));
                }
                else
                {
                    validPaths.Add(new PathExplorer(newPos, dir, CummulativeHeatLoss, 1));
                }
        }
        return validPaths;
    }
    public bool IsValid(Position pos)
    {
        bool inBounds =
            pos.X >= 0 &&
            pos.X <= MaxX &&
            pos.Y >= 0 &&
            pos.Y <= MaxY;
        return inBounds;
    }
}
class Position
{
    public int X;
    public int Y;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Position Next(Dir dir)
    {
        switch (dir)
        {
            case Dir.N:
                return new Position(X, Y - 1);
            case Dir.E:
                return new Position(X + 1, Y);
            case Dir.S:
                return new Position(X, Y + 1);
            case Dir.W:
                return new Position(X - 1, Y);
            default:
                throw new Exception("Can't logically get this error.");
        }
    }
    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

class Map
{
    public int MaxX { get; init; }
    public int MaxY { get; init; }
    public readonly int[,] Grid;
    public Map(string[] lines)
    {
        MaxY = lines.Length - 1;
        MaxX = lines[0].Length - 1;
        Grid = new int[MaxX + 1, MaxY + 1];
        for (int y = 0; y <= MaxY; y++)
        {
            string line = lines[y];
            for (int x = 0; x <= MaxX; x++)
                Grid[x, y] = int.Parse(line[x].ToString());
        }
    }
    public Map(int initialize, int maxX, int maxY)
    {
        MaxY = maxX;
        MaxX = maxY;
        Grid = new int[MaxX + 1, MaxY + 1];
        for (int y = 0; y <= MaxY; y++)
        {
            for (int x = 0; x <= MaxX; x++)
                Grid[x, y] = initialize;
        }
    }
}

public enum Dir { N, E, S, W }

public static class MyExtensions
{
    public static IEnumerable<Dir> Next(this Dir dir)
    {
        Dir[] all = { Dir.N, Dir.E, Dir.S, Dir.W };
        return all.Where(d => d != dir);
    }
    public static Dir Opposite(this Dir dir)
    {
        switch (dir)
        {
            case Dir.N: return Dir.S;
            case Dir.E: return Dir.W;
            case Dir.S: return Dir.N;
            case Dir.W: return Dir.E;
            default: throw new Exception("This can't happen");
        }
    }
}