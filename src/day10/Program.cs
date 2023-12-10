// https://adventofcode.com/2023/day/10
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string rawInput = System.IO.File.ReadAllText(@"C:\Users\DanTh\github\aoc2023\inputs\day10.txt");

// Example 1 input
rawInput = @"..F7.
.FJ|.
SJ.L7
|F--J
LJ...";

//https://en.wikipedia.org/wiki/Box-drawing_character
string boxChars = "┓┏┛┗┃━";
string boxInput = string.Concat(rawInput.Select(c =>
{
    switch (c)
    {
        case 'F': return '┏';
        case 'L': return '┗';
        case '7': return '┓';
        case 'J': return '┛';
        case '|': return '┃';
        case '-': return '━';
        default: return c;
    }
}));

System.IO.File.WriteAllText(@"C:\Users\DanTh\github\aoc2023\out\day10_out.txt", boxInput);

string[] lines = boxInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

char[][] grid = lines.Select(s => s.ToArray()).ToArray();

int ansPart1 = 0;
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End
