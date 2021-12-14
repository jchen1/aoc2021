namespace aoc2021;

public static class Day12
{
    private class Graph
    {
        public Dictionary<string, List<string>> Nodes { get; set; }

        public Graph(IEnumerable<(string a, string b)> edges)
        {
            Nodes = new();
            foreach (var edge in edges)
            {
                if (!Nodes.ContainsKey(edge.a))
                {
                    Nodes.Add(edge.a, new());
                }
                if (!Nodes.ContainsKey(edge.b))
                {
                    Nodes.Add(edge.b, new());
                }
                Nodes[edge.a].Add(edge.b);
                Nodes[edge.b].Add(edge.a);
            }
        }
    }

    private static readonly Graph Input = new(File.ReadAllLines("inputs/day12.txt")
        .Select(line => (line.Split("-")[0], line.Split("-")[1])));

    private static bool DoubleAllowed(List<string> path)
    {
        var filtered = path.Where(node => node == node.ToLowerInvariant()).ToList();
        return filtered.ToHashSet().Count == filtered.Count;
    }

    private static List<List<string>> ListPaths(string start, string target, List<string> path, bool doubleAllowed)
    {
        if (start == target) return new List<List<string>>() { path };

        return Input.Nodes[start]
            .Where(node => node != "start" && (node != node.ToLowerInvariant() || path.FindAll(x => x == node).Count < (doubleAllowed && DoubleAllowed(path) ? 2 : 1)))
            .SelectMany(node => ListPaths(node, target, new List<string>(path) { node }, doubleAllowed))
            .ToList();
    }

    public static void Part1()
    {
        var paths = ListPaths("start", "end", new List<string>() { "start" }, false);
        Console.WriteLine(paths.Count);
    }

    public static void Part2()
    {
        var paths = ListPaths("start", "end", new List<string>() { "start" }, true);
        Console.WriteLine(paths.Count);
    }
}
