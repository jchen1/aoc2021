using System.Collections;
using System.Text;

namespace aoc2021;

public static class Day23
{
    private static readonly Dictionary<char, int> CostPerMove = new()
    {
        { 'A', 1 },
        { 'B', 10 },
        { 'C', 100 },
        { 'D', 1000 }
    };

    private static readonly Dictionary<char, int> AmphipodRooms = new()
    {
        { 'A', 3 },
        { 'B', 5 },
        { 'C', 7 },
        { 'D', 9 }
    };

    private readonly record struct Point(int X, int Y)
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator +(Point a, (int X, int Y) b) => new(a.X + b.X, a.Y + b.Y);

        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
        public static Point operator -(Point a, (int X, int Y) b) => new(a.X - b.X, a.Y - b.Y);

        public static bool operator ==(Point a, (int X, int Y) b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point a, (int X, int Y) b) => !(a == b);


        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    private readonly record struct Grid
    {
        private readonly char[] _grid;
        private readonly int _height, _width;
        private readonly Dictionary<char, List<Point>> _rooms;

        public int Hash()
        {
            return new string(_grid[_width..^_width]).GetHashCode();
        }

        public Grid(string input)
        {
            var lines = input.Split('\n').Where(l => !string.IsNullOrEmpty(l)).ToArray();
            _height = lines.Length;
            _width = lines[0].Length;
            _grid = new char[_height * _width];
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    _grid[y * _width + x] = lines[y][x];
                }
            }

            var depth = _height - 3;
            _rooms = new Dictionary<char, List<Point>>
            {
                {
                    'A',
                    Enumerable.Range(2, depth).Select(y => new Point(3, y)).ToList()
                },
                {
                    'B',
                    Enumerable.Range(2, depth).Select(y => new Point(5, y)).ToList()
                },
                {
                    'C',
                    Enumerable.Range(2, depth).Select(y => new Point(7, y)).ToList()
                },
                {
                    'D',
                    Enumerable.Range(2, depth).Select(y => new Point(9, y)).ToList()
                }
            };
        }

        private Grid(Grid other)
        {
            _rooms = other._rooms;
            _height = other._height;
            _width = other._width;
            _grid = new char[other._grid.Length];
            Buffer.BlockCopy(other._grid, 0, _grid, 0, other._grid.Length * sizeof(char));
        }

        public bool Solved()
        {
            var self = this;
            return _rooms.All(kv => kv.Value.All(p => self.PointAt(p) == kv.Key));
        }

        private static Grid Move(Grid grid, Point from, Point to)
        {
            var other = new Grid(grid);
            other._grid[to.Y * grid._width + to.X] = other.PointAt(from);
            other._grid[from.Y * grid._width + from.X] = '.';
            return other;
        }

        private static bool OutsideRoom(Point pos)
        {
            return pos == new Point(3, 1) || pos == new Point(5, 1) || pos == new Point(7, 1) || pos == new Point(9, 1);
        }

        private static bool InHallway(Point pos)
        {
            return pos.Y == 1;
        }

        private char PointAt(Point pos)
        {
            if (pos.X < 0 || pos.X >= _width || pos.Y < 0 || pos.Y >= _height)
            {
                return '#';
            }

            return _grid[pos.Y * _width + pos.X];
        }

        private bool PointIsOrEmpty(char match, Point pos)
        {
            var c = PointAt(pos);
            return (c == match || c == '.');
        }

        private bool PointIsEmpty(Point pos)
        {
            return PointAt(pos) == '.';
        }

        private static int Cost(Point a, Point b)
        {
            var diff = a - b;
            return Math.Abs(diff.X) + Math.Abs(diff.Y);
        }

        private List<(int cost, Point pos)> ValidMovesFrom(Point pos)
        {
            var c = PointAt(pos);
            if (CostPerMove.TryGetValue(c, out var cpm))
            {
                var self = this;
                var ret = new List<(int cost, Point pos)>();

                var room = _rooms[c];
                // room is already (partially) completed, do nothing
                if (room.All(pt => self.PointIsOrEmpty(c, pt)) && room.Contains(pos))
                {
                    return ret;
                }

                // hallway: room is empty or partially complete and we can move to it
                if (InHallway(pos) && room.All(pt => self.PointIsOrEmpty(c, pt)) &&
                    Enumerable.Range(Math.Min(room[0].X, pos.X), Math.Abs(pos.X - room[0].X)).All(x =>
                    {
                        var pt = new Point(x, 1);
                        return pos == pt || self.PointIsOrEmpty('.', pt);
                    }))
                {
                    var firstEmpty = _rooms[c].LastOrDefault(p => self.PointIsOrEmpty('.', p), new Point(-1, -1));
                    if (firstEmpty != (-1, -1))
                    {
                        ret.Add((cpm * Cost(pos, firstEmpty), firstEmpty));
                    }
                }

                // room: in a room with empty space above, move to a hallway
                if (!InHallway(pos) && Enumerable.Range(1, pos.Y - 2).All(y => self.PointIsEmpty(new Point(pos.X, pos.Y - y))))
                {
                    for (var x = pos.X; x >= 0 && PointIsEmpty(new Point(x, 1)); x--)
                    {
                        if (!OutsideRoom(new Point(x, 1)))
                        {
                            ret.Add((cpm * Cost(pos, new Point(x, 1)), new Point(x, 1)));
                        }
                    }
                    for (var x = pos.X; x < _grid.GetLength(0) && PointIsEmpty(new Point(x, 1)); x++)
                    {
                        if (!OutsideRoom(new Point(x, 1)))
                        {
                            ret.Add((cpm * Cost(pos, new Point(x, 1)), new Point(x, 1)));
                        }
                    }
                }

                return ret;
            }
            throw new InvalidOperationException($"Point ${pos} does not contain an amphipod");
        }

        public List<(int cost, Grid grid)> ValidMoves()
        {
            var ret = new List<(int cost, Grid grid)>();
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var from = new Point(x, y);
                    if (AmphipodRooms.ContainsKey(PointAt(from)))
                    {
                        var self = this;
                        ret.AddRange(ValidMovesFrom(from).Select(mv => (mv.cost, Move(self, from, mv.pos))));
                    }
                }
            }
            return ret;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    sb.Append(_grid[y * _width + x]);
                }

                sb.Append('\n');
            }

            return sb.ToString();
        }
    }

    private static readonly Grid Input;
    private static readonly Grid Part2Input;

    static Day23()
    {
        var lines = File.ReadAllText("inputs/day23.txt").Split("\n\n");
        Input = new Grid(lines[0]);
        Part2Input = new Grid(lines[1]);
    }

    private static int Solve(Grid input)
    {
        var visited = new Dictionary<int, int>();
        var pq = new PriorityQueue<Grid, int>();
        pq.Enqueue(input, 0);
        visited[input.Hash()] = 0;

        while (pq.TryDequeue(out var grid, out var cost))
        {
            if (grid.Solved())
            {
                Console.WriteLine(grid);
                return cost;
            }

            foreach (var tuple in grid.ValidMoves())
            {
                var h = tuple.grid.Hash();
                var fullCost = cost + tuple.cost;
                if (!visited.ContainsKey(h) || visited[h] > fullCost)
                {
                    visited[h] = fullCost;
                    pq.Enqueue(tuple.grid, fullCost);
                }
            }
        }

        throw new InvalidOperationException("never solved??");
    }

    public static void Part1()
    {
        Console.WriteLine(Solve(Input));
    }

    public static void Part2()
    {
        Console.WriteLine(Solve(Part2Input));
    }
}
