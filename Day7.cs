namespace aoc2021;

public static class Day7
{
    private static readonly List<int> Crabs = File.ReadAllText("inputs/day7.txt").Split(",").Where(s => !string.IsNullOrEmpty(s))
        .Select(int.Parse).ToList();

    public static void Part1()
    {
        var median = Crabs.OrderBy(i => i).Skip(Crabs.Count / 2).First();

        var diff = Crabs.Select(c => Math.Abs(c - median)).Sum();
        Console.WriteLine(diff);
    }

    public static void Part2()
    {
        var mean = (int) Math.Floor(Crabs.Average());

        var diff = Crabs.Select(c =>
        {
            var d = Math.Abs(c - mean);
            return (d * (d + 1)) / 2;
        }).Sum();

        Console.WriteLine(diff);
    }
}
