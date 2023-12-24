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

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day19.txt");

// Example 1 input
lines = new string[] {
"px{a<2006:qkq,m>2090:A,rfg}",
"pv{a>1716:R,A}",
"lnx{m>1548:A,A}",
"rfg{s<537:gd,x>2440:R,A}",
"qs{s>3448:A,lnx}",
"qkq{x<1416:A,crn}",
"crn{x>2662:A,R}",
"in{s<1351:px,qqz}",
"qqz{s>2770:qs,m<1801:hdj,R}",
"gd{a>3333:R,R}",
"hdj{m>838:A,pv}",
"",
"{x=787,m=2655,a=1222,s=2876}",
"{x=1679,m=44,a=2067,s=496}",
"{x=2036,m=264,a=79,s=2244}",
"{x=2461,m=1339,a=466,s=291}",
"{x=2127,m=1623,a=2188,s=1013}",
};

string[] stateMachineRaw = lines.TakeWhile(s => s.Length > 0).ToArray();
Dictionary<string, List<(XMAS?, bool?, int?, string)>> stateMachine = 
    stateMachineRaw.Select(s =>
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
    }).ToDictionary();

//string[] inventoryRaw = lines.SkipWhile(s => s.Length > 0).Where(s => s.Length > 0).ToArray();

foreach (var line in lines)
{
    Console.WriteLine(line);
}

int ansPart1 = 0;
int ansPart2 = 0;

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