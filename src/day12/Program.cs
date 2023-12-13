// https://adventofcode.com/2023/day/12
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Runtime.CompilerServices;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day12.txt");

// // Example 1 input
// lines = new string[]
// {
//    "???.### 1,1,3",
//    ".??..??...?##. 1,1,3",
//    "?#?#?#?#?#?#?#? 1,3,1,6",
//    "????.#...#... 4,1,1",
//    "????.######..#####. 1,6,5",
//    "?###???????? 3,2,1"
//};

// // Example 2 input
// lines = new string[] {
//     "bla bla ...",
//     };

// Extract into 2 strings
string[][] rawStatus = lines.Select(line =>
    line.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToArray();
// Then a string and an array of ints
(string, int[])[] wildStatus = rawStatus.Select(half =>
    (half[0], half[1].Split(',').Select(int.Parse).Reverse().ToArray())).ToArray();
// Then enumerate the strings with unknowns (wildcards)
// into bitfields representing both possibilities for every unknown
// 8 unknowns -> 256 bitfields
(int[], int[])[] enumeratedStatus = wildStatus.Select(half =>
    (EnumerateUnknowns(half.Item1), half.Item2)).ToArray();

int ansPart1 = enumeratedStatus.Select(data =>
    data.Item1.Where(maybe => IsValid(maybe, data.Item2)).Count()).Sum();

foreach (var line in lines)
{
    Console.WriteLine(line);
}

int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

int[] EnumerateUnknowns(string inputWithUknowns)
{
    // Convert string into known and unknown bits into separate words
    int knownBitsOnly = 0;
    int unknownMask = 0;
    foreach (char c in inputWithUknowns)
    {
        knownBitsOnly <<= 1;
        unknownMask <<= 1;
        if (c == '#')
            knownBitsOnly += 1;
        if (c == '?')
            unknownMask += 1;
    }
    List<int> weightedUnknowns = new();
    weightedUnknowns.Add(0); // Initialize list with 1 item, no bits set
    int bitweight = 1;
    while (unknownMask > 0)
    {
        if ((unknownMask & 1) == 1)
        {
            weightedUnknowns = weightedUnknowns.Concat(weightedUnknowns.Select(w => w + bitweight)).ToList();
        }
        unknownMask >>= 1;
        bitweight *= 2;
    }
    return weightedUnknowns.Select(w => w + knownBitsOnly).ToArray();
}

bool IsValid(int springs, int[] validationData)
{
    List<int> groups = new();
    int contiguous = 0;
    bool done = false;
    while (springs > 0 || !done)
    {
        done = springs == 0; // forces one extra loop through to process contiguous bits at the end
        if ((springs & 1) == 1)
            contiguous += 1;
        else if (contiguous > 0)
        {
            groups.Add(contiguous);
            if (validationData.Length < groups.Count
             || contiguous != validationData[groups.Count - 1])
                return false; // early termination to save compute
            contiguous = 0;
        }
        springs >>= 1;
    }

    if (groups.Count != validationData.Length)
        return false;
    return true;
}

