namespace aoc2021;

public static class Day17
{
    private static readonly int MinX, MinY, MaxX, MaxY;

    static Day17()
    {
        var input = File.ReadAllText("inputs/day17.txt")["target area: ".Length..].Split(", ");
        var xInput = input[0]["x=".Length..].Split("..");
        var yInput = input[1]["y=".Length..].Split("..");
        MinX = int.Parse(xInput[0]);
        MaxX = int.Parse(xInput[1]);
        MinY = int.Parse(yInput[0]);
        MaxY = int.Parse(yInput[1]);
    }

    private static (int x, int y) Step((int x, int y) position, int xVelocity, int yVelocity)
    {
        return (position.x + xVelocity, position.y + yVelocity);
    }

    private static (bool hitsTarget, int maxYReached) CheckTarget(int xVelocity, int yVelocity)
    {
        (int x, int y) position = (0, 0);
        var maxYReached = 0;

        for (var step = 0; step < 1000; step++)
        {
            position = Step(position, xVelocity, yVelocity);
            maxYReached = Math.Max(position.y, maxYReached);

            xVelocity = Math.Max(0, xVelocity - 1);
            yVelocity--;

            if (position.x >= MinX && position.x <= MaxX && position.y >= MinY && position.y <= MaxY)
            {
                return (true, maxYReached);
            }
        }

        return (false, maxYReached);
    }

    public static void Part1()
    {
        int maxYReached = 0;
        for (var xVelocity = 0; xVelocity < 100; xVelocity++)
        {
            for (var yVelocity = 0; yVelocity < 100; yVelocity++)
            {
                var (hitsTarget, maxYReachedInStep) = CheckTarget(xVelocity, yVelocity);
                if (hitsTarget)
                {
                    maxYReached = Math.Max(maxYReached, maxYReachedInStep);
                }
            }
        }

        Console.WriteLine(maxYReached);
    }

    public static void Part2()
    {
        var numHits = 0;
        for (var xVelocity = 0; xVelocity <= MaxX; xVelocity++)
        {
            for (var yVelocity = -100; yVelocity < 100; yVelocity++)
            {
                var (hitsTarget, _) = CheckTarget(xVelocity, yVelocity);
                if (hitsTarget)
                {
                    numHits++;
                }
            }
        }

        Console.WriteLine(numHits);
    }
}
