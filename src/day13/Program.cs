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

//// Example 1 input
//lines = new string[]
//{
//    "#.##..##.",
//    "..#.##.#.",
//    "##......#",
//    "##......#",
//    "..#.##.#.",
//    "..##..##.",
//    "#.#.##.#.",
//    "",
//    "#...##..#",
//    "#....#..#",
//    "..##..###",
//    "#####.##.",
//    "#####.##.",
//    "..##..###",
//    "#....#..#",
//    "",
//};

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
                // Ndx starts at zero on BOTH sides of this !calculation!  Thus, the -2.
                foldRow = patternAsArray.Length - rowInvertedFoldRow - 2;
        }
        if (foldRow is not null)
        {
            int rowBonus = i == 0 ? 100 : 1;
            foldscore.Add(rowBonus * ((int)foldRow + 1));
            break;
        }
        else if (!rowSearch)
        {
            throw new Exception("Didn't find a fold!");
        }
        // Else search for column...
        rowSearch = false;
    }
}
Debug.Assert(foldscore.Count == patterns.Count);

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
    int lastOddNdx = pattern.Length - 1 - (pattern.Length % 2);
    for (int row = lastOddNdx; row > 0 ; row -= 2)
    {
        bool foundFold = true; // optimistic default
        // Test for reflective match to from 0 to row
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