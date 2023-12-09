// https://adventofcode.com/2023/day/9
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day9.txt");

//if (aocPart == 1)
//    // Part 1 example input
//    lines = new string[] {
//        "0 3 6 9 12 15",
//        "1 3 6 10 15 21",
//        "10 13 16 21 30 45",
//    };

//else if (aocPart == 2)
//    // Part 2 example input
//    lines = new string[] {
//        };
//else
//    Debug.Assert(false, "aocPart should be 1 or 2");

List<int> newStartVals = new();
List<int> newEndVals = new();
foreach (var line in lines)
{
    int[] nums = line.Split(' ').Select(s => int.Parse(s)).ToArray();
    List<int> firstVals = new();
    List<int> lastVals = new();
    int zeroCount;
    //DumpNums(nums);
    do
    {
        firstVals.Add(nums.First());
        lastVals.Add(nums.Last());
        zeroCount = 0;
        int? prev = null;
        nums = nums.Select(n =>
        {
            int? diff = n - prev;
            prev = n; return diff;
        }).Where(a => a.HasValue).Select(a => a.Value).ToArray();
        zeroCount = nums.TakeWhile(n => n == 0).Count();
    } while (zeroCount < nums.Length);
    firstVals.Reverse();
    int newFirst = 0;
    foreach (int f in firstVals)
    {
        newFirst = f - newFirst;
    }
    newStartVals.Add(newFirst);
    newEndVals.Add(lastVals.Sum());
}

int ansPart1 = newEndVals.Sum();
int ansPart2 = newStartVals.Sum();

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

static void DumpNums(IEnumerable<int> nums)
{
    foreach (int n in nums) Console.Write($"{n} ");
    Console.WriteLine();
}