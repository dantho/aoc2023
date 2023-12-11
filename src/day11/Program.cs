// https://adventofcode.com/2023/day/11
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Text;
using System.Diagnostics.CodeAnalysis;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day11.txt");

// Example 1 input
//lines = @"
//...#......
//.......#..
//#.........
//..........
//......#...
//.#........
//.........#
//..........
//.......#..
//#...#....."
//.Split(Environment.NewLine);
//lines = lines.Skip(1).ToArray(); // Remove blank line added for formatting at top

foreach (var line in lines)
{
    Console.WriteLine(line);
}
List<string> map = lines.ToList();

// Find blank rows -- they are points of expansion
List<int> blankRows = new();
for (int y = 0; y < map.Count; y++)
{
    if (map[y].IndexOf('#') < 0)
    {
        Debug.Assert(y != 0);
        blankRows.Add(y);
    }
}

// Remove blank rows (in reverse) to remap galaxies
blankRows.Reverse();
foreach (int y in blankRows) map.RemoveAt(y);

// Refactor blank row indices to match above remap
// And to now point to the preceeding non-blank row
blankRows.Reverse();
int blankCount = 0;
blankRows = blankRows.Select(b =>
{
    blankCount++;
    return b - blankCount;
}).ToList();

// Repeat above sequence for columns
List<int> blankCols = new();
for (int x = 0; x < map[0].Length; x++)
{
    bool isBlank = true;
    for (int y = 0; y < map.Count; y++)
    {
        if (map[y][x] == '#')
        {
            isBlank = false;
        }
    }
    if (isBlank)
    {
        Debug.Assert(x != 0);
        blankCols.Add(x);
    }
}

// Remove blank Cols to remap galaxies
// This method extracts sections & rebuilds each line
// (reverse order not required)
int startOfNonBlank = 0;
List<(int, int)> subStrings = new();
foreach (int x in blankCols)
{
    Debug.Assert(x != startOfNonBlank-1); // No double blanks to start!
    subStrings.Add((startOfNonBlank, x - startOfNonBlank));
    startOfNonBlank = x + 1;
}
// Add end-of-line
subStrings.Add((startOfNonBlank, map[0].Length - startOfNonBlank));
for (int y = 0; y < map.Count; y++)
{
    StringBuilder newRow = new();
    foreach ((int x, int len) in subStrings)
    {
        newRow.Append(map[y].Substring(x, len));
    }
    map[y] = newRow.ToString();
}

// Refactor blank Col indices to match above remap
// And to now point to the Col ndx AFTER WHICH the blank would be
blankCount = 0;
blankCols = blankCols.Select(b => 
{
    blankCount++;
    return b - blankCount;
}).ToList();

// Find galaxies -- on our map SANS blank rows and columns
List<(int, int)> galaxies = new();
for (int y = 0; y < map.Count; y++)
{
    int x = map[y].IndexOf('#');
    while (x >= 0)
    {
        galaxies.Add((x, y));
        x = map[y].IndexOf('#', x + 1);
    }
}

// Dump new map
foreach (string row in map) Console.WriteLine(row);

// Enumerate all pairs. Count == (n-1) + (n-2) + ... + 1
List<((int, int), (int, int))> galaxy_pairs = new();
for (int ndx1 = 0; ndx1 < galaxies.Count; ndx1++)
{
    (int, int) g1 = galaxies[ndx1];
    for (int ndx2 = ndx1 + 1; ndx2 < galaxies.Count; ndx2++)
    {
        galaxy_pairs.Add((galaxies[ndx1], galaxies[ndx2]));
    }
}

// Calculate manhattan distances between galaxies with a twist
// Compensate for any missing rows/colums between galaxies
// And include universe expansion coefficient

int universeExpansionCoefficient = 2;
long ansPart1 = galaxy_pairs.Select(gPair =>
    calcManhattanSpecial(gPair, blankRows, blankCols, universeExpansionCoefficient)).Sum();
universeExpansionCoefficient = 1000000;
long ansPart2 = galaxy_pairs.Select(gPair =>
    calcManhattanSpecial(gPair, blankRows, blankCols, universeExpansionCoefficient)).Sum();

long calcManhattanSpecial(((int, int), (int, int)) galaxyPair, List<int> blankRows, List<int> blankCols, int uec)
{
    var ((gx1, gy1), (gx2, gy2)) = galaxyPair;
    int manhattan = Math.Abs(gx1 - gx2) + Math.Abs(gy1 - gy2);
    long spaceExpansion = blankRows.Where(b =>
        b >= (gy1 < gy2 ? gy1 : gy2) &&
        b < (gy1 < gy2 ? gy2 : gy1)
        ).Count();
    spaceExpansion += blankCols.Where(b =>
        b >= (gx1 < gx2 ? gx1 : gx2) &&
        b < (gx1 < gx2 ? gx2 : gx1)
        ).Count();
    return manhattan + spaceExpansion * uec;
}

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End
