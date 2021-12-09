namespace aoc2021;

public class Day9
{

    private static readonly int[][] Input = File.ReadAllLines("inputs/day9.txt").Where(x => !string.IsNullOrEmpty(x)).Select(line => line.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

    public static void Part1()
    {
        var ret = 0;
        for (var i = 0; i < Input.Length; i++)
        {
            for (var j = 0; j < Input[i].Length; j++)
            {
                var num = Input[i][j];
                if ((i == 0 || Input[i - 1][j] > num) &&
                    (i == Input.Length - 1 || Input[i + 1][j] > num) &&
                    (j == 0 || Input[i][j - 1] > num) &&
                    (j == Input[i].Length - 1 || Input[i][j + 1] > num))
                {
                    ret += 1 + num;
                }
            }
        }

        Console.WriteLine(ret);
    }

    private static List<(int i, int j)> GetNeighbors(int i, int j)
    {
        var ret = new List<(int, int)>();

        if (i != 0)
        {
            ret.Add((i - 1, j));
        }
        if (i != Input.Length - 1)
        {
            ret.Add((i + 1, j));
        }
        if (j != 0)
        {
            ret.Add((i, j - 1));
        }
        if (j != Input[i].Length - 1)
        {
            ret.Add((i, j + 1));
        }

        return ret;
    }

    public static void Part2()
    {
        var basinId = 1;
        var basins = new int[Input.Length, Input[0].Length];
        var basinSizes = new PriorityQueue<int, int>();

        for (var i = 0; i < Input.Length; i++)
        {
            for (var j = 0; j < Input[i].Length; j++)
            {

                if (basins[i, j] != 0 || Input[i][j] == 9)
                {
                    continue;
                }

                var num = Input[i][j];
                if ((i == 0 || Input[i - 1][j] > num) &&
                    (i == Input.Length - 1 || Input[i + 1][j] > num) &&
                    (j == 0 || Input[i][j - 1] > num) &&
                    (j == Input[i].Length - 1 || Input[i][j + 1] > num))
                {
                    var basinSize = 0;
                    var queue = new Queue<(int, int)>();

                    queue.Enqueue((i, j));
                    while (queue.Count > 0)
                    {
                        var (x, y) = queue.Dequeue();
                        if (basins[x, y] != 0 || Input[x][y] == 9)
                        {
                            continue;
                        }

                        basins[x, y] = basinId;
                        basinSize++;

                        foreach (var neighbor in GetNeighbors(x, y))
                        {
                            queue.Enqueue(neighbor);
                        }
                    }

                    basinSizes.Enqueue(basinSize, -1 * basinSize);
                    basinId++;
                }
            }
        }

        var ret = 1;
        for (var i = 0; i < 3; i++)
        {
            var basinSize = basinSizes.Dequeue();
            ret *= basinSize;
        }
        Console.WriteLine(ret);
    }
}
