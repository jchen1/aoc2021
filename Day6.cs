namespace aoc2021;

public static class Day6
{
    private static List<int> ReadInput()
    {
        return File.ReadAllText("inputs/day6.txt").Split(",").Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
    }

    private static ulong LanternFishReproduction(int days)
    {
        var fish = new ulong[9];
        foreach (var input in ReadInput())
        {
            fish[input]++;
        }

        for (var day = 0; day < days; day++)
        {
            var babyFish = fish[0];

            for (var i = 0; i < 8; i++)
            {
                fish[i] = fish[i + 1];
            }

            fish[8] = babyFish;
            fish[6] += babyFish;
        }

        return fish.ToList().Aggregate((x, y) => x + y);
    }

    public static void Part1()
    {
        Console.WriteLine(LanternFishReproduction(80));
    }

    public static void Part2()
    {
        Console.WriteLine(LanternFishReproduction(256));
    }
}
