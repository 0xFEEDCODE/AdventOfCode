using Framework;

namespace Solutions2023;

public class Solution14() : SolutionFramework(14)
{
    sealed record Entries(GridPos[] Positions)
    {
        public bool Equals(Entries? other) => Positions.SequenceEqual(other.Positions);
        public override int GetHashCode() => Positions.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())); 
    }
    
    public override string[] Solve()
    {
        var gr = InpGr<char>();
        
        var roundedRocks = gr.AllCellsWhere(c => c == 'O').ToArray();

        var seen = new HashSet<Entries> {new(roundedRocks.Select(x=>x.Pos).ToArray())};
        var arr = new List<Entries> {new(roundedRocks.Select(x=>x.Pos).ToArray())};

        for (var i = 1; i <= 1000000000; i++)
        {
            Cycle();
            var entries = new Entries(GetRoundedRocks());

            if (!seen.Add(entries))
            {
                var first = arr.IndexOf(entries);
                var x = (1000000000 - first) % (i - first) + first;
                for (var j = 0; j < gr.Length; j++)
                {
                    var c = arr.ElementAt(x).Positions.Count(r => r.R == j) * (gr.Length - j);
                    NSlot += c;
                }
                AssignAnswer2();
                return Answers;
            }
            arr.Add(entries);
        }
        
        /*
        for (i = 0; i < gr.Length; i++)
        {
            var c = roundedRocks.Count(r => r.Pos.R == i) * (gr.Length - i);
            NSlot += c;
        }
        AssignAnswer1();
        */
        
        return Answers;

        GridPos[] GetRoundedRocks() => gr.AllCellsWhere(c => c == 'O').Select(x=> x.Pos).ToArray();

        void Cycle()
        {
            RollN(); RollW(); RollS(); RollE();
            return;

            void RollN()
            {
                for (var i = 0; i < gr.Length; i++)
                {
                    for (var j = 0; j < gr[0].Length; j++)
                    {
                        if (gr[i][j] != 'O')
                        {
                            continue;
                        }
                        var offset = 0;
                        while (i - offset > 0)
                        {
                            if (gr.TryGetNeighborCellUp(i-offset, j, out var nb))
                            {
                                if (nb == '.')
                                {
                                    gr[i-offset][j] = '.';
                                    gr[i-offset-1][j] = 'O';
                                    offset++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            void RollS()
            {
                for (var i = gr.Length-1; i >= 0; i--)
                {
                    for (var j = 0; j < gr[0].Length; j++)
                    {
                        if (gr[i][j] != 'O')
                        {
                            continue;
                        }
                        var offset = 0;
                        while (i + offset < gr.Length - 1)
                        {
                            if (gr.TryGetNeighborCellDown(i+offset, j, out var nb))
                            {
                                if (nb == '.')
                                {
                                    gr[i+offset][j] = '.';
                                    gr[i+offset+1][j] = 'O';
                                    offset++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            void RollW()
            {
                for (var i = 0; i < gr.Length; i++)
                {
                    for (var j = 0; j < gr[0].Length; j++)
                    {
                        if (gr[i][j] != 'O')
                        {
                            continue;
                        }
                        var offset = 0;
                        while (j-offset > 0)
                        {
                            if (gr.TryGetNeighborCellLeft(i, j-offset, out var nb))
                            {
                                if (nb == '.')
                                {
                                    gr[i][j-offset] = '.';
                                    gr[i][j-offset-1] = 'O';
                                    offset++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            void RollE()
            {
                for (var i = 0; i < gr.Length; i++)
                {
                    for (var j = gr[0].Length-1; j >= 0; j--)
                    {
                        if (gr[i][j] != 'O')
                        {
                            continue;
                        }
                        var offset = 0;
                        while (j + offset < gr[0].Length-1)
                        {
                            if (gr.TryGetNeighborCellRight(i, j+offset, out var nb))
                            {
                                if (nb == '.')
                                {
                                    gr[i][j+offset] = '.';
                                    gr[i][j+offset+1] = 'O';
                                    offset++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    
    bool Is(int x, int y)
    {
        while (x < 1000000000-1)
        {
            x += y;
            if (x == 1000000000-1)
            {
                return true;
            }
        }
        return false;
    }
    
}