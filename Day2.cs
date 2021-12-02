namespace aoc2021;

public class Day2
{
    private enum Direction
    {
        Up,
        Down,
        Forward,
    };

    private static IEnumerable<(Direction Direction, int Distance)> ReadInput()
    {
        return File.ReadAllLines("inputs/day2.txt").Select(line =>
        {
            var split = line.Split(" ");
            var direction = Enum.Parse<Direction>(split[0], true);
            var distance = int.Parse(split[1]);

            return (direction, distance);
        });
    }

    public static void Part1()
    {
        var commands = ReadInput();

        int position = 0, depth = 0;
        foreach (var (direction, distance) in commands)
        {
            switch (direction)
            {
                case Direction.Up:
                    depth -= distance;
                    break;
                case Direction.Down:
                    depth += distance;
                    break;
                case Direction.Forward:
                    position += distance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Console.WriteLine(depth * position);
    }

    public static void Part2()
    {
        var commands = ReadInput();
        int position = 0, depth = 0, aim = 0;
        foreach (var (direction, distance) in commands)
        {
            switch (direction)
            {
                case Direction.Down:
                    aim += distance;
                    break;
                case Direction.Up:
                    aim -= distance;
                    break;
                case Direction.Forward:
                    position += distance;
                    depth += distance * aim;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Console.WriteLine(position * depth);
    }
}
