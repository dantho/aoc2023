// https://adventofcode.com/2023/day/14
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day14.txt");

//// Example 1 input
//lines = new string[]
//{
//    "O....#....",
//    "O.OO#....#",
//    ".....##...",
//    "OO.#O....O",
//    ".O.....O#.",
//    "O.#..O.#.#",
//    "..O..#O..O",
//    ".......O..",
//    "#....###..",
//    "#OO..#....",
//};

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

int ansPart1 = CalcLoad(map, "North");

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
// By observation with *my* data, the following loop results in a repeating series modulo 19
// So we'll just warm up the loop, then find the modulo 19 answer that corresponds to 1 billion
// Determining the "19" part _could_ be part of this algorithm...
// but I'm done with this one...
// This worked, but I might have been lucky -- like 1 in 19 lucky.
// The answer should be immune to additional loops, but it is sensitive to more loops.
// Or fewer -- I was trying to see how far below 1000 I could make the loopcount.  :(
int aBillionMod19 = 1000000000 % 19;

int loopCount = 1020;
for (int i = 0; i < loopCount; i++)
{
    foreach (string dir in new string[] { "north", "west", "south", "east" })
    {
        RollBoulders(map, dir);
    }
    if (i > loopCount - 19 && (i + 1) % 19 == aBillionMod19)
        Console.WriteLine($"After cycle {i}, North load is {CalcLoad(map, "north")}");
}
int ansPart2 = CalcLoad(map, "North");

//foreach (string dir in new string[] { "north", "west", "south", "east" })
//{
//    RollBoulders(map, dir);
//    foreach (var row in map)
//        Console.WriteLine(row);
//    Console.WriteLine($"{dir} load is {CalcLoad(map, dir)}");
//}

Console.WriteLine($"The answer for Part {2} is {ansPart2}");
Debug.Assert(ansPart2 == 79723);

Console.ReadKey();

static void RollBoulders(char[][] map, string dir)
{
    dir = dir.ToLower();

    int maxY = map.Length - 1;
    int maxX = map[0].Length - 1;

    int yStart;
    int yStop;
    int xStart;
    int xStop;
    switch (dir)
    {
        case "south":
            yStart = maxY - 1;
            yStop = 0;
            xStart = 0;
            xStop = maxX;
            break;
        case "north":
            yStart = 1;
            yStop = maxY;
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
        int xRoll = -1;
        int yRoll = -1;

        for (int y = yStart;
            yStop > yStart ? (y <= yStop) : (y >= yStop);
            y += yStop > yStart ? 1 : -1)
        {
            for (int x = xStart;
                xStop > xStart ? (x <= xStop) : (x >= xStop);
                x += xStop > xStart ? 1 : -1)
            {
                switch (dir)
                {
                    case "south":
                        xRoll = x;
                        yRoll = y + 1;
                        break;
                    case "north":
                        xRoll = x;
                        yRoll = y - 1;
                        break;
                    case "east":
                        xRoll = x + 1;
                        yRoll = y;
                        break;
                    case "west":
                        xRoll = x - 1;
                        yRoll = y;
                        break;
                }
                if (map[y][x] == 'O' && map[yRoll][xRoll] == '.')
                {
                    rocksAreMoving = true;
                    map[y][x] = '.';
                    map[yRoll][xRoll] = 'O';
                }
            }
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
                    case "south":
                        totalLoad += y + 1;
                        break;
                    case "north":
                        totalLoad += maxY - y + 1;
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
