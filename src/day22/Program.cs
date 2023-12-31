// https://adventofcode.com/2023/day/22
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using AoCDay22;

string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day22.txt");

//// Example 1 input
//lines = new string[] {
//    "1,0,1~1,2,1",
//    "0,0,2~2,0,2",
//    "0,2,3~2,2,3",
//    "0,0,4~0,2,4",
//    "2,0,5~2,2,5",
//    "0,1,6~2,1,6",
//    "1,1,8~1,1,9",
//};

List<Brick> bricks = new();

foreach (var line in lines)
{
    var arr = line.Split('~')
        .Select(s => 
            s.Split(',', StringSplitOptions.TrimEntries).ToPoint3D())
        .ToArray();
    bricks.Add(new Brick(arr[0], arr[1]));
    Console.WriteLine(line);
}

// Find dimensional limits
Point3D max3D = bricks.SelectMany(tup => new Point3D[] { tup.A, tup.B })
    .Aggregate(new Point3D(int.MinValue, int.MinValue, int.MinValue),
    (maxSoFar, next) => new Point3D(
        next.X > maxSoFar.X ? next.X : maxSoFar.X, 
        next.Y > maxSoFar.Y ? next.Y : maxSoFar.Y, 
        next.Z > maxSoFar.Z ? next.Z : maxSoFar.Z));

// Create empty grid
Brick?[,,] grid =
    new Brick[max3D.X + 1, max3D.Y + 1, max3D.Z + 1];
grid.Validate();

// Fill grid with bricks
foreach (Brick brick in bricks)
{
    grid.Add(brick);
    grid.Validate();
}

// Let all bricks fall
grid.InvokeGravity();
grid.Validate();
bricks = grid.Bricks();

// Now remove each brick to see if any others would fall
// Put the brick back after each test, regardless
List<Brick> nonStructural = new();
foreach (var b in bricks)
{
    grid.Disintegrate(b);
    grid.Validate();
    bool anyWillFall = false;
    foreach (var bb in grid.Bricks())
    {
        if (b.Equals(bb)) continue;
        if (grid.IsFloating(bb))
        {
            anyWillFall = true;
            break;
        }
    }
    if (!anyWillFall) nonStructural.Add(b);
    grid.Add(b);
    grid.Validate();
}

Console.WriteLine();
foreach (var b in grid.Bricks()) Console.WriteLine(b);

int ansPart1 = nonStructural.Count;

// Part 2 **************************

// Test-Disolve each brick, and let all others fall if as they will.
// Count number of bricks that move (aka "fall")
// Note that disolved brick should NOT be counted among moved bricks
int brickFall = 0;
var saveGrid = grid.DeepCopy();
var before = saveGrid.Bricks();
foreach (var testBrick in grid.Bricks())
{
    var during = grid.Bricks();

    grid.Disintegrate(testBrick);
    // let all bricks fall
    grid.InvokeGravity();
    // Count other "before" items which are missing "after" brick removal
    // Don't count the testBrick
    var after = grid.Bricks();
    after = grid.Bricks();
    Debug.Assert(after.Count == before.Count - 1);
    brickFall += before.Where(b => !after.Contains(b)).Count() - 1;
    grid = saveGrid.DeepCopy();
}

int ansPart2 = brickFall;
Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");
Debug.Assert(ansPart2 > 65084); // Prior wrong answer

Console.ReadKey();
