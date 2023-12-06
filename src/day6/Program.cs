// https://adventofcode.com/2023/day/6
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
// Dan T's Day6 input:
//long[] time = { 49, 97, 94, 94 };
//long[] distance = { 263, 1532, 1378, 1851 };
//Part2
long[] time = { 49979494 };
long[] distance = { 263153213781851 };

// Example input
//long[] time = {7, 15, 30};
//long[] distance = {9, 40, 200};
//Part2
//long[] time = { 71530 };
//long[] distance = { 940200 };
//string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day6.txt");

// Starting at 0 milliseconds, hold the button for x milliseconds to increase engine spead to x millimeters/sec (holding button also holds the boat)
// At x milliseconds, lift your finger and the boat flies off at it's accumulated engine speed.
// When the time's up, how far have you gone?  If you exceeded the best distances so far... good.

// distance = speed * time, so distance travelled is (time - x) * x
long winCountMultiple = 1;
for (int race = 0; race < time.Length; race++)
{
    int raceWins = 0;
    for (int t = 0; t < time[race]; t++)
    {
        long calcDistance = (time[race] - t) * t;
        if (calcDistance > distance[race])
            raceWins++;
    }
    winCountMultiple *= raceWins;
}
long ansPart1 = winCountMultiple;
long ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End
