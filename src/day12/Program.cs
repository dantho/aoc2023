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

// Example input
lines = new string[]
{
    "???.### 1,1,3",
    ".??..??...?##. 1,1,3",
    "?#?#?#?#?#?#?#? 1,3,1,6",
    "????.#...#... 4,1,1",
    "????.######..#####. 1,6,5",
    "?###???????? 3,2,1"
};

// Extract into 2 strings
string[][] rawStatus = lines.Select(line =>
    line.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToArray();
// Then a string and an array of ints
(string, int[])[] wildStatus = rawStatus.Select(half =>
    (half[0], half[1].Split(',').Select(int.Parse).Reverse().ToArray())).ToArray();
// Then enumerate the strings with unknowns (wildcards)
// into bitfields representing both possibilities for every unknown

// *** Part 1 ***
// *** Part 1 ***
// *** Part 1 ***

// 8 unknowns -> 256 bitfields
// Concrete type would be (int[], int[])[]
var enumeratedStatus = wildStatus.Select(half =>
    (EnumerateUnknowns(half.Item1), half.Item2));

// Now count ONLY valid possibilities and sum the counts
int ansPart1 = enumeratedStatus.Select(data =>
    data.Item1.Where(maybe => IsValid(maybe, data.Item2)).Count()).Sum();

if (ansPart1 > 1000)
    Debug.Assert(ansPart1 == 8193);

Console.WriteLine($"The answer for Part {1} is {ansPart1}");

// *** Part 2 ***
// *** Part 2 ***
// *** Part 2 ***

// Because the scale is exponentially larger by 5x bits, the worst case
// is ~100bits of binary possibilities!  This is too big.
// So we need to validate AS we're enumerating.  So we only enumerate valid choices.
// This will keep the memory size and calculation size and variable size manageable.
// Kinda wish our solution didn't use "bits" now.  Combining strings is easy.
// bit lengths are arbitrary, too.  Another problem.  :(

//long ansPart2 = wildStatus.Select(half =>
//    EnumerateValidateCountUnknowns(half.Item1, half.Item2)).sum();

//Console.WriteLine($"The answer for Part {2} is {ansPart2}");

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

