// https://adventofcode.com/2023/day/8
using System.Diagnostics;

int aocPart = 2;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day8.txt");

//// Part 1 example input
//lines = new string[]
//{
//    "RL",
//    "",
//    "AAA = (BBB, CCC)",
//    "BBB = (DDD, EEE)",
//    "CCC = (ZZZ, GGG)",
//    "DDD = (DDD, DDD)",
//    "EEE = (EEE, EEE)",
//    "GGG = (GGG, GGG)",
//    "ZZZ = (ZZZ, ZZZ)",
//};

//// Example 2
//lines = new string[]
//{
//    "LLR",
//    "",
//    "AAA = (BBB, BBB)",
//    "BBB = (AAA, ZZZ)",
//    "ZZZ = (ZZZ, ZZZ)",
//};

//// Part 2 Example
//lines = new string[]
//{
//    "LR",
//    "",
//    "11A = (11B, XXX)",
//    "11B = (XXX, 11Z)",
//    "11Z = (11B, XXX)",
//    "22A = (22B, XXX)",
//    "22B = (22C, 22C)",
//    "22C = (22Z, 22Z)",
//    "22Z = (22B, 22B)",
//    "XXX = (XXX, XXX)",
//};

string lr = lines[0];

Dictionary<string, (string, string)> rawMap = new();

foreach (var line in lines[2..])
{
    rawMap.Add(line[0..3], (line[7..10], line[12..15]));
}

foreach (var item in rawMap)
{
    Console.WriteLine(item);
}

int ansPart1 = 0;
if (aocPart == 1)
{
    Debug.Assert(rawMap.ContainsKey("AAA"));
    Debug.Assert(rawMap.ContainsKey("ZZZ"));
    string node = "AAA";
    int stepCount = 0;
    while (node != "ZZZ")
    {
        node = lr[stepCount % lr.Length] == 'L' ? rawMap[node].Item1 : rawMap[node].Item2;
        stepCount++;
    }
    ansPart1 = stepCount;
}
long ansPart2 = 0;
if (aocPart == 2)
{
    string[] nodes = rawMap.Keys.Where(key => key[2] == 'A').ToArray();
    if (nodes.Length >= 6)
        Console.WriteLine($"{nodes[0]},{nodes[1]},{nodes[2]},{nodes[3]},{nodes[4]},{nodes[5]}");
    int[] stepCounts = nodes.Select(node =>
    {
        int stepCount = 0;
        while (node[2] != 'Z')
        {
            node = lr[stepCount % lr.Length] == 'L' ? rawMap[node].Item1 : rawMap[node].Item2;
            stepCount++;
        }
        return stepCount;
    }).ToArray();
    long commonStepCount = stepCounts[0];
    for (int i = 1; i < stepCounts.Length; i++)
    {
        commonStepCount = IntegerMath.lcm(commonStepCount, (long)stepCounts[i]);
    }
    ansPart2 = commonStepCount;
}

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

public static class IntegerMath
{
    public static long lcm(long a, long b)
    {
        return (a / gcf(a, b)) * b;
    }
    static long gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
class Node
{
    public string Name;
    public string L;
    public string R;

    public Node(string name, string left, string right)
    {
        Name = name;
        L = left;
        R = right;
    }
}