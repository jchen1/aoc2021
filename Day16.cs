using System.Collections;

namespace aoc2021;

public static class Day16
{
    private static readonly BitArray Input;

    private static readonly Dictionary<char, byte> Pattern = new()
    {
        {'0', 0},
        {'1', 1},
        {'2', 2},
        {'3', 3},
        {'4', 4},
        {'5', 5},
        {'6', 6},
        {'7', 7},
        {'8', 8},
        {'9', 9},
        {'A', 10},
        {'B', 11},
        {'C', 12},
        {'D', 13},
        {'E', 14},
        {'F', 15},
    };

    private static byte ReverseByte(byte b)
    {
        b = (byte) ((b & 0xF0) >> 4 | (b & 0x0F) << 4);
        b = (byte) ((b & 0xCC) >> 2 | (b & 0x33) << 2);
        b = (byte) ((b & 0xAA) >> 1 | (b & 0x55) << 1);
        return b;
    }

    static Day16()
    {
        var hex = File.ReadAllText("inputs/day16.txt").Trim().Select(c => Pattern[c]).ToList();
        // BitArray is little-endian, so we need to reverse each byte pair
        var bytes = Enumerable.Range(0, hex.Count / 2).Select(i => ReverseByte((byte)(hex[2 * i] << 4 | hex[2 * i + 1]))).ToArray();
        Input = new BitArray(bytes);
    }

    private static uint ReadFromBitArray(BitArray data, int offset, int numBits)
    {
        uint ret = 0;
        for (var i = 0; i < numBits; i++)
        {
            ret = (uint) ((ret << 1) + (data[offset + i] ? 1 : 0));
        }

        return ret;
    }

    private enum PacketType
    {
        Sum,
        Product,
        Minimum,
        Maximum,
        Literal,
        GreaterThan,
        LessThan,
        EqualTo
    }

    private abstract class Packet
    {
        public int NextOffset;
        public ulong Value;

        private ushort _version;

        public static Packet ParsePacket(BitArray data, int offset)
        {
            var version = (ushort) ReadFromBitArray(data, offset, 3);
            var type = (PacketType) ReadFromBitArray(data, offset + 3, 3);

            return type switch
            {
                PacketType.Literal => new LiteralPacket(data, offset + 6) {_version = version},
                _ => new OperatorPacket(data, type, offset + 6) {_version = version}
            };
        }

        public static int SumVersions(Packet packet)
        {
            return packet switch
            {
                OperatorPacket operatorPacket => operatorPacket._version + operatorPacket.Children.Sum(SumVersions),
                _ => packet._version
            };
        }
    }

    private class LiteralPacket : Packet
    {
        public LiteralPacket(BitArray data, int offset)
        {
            Value = (Value << 4) + ReadFromBitArray(data, offset + 1, 4);
            while (data[offset])
            {
                offset += 5;
                Value = (Value << 4) + ReadFromBitArray(data, offset + 1, 4);
            }

            NextOffset = offset + 5;
        }
    }

    private class OperatorPacket : Packet
    {
        public readonly List<Packet> Children = new();

        public OperatorPacket(BitArray data, PacketType type, int offset)
        {
            var mode = data[offset];
            offset++;

            // next 11 bits are the number of sub-packets
            if (mode)
            {
                var numChildren = (int) ReadFromBitArray(data, offset, 11);
                offset += 11;

                for (var i = 0; i < numChildren; i++)
                {
                    var child = ParsePacket(data, offset);
                    offset = child.NextOffset;
                    Children.Add(child);
                }

                NextOffset = offset;
            }
            // next 15 bits are the total length in bits
            else
            {
                var bits = (int) ReadFromBitArray(data, offset, 15);
                offset += 15;

                NextOffset = offset + bits;
                while (offset < NextOffset)
                {
                    var child = ParsePacket(data, offset);
                    offset = child.NextOffset;
                    Children.Add(child);
                }
            }

            Value = type switch
            {
                PacketType.Sum => Children.Aggregate(0ul, (a, c) => a + c.Value),
                PacketType.Product => Children.Aggregate(1ul, (a, c) => (a * c.Value)),
                PacketType.Minimum => Children.Min(c => c.Value),
                PacketType.Maximum => Children.Max(c => c.Value),
                PacketType.GreaterThan => Children[0].Value > Children[1].Value ? 1u : 0u,
                PacketType.LessThan => Children[0].Value < Children[1].Value ? 1u : 0u,
                PacketType.EqualTo => Children[0].Value == Children[1].Value ? 1u : 0u,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    public static void Part1()
    {
        var packet = Packet.ParsePacket(Input, 0);
        Console.WriteLine(Packet.SumVersions(packet));
    }

    public static void Part2()
    {
        var packet = Packet.ParsePacket(Input, 0);
        Console.WriteLine(packet.Value);
    }
}
