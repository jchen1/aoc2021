namespace aoc2021;

public class Day5
{
    private static List<((int x, int y) p1, (int x, int y) p2)> ReadInput()
    {
        return File.ReadAllLines("inputs/day5.txt").Select(line =>
        {
            var parts = line.Split(" -> ");
            var left = parts[0].Split(",").Select(int.Parse).ToList();
            var right = parts[1].Split(",").Select(int.Parse).ToList();

            if (left[0] < right[0] || left[0] == right[0] && left[1] < right[1])
            {
                return ((left[0], left[1]), (right[0], right[1]));
            }
            return ((right[0], right[1]), (left[0], left[1]));
        }).ToList();
    }

    private enum LineDirection
    {
        Horizontal,
        Vertical,
        DiagonalUp,
        DiagonalDown
    }

    private static LineDirection GetLineDirection(((int x, int y) p1, (int x, int y) p2) line)
    {
        if (line.p1.x == line.p2.x)
        {
            return LineDirection.Vertical;
        }
        if (line.p1.y == line.p2.y)
        {
            return LineDirection.Horizontal;
        }

        if (line.p1.y < line.p2.y)
        {
            return LineDirection.DiagonalDown;
        }

        return LineDirection.DiagonalUp;
    }

    public static void Part1()
    {
        var lines = ReadInput();
        var pts = new Dictionary<(int x, int y), int>();

        foreach (var line in lines)
        {
            switch (GetLineDirection(line))
            {
                case LineDirection.Horizontal:
                    for (var x = line.p1.x; x <= line.p2.x; x++)
                    {
                        pts[(x, line.p1.y)] = pts.GetValueOrDefault((x, line.p1.y), 0) + 1;
                    }
                    break;
                case LineDirection.Vertical:
                    for (var y = line.p1.y; y <= line.p2.y; y++)
                    {
                        pts[(line.p1.x, y)] = pts.GetValueOrDefault((line.p1.x, y), 0) + 1;
                    }
                    break;
                default: continue;
            }
        }

        Console.WriteLine(pts.Where((pair => pair.Value > 1)).Count());
    }

    public static void Part2()
    {
        var lines = ReadInput();
        var pts = new Dictionary<(int x, int y), int>();

        foreach (var line in lines)
        {
            switch (GetLineDirection(line))
            {
                case LineDirection.Horizontal:
                    for (var x = line.p1.x; x <= line.p2.x; x++)
                    {
                        pts[(x, line.p1.y)] = pts.GetValueOrDefault((x, line.p1.y), 0) + 1;
                    }
                    break;
                case LineDirection.Vertical:
                    for (var y = line.p1.y; y <= line.p2.y; y++)
                    {
                        pts[(line.p1.x, y)] = pts.GetValueOrDefault((line.p1.x, y), 0) + 1;
                    }
                    break;
                case LineDirection.DiagonalDown:
                    for (var offset = 0; offset <= line.p2.x - line.p1.x; offset++)
                    {
                        pts[(line.p1.x + offset, line.p1.y + offset)] = pts.GetValueOrDefault((line.p1.x + offset, line.p1.y + offset), 0) + 1;
                    }
                    break;
                case LineDirection.DiagonalUp:
                    for (var offset = 0; offset <= line.p2.x - line.p1.x; offset++)
                    {
                        pts[(line.p1.x + offset, line.p1.y - offset)] = pts.GetValueOrDefault((line.p1.x + offset, line.p1.y - offset), 0) + 1;
                    }
                    break;
            }
        }

        Console.WriteLine(pts.Where((pair => pair.Value > 1)).Count());
    }

}
