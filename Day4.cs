namespace aoc2021;

using System.Linq;

public static class Day4
{
    private struct BingoNumber
    {
        public int value;
        public bool marked;

        public BingoNumber(int value, bool marked = false)
        {
            this.value = value;
            this.marked = marked;
        }
    }

    private class BingoBoard
    {
        private BingoNumber[,] _board = new BingoNumber[5, 5];

        public BingoBoard(string board)
        {
            var rows = board.Split("\n").Where(s => !string.IsNullOrEmpty(s));
            foreach (var (row, i) in rows.Select((x, i) => (x, i)))
            {
                var cols = row.Split(" ").Where(s => !string.IsNullOrEmpty(s)).ToList();
                for (var j = 0; j < 5; j++)
                {
                    _board[i, j] = new BingoNumber(int.Parse(cols[j]));
                }
            }
        }

        public void Mark(int value)
        {
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    if (_board[i, j].value == value)
                    {
                        _board[i, j].marked = true;
                    }
                }
            }
        }

        public int Score()
        {
            return _board.Cast<BingoNumber>().Where(x => !x.marked).Select(x => x.value).Sum();
        }

        public bool Check()
        {
            // horizontal
            for (var i = 0; i < 5; i++)
            {
                var marked = 0;
                for (var j = 0; j < 5; j++)
                {
                    if (_board[i, j].marked)
                    {
                        marked++;
                    }
                }
                if (marked == 5)
                {
                    return true;
                }
            }
            // vertical
            for (var i = 0; i < 5; i++)
            {
                var marked = 0;
                for (var j = 0; j < 5; j++)
                {
                    if (_board[j, i].marked)
                    {
                        marked++;
                    }
                }
                if (marked == 5)
                {
                    return true;
                }
            }
            // diagonal
            int diag1 = 0, diag2 = 0;
            for (var i = 0; i < 5; i++)
            {
                if (_board[i, i].marked)
                {
                    diag1++;
                }

                if (_board[i, 5 - i - 1].marked)
                {
                    diag2++;
                }
            }

            if (diag1 == 5 || diag2 == 5)
            {
                return true;
            }

            return false;
        }
    }

    private static (IEnumerable<int> draws, List<BingoBoard> boards) ReadInput()
    {
        var tuple = File.ReadAllText("inputs/day4.txt").Split("\n\n");
        var draws = tuple[0].Split(",").Select(int.Parse);
        var boards = tuple.Skip(1).Select(s => new BingoBoard(s));

        return (draws, boards.ToList());
    }
    public static void Part1()
    {
        var (draws, boards) = ReadInput();
        foreach (var draw in draws)
        {
            foreach (var board in boards)
            {
                board.Mark(draw);
                if (board.Check())
                {
                    Console.WriteLine(board.Score() * draw);
                    return;
                }
            }
        }
    }

    public static void Part2()
    {
        var (draws, boards) = ReadInput();
        foreach (var draw in draws)
        {
            foreach (var board in boards)
            {
                board.Mark(draw);
            }

            if (boards.Count == 1 && boards[0].Check())
            {
                Console.WriteLine(boards[0].Score() * draw);
                return;
            }

            boards = boards.Where(b => !b.Check()).ToList();
        }
    }
}
