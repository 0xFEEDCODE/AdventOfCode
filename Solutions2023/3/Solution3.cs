using Framework;

namespace Solutions2023;

public class Solution3 : SolutionFramework
{
    public Solution3() : base(3) { }

    public override string[] Solve()
    {
        var grid = InputAsGrid<char>();
        var numWithPositions = new List<(int, List<GridPos>)>();
        
        var n = string.Empty;
        var nP = new List<GridPos>();
        var parsingN = false;
        grid.ForEachCell(pos =>
        {
            var cell = grid.GetCell(pos);
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
                    nP = new List<GridPos>();
                    n = string.Empty;
                }
                parsingN = false;
            }
        });

        bool IsSymbol(char c) => !char.IsDigit(c) && c != '.';
        
        foreach (var (num, positions) in numWithPositions)
        {
            var valid = false;
            foreach (var pos in positions)
            {
                if (grid.IsAnyNeighbor(pos, IsSymbol))
                {
                    NSlot+=num;
                    break;
                }
            }

        }
        AssignAnswer1();
        
        n = string.Empty;
        nP = new List<GridPos>();
        parsingN = false;
        grid.ForEachCell(pos =>
        {
            var cell = grid.GetCell(pos);
            if (cell == '*')
            {
                var adjacent = grid.GetAllAdjacentCells(pos);
                var found = numWithPositions.Where(np => adjacent.Any(vk => np.Item2.Contains(vk.pos)));
                if (found.Count() == 2)
                {
                    NSlot += found.First().Item1 * found.Last().Item1;
                }
            }
        });
        AssignAnswer2();
        
        return Answers;
    }
}
