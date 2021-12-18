namespace aoc2021;

interface ISnailfishNumber
{
    public PairSnailfishNumber? Parent { get; set; }

    public static ISnailfishNumber FromString(string input)
    {
        var stack = new Stack<PairSnailfishNumber>();
        foreach (var c in input)
        {
            switch (c)
            {
                case '[':
                    if (stack.TryPeek(out var parent))
                    {
                        stack.Push(new PairSnailfishNumber());
                        parent.AddChild(stack.Peek());
                    }
                    else
                    {
                        stack.Push(new PairSnailfishNumber());
                    }
                    break;
                case ']':
                    var number = stack.Pop();
                    if (stack.Count == 0)
                    {
                        return number;
                    }

                    break;
                case >= '0' and <= '9':
                    stack.Peek().AddChild(new RegularSnailfishNumber(c - '0'));
                    break;
            }
        }

        throw new InvalidOperationException("Invalid input");
    }

    public static ISnailfishNumber operator +(ISnailfishNumber first, ISnailfishNumber second)
    {
        return new PairSnailfishNumber(first, second).Reduce();
    }
}

class RegularSnailfishNumber : ISnailfishNumber
{
    public int Value { get; set; }
    public PairSnailfishNumber? Parent { get; set; }
    public RegularSnailfishNumber(int value)
    {
        Value = value;
    }

    public (bool changed, ISnailfishNumber number) Split()
    {
        if (Value < 10) return (false, this);

        return (true, new PairSnailfishNumber(
            new RegularSnailfishNumber((int) Math.Floor((Value * 1.0) / 2)),
            new RegularSnailfishNumber((int) Math.Ceiling((Value * 1.0) / 2))) { Parent = Parent });
    }
}

class PairSnailfishNumber : ISnailfishNumber
{
    public ISnailfishNumber? Left { get; set; }
    public ISnailfishNumber? Right { get; set; }
    public PairSnailfishNumber? Parent { get; set; }

    public PairSnailfishNumber() { }

    public PairSnailfishNumber(ISnailfishNumber? left, ISnailfishNumber? right)
    {
        Left = left;
        Right = right;

        if (Left != null) Left.Parent = this;
        if (Right != null) Right.Parent = this;
    }

    public void AddChild(ISnailfishNumber child)
    {
        if (Left is null)
        {
            Left = child;
        }
        else if (Right is null)
        {
            Right = child;
        }

        child.Parent = this;
    }

    public (bool changed, ISnailfishNumber number) Explode()
    {
        var siblingLeft = Left?.FindSiblingLeft();
        if (Left is RegularSnailfishNumber rnl && siblingLeft is not null)
        {
            siblingLeft.Value += rnl.Value;
        }
        var siblingRight = Right?.FindSiblingRight();
        if (Right is RegularSnailfishNumber rnr && siblingRight is not null)
        {
            siblingRight.Value += rnr.Value;
        }

        var newThis = new RegularSnailfishNumber(0) { Parent = Parent };

        if (Parent is not null)
        {
            Parent.Left = Parent.Left == this ? newThis : Parent.Left;
            Parent.Right = Parent.Right == this ? newThis : Parent.Right;
        }

        return (true, newThis);
    }
}

static class SnailfishNumberExtensions
{
    private static (bool changed, ISnailfishNumber number) ExplodeInner(this ISnailfishNumber number, int level)
    {
        switch (number)
        {
            case RegularSnailfishNumber rn:
                return (false, rn);
            case PairSnailfishNumber pn when level >= 4:
                return pn.Explode();
            case PairSnailfishNumber { Left: { }, Right: { } } pn:
            {
                var (changedLeft, left) = pn.Left.ExplodeInner(level + 1);
                pn.Left = left;
                if (changedLeft) return (changedLeft, pn);

                var (changedRight, right) = pn.Right.ExplodeInner(level + 1);
                pn.Right = right;
                return (changedRight, pn);
            }
            default:
                return (false, number);
        }
    }
    private static (bool changed, ISnailfishNumber number) SplitInner(this ISnailfishNumber number)
    {
        switch (number)
        {
            case RegularSnailfishNumber rn:
                return rn.Split();
            case PairSnailfishNumber { Left: { }, Right: { } } pn:
            {
                var (changedLeft, left) = pn.Left.SplitInner();
                pn.Left = left;
                if (changedLeft) return (changedLeft, pn);

                var (changedRight, right) = pn.Right.SplitInner();
                pn.Right = right;
                return (changedRight, pn);
            }
            default:
                return (false, number);
        }
    }

