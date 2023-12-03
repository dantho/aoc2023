// https://adventofcode.com/2023/day/3
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day3.txt");

//if (aocPart == 1)
//    // Part 1 example input
//    lines = new string[] {
//        "467..114..",
//        "...*......",
//        "..35..633.",
//        "......#...",
//        "617*......",
//        ".....+.58.",
//        "..592.....",
//        "......755.",
//        "...$.*....",
//        ".664.598..",
//        };
//else if (aocPart == 2)
//    // Part 2 example input
//    lines = new string[] {
//        "two1nine",
//        "eightwothree",
//        "abcone2threexyz",
//        "xtwone3four",
//        "4nineeightseven2",
//        "zoneight234",
//        "7pqrstsixteen"
//        };
//else
//    Debug.Assert(false, "aocPart should be 1 or 2");


List<Point> symbols = new();
List<Tuple<Point, Point>> numsFound = new();
int maxX = lines[0].Length - 1;
int maxY = lines.Length - 1;
for (int y = 0; y <= maxY; y++)
{
    string line = lines[y];
    for (int x = 0; x <= maxX; x++)
    {
        if (line[x] == '.') ; // do nothing
        else if (line[x] >= '0' && line[x] <= '9')
        {
            // find end of this number
            int x0 = x;
            for (int xx = x+1; xx <= maxX && System.Char.IsDigit(line[xx]); xx++) 
                x++;
            numsFound.Add(new(new(x0,y), new(x,y)));
            // continue search _past_ this number
        }
        else
            // must be a symbol
            symbols.Add(new Point(x, y));
    }
}
foreach (var line in lines)
{
    Console.WriteLine(line);
}

uint ansPart1 = 0;
foreach (var num in numsFound)
{
    if (SymbolFoundInBox(CreateBoundingBox(num),symbols))
        ansPart1 += ParseNum(num, lines);
}

uint ansPart2 = 0;
foreach (Point symbolPos in symbols)
{
    char symbol = lines[symbolPos.Y][symbolPos.X];
    if (symbol == '*')
    {
        ansPart2 += CalcGearRatio(symbolPos, numsFound, lines);
    }
}

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

static bool SymbolFoundInBox(Tuple<Point,Point> box, List<Point> symbols)
{
    foreach (var pos in symbols)
        if (pos.X >= box.Item1.X && pos.X <= box.Item2.X &&
            pos.Y >= box.Item1.Y && pos.Y <= box.Item2.Y)
            return true;
    return false;
}

static Tuple<Point, Point> CreateBoundingBox(Tuple<Point, Point> digits)
{
    Debug.Assert(digits.Item1.Y == digits.Item2.Y);
    int y = digits.Item1.Y;
    Debug.Assert(digits.Item1.X <= digits.Item2.X);
    int x1 = digits.Item1.X;
    int x2 = digits.Item2.X;
    return new(new(x1 - 1, y - 1), new(x2 + 1, y + 1));
}

static uint ParseNum(Tuple<Point,Point> numberCoords, string[] grid)
{
    return uint.Parse(grid[numberCoords.Item1.Y].Substring(numberCoords.Item1.X, numberCoords.Item2.X - numberCoords.Item1.X + 1));
}

static uint CalcGearRatio(Point gearPos, List<Tuple<Point, Point>> numsFound, string[] grid)
{
    int countOfAdjacentNums = 0;
    uint gearRatio = 1;
    foreach (Tuple<Point,Point> num in numsFound)
    {
        List<Point> singleGear = new();
        singleGear.Add(gearPos);

        if (SymbolFoundInBox(CreateBoundingBox(num), singleGear))
        {
            countOfAdjacentNums++;
            gearRatio *= ParseNum(num, grid);
        }
    }
    if (countOfAdjacentNums < 2)
        return 0;
    else
        return gearRatio;
}

