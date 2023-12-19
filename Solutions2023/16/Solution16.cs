using Framework;

namespace Solutions2023;

public class Solution16() : SolutionFramework(16)
{
    enum Dir { N, S, W, E };
    
    public override string[] Solve()
    {
        var gr = InpGr<char>();
        var visited = new Dictionary<(GridPos, Dir), bool>();
        TravelBeam(0,0, Dir.E);
        AssignAnswer1(GetEnergized(visited));

        for (var i = 0; i < gr.Length; i++)
        {
            visited.Clear();
            TravelBeam(i, 0, Dir.E);
            NSlot.AssignIfBigger(GetEnergized(visited));
            visited.Clear();
            TravelBeam(i, gr[0].Length, Dir.W);
            NSlot.AssignIfBigger(GetEnergized(visited));
        }
        for (var j = 0; j < gr[0].Length; j++)
        {
            visited.Clear();
            TravelBeam(0, j, Dir.S);
            NSlot.AssignIfBigger(GetEnergized(visited));
            visited.Clear();
            TravelBeam(gr.Length, j, Dir.N);
            NSlot.AssignIfBigger(GetEnergized(visited));
        }
        AssignAnswer2();

        return Answers;
        
        void TravelBeam(int i, int j, Dir dir)
        {
            while (i >= 0 && i < gr.Length && j >= 0 && j < gr[0].Length)
            {
                var pos = new GridPos(i, j);
                var cell = gr.GetCell(pos);
                if (!visited.TryAdd((pos, dir), true))
                {
                    break;
                }
                switch (cell)
                {
                    case '|' when dir is Dir.W or Dir.E:
                        TravelBeam(i-1, j, Dir.N);
                        TravelBeam(i+1, j, Dir.S);
                        return;
                    case '-' when dir is Dir.S or Dir.N:
                        TravelBeam(i, j-1, Dir.W);
                        TravelBeam(i, j+1, Dir.E);
                        return;
                    case '/':
                        var (ni, nj, nd) = TurnLR();
                        TravelBeam(ni, nj, nd);
                        return;
                    case '\\':
                        (ni, nj, nd) = TurnRL();
                        TravelBeam(ni, nj, nd);
                        return;
                }
                Move();
            }
            return;

            
            (int, int, Dir) TurnLR()
            {
                /*
                     |
                    -\-
                     |
                */
                switch (dir)
                {
                    case Dir.N:
                        return (i, j+1, Dir.E);
                    case Dir.S:
                        return (i, j-1, Dir.W);
                    case Dir.E:
                        return (i-1, j, Dir.N);
                    case Dir.W:
                        return (i+1, j, Dir.S);
                }
                throw new InvalidOperationException();
            }
            (int, int, Dir) TurnRL()
            {
                /*
                     |
                    -/-
                     |
                */
                switch (dir)
                {
                    case Dir.N:
                        return (i, j-1, Dir.W);
                    case Dir.S:
                        return (i, j+1, Dir.E);
                    case Dir.E:
                        return (i+1, j, Dir.S);
                    case Dir.W:
                        return (i-1, j, Dir.N);
                }
                throw new InvalidOperationException();
            }

            void Move()
            {
                switch (dir)
                {
                    case Dir.N or Dir.S:
                        i = dir is Dir.S ? i + 1 : i - 1;
                        break;
                    case Dir.W or Dir.E:
                        j = dir is Dir.E ? j + 1 : j - 1;
                        break;
                }
            }
        }
        
        void PrintVisited()
        {
            for (var i = 0; i < gr.Length; i++)
            {
                for (var j = 0; j < gr[0].Length; j++)
                {
                    var p = new GridPos(i, j);
                    Console.Write(visited.Keys.Any(k => k.Item1 == p) ? '#' : '.');
                }
                Console.WriteLine();
            }
        }
    }
    private static int GetEnergized(Dictionary<(GridPos, Dir), bool> visited) => visited.DistinctBy(x=>x.Key.Item1).Count();
}