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
// Now consume the string one chunk at a time, based on the first integer, and recurse
// with remainder of string and remainder of ints

int ansPart1 = 0;
foreach ((string wildcardSprings, int[] validationRecords) in wildStatus)
{
    var validRecords = validationRecords.ToList();
    string wildcardSpringsReversed = new string(wildcardSprings.Reverse().ToArray());
    ansPart1 += PossibleValidArrangementCount(wildcardSpringsReversed, validRecords);
}

if (ansPart1 > 1000)
    Debug.Assert(ansPart1 == 8193);

Console.WriteLine($"The answer for Part {1} is {ansPart1}");

// *** Part 2 ***
// *** Part 2 ***
// *** Part 2 ***

// Because the scale is exponentially larger by 5x bits, the worst case is ~100bits
// of binary possibilities!  This is too big.
// Perhaps we can validate AS we're enumerating. So we only enumerate valid choices.
// This will keep the memory size and calculation size and variable size manageable.
// Kinda wish our solution didn't use "bits" now.  Combining strings is easy.
// bit lengths are arbitrary, too.  Another problem with bits.  :(

//long ansPart2 = wildStatus.Select(half =>
//    EnumerateValidateCountUnknowns(half.Item1, half.Item2)).sum();

//Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

int PossibleValidArrangementCount(string springs, List<int> validationData)
{
    // '#' == known spring (so _must_ be used)
    // '.' == known empty spot (so _cannot_ be used)
    // '?' == could be either (so can be used or not)
    //
    // Process the right side of the string in the following recursive fashion
    // Eliminate chars that cannot match the size of the first validationData element.
    // If those chars include a known spring, return 0, terminating this recursion.
    // Count the span of chars that could match the validationData element.
    // Keeping in mind the constraints of Must Be Used and Cannot Be Used spaces.
    // Must Be Used locations anchor the validation length
    // Cannot Be Used locations block the validation length
    //
    // Terminate the recursion with the multiplicative identity (1) or cancelation (0)
    if (validationData.Count == 0)
    {
        bool noSpringsRemain = springs.IndexOf('#') < 0;
        return noSpringsRemain ? 1 : 0;
    }
    if (springs.Length == 0)
    {
        return validationData.Count == 0 ? 1 : 0;
    }

    // Grab the validation-record we're working with on this iteration
    int vRecord = validationData[0];

    // Let's trim the string of starting empties
    int ndx = 0;
    while (springs[ndx] == '.')
    {
        ndx++;
        if (ndx == springs.Length) return 0; // Invalid
    }
    springs = springs.Substring(ndx);

    // Reindex pointers
    int firstEmpty = springs.IndexOf('.');
    int firstSpring = springs.IndexOf('#');

    // Let's also trim an initial region that doesn't fit our vData
    // Or terminate if such a region includes a spring (known/required)
    // aka "If it doesn't fit, you must acquit."
    if (firstEmpty >= 0 && firstEmpty + 1 < vRecord)
    {
        if (firstSpring >= 0 && firstSpring < firstEmpty)
        {
            return 0; // "If it doesn't fit, you must acquit."
        }
        else
        {
            int newStart = Math.Min(firstEmpty + 1, springs.Length - 1);
            // We'll recurse after trimming to get back to the top to start over
            return PossibleValidArrangementCount(springs.Substring(newStart), validationData);
        }
    }

    // Now we know we can fit our validated spring into this string...
    // Let's find out how much margin we have to do so...    
    int minNdx = 0;
    int maxNdx = springs.Length - 1;

    // Blocked by empty?
    if (firstEmpty >= 0) maxNdx = firstEmpty - 1;

    // Does an anchor limit the size?
    if (firstSpring >= 0 && firstSpring + vRecord - 1 < maxNdx)
        maxNdx = firstSpring + vRecord - 1;

    if (maxNdx - minNdx + 1 < vRecord)
        return 0; // Invalid

    // Find all known springs in this region, we'll use these locations to reject adjacencies.
    // Overlap of a spring is OK, but adjacencies will EXTEND
    // our field past the validated record length which is not
    string region = springs.Substring(minNdx, maxNdx - minNdx + 1);
    List<int> springNdxes = region.IndicesOf('#');

    // FINALLY, we can slide our vRecord-length candidate through our region
    // validating candidates don't have against adjacent springs
    // (right side checking is all that is required, due to left-side anchoring on first spring)
    // then recurse further for each success,
    // adding the results form each to establish the return value.
    // Phew!
    validationData.RemoveAt(0); // We're done processing this vRecord, remove it before we recurse.
    int returnVal = 0;
    for (int start = minNdx; start <= maxNdx-vRecord+1; start++)
    {
        int end = start + vRecord - 1;
        if (end+1==springs.Length || springs[end+1] != '#')
        {
            returnVal +=
                end + 1 < springs.Length ?
                PossibleValidArrangementCount(springs[(end + 1)..], validationData) :
                PossibleValidArrangementCount("", validationData);
        }
    }
    return returnVal;
}

public static class MyExtensions
{
    public static List<int> IndicesOf(this string input, char searchChar)
    {
        int ndx = input.IndexOf(searchChar);
        if (ndx < 0) return new();
        var indices = input[(ndx + 1)..].IndicesOf(searchChar).Select(i => i + ndx + 1).ToList();
        indices.Add(ndx);
        return indices;
    }

    /// <summary>
    /// This is a quickly written IEnumerable in/out which uses concrete collection
    /// I'm sure this violates a design rule for an IEnumereable
    /// But I haven't thought through how to do this otherwise.  :(
    /// ToDo: Fix this -- use enumerations-only in this routine.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="searchChar"></param>
    /// <returns></returns>
    public static IEnumerable<int> IndicesOf(this IEnumerable<int> input, char searchChar)
    {
        List<int> concrete = input.ToList();
        return concrete.IndicesOf(searchChar);
    }
}