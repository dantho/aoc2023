// https://adventofcode.com/2023/day/22
using AoCDay22;
using System.Diagnostics;
public static class MyExtensions
{
    public static Point3D ToPoint3D(this IEnumerable<int> input)
    {
        int[] inXYZ = input.ToArray();
        if (inXYZ.Length != 3)
            throw new Exception($"ToPoint3D expects exactly 3 items. Got {inXYZ.Length}.");
        return new Point3D(inXYZ[0], inXYZ[1], inXYZ[2]);
    }
    public static Point3D ToPoint3D(this IEnumerable<string> input)
    {
        int[] inXYZ = input.Select(s => int.Parse(s)).ToArray();
        if (inXYZ.Length != 3)
            throw new Exception($"ToPoint3D expects exactly 3 items. Got {inXYZ.Length}.");
        return new Point3D(inXYZ[0], inXYZ[1], inXYZ[2]);
    }
    public static List<Brick> Bricks(this Brick?[,,] grid)
    {
        HashSet<Brick> bricksInGrid = new();
        foreach (var b in grid)
        {
            if (b is not null)
                bricksInGrid.Add((Brick)b);
        }
        var orderedBricksInGrid = bricksInGrid.ToList();
        orderedBricksInGrid.Sort();
        return orderedBricksInGrid;
    }

    public static Brick?[,,] DeepCopy(this Brick?[,,] grid)
    {
        var newGrid = (Brick?[,,])grid.Clone(); // Shallow copy
        // Convert shallow reference sharing into deep value copy
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
            for (int y = 0; y <= grid.GetUpperBound(1); y++)
                for (int z = 0; z <= grid.GetUpperBound(2); z++)
                    if (newGrid[x, y, z] is not null)
                        newGrid[x, y, z] = newGrid[x, y, z].Copy();
        return newGrid;
    }
    public static void Add(this Brick?[,,] grid, Brick brick)
    {
        brick.Enumerate().ForEach(p => grid[p.X,p.Y,p.Z] = brick);
    }
    public static void Disintegrate(this Brick?[,,] grid, Brick brick)
    {
        int bricksBefore = grid.Bricks().Count();
        brick.Enumerate().ForEach(p =>
        {
            Brick? foundBrick = grid.Point(p) ?? throw new Exception($"Removing brick {brick} from already null point at {p}");
            if (foundBrick.Equals(brick))
            {
                grid[p.X, p.Y, p.Z] = null;
            }
            else
                throw new Exception($"Expecting to remove {brick} at {p} but found {foundBrick}");
        });
        int bricksAfter = grid.Bricks().Count();
        Debug.Assert(bricksBefore == 1 + bricksAfter, "brick removal gone wrong");
    }
    public static ref Brick? Point(this Brick?[,,] grid, Point3D point)
    {
        return ref grid[point.X, point.Y, point.Z];
    }
    public static bool IsFloating(this Brick?[,,] grid, Brick brick)
    {
        const int Floor = 1;
        return brick
            .Enumerate()
            .Aggregate(true, (isFloating, p) =>
                isFloating &&
                p.Z > Floor &&
                (grid[p.X, p.Y, p.Z - 1] is null ||
                 grid[p.X, p.Y, p.Z - 1].Equals(brick)));
    }
    public static void Drop(this Brick?[,,] grid, Brick brick)
    {
        const int Floor = 1;
        if (brick.A.Z <= Floor || brick.B.Z <= Floor)
            throw new Exception($"Cannot drop a brick already on the Floor (Z == {Floor})");
        Brick brickBelow = new Brick
            (new Point3D(brick.A.X, brick.A.Y, brick.A.Z - 1),
             new Point3D(brick.B.X, brick.B.Y, brick.B.Z - 1));

        foreach (Point3D p in brick.Enumerate())
        {
            var cell = grid[p.X, p.Y, p.Z];
            Debug.Assert(cell is not null && cell.Equals(brick));
            grid[p.X,p.Y,p.Z] = null;
        }

        foreach (Point3D p in brickBelow.Enumerate())
        {
            Debug.Assert(grid[p.X,p.Y,p.Z] is null);
            grid[p.X,p.Y,p.Z] = brickBelow;
        }
    }
    public static void Validate(this Brick?[,,] grid)
    {
        ////Brick brick1 = new Brick(new Point3D(0, 0, 0), new Point3D(0, 0, 0));
        ////Brick brick2 = new Brick(new Point3D(0, 0, 0), new Point3D(0, 0, 0));
        ////Debug.Assert(brick1.EqualValues(brick2));
        ////Debug.Assert(brick1.Equals(brick2));
        //var filledPoints = grid.Bricks().SelectMany(b => b.Enumerate()).ToList();
        //var emptyPoints = new Brick
        //    (new Point3D(0, 0, 0),
        //     new Point3D(grid.GetUpperBound(0),
        //                 grid.GetUpperBound(1),
        //                 grid.GetUpperBound(2)))
        //    .Enumerate().Where(p => !filledPoints.Contains(p)).ToList();
        //List<Point3D> problems = new();
        //foreach (var brick in grid.Bricks())
        //{
        //    foreach (var p in brick.Enumerate())
        //    {
        //        var cell = grid[p.X, p.Y, p.Z];
        //        if (cell is null || !cell.Equals(brick))
        //            problems.Add(p);
        //    }
        //}
        //// Check cells that should be null are null
        //foreach (var p in emptyPoints)
        //{
        //    if (grid[p.X,p.Y,p.Z] is not null)
        //        problems.Add(p);
        //}
        //if (problems.Count > 0)
        //{
        //    string? firstProb = grid.Point(problems[0]) is null ? "null entry" : grid.Point(problems[0]).ToString();
        //    throw new Exception($"Grid does NOT validate. Found {problems.Count} problems.\n"
        //    + $"First is: At point {problems[0]} found {firstProb}");
        //}
    }
    public static void InvokeGravity(this Brick?[,,] grid)
    {
        bool anyBrickFell;
        int brickCount = grid.Bricks().Count;
        do
        {
            anyBrickFell = false;
            var bricks = grid.Bricks();
            foreach (var brick in bricks)
            {
                if (grid.IsFloating(brick))
                {
                    grid.Drop(brick);
                    grid.Validate();
                    Debug.Assert(grid.Bricks().Count == brickCount);
                    anyBrickFell = true;
                }
            }
        } while (anyBrickFell);
    }
}