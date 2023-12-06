// https://adventofcode.com/2023/day/5
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Globalization;
using System.Diagnostics.Metrics;
using System.Text;

long aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day5.txt");

//if (aocPart == 1)
//    // Part 1 example input
//    lines = new string[]
//    {
//        "seeds: 79 14 55 13",
//        "",
//        "seed-to-soil map:",
//        "50 98 2",
//        "52 50 48",
//        "",
//        "soil-to-fertilizer map:",
//        "0 15 37",
//        "37 52 2",
//        "39 0 15",
//        "",
//        "fertilizer-to-water map:",
//        "49 53 8",
//        "0 11 42",
//        "42 0 7",
//        "57 7 4",
//        "",
//        "water-to-light map:",
//        "88 18 7",
//        "18 25 70",
//        "",
//        "light-to-temperature map:",
//        "45 77 23",
//        "81 45 19",
//        "68 64 13",
//        "",
//        "temperature-to-humidity map:",
//        "0 69 1",
//        "1 0 69",
//        "",
//        "humidity-to-location map:",
//        "60 56 37",
//        "56 93 4",
//        "",
//    };
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

long[] seeds = LineParser.ParseSeeds(lines[0]);
long lineNdx = 2;
List<Map> maps = new();
while (lineNdx < lines.Length)
{
    List<Mapping> mappings = new();
    string mapName = LineParser.ParseMapName(lines[lineNdx]);
    lineNdx++;
    while (lineNdx < lines.Length && lines[lineNdx].Length > 0)
    {
        var (dest, src, length) = LineParser.ParseMapLine(lines[lineNdx]);
        mappings.Add(new Mapping(dest, src, length));
        lineNdx++;
    }
    maps.Add(new Map(mapName, mappings));
    lineNdx++;
}

//foreach (Map m in maps)
//{
//    Console.Write($"{m.Name} contains {m.Mappings.Length} mappings.");
//    long from = m.Mappings[0].From;
//    long to = m.Mappings[0].To;
//    long length = m.Mappings[0].Length;
//    Console.WriteLine($"The first is {from} maps to {to} with {length} elements.");
//}

// Part 1
IEnumerable<long> locations = seeds.Select(seed => TraverseMaps(maps, seed));
long ansPart1 = locations.Min();
Console.WriteLine($"The answer for Part {1} is {ansPart1}");

// Part 2

// Count of seeds in Part 2 is 2.132355834 trillion.

List<(long, long)> seedRanges = new();
for (long pairNdx = 0; pairNdx < seeds.Length; pairNdx += 2)
    seedRanges.Add((seeds[pairNdx], seeds[pairNdx + 1]));
IEnumerable<(long, long)> locationRanges = seedRanges.SelectMany(seedRange => TraverseMapRanges(maps, seedRange));
//var debug = locationRanges.ToArray();
IEnumerable<long> locationRangeStarts = locationRanges.Select(range => range.Item1);
long ansPart2 = locationRangeStarts.Min();
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

