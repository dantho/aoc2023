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

int maxY = map.Length-1;
int maxX = map[0].Length-1;

// Move boulders "North" (to lower y)
bool rocksAreMoving = true;
while (rocksAreMoving)
{
    rocksAreMoving = false;
    for (int y = 1; y <= maxY; y++)
        for (int x = 0; x <= maxX; x++)
            if (map[y][x] == 'O' && map[y - 1][x] == '.')
            {
                rocksAreMoving = true;
                map[y - 1][x] = 'O';
                map[y][x] = '.';
            }
}

foreach (var row in map)
    Console.WriteLine(row);
Console.WriteLine();

int totalLoad = 0;
for (int y = 0; y <= maxY; y++)
    for (int x = 0; x <= maxX; x++)
        if (map[y][x] == 'O')
        {
            totalLoad += maxY - y + 1; 
        }

int ansPart1 = totalLoad;
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End
