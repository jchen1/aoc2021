using System.Diagnostics;

namespace aoc2021;

public static class Day22
{
    private enum CubeState
    {
        Off,
        On
    }

    private readonly record struct Point(long X, long Y, long Z);

    private readonly record struct Cuboid(CubeState State, Point P1, Point P2)
    {
        public Cuboid(string s) : this(CubeState.Off, new Point(0, 0, 0), new Point(0, 0, 0))
        {
            State = Enum.Parse<CubeState>(s.Split(" ")[0], true);
            var dimensions = s.Split(" ")[1].Split(",").Select(dim =>
            {
                var parts = dim.Split("..");
                var min = int.Parse(parts[0][2..]);
                var max = int.Parse(parts[1]);

                return min > max ? (max, min) : (min, max);
            }).ToList();

            Debug.Assert(dimensions.Count == 3);
            P1 = new Point(dimensions[0].Item1, dimensions[1].Item1, dimensions[2].Item1);
            P2 = new Point(dimensions[0].Item2, dimensions[1].Item2, dimensions[2].Item2);
            Debug.Assert(P1.X <= P2.X && P1.Y <= P2.Y && P1.Z <= P2.Z);
        }

        public long Volume()
        {
            return (State == CubeState.Off ? -1 : 1) * ((P2.X - P1.X + 1) * (P2.Y - P1.Y + 1) * (P2.Z - P1.Z + 1));
        }

        public override string ToString()
        {
            return $"{State} {P1.X}..{P2.X},{P1.Y}..{P2.Y},{P1.Z}..{P2.Z}";
        }

        public bool Intersects(Cuboid other)
        {
            if (!(P1.X <= other.P2.X && P2.X >= other.P1.X))
                return false;

            if (!(P1.Y <= other.P2.Y && P2.Y >= other.P1.Y))
                return false;

            if (!(P1.Z <= other.P2.Z && P2.Z >= other.P1.Z))
                return false;

            return true;
            //
            // return (P1.X <= other.P2.X || P2.X >= other.P1.X) &&
            //        (P1.Y <= other.P2.Y || P2.Y >= other.P1.Y) &&
            //        (P1.Z <= other.P2.Z || P2.Z >= other.P1.Z);
        }

        public Cuboid? Intersect(Cuboid other)
        {
            return Intersect(this, other);
        }

        public static Cuboid? Intersect(Cuboid a, Cuboid b)
        {
            var p1 = new Point(Math.Max(a.P1.X, b.P1.X), Math.Max(a.P1.Y, b.P1.Y), Math.Max(a.P1.Z, b.P1.Z));
            var p2 = new Point(Math.Min(a.P2.X, b.P2.X), Math.Min(a.P2.Y, b.P2.Y), Math.Min(a.P2.Z, b.P2.Z));

            var state = (a.State, b.State) switch
            {
                (CubeState.Off, CubeState.Off) => CubeState.On,
                (CubeState.On, CubeState.Off) => CubeState.On,
                (CubeState.Off, CubeState.On) => CubeState.Off,
                (CubeState.On, CubeState.On) => CubeState.Off,
                _ => throw new InvalidOperationException("Invalid state")
            };

            if (p2.X <= p1.X || p2.Y <= p1.Y || p2.Z <= p1.Z)
                return null;

            return new Cuboid(state, p1, p2);
        }
    }

    private static readonly List<Cuboid> Input = File.ReadAllLines("inputs/day22.txt").Where(l => !string.IsNullOrEmpty(l)).Select(l => new Cuboid(l)).ToList();

    public static void Part1()
    {
        var points = Input.Aggregate(new HashSet<Point>(), (pts, cube) =>
        {
            for (var x = Math.Max(-50, cube.P1.X); x <= Math.Min(50, cube.P2.X); x++)
            {
                for (var y = Math.Max(-50, cube.P1.Y); y <= Math.Min(50, cube.P2.Y); y++)
                {
                    for (var z = Math.Max(-50, cube.P1.Z); z <= Math.Min(50, cube.P2.Z); z++)
                    {
                        switch (cube.State)
                        {
                            case CubeState.Off:
                                pts.Remove(new Point(x, y, z));
                                break;
                            case CubeState.On:
                                pts.Add(new Point(x, y, z));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            return pts;
        });
        Console.WriteLine(points.Count);
    }

    public static void Part2()
    {
        var cuboids = new List<Cuboid>();
        Input.ForEach(current =>
        {
            cuboids.AddRange(cuboids
                .Select(current.Intersect)
                .OfType<Cuboid>()
                .ToList());

            if (current.State == CubeState.On)
            {
                cuboids.Add(current);
            }
        });


        Console.WriteLine(cuboids.Select(c => c.Volume()).Sum());
    }
}
