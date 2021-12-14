namespace aoc2021;

public static class Day11
{
    private static readonly int InputSize = 10;

    private static int[,] GetInput()
    {
        var ret = new int[InputSize, InputSize];
        var input = File.ReadAllLines("inputs/day11.txt")
            .Select(line => line.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

        for (var i = 0; i < InputSize; i++)
        {
            for (var j = 0; j < InputSize; j++)
            {
                ret[i, j] = input[i][j];
            }
        }

        return ret;
    }

    private static List<(int i, int j)> GetNeighbors(int x, int y)
    {
        var ret = new List<(int i, int j)>();

        for (var i = x - 1; i <= x + 1; i++)
        {
            for (var j = y - 1; j <= y + 1; j++)
            {
                if (i == x && j == y) continue;
                if (i < 0 || j < 0) continue;
                if (i >= InputSize || j >= InputSize) continue;
                ret.Add((i, j));
            }
        }

        return ret;
    }

    private static void Increment(int[,] board)
    {
        for (var i = 0; i < InputSize; i++)
        {
            for (var j = 0; j < InputSize; j++)
            {
                board[i, j]++;
            }
        }
    }

    private static int Step(int[,] board)
    {
        var flashes = 0;

        Increment(board);
        while (board.Cast<int>().Any(x => x > 9))
        {
            for (var i = 0; i < InputSize; i++)
            {
                for (var j = 0; j < InputSize; j++)
                {
                    if (board[i, j] > 9)
                    {
                        flashes++;
                        board[i, j] = 0;
                        foreach (var neighbor in GetNeighbors(i, j))
                        {
                            if (board[neighbor.i, neighbor.j] != 0)
                            {
                                board[neighbor.i, neighbor.j]++;
                            }
                        }
                    }
                }
            }        }

        return flashes;
    }

    public static void Part1()
    {
        var board = GetInput();
        var flashes = 0;
        for (var i = 0; i < 100; i++)
        {
            flashes += Step(board);
        }

        Console.WriteLine(flashes);
    }

    public static void Part2()
    {
        var board = GetInput();
        var step = 0;
        while (board.Cast<int>().Any(x => x != 0))
        {
            Step(board);
            step++;
        }

        Console.WriteLine(step);
    }
}
