using Framework;

namespace Solutions2023;

public class Solution3 : SolutionFramework
{
    public Solution3() : base(3) { }

    public override string[] Solve()
    {
        var gr = (RawInputSplitByNl.Length, RawInputSplitByNl[0].Length).CreateGrid<char>();
        gr.ForEachCell((i, j) =>
        {
            gr[i][j] = RawInputSplitByNl[i][j];
        });
        
        var numWithPositions = new List<(int, List<Pos2D>)>();
        
        var n = string.Empty;
        var nP = new List<Pos2D>();
        var parsingN = false;
        gr.ForEachCell(pos =>
        {
            var cell = gr.GetCell(pos);
            if (Char.IsDigit(cell))
            {
                nP.Add(pos);
                n += cell;
                parsingN = true;
            } else
            {
                if (parsingN)
                {
                    numWithPositions.Add((n.ParseInt(), nP));
                    nP = new List<Pos2D>();
                    n = string.Empty;
                }
                parsingN = false;
            }
        });

        bool IsSymbol(char c)
        {
            return !char.IsDigit(c) && c != '.';
        }
        
        foreach (var (num, positions) in numWithPositions)
        {
            var valid = false;
            foreach (var pos in positions)
            {
                if (gr.IsAnyNeighbor(pos, IsSymbol))
                {
                    NumSlot+=num;
                    break;
                }
            }

        }
        AssignAnswer1();
        
        n = string.Empty;
        nP = new List<Pos2D>();
        parsingN = false;
        gr.ForEachCell(pos =>
        {
            var cell = gr.GetCell(pos);
            if (cell == '*')
            {
                var adjacent = gr.GetAllAdjacentCells(pos);
                var found = numWithPositions.Where(np => adjacent.Any(vk => np.Item2.Contains(vk.pos)));
                if (found.Count() == 2)
                {
                    NumSlot += found.First().Item1 * found.Last().Item1;
                }
            }
        });
        AssignAnswer2();
        
        return Answers;
    }
}
