using System.Diagnostics;
using Framework;

namespace Challenges2022;

public static partial class Ext
{
    public static bool IsEq(this Solution9.Coord thisCoord, Solution9.Coord otherCoord)
    {
        return thisCoord.X == otherCoord.X && thisCoord.Y == otherCoord.Y;
    }
}

public class Solution9 : SolutionFramework
{
    public Solution9() : base(9) { }

    public override string[] Solve()
    {
        var instrs = new List<(Direction, int)>();
        foreach (var line in RawInput.SplitByNewline())
        {
            var direction = (char)line[0] switch
            {
                'R' => Direction.Right,
                'L' => Direction.Left,
                'U' => Direction.Up,
                'D' => Direction.Down,
            };

            var amount = int.Parse(line.Split(' ').Last());

            instrs.Add((direction, amount));
        }

        var rope = new Rope(1);
        foreach (var instr in instrs)
        {
            rope.Move(instr.Item1, instr.Item2);
        }
        AssignAnswer1(rope.coordsThatTailVisited.Count);

        rope = new Rope(9);
        foreach (var instr in instrs)
        {
            rope.Move(instr.Item1, instr.Item2);
        }
        AssignAnswer2(rope.coordsThatTailVisited.Count);
        return Answers;
    }

    public struct Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    enum Direction
    {
        Up,Down,Left,Right
    }


    struct Rope
    {
        public Coord H;
        public List<Coord> Knots;

        public HashSet<(int, int)> coordsThatTailVisited;

        public Rope(int length)
        {
            this.H = new Coord(0, 0);
            this.Knots = new List<Coord>();
            for (int i = 0; i < length; i++)
            {
                this.Knots.Add(new Coord(0, 0));
            }
            coordsThatTailVisited = new HashSet<(int, int)>{(0, 0)};
        }


    private void MoveRestOfTheBody(Direction dir)
    {
        for(int i = this.Knots.Count-1; i >= 0; i--)
        {
            var head = i == this.Knots.Count-1 ? H : this.Knots[i + 1];
            var tail = this.Knots[i];

            if (head.X < tail.X)
                dir = Direction.Left;
            if (head.X > tail.X)
                dir = Direction.Right;
            if (head.Y < tail.Y)
                dir = Direction.Up;
            if (head.Y > tail.Y)
                dir = Direction.Down;

            //diff
            if (head.X != tail.X && head.Y != tail.Y)
            {
                if (Math.Abs(head.X - tail.X) == 1 && Math.Abs(head.Y - tail.Y) == 1)
                {
                    break;
                }

                switch (dir)
                {
                    case { } when dir == Direction.Up:
                        if (head.X != tail.X)
                            tail.X += head.X > tail.X ? 1 : -1;
                        if (head.Y != tail.Y)
                            tail.Y--;
                        break;
                    case { } when dir == Direction.Down:
                        if (head.X != tail.X)
                            tail.X += head.X > tail.X ? 1 : -1;
                        if (head.Y != tail.Y)
                            tail.Y++;
                        break;
                    case { } when dir == Direction.Left:
                        if (head.X != tail.X)
                            tail.X--;
                        if (head.Y != tail.Y)
                            tail.Y += head.Y > tail.Y ? 1 : -1;
                        break;
                    case { } when dir == Direction.Right:
                        if (head.X != tail.X)
                            tail.X++;
                        if (head.Y != tail.Y)
                            tail.Y += head.Y > tail.Y ? 1 : -1;
                        break;

                }
            }
            //mR
            else if (head.X > tail.X)
                tail.X = head.X-1;
            //mL
            else if (head.X < tail.X)
                tail.X = head.X+1;
            //mD
            else if (head.Y < tail.Y)
                tail.Y = head.Y+1;
            //mU
            else if (head.Y > tail.Y)
                tail.Y = head.Y-1;

            if (i == 0)
            {
                this.coordsThatTailVisited.Add((tail.X, tail.Y));
            }

            Knots[i] = tail;
        }
    }


        public void Move(Direction dir, int amount)
        {
            var stepsLeft = amount;
            switch (dir)
            {
                case { } when dir == Direction.Up:
                    while (stepsLeft > 0)
                    {
                        H.Y -= 1;
                        MoveRestOfTheBody(dir);
                        stepsLeft--;
                    }
                    break;
                case { } when dir == Direction.Down:
                    while (stepsLeft > 0)
                    {
                        H.Y += 1;
                        MoveRestOfTheBody(dir);
                        stepsLeft--;
                    }
                    break;
                case { } when dir == Direction.Left:
                    while (stepsLeft > 0)
                    {
                        H.X -= 1;
                        MoveRestOfTheBody(dir);
                        stepsLeft--;
                    }
                    break;
                case { } when dir == Direction.Right:
                    while (stepsLeft > 0)
                    {
                        H.X += 1;
                        MoveRestOfTheBody(dir);
                        stepsLeft--;
                    }
                    break;
            }
        }


        void printBoard(Rope rope)
        {
            var grid = (50, 50).CreateGrid<char>();
            foreach (var knot in rope.Knots)
            {
                grid[knot.Y+25][knot.X+25] = 'T';
            }
            grid[rope.H.Y+25][rope.H.X+25] = 'H';
            Debug.WriteLine("START");
            for (int i = 0; i < 50; i++)
            {
                Debug.WriteLine(' ');
                for (int j = 0; j < 50; j++)
                {
                    if (grid[i][j] == '\0')
                    {
                        grid[i][j] = '.';
                    }
                    Debug.Write(grid[i][j]);

                }
            }
            Debug.WriteLine("END\n");
        }
    }
}
