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

int aocPart = 2;
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

        int? foldRow = aocPart == 1 ?
             findFold(patternAsArray) :
             findFold2(patternAsArray);
        if (foldRow is null)
        {
            patternAsArray = patternAsArray.Reverse().ToArray();
            int? rowInvertedFoldRow = aocPart == 1 ?
                 findFold(patternAsArray) :
                 findFold2(patternAsArray);
            if (rowInvertedFoldRow is not null)
                // Ndx starts at zero on BOTH sides of this calculation.  Thus, the -2.
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

int ans = foldscore.Sum();

Console.WriteLine($"The answer for Part {aocPart} is {ans}");

Console.ReadKey();

// End
// End
// End

bool singleCharDiff(string s1, string s2)
{
    Debug.Assert(s1.Length == s2.Length);
    bool diffFound = false;
    for (int ndx = 0; ndx < s1.Length; ndx++)
    {
        if (s1[ndx] != s2[ndx])
        {
            if (diffFound) return false; // 2nd diff found!
            diffFound = true;
        }
    }
    return diffFound;
}

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
    for (int row = lastOddNdx; row > 0; row -= 2)
    {
        bool foldFound = true; // optimistic default
        // Test for reflective match from 0 to row
        for (int bottom = 0; bottom <= row / 2; bottom++)
        {
            if (pattern[bottom] != pattern[row - bottom])
            {
                foldFound = false;
                break;
            }
        }
        // Fold found?
        if (foldFound) return row / 2;
    }
    // No fold found
    return null;
}
int? findFold2(string[] pattern)
{
    int lastOddNdx = pattern.Length - 1 - (pattern.Length % 2);
    for (int row = lastOddNdx; row > 0; row -= 2)
    {
        bool foldFound = true; // optimistic default
        bool singleCharDiffFound = false;
        // Test for reflective match from 0 to row
        // With a SINGLE char difference (aka "smudge")
        // in a single pair
        for (int bottom = 0; bottom <= row / 2; bottom++)
        {
            if (pattern[bottom] != pattern[row - bottom])
            {
                if (singleCharDiff(pattern[bottom], pattern[row-bottom])
                && !singleCharDiffFound)
                    singleCharDiffFound = true;
                else
                {
                    foldFound = false;
                    break;
                }
            }
        }
        // Fold found?
        if (foldFound & singleCharDiffFound) return row / 2;
    }
    // No fold found
    return null;
}