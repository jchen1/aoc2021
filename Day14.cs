namespace aoc2021;

public static class Day14
{
    private static readonly string Template;
    private static readonly Dictionary<(char left, char right), char> Rules;
    private static readonly Dictionary<(char left, char right), ulong> PairFrequencies;

    static Day14()
    {
        var file = File.ReadAllText("inputs/day14.txt").Split("\n\n");
        Template = file[0];
        Rules = file[1].Split("\n").Where(line => !string.IsNullOrEmpty(line))
            .Select(line => (line.Split(" -> ")[0], line.Split(" -> ")[1]))
            .ToDictionary(tuple => (tuple.Item1[0], tuple.Item1[1]), tuple => tuple.Item2[0]);

        PairFrequencies = Enumerable.Range(0, Template.Length - 1).Select(i => (Template[i], Template[i + 1]))
            .Aggregate(new Dictionary<(char left, char right), ulong>(), (dict, pair) =>
            {
                dict[pair] = dict.GetValueOrDefault(pair) + 1;
                return dict;
            });
    }

    private static Dictionary<char, ulong> GetFrequencies(int numSteps)
    {
        var pairFrequencies = Enumerable.Range(0, numSteps).Aggregate(new Dictionary<(char left, char right), ulong>(PairFrequencies), (pairFrequencies, _) =>
            pairFrequencies.Aggregate(new Dictionary<(char left, char right), ulong>(), (dict, kv) =>
                {
                    var (pair, frequency) = kv;
                    Rules.TryGetValue(pair, out var newChar);
                    if (newChar != default(char))
                    {
                        var newLeftPair = (pair.left, newChar);
                        var newRightPair = (newChar, pair.right);
                        dict[newLeftPair] = dict.GetValueOrDefault(newLeftPair) + frequency;
                        dict[newRightPair] = dict.GetValueOrDefault(newRightPair) + frequency;
                    }
                    else
                    {
                        dict[pair] = dict.GetValueOrDefault(pair) + frequency;
                    }

                    return dict;
                })
        );

        var frequencies = pairFrequencies.Aggregate(new Dictionary<char, ulong>(), (frequencies, pair) =>
        {
            var key = pair.Key.left;
            frequencies[key] = frequencies.GetValueOrDefault(key) + pair.Value;
            return frequencies;
        });
        frequencies[Template[^1]] = frequencies.GetValueOrDefault(Template[^1]) + 1;

        return frequencies;
    }

    private static ulong Answer(Dictionary<char, ulong> frequencies)
    {
        return frequencies.MaxBy(kvp => kvp.Value).Value - frequencies.MinBy(kvp => kvp.Value).Value;
    }

    public static void Part1()
    {
        Console.WriteLine(Answer(GetFrequencies(10)));
    }

    public static void Part2()
    {
        Console.WriteLine(Answer(GetFrequencies(40)));
    }
}
