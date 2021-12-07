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
        var mean = Crabs.Average();

        var floorDiff = Crabs.Select(c =>
        {
            var d = Math.Abs(c - Math.Floor(mean));
            return (d * (d + 1)) / 2;
        }).Sum();

        var ceilDiff = Crabs.Select(c =>
        {
            var d = Math.Abs(c - Math.Ceiling(mean));
            return (d * (d + 1)) / 2;
        }).Sum();

        Console.WriteLine(Math.Min(floorDiff, ceilDiff));
    }
}
