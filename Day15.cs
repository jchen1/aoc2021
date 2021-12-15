namespace aoc2021;

public static class Day15
{
    private static readonly int[,] Input;

    static Day15()
    {
        var lines = File.ReadAllLines("inputs/day15.txt");
        Input = new int[lines.Length, lines[0].Length];
        for (var i = 0; i < lines.Length; i++)
        {
            for (var j = 0; j < lines[i].Length; j++)
            {
                Input[i, j] = int.Parse(lines[i][j].ToString());
            }
        }
    }

    private static List<(int x, int y)> GetNeighbors(int[,] input, (int x, int y) point)
    {
        var (x, y) = point;
        var ret = new List<(int x, int y)>();
        if (x > 0)
        {
            ret.Add((x - 1, y));
        }
        if (x < input.GetLength(0) - 1)
        {
            ret.Add((x + 1, y));
        }
        if (y > 0)
        {
            ret.Add((x, y - 1));
        }
        if (y < input.GetLength(1) - 1)
        {
            ret.Add((x, y + 1));
        }
        return ret;
    }

    private static int MinDistance(int[,] input)
    {
        var distances = new Dictionary<(int x, int y), int>() {{ (0, 0), 0 }};
        var queue = new PriorityQueue<(int x, int y), int>();

        queue.Enqueue((0, 0), 0);

        while (queue.Count > 0)
        {
            queue.TryDequeue(out var pt, out var dist);
            if (pt == (input.GetLength(0) - 1, input.GetLength(1) - 1))
            {
                return dist;
            }

            foreach (var neighbor in GetNeighbors(input, pt))
            {
                var newDist = dist + input[neighbor.x, neighbor.y];
                if (!distances.ContainsKey(neighbor) || newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    queue.Enqueue(neighbor, distances[neighbor]);
                }
            }
        }

        return -1;
    }

    public static void Part1()
    {
        Console.WriteLine(MinDistance(Input));
    }

    private static void PopulateTile(int[,] output, int tileX, int tileY)
    {
        for (var x = 0; x < Input.GetLength(0); x++)
        {
            for (var y = 0; y < Input.GetLength(1); y++)
            {
                var source = Input[x, y];
                var outputValue = 1 + ((8 + source + tileX + tileY) % 9);

                output[tileX * Input.GetLength(1) + x, tileY * Input.GetLength(0) + y] = outputValue;
            }
        }
    }

    public static void Part2()
    {
        var bigInput = new int[Input.GetLength(0) * 5, Input.GetLength(1) * 5];
        for (var tileX = 0; tileX < 5; tileX++)
        {
            for (var tileY = 0; tileY < 5; tileY++)
            {
                PopulateTile(bigInput, tileX, tileY);
            }
        }

        Console.WriteLine(MinDistance(bigInput));
    }
}
