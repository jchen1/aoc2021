using System.Diagnostics;

namespace aoc2021;

public static class Day24
{
    private enum Instruction
    {
        Inp,
        Add,
        Mul,
        Div,
        Mod,
        Eql
    }

    private class ALU
    {
        private static readonly Dictionary<string, int> RegistryAddresses = new()
        {
            ["w"] = 0,
            ["x"] = 1,
            ["y"] = 2,
            ["z"] = 3
        };

        public long[] Registers { get; }

        public long W => Registers[RegistryAddresses["w"]];
        public long X => Registers[RegistryAddresses["x"]];
        public long Y => Registers[RegistryAddresses["y"]];
        public long Z => Registers[RegistryAddresses["z"]];
        public bool Valid { get; private set; }

        public ALU()
        {
            Registers = new long[4];
            Valid = true;
        }

        public override string ToString()
        {
            return $"({Valid}) W: {W}, X: {X}, Y: {Y}, Z: {Z}";
        }

        private long RegisterOrValue(string addressOrValue)
        {
            return long.TryParse(addressOrValue, out var num) ? num : Registers[RegistryAddresses[addressOrValue]];
        }

        public void Process(string input, Stack<int> digits)
        {
            if (!Valid) return;

            var instructions = input.Split(' ');
            var instruction = Enum.Parse<Instruction>(instructions[0], true);
            var reg = RegistryAddresses[instructions[1]];
            var b = instructions.Length == 3 ? RegisterOrValue(instructions[2]) : 0;

            switch (instruction)
            {
                case Instruction.Inp:
                    if (digits.TryPop(out var digit))
                    {
                        Registers[reg] = digit;
                    }
                    else
                    {
                        throw new InvalidOperationException("ran out of digits!");
                    }

                    break;
                case Instruction.Add:
                    Registers[reg] += b;
                    break;
                case Instruction.Mul:
                    Registers[reg] *= b;
                    break;
                case Instruction.Div:
                    if (b == 0)
                    {
                        Valid = false;
                    }
                    else
                    {
                        Registers[reg] = ((Registers[reg]) / b);
                    }

                    break;
                case Instruction.Mod:
                    if (Registers[reg] < 0 || b <= 0)
                    {
                        Valid = false;
                    }
                    else
                    {
                        Registers[reg] %= b;
                    }
                    break;
                case Instruction.Eql:
                    Registers[reg] = Registers[reg] == b ? 1 : 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }

    private static readonly string[] Input = File.ReadAllLines("inputs/day24.txt");

    private static Stack<int>? NumToDigits(long num)
    {
        var stack = new Stack<int>();
        while (num > 0)
        {
            var digit = num % 10;
            if (digit == 0)
            {
                return null;
            }
            stack.Push((int) digit);
            num /= 10;
        }

        return stack.Count == 14 ? stack : null;
    }

    /*
     * Basically pushing and popping digits from a stack, where a sequence with `div z 1` is PUSH,
     * and `div z 26` is POP. Then `z == 0` at the end if the digits on each end of the stack match according
     * to the equation `input[POP] + b[PUSH] + c[POP] == input[PUSH]`, where `c` is the value of `add y c`
     * in step 16 of a loop and `b` is the value of `add x b` in step 6.
     *
     * For the given input, the equations are:
     *
     * w[0] + 8 - 16 = w[13]
     * w[1] + 8 - 10 = w[12]
     * w[4] + 2 - 1 = w[11]
     * w[2] + 12 - 8 = w[3]
     * w[5] + 8 - 11 = w[6]
     * w[7] + 9 - 3 = w[8]
     * w[9] + 3 - 3 = w[10]
     *
     * From here it's fairly easy to manually solve the systems of equations to maximize/minimize the answer.
     * The ALU code above just verifies that `z == 0` for the given input.
     */
    public static void Part1()
    {
        const long largest = 99598963999971;

        var digits = NumToDigits(largest);
        var alu = new ALU();

        Debug.Assert(digits is not null);
        foreach (var line in Input)
        {
            alu.Process(line, digits);
        }
        Debug.Assert(alu.Z == 0);
        Debug.Assert(alu.Valid);
        Console.WriteLine(largest);
    }

    public static void Part2()
    {
        const long smallest = 93151411711211;

        var digits = NumToDigits(smallest);
        var alu = new ALU();

        Debug.Assert(digits is not null);
        foreach (var line in Input)
        {
            alu.Process(line, digits);
        }
        Debug.Assert(alu.Z == 0);
        Debug.Assert(alu.Valid);
        Console.WriteLine(smallest);
    }
}
