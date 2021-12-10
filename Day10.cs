namespace aoc2021;

public static class Day10
{
    private static readonly List<string> Input = File.ReadAllLines("inputs/day10.txt").ToList();

    private static readonly Dictionary<char, char> CharMap = new()
    {
        {'(', ')'},
        {'{', '}'},
        {'[', ']'},
        {'<', '>'}
    };

    private static readonly HashSet<char> OpeningChars = CharMap.Keys.ToHashSet();
    private static readonly HashSet<char> ClosingChars = CharMap.Values.ToHashSet();

    private static int ScoreLine(string line)
    {
        
        Dictionary<char, int> scores = new()
        {
            {')', 3},
            {']', 57},
            {'}', 1197},
            {'>', 25137}
        };
        
        var chars = line.ToCharArray();
        var stack = new Stack<char>();

        foreach (var c in chars)
        {
            if (OpeningChars.Contains(c))
            {
                stack.Push(c);
            }
            else if (ClosingChars.Contains(c))
            {
                var match = stack.Pop();
                if (CharMap[match] != c)
                {
                    return scores[c];
                }
            }
        }

        return 0;
    }

    public static void Part1()
    {
        var scores = Input.Select(ScoreLine).Sum();
        Console.WriteLine(scores);
    }

    private static long CompleteAndScoreLine(string line)
    {
        var scores = new Dictionary<char, int>()
        {
            {'(', 1},
            {'[', 2},
            {'{', 3},
            {'<', 4}
        };

        var chars = line.ToCharArray();
        var stack = new Stack<char>();
        long score = 0;

        foreach (var c in chars)
        {
            if (OpeningChars.Contains(c))
            {
                stack.Push(c);
            }
            else if (ClosingChars.Contains(c))
            {
                var match = stack.Pop();
                if (CharMap[match] != c)
                {
                    // syntax error, filter these out
                    return -1;
                }
            }
        }

        while (stack.Count > 0)
        {
            var c = stack.Pop();
            var newScore = score * 5 + scores[c];

            score = newScore;
        }

        return score;
    }

    public static void Part2()
    {
        var scores = Input
            .Select(CompleteAndScoreLine)
            .Where(score => score != -1)
            .OrderBy(x => x)
            .ToList();

        var median = scores.Skip(scores.Count / 2).First();
        Console.WriteLine(median);
    }
}
