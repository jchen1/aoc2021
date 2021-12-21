using System.Collections;

namespace aoc2021;

public static class Day20
{
    private readonly record struct Point(int X, int Y)
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    };

    private static readonly BitArray Algorithm = new(512);
    private static readonly HashSet<Point> Input;
    private static readonly int MinX, MaxX, MinY, MaxY;

    static Day20()
    {
        var splitInput = File.ReadAllText("inputs/day20.txt").Split("\n\n");
        var _ = Enumerable.Range(0, 512).Select(i => Algorithm[i] = splitInput[0][i] == '#').ToList();

        Input = splitInput[1].Split('\n').Where(s => !string.IsNullOrEmpty(s))
            .SelectMany((line, y) => line.Select((c, x) => (c, new Point(x, y))).Where(t => t.c == '#'))
            .Select(t => t.Item2)
            .ToHashSet();

        MinX = 0;
        MinY = 0;
        MaxX = Input.MaxBy(p => p.X).X + 1;
        MaxY = Input.MaxBy(p => p.Y).Y + 1;
    }

    private static bool EnhancedPixelAt(IReadOnlySet<Point> input, BitArray algorithm, Point p)
    {
        var pixel =
            ((input.Contains(p + new Point(-1, -1)) ? 1 : 0) << 8) +
            ((input.Contains(p + new Point(0, -1)) ? 1 : 0)  << 7) +
            ((input.Contains(p + new Point(1, -1)) ? 1 : 0)  << 6) +
            ((input.Contains(p + new Point(-1, 0)) ? 1 : 0)  << 5) +
            ((input.Contains(p + new Point(0, 0)) ? 1 : 0)   << 4) +
            ((input.Contains(p + new Point(1, 0)) ? 1 : 0)   << 3) +
            ((input.Contains(p + new Point(-1, 1)) ? 1 : 0)  << 2) +
            ((input.Contains(p + new Point(0, 1)) ? 1 : 0)   << 1) +
            ((input.Contains(p + new Point(1, 1)) ? 1 : 0)   << 0);

        return algorithm[pixel];
    }

    private static HashSet<Point> AddEdges(IReadOnlySet<Point> input, int step)
    {
        var invert = step % 2 == 0;

        var output = new HashSet<Point>();
        for (var x = MinX - step - 1; x < MaxX + step + 1; x++)
        {
            for (var y = MinY - step - 1; y < MaxY + step + 1; y++)
            {
                var pt = new Point(x, y);
                if (input.Contains(pt) ||
                    (invert && (x == MinX - step - 1 || x == MaxX + step || y == MinY - step - 1 || y == MaxY + step)))
                {
                    output.Add(pt);
                }
            }
        }

        return output;
    }

    private static HashSet<Point> Enhance(IReadOnlySet<Point> input, int step)
    {
        var withEdges = AddEdges(input, step);
        var output = new HashSet<Point>();

        for (var x = MinX - step; x < MaxX + step; x++)
        {
            for (var y = MinY - step; y < MaxY + step; y++)
            {
                var p = new Point(x, y);
                if (EnhancedPixelAt(withEdges, Algorithm, p))
                {
                    output.Add(p);
                }
            }
        }

        return output;
    }

    private static void PrintGrid(IReadOnlySet<Point> grid, int step)
    {
        Console.WriteLine("grid step {0}", step);
        for (var y = grid.MinBy(p => p.Y).Y; y <= grid.MaxBy(p => p.Y).Y; y++)
        {
            for (var x = grid.MinBy(p => p.X).X; x <= grid.MaxBy(p => p.X).X; x++)
            {
                Console.Write(grid.Contains(new(x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n\n");
    }

    public static void Part1()
    {
        var output = Input;
        for (var i = 1; i <= 2; i++)
        {
            output = Enhance(output, i);
            Console.WriteLine("\n\ni={0}", i);
            PrintGrid(output, i);
        }

        var trimmed = output.Count(pt => pt.X >= MinX - 2 && pt.X <= MaxX + 2 && pt.Y >= MinY - 2 && pt.Y <= MaxY + 2);

        Console.WriteLine("{0} {1}", trimmed, output.Count());

        // 10179 high
        // 5136 low
        // 5247 (wrong)
        // 5157 (wrong)
        // 5336??
        // 5301, 19492
    }

    public static void Part2()
    {
        var output = Input;
        for (var i = 1; i <= 50; i++)
        {
            output = Enhance(output, i);
        }
        
        Console.WriteLine(output.Count);
    }
}
