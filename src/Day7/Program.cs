// https://adventofcode.com/2023/day/7
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Threading;
using System.Collections.Generic;

// Every hand is exactly one type. From strongest to weakest, they are:
//    Five of a kind, where all five cards have the same label: AAAAA
//    Four of a kind, where four cards have the same label and one card has a different label: AA8AA
//    Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
//    Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
//    Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
//    One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
//    High card, where all cards' labels are distinct: 23456

// If two hands have the same type, a second ordering rule takes effect. The hand with the stronger first card is considered stronger.
//    "3QQQQ" beats "2AAAA"

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day7.txt");

if (aocPart == 1)
    // Part 1 example input
    lines = new string[] 
    {
        "32T3K 765",
        "T55J5 684",
        "KK677 28",
        "KTJJT 220",
        "QQQJA 483",
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


foreach (var line in lines)
{
    List<Hand> hands = new();
    var fields = line.Split(" ", StringSplitOptions.TrimEntries);
    if (fields == null) throw new Exception("Parsing error");
    hands.Add(new Hand(fields[0], fields[1]));
    Console.WriteLine(line);
    Console.WriteLine($"{hands.Last().Cards} {hands.Last().Bet}");
}

uint ansPart1 = 0;
uint ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

class Hand
{
    public string Cards;
    public int Bet;
    int handScore;

    public Hand(string rawHand, string rawBet) : this(rawHand, int.Parse(rawBet)) { }
    public Hand(string rawHand, int bet)
    {
        Debug.Assert(rawHand.Length == 5);
        Cards = rawHand;
        Bet = bet;
        handScore = CalcScore(Cards);
    }

    enum HandScore
    {
        FiveOfaKind = 5,
        FourOfaKind = 4,
        ThreeOfaKind = 3,
        TwoPair = 2,
        OnePair = 1,
        HighCard = 0,
    }

    private int CalcScore(string cards)
    {
        Dictionary<char,int> card_counter = new();
        foreach (char c in cards)
        {
            if (card_counter.TryGetValue(c, out int count))
                card_counter[c]++;
            else
                card_counter.Add(c, 1);
        }
        int same = card_counter.Values.Max();
        if (same >= 3) return same;
        if (same == 2)
            return (card_counter.Where((c, v) => v == 2).Count() > 1) ? 2 : 1;
        return 0;
    }

    private int CardValue(char card)
    {
        if (char.IsDigit(card))
            return card - '0';
        else
            switch (card)
            {
                case 'J': return 11;
                case 'Q': return 12;
                case 'K': return 13;
                case 'A': return 14;
                default:
                    throw new Exception("Illegal card ('{card}')");
            }
    }
    private bool IsCard(char candidate)
    {
        return char.IsDigit(candidate) && candidate != 0 ||
            candidate == 'J' ||
            candidate == 'Q' ||
            candidate == 'K' ||
            candidate == 'A'
            ;
    }
}