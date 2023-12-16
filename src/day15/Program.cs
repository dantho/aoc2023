// https://adventofcode.com/2023/day/15
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Reflection.Emit;
using System.Collections;
using System.Runtime.CompilerServices;

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

//cslToHash = example;

var boxes = new List<(string, int)>[256];
for (int b = 0; b < boxes.Length; b++)
    boxes[b] = new();

foreach (string instruction in cslToHash.Split(','))
{
    if (instruction.Last() == '-')
    {
        string label = instruction[..^1];
        int boxNum = label.AocHash();
        int removeAt = boxes[boxNum].IndexOfItem1(label);
        if (removeAt >= 0) boxes[boxNum].RemoveAt(removeAt);
    }
    else if (char.IsDigit(instruction.Last()))
    {
        int digit = int.Parse(instruction[^1].ToString());
        string label = instruction[..^2];
        int boxNum = label.AocHash();
        int replaceAt = boxes[boxNum].IndexOfItem1(label);
        if (replaceAt >= 0)
            boxes[boxNum][replaceAt] = (label, digit);
        else
            boxes[boxNum].Add((label, digit));
    }
    else
        throw new Exception($"Illegal instruction {instruction}");
}

int focusingTotal = 0;
for (int b=0; b < boxes.Length; b++)
{
    int lensPos = 0;
    foreach ((_,int lens) in boxes[b])
    {
        focusingTotal += (b + 1) * lens * ++lensPos;
    }
}

int ansPart2 = focusingTotal;
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
    public static int IndexOfItem1<T>(this List<(string, T)> input, string label)
    {
        for (int ndx = 0; ndx < input.Count; ndx++)
        {
            if (input[ndx].Item1 == label) return ndx;
        }
        return -1;
    }

}
