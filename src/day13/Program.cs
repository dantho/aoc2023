// https://adventofcode.com/2023/day/13
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day13.txt");

// Example 1 input
lines = new string[]
{
    "#.##..##.",
    "..#.##.#.",
    "##......#",
    "##......#",
    "..#.##.#.",
    "..##..##.",
    "#.#.##.#.",
    "",
    "#...##..#",
    "#....#..#",
    "..##..###",
    "#####.##.",
    "#####.##.",
    "..##..###",
    "#....#..#",
    "",
};

List<List<string>> patterns = new();
{ 
    List<string> pattern = new();
    foreach (string line in lines)
    {
        if (line.Length == 0)
        {
            patterns.Add(pattern);
            pattern = new();
        }
        else
        {
            pattern.Add(line);
        }
    }
}

List<int> foldscore = new();
foreach (List<string> pattern in patterns)
{
    bool rowSearch = true;
    for (int i = 0; i < 2; i++)
    {
        string[] patternAsArray = pattern.ToArray();
        if (!rowSearch)
            patternAsArray = transpose(patternAsArray);

        int? foldRow = findFold(patternAsArray);
        if (foldRow is null)
        {
            patternAsArray = patternAsArray.Reverse().ToArray();
            int? rowInvertedFoldRow = findFold(patternAsArray);
            if (rowInvertedFoldRow is not null)
                foldRow = patternAsArray.Length - rowInvertedFoldRow - 2;
        }
        if (foldRow is not null)
        {
            foldscore.Add(rowSearch ? 100 : 1 * ((int)foldRow + 1));
            continue;
        }
        // Else search for column...
        rowSearch = false;
    }
}

foreach (var line in lines)
{
    Console.WriteLine(line);
}

int ansPart1 = foldscore.Sum();
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

string[] transpose(string[] input)
{
    StringBuilder[] trans = new StringBuilder[input[0].Length];
    for (int i = 0; i < trans.Length; i++)
        trans[i] = new StringBuilder();

    for (int i = 0; i < input.Length; i++)
    {
        for (int j = 0; j < input[0].Length; j++)
        {
            trans[j].Append(input[i][j]);
        }
    }
    return trans.Select(sb => sb.ToString()).ToArray();
}
int? findFold(string[] pattern)
{
    for (int row = 1; row < pattern.Length; row += 2)
    {
        bool foundFold = true; // optimistic default
        // Test for reflective match to row
        for (int bottom = 0; bottom <= row / 2; bottom++)
        {
            if (pattern[bottom] != pattern[row - bottom])
            {
                foundFold = false;
                break;
            }
        }
        // Fold found?
        if (foundFold) return row / 2;
    }
    // No fold found
    return null;
}