using Framework;

namespace Solutions2023;

public class Solution23() : SolutionFramework(23)
{
    enum Dir { L, D, R, U };
    public override string[] Solve()
    {
        var gr = InputAsGrid<char>();
        var start = new GridPos(0, 1);
        var end = new GridPos(gr.Length - 1, gr[0].Length - 2);

        var seen = new HashSet<GridPos>();

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var points = new List<GridPos>() { start, end };
        var conjunctions = GetConjunctions();
        points.AddRange(conjunctions);
        
        var graph = new Dictionary<GridPos, Dictionary<GridPos, int>>();
        
        foreach (var pos in points)
        {
            var connected = new Dictionary<GridPos, int>();
            graph.Add(pos, connected);
            
            seen.Clear();
            
            var st = new Stack<(GridPos, int)>();
            st.Push((pos, 0));

            while (st.Any())
            {
                var (curr, dist) = st.Pop();
                seen.Add(curr);
                
                if (dist != 0 && points.Contains(curr))
                {
                    connected[curr] = dist;
                    continue;
                }
                
                foreach (var (next, _) in gr.GetAllAdjacentCells(curr).Where(c => c.val != '#' && !seen.Contains(c.pos)))
                {
                    st.Push((next, dist + 1));
                }
            }
        }
        seen.Clear();

        var longestPath = int.MinValue;
        //Dfs2();
        var considered = 0d;
        longestPath = Dfs(start);
        Console.WriteLine(considered);
        AssignAnswer1(longestPath);
        return Answers;

        AssignAnswer1(longestPath);

        return Answers;

        void Dfs2()
        {
            var st = new Stack<(GridPos, int, bool)>();
            st.Push((start, 0, false));

            while (st.Any())
            {
                var (curr, dist, isRemoval) = st.Pop();

                if (curr == end)
                {
                    longestPath.AssignIfBigger(dist);
                }

                if (isRemoval)
                {
                    seen.Remove(curr);
                    continue;
                }

                st.Push((curr, dist, true));
                foreach (var (next, nextDist) in graph[curr])
                {
                    if (seen.Contains(next))
                    {
                        continue;
                    }
                    st.Push((next, nextDist + dist, false));
                }
                seen.Add(curr);
            }
        }
            
        int Dfs(GridPos point)
        {
            if (point == end)
            {
                return 0;
            }

            var dist = int.MinValue;

            seen.Add(point);
            foreach (var (next, nextDist) in graph[point])
            {
                if (seen.Contains(next))
                {
                    continue;
                }
                dist = Math.Max(dist, Dfs(next) + nextDist);
            }
            seen.Remove(point);
            
            return dist;
        }

        IEnumerable<GridPos> GetConjunctions()
        {
            var conjunctions = new List<GridPos>();
            
            for (var i = 0; i < gr.Length; i++)
            {
                for (var j = 0; j < gr[0].Length; j++)
                {
                    var pos = new GridPos(i, j);
                    gr.GetCell(pos);
                    if (gr[i][j] != '#')
                    {
                        var pathsCount = gr.GetAllAdjacentCells(pos).Count(c => c.val != '#');
                        if (pathsCount > 2)
                        {
                            conjunctions.Add(pos);
                        }
                    }
                }
            }
            return conjunctions;
        }
    }
}
