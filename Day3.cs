namespace aoc2021;

public class Day3
{
    private static List<string> ReadInput()
    {
        return File.ReadAllLines("inputs/day3.txt").ToList();
    }

    public static void Part1()
    {
        var lines = ReadInput();
        var numBits = lines.First().Length;

        var counts = new int[numBits];
        foreach (var line in lines)
        {
            for (var i = 0; i < numBits; i++)
            {
                counts[i] += (line[i] == '1') ? 1 : 0;
            }
        }

        int gamma = 0, epsilon = 0;
        for (var i = 0; i < numBits; i++)
        {
            if (counts[i] > (lines.Count / 2))
            {
                gamma |= (1 << (numBits - i - 1));
            }
            else
            {
                epsilon |= (1 << (numBits - i - 1));
            }
        }

        Console.WriteLine(gamma * epsilon);
    }

    private static int NumOnesInPosition(List<string> bitstrings, int pos)
    {
        return bitstrings.Select(x => x[pos] == '1' ? 1 : 0).Sum();
    }

    private static int BitStringToInt(string bitstring)
    {
        var result = 0;
        for (var i = 0; i < bitstring.Length; i++)
        {
            result |= (bitstring[i] == '1') ? (1 << (bitstring.Length - i - 1)) : 0;
        }
        return result;
    }

    private static int FilterBitStrings(List<string> bitstrings, Func<bool, char> filterFn)
    {
        var pos = 0;
        while (bitstrings.Count > 1)
        {
            var mostCommon = filterFn(NumOnesInPosition(bitstrings, pos) * 2 >= bitstrings.Count);
            bitstrings = bitstrings.Where(x => x[pos] == mostCommon).ToList();
            pos++;
        }

        return BitStringToInt(bitstrings.First());
    }

    public static void Part2()
    {
        var oxygen = FilterBitStrings(ReadInput(), x => x ? '1' : '0');
        var co2 = FilterBitStrings(ReadInput(), x => x ? '0' : '1');

        Console.WriteLine(oxygen * co2);
    }
}