List<(long,long)> TraverseMapRanges(List<Map> maps, (long,long) seedRange)
{
    List<(long, long)> ranges = new();
    ranges.Add(seedRange);
    foreach (Map map in maps)
    {
        List<(long, long)> traversedRanges = new();
        foreach ((long,long) range in ranges)
            traversedRanges = traversedRanges.Concat(TraverseRange(map, range)).ToList();
        ranges = traversedRanges;
    }
    return ranges;
}
List<(long,long)> TraverseRange(Map map, (long,long) sourceRange)
{
    // Take the single input range and break it up into the intersections with each of the present map's source ranges.
    // Then connect those sub-sections with new ranges as necessary (representing the implicit one-to-one mapping)
    // For each sub-range, transform the starting points and return the tranformed list of ranges.
    // 
    List<(long,long)> sRanges = new();
    sRanges.Add(sourceRange);
    foreach (Mapping mapping in map.Mappings)
    {
        (long, long) mapRange = new(mapping.From, mapping.Length);
        List<(long, long)> newSRanges = new();
        foreach ((long, long) sRange in sRanges)
        {
            (long? xStartNullable, long xLength) = findRangeIntersection(sRange, mapRange);
            if (xStartNullable == null)
                newSRanges.Add(sRange); // Unmodified due to no-overlap
            else
            {
                // overlap
                long xStart = (long)xStartNullable;
                newSRanges.Add((xStart, xLength));
                // before if any
                if (sRange.Item1 < xStart)
                    newSRanges.Add((sRange.Item1, xStart - sRange.Item1));
                // after if any
                if (sRange.Item1 + sRange.Item2 > xStart + xLength)
                    newSRanges.Add((xStart + xLength, sRange.Item1 + sRange.Item2 - (xStart + xLength)));
            }
        }
        sRanges = newSRanges;
    }
    // Now,finally, actually tranform those segments
    // Just the starting points (they are minumums aready
    List<(long, long)> dRanges = sRanges.Select(sr => (Traverse(map, sr.Item1), sr.Item2)).ToList();
    return dRanges;
}

(long?, long) findRangeIntersection((long,long) sourceRange, (long,long) mapRange)
{
    // Intersection == max start to min end with non-negative length
    long xStart = sourceRange.Item1 > mapRange.Item1 ? sourceRange.Item1 : mapRange.Item1;
    // Ends are non-inclusive (so we can avoid annoying -1's)
    long sEnd = sourceRange.Item1 + sourceRange.Item2;
    long mEnd = mapRange.Item1 + mapRange.Item2;
    long xEnd = sEnd < mEnd ? sEnd : mEnd;
    long xLength = xEnd - xStart;
    if (xLength < 1)
        return (null, 0);
    return (xStart, xLength);
}

long TraverseMaps(List<Map> maps, long seed)
{
    long start = seed;
    foreach (Map map in maps)
        start = Traverse(map, start);
    return start;
}

long Traverse(Map map, long source)
{
    foreach (Mapping m in map.Mappings)
    {
        if (source >= m.From && source < m.From + m.Length)
            return m.To + source - m.From;
    }
    // Didn't translate?  Return self.
    return source;
}

public struct Map
{
    public string Name;
    public Mapping[] Mappings;

    public Map(string mapName, List<Mapping> mappings)
    {
        this = new(mapName, mappings.ToArray());
    }
    public Map(string mapName, Mapping[] mappings)
    {
        Name = mapName;
        Mappings = mappings;
    }
}
public struct Mapping
{
    public long From;
    public long To;
    public long Length;

    public Mapping(long to, long from, long length)
    {
        To = to;
        From = from;
        Length = length;
    }
    public override string ToString() => $"Destination is {To}, source is {From}, and length is {Length})";
}

public static class LineParser
{
    static string numListPattern = @"(\d+) *";
    static Regex numListRE = new(numListPattern);
    static string mapNamePattern = @"([a-z\-]+) map:";
    static Regex mapNameRE = new(mapNamePattern);

    public static long[] ParseSeeds(string text)
    {
        Debug.Assert(text.IndexOf("seeds: ") == 0);
        long[] nums = ParseNumArray(numListRE, text.Substring("seeds: ".Length));
        return nums;
    }
    public static (long,long,long) ParseMapLine(string text)
    {
        long[] nums = ParseNumArray(numListRE, text);
        return (nums[0], nums[1], nums[2]);
    }
    public static string ParseMapName(string text)
    {
        var match = mapNameRE.Match(text);
        if (!match.Success)
            throw new Exception($"Line Parser Error on '{text}'");
        return match.Groups[1].Value;
    }
    static long[] ParseNumArray(Regex RE, string text)
    {
        var match = RE.Match(text);
        List<long> nums = new();
        while (match.Success)
        {
            string s = match.Groups[1].Value;
            if (!long.TryParse(s, out long capVal))
                throw new Exception($"Num Parser Error on '{s}'");
            nums.Add(capVal);
            match = match.NextMatch();
        }
        return nums.ToArray();
    }
};
