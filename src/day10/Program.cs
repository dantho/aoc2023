// https://adventofcode.com/2023/day/10
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Net.Http.Headers;

int aocPart = 1;
string rawInput = System.IO.File.ReadAllText(@"C:\Users\DanTh\github\aoc2023\inputs\day10.txt");

//// Example 1 input
//rawInput = @"..F7.
//.FJ|.
//SJ.L7
//|F--J
//LJ...";

//rawInput = @"7-F7-
//.FJ|7
//SJLL7
//|F--J
//LJ.LJ";

//rawInput = @"...........
//.S-------7.
//.|F-----7|.
//.||.....||.
//.||.....||.
//.|L-7.F-J|.
//.|..|.|..|.
//.L--J.L--J.
//...........";

//rawInput = @".F----7F7F7F7F-7....
//.|F--7||||||||FJ....
//.||.FJ||||||||L7....
//FJL7L7LJLJ||LJ.L-7..
//L--J.L7...LJS7F-7L7.
//....F-J..F7FJ|L7L7L7
//....L7.F7||L7|.L7L7|
//.....|FJLJ|FJ|F7|.LJ
//....FJL-7.||.||||...
//....L---J.LJ.LJLJ...";

Dir N = new Dir("North");
Dir S = new Dir("South");
Dir E = new Dir("East");
Dir W = new Dir("West");
Debug.Assert(N.Equals(new Dir("North")));
Debug.Assert(E.Equals(new Dir("East")));
Debug.Assert(S.Equals(new Dir("South")));
Debug.Assert(W.Equals(new Dir("West")));
Debug.Assert(!N.Equals(new Dir("East")));
Debug.Assert(!E.Equals(new Dir("South")));
Debug.Assert(!S.Equals(new Dir("West")));
Debug.Assert(!W.Equals(new Dir("North")));

string boxInput = string.Concat(rawInput.Select(c =>
{
    switch (c)
    {
        case 'F': return '┏';
        case 'L': return '┗';
        case '7': return '┓';
        case 'J': return '┛';
        case '|': return '┃';
        case '-': return '━';
        default: return c;
    }
}));

System.IO.File.WriteAllText(@"C:\Users\DanTh\github\aoc2023\out\day10_out.txt", boxInput);

string[] lines = boxInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

char[][] grid = lines.Select(s => s.ToArray()).ToArray();

// Find S[tart]
Point? maybeStart = null;
for (int x = 0; x < grid[0].Length; x++)
{
    for (int y = 0; y < grid.Length; y++)
    {
        if ('S' == grid[y][x])
        {
            maybeStart = new Point(x, y);
            break;
        }
    }
}
Point start = maybeStart ?? throw new Exception("'S' not found!");
Console.WriteLine($"Start is ({start})");

// Find _any_ connected direction (worst case should be 3 tries)
Dir? startTravelDir = null;
int maxY = grid.Length - 1;
int maxX = grid[0].Length - 1;
foreach (Dir dir in new[] { N, E, S, W })
{
    Point tryHere = dir.Step(start);
    Dir backDir = dir.Opposite();
    if (!PointIsValid(tryHere))
        continue;
    List<Dir> candidateConns = Dir.GetValidDirs(grid[tryHere.Y][tryHere.X]);
    if (!candidateConns.Contains(backDir))
        continue;
    startTravelDir = dir;
        break;
}

// Now search for the end of the loop (back to start)
Dir travelDir = startTravelDir;
Point searchPoint = travelDir.Step(start);

HashSet<Point> loop = new(); // For Part 2, below.
loop.Add(start); // Start is in the loop

int stepcount = 1;
while (searchPoint != start)
{
    loop.Add(searchPoint); // See Part 2, below
    Dir backDir = travelDir.Opposite();
    char mapChar = grid[searchPoint.Y][searchPoint.X];
    List<Dir> options = Dir.GetValidDirs(mapChar);
    if (!options.Contains(backDir))
        throw new Exception("Pipes are not connected");
    travelDir = options.Where(d => d != backDir).First();
    searchPoint = travelDir.Step(searchPoint);
    if (!PointIsValid(searchPoint))
        throw new Exception("Stepped off the map!");
    stepcount++;
}

Dir secondStartTravelDir = travelDir.Opposite();
int ansPart1 = stepcount/2;

// ********* Part 2  ****************

// find all grid locations "inside the loop"
// The definition is complicated, algorithmically speaking, but
// works out to having an odd number of "loop" crossings in any (and all)
// cardinal directions of travel to an edge of the map. We'll go right.
// Even a "crossing" is hard to define...
// Intersecting a section of loop that enters one way, then exits
// back the way it came... can be ignored.  That's not a "crossing".

