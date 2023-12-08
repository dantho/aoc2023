// https://adventofcode.com/2023/day/8
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day8.txt");

//if (aocPart == 1)
//    // Part 1 example input
//    lines = new string[] {
//        "467..114..",
//        "...*......",
//        "..35..633.",
//        "......#...",
//        "617*......",
//        ".....+.58.",
//        "..592.....",
//        "......755.",
//        "...$.*....",
//        ".664.598..",
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


foreach (var line in lines)
{
    Console.WriteLine(line);
}

uint ansPart1 = 0;
uint ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End
