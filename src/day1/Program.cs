// https://adventofcode.com/2023/day/1
using System.Diagnostics;
using System;

int aocPart = 2;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day1.txt");
string[] digitWords = { "zero_isnotallowed", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
List<List<int>> cals = new();

//if (aocPart == 1)
//    // Part 1 example input
//    lines = new string[] {
//        "1abc2",
//        "pqr3stu8vwx",
//        "a1b2c3d4e5f",
//        "treb7uchet"
//        };
//else if (aocPart == 2)
//    // Part 2 example input
//    lines = new string[] {
//        "two1nine",
//        "eightwothree",
//        "abcone2threexyz",
//        "xtwone3four",
//        "4nineeightseven2",
//        "zoneight234",
//        "7pqrstsixteen"
//        };
//else
//    Debug.Assert(false, "aocPart should be 1 or 2");

foreach (string rawline in lines)
{
    var line = rawline;
    if (aocPart == 2)
    {
        int? firstNdx;
        int firstPos;
        do
        {
            firstNdx = null;
            firstPos = int.MaxValue;
            for (int i = 0; i <= 9; i++)
            {
                if (line.Contains(digitWords[i]))
                {
                    int pos = line.IndexOf(digitWords[i]);
                    if (firstNdx == null || firstPos > pos)
                    {
                        firstNdx = i;
                        firstPos = pos;
                    }
                }
            }
            // '.Length-1' at end is a hack to fix Part 2. Retain the last letter of each digit-word in case the next word is using that letter.
            // so "twone" becomes "21" instead of "2ne".  Works!
            if (firstNdx != null)
                line = string.Concat(line[..firstPos], firstNdx.ToString(), line.AsSpan(firstPos + digitWords[(int)firstNdx].Length-1));
        } while (firstNdx != null);
    }
    List<int> cal = new();
    foreach (char c in line)
    {
        if (c >= '0' && c <= '9')
        {
            int num;
            if (int.TryParse(c.ToString(), out num))
                cal.Add(num);
        }
    }
    Debug.Assert(cal.Count > 0, $"Empty list of number found for {line}");
    cals.Add(cal);
}

int sum = 0;
foreach (var cal in cals)
{
    int value = cal.First() * 10 + cal.Last();
    sum += value;
    foreach (var n in cal)
        Console.Write(n);
    Console.WriteLine($" -> {value}");
}

Debug.Assert(cals.Count == lines.Length);

Console.WriteLine($"Part {aocPart} answer is {sum}");