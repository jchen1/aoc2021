namespace aoc2021;

public static class Day21
{
    private static readonly List<int> StartingPositions = File.ReadAllLines("inputs/day21.txt")
        .Select(l => int.Parse(l.Split(": ")[1].Trim()))
        .ToList();

    private static readonly Dictionary<int, ulong> DiracRolls = Enumerable.Range(1, 3)
        .SelectMany(x => Enumerable.Range(1, 3).SelectMany(y => Enumerable.Range(1, 3).Select(z => x + y + z)))
        .Aggregate(new Dictionary<int, ulong>(), (acc, score) =>
        {
            acc[score] = acc.GetValueOrDefault(score, 0ul) + 1;
            return acc;
        });

    private readonly record struct Player(int Id, int Position, int Score)
    {
        public int Position { get; } = 1 + ((Position - 1) % 10);

        public static Player operator +(Player p, int roll)
        {
            if (roll == 0)
            {
                return p;
            }
            var newPosition = 1 + ((p.Position + roll - 1) % 10);
            return new Player(p.Id, newPosition, p.Score + newPosition);
        }
    }

    private class DeterministicDie
    {
        private int _next = 0;
        public int NumRolls { get; private set; } = 0;

        private int Roll()
        {
            var result = _next + 1;
            _next = (_next + 1) % 100;
            NumRolls++;
            return result;
        }

        public List<int> RollMany(int times)
        {
            return Enumerable.Range(0, times).Select(_ => Roll()).ToList();
        }
    }

    public static void Part1()
    {
        var die = new DeterministicDie();
        var turn = 0;

        var players = StartingPositions.Select((pos, id) => new Player(id + 1, pos, 0)).ToList();
        players.ForEach(player => Console.WriteLine("Player {0} starts at {1}", player.Id, player.Position));
        while (true)
        {
            var rolls = die.RollMany(3);
            players[turn] += rolls.Sum();
            Console.WriteLine("Player {0} rolls {1} and moves to {2} for a total score of {3}", players[turn].Id,
                string.Join('+', rolls.Select(r => r.ToString())), players[turn].Position, players[turn].Score);

            if (players[turn].Score >= 1000)
            {
                Console.WriteLine(players[(turn + 1) % players.Count].Score * die.NumRolls);
                return;
            }
            turn = (turn + 1) % players.Count;
        }
    }

    private static (ulong p1, ulong p2) Part2Inner(Dictionary<(Player p1, Player p2, int turn), (ulong p1, ulong p2)> table, (Player p1, Player p2) currentPlayers, int turn)
    {
        if (currentPlayers.p1.Score >= 21)
        {
            table[(currentPlayers.p1, currentPlayers.p2, turn)] = (1, 0);
        }
        else if (currentPlayers.p2.Score >= 21)
        {
            table[(currentPlayers.p1, currentPlayers.p2, turn)] = (0, 1);
        }

        if (table.ContainsKey((currentPlayers.p1, currentPlayers.p2, turn)))
        {
            return table[(currentPlayers.p1, currentPlayers.p2, turn)];
        }

        var nextTurn = (turn + 1) % 2;
        var ret = DiracRolls.Select(kv =>
        {
            var players = (turn == 0 ? currentPlayers.p1 + kv.Key : currentPlayers.p1, turn == 1 ? currentPlayers.p2 + kv.Key : currentPlayers.p2);
            return new
            {
                UniverseMultiplier = kv.Value,
                Universes = Part2Inner(table, players, nextTurn)
            };
        }).Aggregate((0ul, 0ul), (acc, tuple) => (acc.Item1 + tuple.UniverseMultiplier * tuple.Universes.p1, acc.Item2 + tuple.UniverseMultiplier * tuple.Universes.p2));

        table[(currentPlayers.p1, currentPlayers.p2, turn)] = ret;
        return ret;
    }

    public static void Part2()
    {
        var table = new Dictionary<(Player p1, Player p2, int turn), (ulong p1, ulong p2)>();
        var p1 = new Player(1, StartingPositions[0], 0);
        var p2 = new Player(2, StartingPositions[1], 0);

        var winners = Part2Inner(table, (p1, p2), 0);
        var winner = Math.Max(winners.p1, winners.p2);
        Console.WriteLine(winner);
    }
}
