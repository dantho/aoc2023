// https://adventofcode.com/2023/day/12
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day12.txt");

 // Example 1 input
 lines = new string[]
 {
    "???.### 1,1,3",
    ".??..??...?##. 1,1,3",
    "?#?#?#?#?#?#?#? 1,3,1,6",
    "????.#...#... 4,1,1",
    "????.######..#####. 1,6,5",
    "?###???????? 3,2,1"
};

// // Example 2 input
// lines = new string[] {
//     "bla bla ...",
//     };

string[][] rawStatus = lines.Select(line =>
    line.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToArray();
(string, int[])[] status = rawStatus.Select(halves =>
    (halves[0], halves[1].Split(',').Select(int.Parse).ToArray())).ToArray();
Console.WriteLine(status[0].Item1);
Console.WriteLine(status[0].Item2[2]);

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
