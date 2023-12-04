// https://adventofcode.com/2023/day/2
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;

int aocPart = 2;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day2.txt");

if (aocPart == 1)
    // Part 1 example input
    lines = new string[] {
    "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
    "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
    "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
    "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
    "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
        };
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

Game[] games = new Game[lines.Length];
int ndx = 0;
foreach (var line in lines)
{
    line.Append(';');
    // add termiating semicolon to simplify regex in Game constructor
    games[ndx] = new(line+';');
    ndx++;
    // Console.WriteLine(line);
}

Grab[] maxKnownCounts = new Grab[games.Length];
for (int gameNdx = 0; gameNdx < games.Length; gameNdx++)
{
    Debug.Assert(games[gameNdx].gameNum == gameNdx + 1);
    foreach (Grab grab in games[gameNdx].grabs)
    {
        if (grab.nRed > maxKnownCounts[gameNdx].nRed)
            maxKnownCounts[gameNdx].nRed = grab.nRed;
        if (grab.nGreen > maxKnownCounts[gameNdx].nGreen)
            maxKnownCounts[gameNdx].nGreen = grab.nGreen;
        if (grab.nBlue > maxKnownCounts[gameNdx].nBlue)
            maxKnownCounts[gameNdx].nBlue = grab.nBlue;
    }
}

uint ansPart1 = 0;
uint ansPart2 = 0;
Grab part1test = new(12, 13, 14);
for (int gameNdx = 0; gameNdx < games.Length; gameNdx++)
{
    Grab maxxes = maxKnownCounts[gameNdx];
    if (maxxes.nRed <= part1test.nRed && maxxes.nGreen <= part1test.nGreen && maxxes.nBlue <= part1test.nBlue)
        ansPart1 += games[gameNdx].gameNum;
    ansPart2 += maxxes.nRed * maxxes.nGreen * maxxes.nBlue;
}

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

public class Game
{
    public List<Grab> grabs;
    public uint gameNum;

    static string parseGameNumPattern = @"^Game (\d+):";
    static string parseGrabsPattern = @"(?: (\d+) (red|green|blue),){0,1}(?: (\d+) (red|green|blue),){0,1} (\d+) (red|green|blue);";
    static Regex parseGameNumRE = new(parseGameNumPattern);
    static Regex parseGrabsRE = new(parseGrabsPattern);

    public Game(string txtDefinition)
    {
        //if (parseGames == null)
        //    parseGames = new Regex(parseGamesRegex);
        //if (parseGameNum == null)
        //    parseGameNum = new Regex(parseGameNumRegex);
        grabs = new();
        var numString = parseGameNumRE.Match(txtDefinition).Groups[1].Value;
        if (!uint.TryParse(numString, out gameNum))
            throw new Exception("Parse Error: GameNum");
        Debug.Assert(gameNum > 0, $"Game number is ZERO!");
        Debug.Assert(gameNum <= 100, $"Game number too big! ({gameNum})");
        Match match = parseGrabsRE.Match(txtDefinition);
        while (match.Success)
        {
            Grab grab = new();
            // Groups 1, 3, 5 contain (possibly empty) stone counts, 2, 4, 6 contain colors
            for (int g = 1; g <= 5; g+=2)
            {
                if (match.Groups[g].Length > 0)
                {
                    if (!uint.TryParse(match.Groups[g].Value, out uint stoneCount))
                        throw new Exception("Bad stone count.");
                    string stoneColor = match.Groups[g+1].Value;
                    if (stoneColor == "red")
                        grab.nRed = stoneCount;
                    else if (stoneColor == "green")
                        grab.nGreen = stoneCount;
                    else if (stoneColor == "blue")
                        grab.nBlue = stoneCount;
                    else
                        throw new Exception("Bad color.");
                }
            }
            grabs.Add(grab);
            match = match.NextMatch();
        }
    }
};
public struct Grab
{
    public uint nRed;
    public uint nGreen;
    public uint nBlue;

    public Grab(uint red, uint green, uint blue)
    {
        nRed = red;
        nGreen = green;
        nBlue = blue;
    }
};