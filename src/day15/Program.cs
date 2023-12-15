// https://adventofcode.com/2023/day/15
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string cslToHash = System.IO.File.ReadLines(@"C:\Users\DanTh\github\aoc2023\inputs\day15.txt").First();

// Example
string example = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
int exampleHashSum = example.Split(',').AocHash().Sum();
Debug.Assert(exampleHashSum == 1320);

// Single line solution!  :)  Looks can be deceiving, but the extension method(s) is(are) simple.
int ansPart1 = cslToHash.Split(',').AocHash().Sum();
Console.WriteLine($"The answer for Part {1} is {ansPart1}");

// ************** Part 2 ***************
// ************** Part 2 ***************
// ************** Part 2 ***************




int ansPart2 = 0;
Console.WriteLine($"The answer for Part {2} is {ansPart2}");
Console.ReadKey();

// End
// End
// End

/// <summary>
/// HASH algorithm yields a single number in the range 0 to 255.
/// Start with a current value of 0.  For each char...
///  o Determine the ASCII code for the current character of the string.
///  o Increase the current value by the ASCII code you just determined.
///  o Set the current value to itself multiplied by 17.
///  o Set the current value to the remainder of dividing itself by 256.
///
/// </summary>
public static class MyExtensions
{
    public static int AocHash(this IEnumerable<char> input)
    {
        int currentVal = 0;
        foreach (char c in input)
        {
            currentVal += (int)c;
            currentVal *= 17;
            currentVal %= 256;
        }
        return currentVal;
    }

    public static IEnumerable<int> AocHash(this IEnumerable<string> input)
    {
        return input.Select(s => AocHash(s));
    }

    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> input)
    {
        int ndx = 0;
        return input.Select(item => (ndx++, item));
    }
}