    private static (bool changed, ISnailfishNumber number) ReduceInner(this ISnailfishNumber number)
    {
        var (changed, result) = number.ExplodeInner(0);
        if (!changed)
        {
            (changed, result) = number.SplitInner();
        }

        return (changed, result);
    }

    public static ISnailfishNumber Reduce(this ISnailfishNumber number)
    {
        bool changed;
        var result = number;
        do
        {
            (changed, result) = result.ReduceInner();
        } while (changed);

        return result;
    }

    private static RegularSnailfishNumber? Rightmost(this ISnailfishNumber number)
    {
        return number switch
        {
            RegularSnailfishNumber rn => rn,
            PairSnailfishNumber pn => pn.Right?.Rightmost(),
            _ => throw new Exception("Unknown snailfish number type")
        };
    }

    private static RegularSnailfishNumber? Leftmost(this ISnailfishNumber number)
    {
        return number switch
        {
            RegularSnailfishNumber rn => rn,
            PairSnailfishNumber pn => pn.Left?.Leftmost(),
            _ => throw new Exception("Unknown snailfish number type")
        };
    }

    public static RegularSnailfishNumber? FindSiblingLeft(this ISnailfishNumber number)
    {
        return number.Parent?.Left == number ? number.Parent.FindSiblingLeft() : number.Parent?.Left?.Rightmost();
    }

    public static RegularSnailfishNumber? FindSiblingRight(this ISnailfishNumber number)
    {
        return number.Parent?.Right == number ? number.Parent.FindSiblingRight() : number.Parent?.Right?.Leftmost();
    }

    public static string ToReadableString(this ISnailfishNumber number)
    {
        return number switch
        {
            RegularSnailfishNumber rn => rn.Value.ToString(),
            PairSnailfishNumber pn => $"[{pn.Left?.ToReadableString() ?? ""},{pn.Right?.ToReadableString() ?? ""}]",
            _ => throw new Exception("Unknown snailfish number type")
        };
    }

    public static ulong Magnitude(this ISnailfishNumber number)
    {
        return number switch
        {
            RegularSnailfishNumber rn => (ulong) rn.Value,
            PairSnailfishNumber pn => (3 * pn.Left?.Magnitude() ?? 0) + (2 * pn.Right?.Magnitude() ?? 0),
            _ => throw new Exception("Unknown snailfish number type")
        };
    }

    public static ISnailfishNumber DeepCopy(this ISnailfishNumber number, PairSnailfishNumber? parent = null)
    {
        return number switch
        {
            RegularSnailfishNumber rn => new RegularSnailfishNumber(rn.Value) {Parent = parent},
            PairSnailfishNumber pn => new PairSnailfishNumber(pn.Left?.DeepCopy(pn), pn.Right?.DeepCopy(pn)) { Parent = parent },
            _ => throw new Exception("Unknown snailfish number type")
        };
    }
}

public static class Day18
{
    private static readonly List<ISnailfishNumber> Input = File.ReadAllLines("inputs/day18.txt")
        .Where(x => !string.IsNullOrEmpty(x)).Select(ISnailfishNumber.FromString).ToList();

    public static void Part1()
    {
        Console.WriteLine(Input.Aggregate((x, y) => x + y).Magnitude());
    }

    public static void Part2()
    {
        var max = Enumerable.Range(0, Input.Count)
            .SelectMany(x => Enumerable.Range(0, Input.Count).Select(y => (x, y)))
            .Where(tuple => tuple.x != tuple.y)
            .Select(tuple => (Input[tuple.x].DeepCopy() + Input[tuple.y].DeepCopy()).Magnitude())
            .Max();

        Console.WriteLine(max);
    }
}