// First, let's litterally patch/remove the S from the map.
char pipeUnderStartChar;
foreach (char c in "┏┓┗┛┃━")
{
    List<Dir> dirs = Dir.GetValidDirs(c);
    Debug.Assert(dirs.Count() == 2);
    if (dirs.Contains(startTravelDir)
     && dirs.Contains(secondStartTravelDir))
    {
        grid[start.Y][start.X] = c;
        break;
    }
}
int insideTheLoop = 0;
for (int y = 0; y <= maxY; y++)
{
    for (int x = 0; x <= maxX; x++)
    {
        if (loop.Contains(new Point(x, y)))
            continue;
        int loop_crossings = 0;
        Dir? enteredFromNorthSouth = null;
        // safely going one BEYOND the map edge here:
        for (int xx = x + 1; xx <= maxX+1; xx++)
        {
            if (loop.Contains(new Point(xx, y)))
            {
                List<Dir> pipeConns = Dir.GetValidDirs(grid[y][xx]);
                if (enteredFromNorthSouth is not null)
                {
                    if (pipeConns.Contains(enteredFromNorthSouth) || pipeConns.Contains(enteredFromNorthSouth.Opposite()))
                    {
                        // We're technically stepping off the pipe section we were on
                        // Even if the very next grid cell contains more pipe!
                        // Confusing!
                        // Now, depending if this pipe left back the way it came,
                        // Or out the other way...
                        if (pipeConns.Contains(enteredFromNorthSouth.Opposite()))
                        {
                            loop_crossings++;
                        }
                        enteredFromNorthSouth = null;
                    }
                }
                else // entering loop section from non-loop (or interstitial)
                {
                    List<Dir> ns = pipeConns.Where(d => d == N || d == S).ToList();
                    if (ns.Count == 2)
                        // stepped right over the loop (a vertical section!)
                        loop_crossings++;
                    else if (ns.Count == 1)
                        // stepped on, but not over, the loop
                        enteredFromNorthSouth = ns.First();
                    else
                        Debug.Assert(false, "This can't happen");
                }
            }
            else
                Debug.Assert(enteredFromNorthSouth is null);
        }
        if ((loop_crossings & 1) == 1)
            insideTheLoop++;
    }
}
int ansPart2 = insideTheLoop;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End
bool PointIsValid(Point tryHere) {
    return tryHere.X >= 0 && tryHere.X <= maxX
        && tryHere.Y >= 0 && tryHere.Y <= maxY;
}

public class Dir : IEquatable<Dir>
{
    const int UP = -1; // Inverted Y
    const int DOWN = -UP;
    public int Xd { get; init; }
    public int Yd { get; init; }

    public static Dir N = new("North");
    public static Dir S = new("South");
    public static Dir E = new("East");
    public static Dir W = new("West");

    private Dir(int xd, int yd)
    {
        Xd = xd;
        Yd = yd;
        Validate();
    }
    public Dir(string compassDirection)
    {
        string cardinal = compassDirection.ToLower().Trim();
        if (cardinal == "north")
            (Xd, Yd) = (0, UP);
        else if (cardinal == "south")
            (Xd, Yd) = (0, DOWN);
        else if (cardinal == "east")
            (Xd, Yd) = (1, 0);
        else if (cardinal == "west")
            (Xd, Yd) = (-1, 0);
        else
            throw new Exception($"Illegal compass direction {compassDirection}");

        Validate();
    }
    // ToDo: Refactor this puzzle-specific code elsewhere
    static public List<Dir> GetValidDirs(char gridChar)
    {
        if (!charToDirsDict.ContainsKey(gridChar))
            throw new Exception($"Illegal map character '{gridChar}'");
        return charToDirsDict[gridChar];
    }
    public Point Step(Point from)
    {
        return new Point(from.X+Xd, from.Y+Yd);
    }
    public Dir Opposite()
    {
        return new Dir(-Xd, -Yd);
    }

    // this is first one '=='
    public static bool operator ==(Dir obj1, Dir obj2)
    {
        return (obj1.Xd == obj2.Xd
             && obj1.Yd == obj2.Yd);
    }

    // this is second one '!='
    public static bool operator !=(Dir obj1, Dir obj2)
    {
        return !(obj1.Xd == obj2.Xd
              && obj1.Yd == obj2.Yd);
    }

    // this is third one 'Equals'
    public bool Equals(Dir? obj)
    {
        if (obj is null) return false;
        return (Xd == obj.Xd
             && Yd == obj.Yd);
    }
    public override bool Equals(object? obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        // TODO: write your implementation of Equals() here
        return (Xd == (obj as Dir).Xd
             && Yd == (obj as Dir).Yd);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + Xd.GetHashCode();
            hash = hash * 23 + Yd.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (Xd == 0 && Yd == UP)
            return "North";
        else if (Xd == 0 && Yd == DOWN)
            return "South";
        else if (Xd == 1 && Yd == 0)
            return "East";
        else if (Xd == -1 && Yd == 0)
            return "West";
        else
            throw new Exception("Dir defies [validated] logic!");
    }
    private void Validate()
    {
        if (Xd == 0)
        {
            Debug.Assert(Yd == 1 || Yd == -1);
        }
        else
        {
            Debug.Assert(Yd == 0);
            Debug.Assert(Xd == 1 || Xd == -1);
        }
    }

    // ToDo: Refactor this puzzle-specific code elsewhere
    // https://en.wikipedia.org/wiki/Box-drawing_character
    static readonly string boxChars = "┓┏┛┗┃━";
    static readonly Dictionary<char, List<Dir>> charToDirsDict = new()
    {
        {'┓', new() {W, S}},
        {'┏', new() {E, S}},
        {'┛', new() {W, N}},
        {'┗', new() {E, N}},
        {'┃', new() {N, S}},
        {'━', new() {E, W}},
        {'.', new() {}},
        {'S', new() {N, E, S, W}},
    };
}