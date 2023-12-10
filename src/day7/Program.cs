// https://adventofcode.com/2023/day/7
#define PART2

using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

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

string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day7.txt");

//lines = new string[]
//{
//    "23456 765",
//    "22456 684",
//    "22446 28",
//    "22256 220",
//    "22255 220",
//    "22226 483",
//    "22222 483",
//};

//    // Part 1 example input
//lines = new string[]
//{
//        "32T3K 765",
//        "T55J5 684",
//        "KK677 28",
//        "KTJJT 220",
//        "QQQJA 483",
//};

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

List<Hand> hands = new();
foreach (var line in lines)
{
    var fields = line.Split(" ", StringSplitOptions.TrimEntries) ?? throw new Exception("Parsing error");
    hands.Add(new Hand(fields[0], fields[1]));
    //Console.WriteLine(line);
    Console.WriteLine($"{hands.Last().Cards} {hands.Last().Bid} has value {hands.Last().Score}");
}
hands.Sort();
int totalWinnings = 0;
for (int ndx = 0; ndx < hands.Count; ndx++)
{
    totalWinnings += (ndx + 1) * hands[ndx].Bid;
}
int ansPart1 = totalWinnings;
int ansPart2 = 0;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

// End
// End
// End

class Hand : IComparable<Hand>
{
    public string Cards;
    public int Bid;
    public int Score;

    public Hand(string rawHand, string rawBet) : this(rawHand, int.Parse(rawBet)) { }
    public Hand(string rawHand, int bet)
    {
        Debug.Assert(rawHand.Length == 5);
        Cards = rawHand;
        Bid = bet;
        Score = HandValue();
    }

    public int CompareTo(Hand? other)
    {
        if (other == null) return -1;

        int val = Score.CompareTo(other.Score);
        if (val == 0)
        {
            for (int ndx = 0; ndx < Cards.Length; ndx++)
            {
                val = Hand.CardValue(Cards[ndx]).CompareTo(Hand.CardValue(other.Cards[ndx]));
                if (val != 0) break;
            }
        }
        return val;
    }
    enum HandScore
    {
        FiveOfaKind = 6,
        FourOfaKind = 5,
        FullHouse = 4,
        ThreeOfaKind = 3,
        TwoPair = 2,
        OnePair = 1,
        HighCard = 0,
    }

    private int HandValue()
    {
        Dictionary<char,int> card_counter = new();
        foreach (char c in Cards)
        {
            if (card_counter.TryGetValue(c, out int count))
                card_counter[c]++;
            else
                card_counter.Add(c, 1);
        }
        int same = card_counter.Values.Max();
        if (same == 5) return same + 1;
#if PART2
        // "Jacks are wild" processing
        // Remove jacks and boost max count by count of jacks removed
        if (card_counter.ContainsKey('J'))
        {
            int jokers = card_counter['J'];
            card_counter.Remove('J');
            same = card_counter.Values.Max() + jokers;
        }
#endif
        if (same >= 4) return same + 1;
        if (same == 3) return card_counter.Count == 2 ? 4 : 3;
        if (same == 2) return card_counter.Count == 3 ? 2 : 1;
        return 0;
    }

    private static int CardValue(char card)
    {
        if (char.IsDigit(card) && card - '0' >= 2)
            return card - '0';
        switch (card)
        {
            case 'T': return 10;
            case 'J':
                {
#if PART2
                    return 0;
#else
                    return 11;
#endif
                }
            case 'Q': return 12;
            case 'K': return 13;
            case 'A': return 14;
        }
        throw new Exception("Illegal card ('{card}')");
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