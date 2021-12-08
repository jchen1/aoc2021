using System.Collections.Immutable;

namespace aoc2021;

public static class Day8
{
    private static List<(List<ImmutableHashSet<char>> patterns, List<string> outputs)> input = File.ReadAllLines("inputs/day8.txt")
        .Select(
            line =>
            {
                var split = line.Split("|");
                return (
                    patterns: split[0].Split(" ").Where(s => !string.IsNullOrEmpty(s)).Select(s => s.ToCharArray().ToImmutableHashSet()).ToList(),
                    outputs: split[1].Split(" ").Where(s => !string.IsNullOrEmpty(s)).ToList()
                );
            })
        .ToList();
    
    public static void Part1()
    {
        var counts = new[] {2, 4, 3, 7}; // 1, 4, 7, 8
        var result = input.Select(tuple => tuple.outputs.Count(output => counts.Contains((output.Length)))).Sum();

        Console.WriteLine(result);
    }

//  aaaa
// b    c
// b    c
//  dddd
// e    f
// e    f
//  gggg

    [Flags]
    private enum Wire
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3,
        E = 1 << 4,
        F = 1 << 5,
        G = 1 << 6
    }

    private static Dictionary<int, int> WireIntToDigit = new ()
    {
        {(int) (Wire.A | Wire.B | Wire.C | Wire.E | Wire.F | Wire.G), 0},
        {(int) (Wire.C | Wire.F), 1},
        {(int) (Wire.A | Wire.C | Wire.D | Wire.E | Wire.G), 2},
        {(int) (Wire.A | Wire.C | Wire.D | Wire.F | Wire.G), 3},
        {(int) (Wire.B | Wire.C | Wire.D | Wire.F), 4},
        {(int) (Wire.A | Wire.B | Wire.D | Wire.F | Wire.G), 5},
        {(int) (Wire.A | Wire.B | Wire.D | Wire.E | Wire.F | Wire.G), 6},
        {(int) (Wire.A | Wire.C | Wire.F), 7},
        {(int) (Wire.A | Wire.B | Wire.C | Wire.D | Wire.E | Wire.F | Wire.G), 8},
        {(int) (Wire.A | Wire.B | Wire.C | Wire.D | Wire.F | Wire.G), 9}
    };

    private static int ConvertDigits(List<int> digits)
    {
        var res = 0;
        for (var i = 0; i < digits.Count; i++)
        {
            res = res * 10 + digits[i];
        }

        return res;
    }

    private static int ParseLine(List<ImmutableHashSet<char>> patterns, List<string> outputs)
    {
        var wires = new Dictionary<char, Wire>();

        var occurrences = patterns.Aggregate(new Dictionary<char, int>(), (acc, pattern) =>
        {
            foreach (var c in pattern)
            {
                if (!acc.ContainsKey(c))
                {
                    acc[c] = 0;
                }

                acc[c]++;
            }

            return acc;
        });

        var one = patterns.First(x => x.Count == 2);
        var seven = patterns.First(x => x.Count == 3);
        var four = patterns.First(x => x.Count == 4);

        var aWire = seven.Except(one).First();

        wires[aWire] = Wire.A;
        wires[occurrences.First(pair => pair.Value == 6).Key] = Wire.B;
        wires[occurrences.First(pair => pair.Value == 8 && pair.Key != aWire).Key] = Wire.C;
        wires[occurrences.First(pair => pair.Value == 7 && four.Contains(pair.Key)).Key] = Wire.D;
        wires[occurrences.First(pair => pair.Value == 4).Key] = Wire.E;
        wires[occurrences.First(pair => pair.Value == 9).Key] = Wire.F;
        wires[occurrences.First(pair => pair.Value == 7 && !four.Contains(pair.Key)).Key] = Wire.G;

        var digits = outputs.Select(chars => chars.ToCharArray().Select(c => wires[c]).Aggregate(0, (a, b) => a | (int) b)).Select(n => WireIntToDigit[n]).ToList();

        return ConvertDigits(digits);
    }

    public static void Part2()
    {
        Console.WriteLine(input.Select(tuple => ParseLine(tuple.patterns, tuple.outputs)).Sum());
    }
}
