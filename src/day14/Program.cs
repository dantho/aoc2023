// https://adventofcode.com/2023/day/14
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day14.txt");

// Example 1 input
lines = new string[]
{
    "O....#....",
    "O.OO#....#",
    ".....##...",
    "OO.#O....O",
    ".O.....O#.",
    "O.#..O.#.#",
    "..O..#O..O",
    ".......O..",
    "#....###..",
    "#OO..#....",
};

List<char[]> mapInit = new();
foreach (var line in lines)
    mapInit.Add(line.ToArray());

char[][] map = mapInit.ToArray();

foreach (var row in map)
    Console.WriteLine(row);
Console.WriteLine();

// Move boulders "North" (to lower y)
RollBoulders(map, "North");

foreach (var row in map)
    Console.WriteLine(row);
Console.WriteLine();

int totalLoad = CalcLoad(map, "North");

int ansPart1 = totalLoad;
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

static void RollBoulders(char[][] map, string dir)
{
    dir = dir.ToLower();
    Debug.Assert(new string[] { "north", "east", "south", "west" }.Contains(dir));

    int maxY = map.Length - 1;
    int maxX = map[0].Length - 1;

    int yStart;
    int yStop;
    int xStart;
    int xStop;
    switch (dir)
    {
        case "north":
            yStart = 1;
            yStop = maxY;
            xStart = 0;
            xStop = maxX;
            break;
        case "south":
            yStart = maxY - 1;
            yStop = 0;
            xStart = 0;
            xStop = maxX;
            break;
        case "east":
            yStart = 0;
            yStop = maxY;
            xStart = maxX - 1;
            xStop = 0;
            break;
        case "west":
            yStart = 0;
            yStop = maxY;
            xStart = 1;
            xStop = maxX;
            break;
        default:
            throw new Exception($"Illegal direction '{dir}'");
    }

    bool rocksAreMoving = true;
    while (rocksAreMoving)
    {
        rocksAreMoving = false;
        for (int y = yStart; y <= yStop; y += yStop > yStart ? 1 : -1)
            for (int x = xStart; x <= xStop; x += xStop > xStart ? 1 : -1)
                switch (dir)
                {
                    case "north":
                        if (map[y][x] == 'O' && map[y - 1][x] == '.')
                        {
                            rocksAreMoving = true;
                            map[y - 1][x] = 'O';
                            map[y][x] = '.';
                        }
                        break;
                    case "south":
                        if (map[y][x] == 'O' && map[y + 1][x] == '.')
                        {
                            rocksAreMoving = true;
                            map[y + 1][x] = 'O';
                            map[y][x] = '.';
                        }
                        break;
                    case "west":
                        if (map[y][x] == 'O' && map[y][x - 1] == '.')
                        {
                            rocksAreMoving = true;
                            map[y][x - 1] = 'O';
                            map[y][x] = '.';
                        }
                        break;
                    case "east":
                        if (map[y][x] == 'O' && map[y][x + 1] == '.')
                        {
                            rocksAreMoving = true;
                            map[y][x + 1] = 'O';
                            map[y][x] = '.';
                        }
                        break;
                }
    }
}

static int CalcLoad(char[][] map, string dir)
{
    dir = dir.ToLower();

    int maxY = map.Length - 1;
    int maxX = map[0].Length - 1;
    int totalLoad = 0;
    for (int y = 0; y <= maxY; y++)
        for (int x = 0; x <= maxX; x++)
            if (map[y][x] == 'O')
            {
                switch (dir)
                {
                    case "north":
                        totalLoad += maxY - y + 1;
                        break;
                    case "south":
                        totalLoad += y + 1;
                        break;
                    case "east":
                        totalLoad += x + 1;
                        break;
                    case "west":
                        totalLoad += maxX - x + 1;
                        break;
                    default:
                        throw new Exception($"Illegal direction '{dir}'");
                }
            }

    return totalLoad;
}

// End
// End
// End
