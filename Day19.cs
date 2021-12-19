using System.Diagnostics;

namespace aoc2021;

public static class Day19
{
    private static int[,] MultiplyMatrix(int[,] A, int[,] B)
    {
        var rA = A.GetLength(0);
        var cA = A.GetLength(1);
        var rB = B.GetLength(0);
        var cB = B.GetLength(1);
        var multiplied = new int[rA, cB];

        Debug.Assert(cA == rB);
        for (var i = 0; i < rA; i++)
        {
            for (var j = 0; j < cB; j++)
            {
                var temp = 0;
                for (var k = 0; k < cA; k++)
                {
                    temp += A[i, k] * B[k, j];
                }
                multiplied[i, j] = temp;
            }
        }
        return multiplied;
    }

    private readonly record struct Point(int X, int Y, int Z) : IComparable
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public int DistanceFrom(Point other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public static Point Rotate(Point point, int angleX, int angleY, int angleZ)
        {
            var radX = angleX * Math.PI / 180;
            var radY = angleY * Math.PI / 180;
            var radZ = angleZ * Math.PI / 180;

            var rotationMatrix = new[,]
            {
                {(int) (Math.Cos(radZ) * Math.Cos(radY)), (int) (Math.Cos(radZ) * Math.Sin(radY) * Math.Sin(radX) - Math.Sin(radZ) * Math.Cos(radX)), (int) (Math.Cos(radZ) * Math.Sin(radY) * Math.Cos(radX) + Math.Sin(radZ) * Math.Sin(radX))},
                {(int) (Math.Sin(radZ) * Math.Cos(radY)), (int) (Math.Sin(radZ) * Math.Sin(radY) * Math.Sin(radX) + Math.Cos(radZ) * Math.Cos(radX)), (int) (Math.Sin(radZ) * Math.Sin(radY) * Math.Cos(radX) - Math.Cos(radZ) * Math.Sin(radX))},
                {(int) (-1 * Math.Sin(radY)),             (int) (Math.Cos(radY) * Math.Sin(radX)),                                                    (int) (Math.Cos(radY) * Math.Cos(radX))}
            };

            var rotated = MultiplyMatrix(rotationMatrix, new[,] { { point.X }, { point.Y }, { point.Z } });
            return new Point(rotated[0, 0], rotated[1, 0], rotated[2, 0]);
        }

        public static Point Translate(Point point, Point offset)
        {
            return new Point(point.X + offset.X, point.Y + offset.Y, point.Z + offset.Z);
        }

        public int CompareTo(object? obj)
        {
            if (obj is Point p)
            {
                return X < p.X ? -1 : X > p.X ? 1 : Y < p.Y ? -1 : Y > p.Y ? 1 : Z < p.Z ? -1 : Z > p.Z ? 1 : 0;
            }

            throw new ArgumentException();
        }
    }
    private static Point ParseLine(string line)
    {
        return new Point(int.Parse(line.Split(",")[0]), int.Parse(line.Split(",")[1]), int.Parse(line.Split(",")[2]));
    }

    private static readonly Dictionary<int, List<Point>> ScannerPositions = File.ReadAllText("inputs/day19.txt")
        .Split("\n\n")
        .Select((scanner, i) => (i, scanner.Split("\n").Where(s => !string.IsNullOrEmpty(s)).Skip(1).Select(ParseLine).ToList()))
        .ToDictionary(kv => kv.i, kv => kv.Item2);

    private static bool TryAlign(List<Point> beacons, List<Point> other, out (List<Point> offsets, Point scannerPosition) result)
    {
        var set = new HashSet<Point>(beacons);
        for (var xAngle = 0; xAngle < 360; xAngle += 90)
        {
            for (var yAngle = 0; yAngle < 360; yAngle += 90)
            {
                for (var zAngle = 0; zAngle < 360; zAngle += 90)
                {
                    var rotated = other.Select(point => Point.Rotate(point, xAngle, yAngle, zAngle)).ToList();
                    // attempt n^2 translations
                    foreach (var point in beacons)
                    {
                        foreach (var candidatePoint in rotated)
                        {
                            var candidateTranslation = point - candidatePoint;
                            var translatedOther = rotated.Select(point => Point.Translate(point, candidateTranslation)).ToHashSet();
                            if (translatedOther.Intersect(set).Count() >= 12)
                            {
                                result = (translatedOther.ToList(), candidateTranslation);
                                return true;
                            }
                        }
                    }
                }
            }
        }

        result = (new List<Point>(), new Point(0, 0, 0));
        return false;
    }

    private static (List<Point> scanners, HashSet<Point> beacons) AlignScanners()
    {
        var positions = new HashSet<Point>(ScannerPositions[0]);
        var seen = new Dictionary<int, Point>() { { 0, new Point(0, 0, 0) } };

        var toProcess = new Queue<(int scannerId, List<Point> beacons)>();
        toProcess.Enqueue((0, ScannerPositions[0]));

        while (toProcess.Count > 0)
        {
            var (_, beacons) = toProcess.Dequeue();
            foreach (var candidateId in ScannerPositions.Keys.Where(key => !seen.ContainsKey(key)))
            {
                if (TryAlign(beacons, ScannerPositions[candidateId], out var aligned))
                {
                    seen[candidateId] = aligned.scannerPosition;
                    aligned.offsets.ForEach(pt => positions.Add(pt));
                    toProcess.Enqueue((candidateId, aligned.offsets));
                }
            }
        }

        return (seen.Values.ToList(), positions);
    }

    public static void Part1()
    {
        Console.WriteLine(AlignScanners().beacons.Count);
    }

    public static void Part2()
    {
        var (scanners, _) = AlignScanners();
        var max = scanners.SelectMany(ptA => scanners.Select(ptA.DistanceFrom)).Max();
        Console.WriteLine(max);
    }
}
