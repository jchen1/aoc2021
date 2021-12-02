namespace aoc2021;

public class Day1
{
    private static List<int> ReadInput()
    {
        return File.ReadAllLines("inputs/day1.txt").Select(int.Parse).ToList();
    }
    public static void Part1()
    {
        var numbers = ReadInput();
        var numIncreased = numbers.Zip(numbers.Skip(1)).Where((n => n.First < n.Second)).Count();

        Console.WriteLine(numIncreased);
    }

    public static void Part2()
    {
        var numbers = ReadInput();

        var rollingSums = numbers.Zip(numbers.Skip(1), numbers.Skip(2))
            .Select(tuple => tuple.First + tuple.Second + tuple.Third).ToList();

        var numIncreased = rollingSums.Zip(rollingSums.Skip(1)).Where((n => n.First < n.Second)).Count();

        Console.WriteLine(numIncreased);
    }
}
