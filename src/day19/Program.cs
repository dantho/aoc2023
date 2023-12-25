// https://adventofcode.com/2023/day/19
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Formats.Asn1;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Data;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day19.txt");

//// Example 1 input
//lines = new string[] {
//"px{a<2006:qkq,m>2090:A,rfg}",
//"pv{a>1716:R,A}",
//"lnx{m>1548:A,A}",
//"rfg{s<537:gd,x>2440:R,A}",
//"qs{s>3448:A,lnx}",
//"qkq{x<1416:A,crn}",
//"crn{x>2662:A,R}",
//"in{s<1351:px,qqz}",
//"qqz{s>2770:qs,m<1801:hdj,R}",
//"gd{a>3333:R,R}",
//"hdj{m>838:A,pv}",
//"",
//"{x=787,m=2655,a=1222,s=2876}",
//"{x=1679,m=44,a=2067,s=496}",
//"{x=2036,m=264,a=79,s=2244}",
//"{x=2461,m=1339,a=466,s=291}",
//"{x=2127,m=1623,a=2188,s=1013}",
//};

string[] stateMachineRaw = lines.TakeWhile(s => s.Length > 0).ToArray();
Dictionary<string, List<(XMAS?, bool?, int?, string)>> stateMachine = stateMachineRaw.Select(s =>
{
    int div = s.IndexOf('{');
    string state = s[..div];
    string[] rulesRaw = s[(div + 1)..^1].Split(',').ToArray();
    List<(XMAS?, bool?, int?, string)> rules = new();
    foreach (var rule in rulesRaw)
    {
        int colon = rule.IndexOf(':');
        if (colon < 0)
        {
            rules.Add((null, null, null, rule));
        }
        else
        {
            int num = int.Parse(rule[2..colon]);
            string dest = rule[(colon + 1)..];
            bool isLessThan = rule[1] == '<';
            rules.Add((rule[0].ToXMAS(), isLessThan, num, dest));
        }
    }
    KeyValuePair<string, List<(XMAS?, bool?, int?, string)>> kv = new(state, rules);
    return kv;
}).Aggregate(new Dictionary<string, List<(XMAS?, bool?, int?, string)>>(), (dict, kv) =>
{
    dict.Add(kv.Key, kv.Value);
    return dict;
});
string[] inventoryRaw = lines.SkipWhile(s => s.Length > 0).Where(s => s.Length > 0).ToArray();
int[][] inventory = inventoryRaw
    .Select(s => s[1..^1]
    .Split(',')
        .Select(numstr => int.Parse(numstr[2..]))
        .ToArray())
    .ToArray();

// Solve puzzle
int ansPart1 = inventory.Select(xmasPart => {
    string state = "in";
    while (!(state == "A" || state == "R"))
    {
        var rules = stateMachine[state];
        foreach (var rule in rules)
        {
            if (rule.Item1 is null) {
                state = rule.Item4;
                break;
            }
            if ((bool)rule.Item2)
            {
                if (xmasPart[(int)rule.Item1] < rule.Item3)
                {
                    state = rule.Item4;
                    break;
                }
            }
            else
            {
                if (xmasPart[(int)rule.Item1] > rule.Item3)
                {
                    state = rule.Item4;
                    break;
                }
            }
        }
    }
    if (state == "A") return xmasPart.Sum();
    if (state == "R") return 0;
    throw new Exception("State machine failure");
}).Sum();
if (ansPart1 > 50000)
    Debug.Assert(ansPart1 == 446935);

// Part 2 *****************

(long, long)[] xmasRangeInit = { (1, 4000), (1, 4000), (1, 4000), (1, 4000) };
Queue<(string, (long Low, long High)[])> options = new();
options.Enqueue(("in", xmasRangeInit));
long possibilityCount = 0;
while (options.Count > 0)
{
    (string state, (long Low, long High)[] xmasRange) = options.Dequeue();

    while (!(state == "A" || state == "R"))
    {
        var rules = stateMachine[state];
        foreach (var rule in rules)
        {
            if (rule.Item1 is null)
            {
                state = rule.Item4;
                break;
            }
            if ((bool)rule.Item2)
            {
                if (xmasRange[(int)rule.Item1].Low >= rule.Item3) // none
                    continue; 
                if (xmasRange[(int)rule.Item1].High < rule.Item3) // all
                {
                    state = rule.Item4;
                    break;
                }
                // some
                (long Low, long High)[] xmasRange2 = ((long Low, long High)[])xmasRange.Clone();
                xmasRange2[(int)rule.Item1].High = (long)rule.Item3 - 1;
                options.Enqueue((rule.Item4, xmasRange2));
                xmasRange[(int)rule.Item1].Low = (long)rule.Item3;
                continue;
            }
            else
            {
                if (xmasRange[(int)rule.Item1].High <= rule.Item3) // none
                    continue; 
                if (xmasRange[(int)rule.Item1].Low > rule.Item3) // all
                {
                    state = rule.Item4;
                    break;
                }
                // some
                (long Low, long High)[] xmasRange2 = ((long Low, long High)[])xmasRange.Clone();
                xmasRange2[(int)rule.Item1].Low = (long)rule.Item3 + 1;
                options.Enqueue((rule.Item4, xmasRange2));
                xmasRange[(int)rule.Item1].High = (long)rule.Item3;
                continue;
            }
        }
    }
    if (state == "A")
        possibilityCount += 
            (xmasRange[0].High - xmasRange[0].Low + 1) * 
            (xmasRange[1].High - xmasRange[1].Low + 1) * 
            (xmasRange[2].High - xmasRange[2].Low + 1) *
            (xmasRange[3].High - xmasRange[3].Low + 1);
    if (state == "R") ;
    //throw new Exception("State machine failure");
}

foreach (var line in lines)
{
    Console.WriteLine(line);
}

long ansPart2 = possibilityCount;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

public enum XMAS
{
    X,
    M,
    A,
    S,
}

public static class MyExtensions
{
    public static XMAS ToXMAS(this char c)
    {
        return char.ToUpper(c) switch
        {
            'X' => XMAS.X,
            'M' => XMAS.M,
            'A' => XMAS.A,
            'S' => XMAS.S,
            _ => throw new Exception($"illegal XMAS char '{c}'")
        };
    }
}