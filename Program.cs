
void Day1Part1()
{
    using var sr = new StreamReader("inputs/day1.txt");

    var numbers = sr.ReadToEnd().Split('\n').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
    var numIncreased = numbers.Zip(numbers.Skip(1)).Where((n => n.First < n.Second)).Count();

    Console.WriteLine(numIncreased);
}

void Day1Part2()
{
    using var sr = new StreamReader("inputs/day1.txt");

    var numbers = sr.ReadToEnd().Split('\n').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
    var rollingSums = numbers.Zip(numbers.Skip(1), numbers.Skip(2))
        .Select(tuple => tuple.First + tuple.Second + tuple.Third).ToList();

    var numIncreased = rollingSums.Zip(rollingSums.Skip(1)).Where((n => n.First < n.Second)).Count();

    Console.WriteLine(numIncreased);
}

Day1Part2();
