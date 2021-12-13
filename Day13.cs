namespace aoc2021;

public class Day13
{
    private enum Direction
    {
        X,
        Y
    }

    private static (HashSet<(int x, int y)> graph, List<(Direction direction, int position)> folds) GetInput()
    {
        var lines = File.ReadAllText("inputs/day13.txt").Split("\n\n");
        var graph = lines[0].Split("\n").Where(s => !string.IsNullOrEmpty(s))
            .Select(line => (int.Parse(line.Split(",")[0]), int.Parse(line.Split(",")[1]))).ToHashSet();

        var folds = lines[1].Split("\n").Where(s => s.StartsWith("fold along "))
            .Select(line => line.Substring("fold along ".Length))
            .Select(command => command.Split("="))
            .Select(split => (split[0] == "x" ? Direction.X : Direction.Y, int.Parse(split[1]))).ToList();

        return (graph, folds);
    }

    private static HashSet<(int x, int y)> Fold(HashSet<(int x, int y)> graph, (Direction direction, int position) fold)
    {
        var newGraph = new HashSet<(int x, int y)>();

        switch (fold.direction)
        {
            case Direction.X:
                foreach (var point in graph)
                {
                    newGraph.Add(point.x < fold.position
                        ? point
                        : (fold.position - (point.x - fold.position), point.y));
                }
                break;
            case Direction.Y:
                foreach (var point in graph)
                {
                    newGraph.Add(point.y < fold.position
                        ? point
                        : (point.x, fold.position - (point.y - fold.position)));
                }
                break;
        }

        return newGraph;
    }

    public static void Part1()
    {
        var (graph, folds) = GetInput();
        var newGraph = Fold(graph, folds[0]);

        Console.WriteLine(newGraph.Count);
    }

    public static void Part2()
    {
        var (graph, folds) = GetInput();
        var newGraph = Fold(graph, folds[0]);

        foreach (var fold in folds.Skip(1))
        {
            newGraph = Fold(newGraph, fold);
        }

        var maxX = newGraph.MaxBy(pt => pt.x).x;
        var maxY = newGraph.MaxBy(pt => pt.y).y;

        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                Console.Write(newGraph.Contains((x, y)) ? "█" : " ");
            }
            Console.WriteLine();
        }
    }
}
