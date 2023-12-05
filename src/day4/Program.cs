// https://adventofcode.com/2023/day/4
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day4.txt");

//if (aocPart == 1)
//    // Part 1 example input
//    lines = new string[] 
//    {
//        "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
//        "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
//        "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
//        "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
//        "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
//        "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11",
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

int ansPart1 = 0;
List<int> cardCounts = new(); // Index is CardID-1, value is count of Card
int finalCard = 0;
foreach (var line in lines)
{
    //Console.WriteLine(line);
    var (cardID, winningNums, myNums) = LineParser.Parser(line);
    int cardScore = 0;
    int winCount = 0;
    foreach (int winningNum in winningNums)
        foreach (int num in myNums)
            if (num == winningNum)
            {
                cardScore = cardScore == 0 ? 1 : cardScore * 2;
                winCount++;
            }
    ansPart1 += cardScore;
    // Part 2 processing
    // make room for present and future cards as needed
    int startingCount = cardCounts.Count;
    int endingCount = cardID + winCount;
    for (int ndx = startingCount; ndx < endingCount; ndx++)
        cardCounts.Add(1); // Initialize new cards with a count of 1
    // tally copies made because of wins (wins on _each_ copy!)
    for (int ndx = cardID-1+1; ndx < cardID + winCount; ndx++)
        cardCounts[ndx]+=cardCounts[cardID-1];
    finalCard = cardID; // Don't process cards past this (they are imaginary!)
}
if (cardCounts.Count > finalCard)
    cardCounts.RemoveRange(finalCard, cardCounts.Count - finalCard);
int ansPart2 = cardCounts.Sum();


Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

public class LineParser
{
    //static string parseScratchCardPattern = @"^Card +(\d+):(?: +(\d+)){5} |(?: +(\d+)){8}";
    static string parseScratchCardPattern = @"^Card +(\d+):(?: +(\d+)){10} |(?: +(\d+)){25}";
    static Regex parseScratchCardRE = new(parseScratchCardPattern);

    public static Tuple<int, int[], int[]> Parser(string txtDefinition)
    {
        var scratchCardData = parseScratchCardRE.Match(txtDefinition);
        if (!scratchCardData.Success)
            throw new Exception("Line Parser Error");
        string numString = scratchCardData.Groups[1].Value;
        int CardID;
        if (!int.TryParse(numString, out CardID))
            throw new Exception("Line Parser Error");
        //Console.Write($"Card {numString}: ");
        var group = scratchCardData.Groups[2];
        int[] winningNums = new int[group.Captures.Count];
        for (int cap = 0; cap < group.Captures.Count; cap++)
        {
            string s = group.Captures[cap].Value;
            if (!int.TryParse(s, out int capVal))
                throw new Exception("Line Parser Error");
            winningNums[cap] = capVal;
            //Console.Write($"{capVal} ");
        }
        scratchCardData = scratchCardData.NextMatch();
        group = scratchCardData.Groups[3];
        int[] myNums = new int[group.Captures.Count];
        for (int cap = 0; cap < group.Captures.Count; cap++)
        {
            string s = group.Captures[cap].Value;
            if (!int.TryParse(s, out int capVal))
                throw new Exception("Line Parser Error");
            myNums[cap] = capVal;
            //Console.Write($"{capVal} ");
        }
        return new Tuple<int, int[], int[]>(CardID, winningNums, myNums);
    }
};
