using Framework;

namespace Solutions2023;

public class Solution3 : SolutionFramework
{
    public Solution3() : base(3) { }

    record Pos(int X, int Y);
    public override string[] Solve()
    {
        var gr = (RawInputSplitByNl.Length, RawInputSplitByNl[0].Length).CreateGrid<char>();
        gr.ForEachCell((i, j) =>
        {
            gr[i][j] = RawInputSplitByNl[i][j];
        });
        
        var nPositions = new List<(int, List<Pos>)>();
        
        var n = string.Empty;
        var nP = new List<Pos>();
        var parsingN = false;
        gr.ForEachCell((i, j) =>
        {
            if (Char.IsDigit(gr[i][j]))
            {
                nP.Add(new Pos(i,j));
                n += gr[i][j];
                parsingN = true;
            } else
            {
                if (parsingN)
                {
                    nPositions.Add((n.ParseInt(), nP));
                    nP = new List<Pos>();
                    n = string.Empty;
                }
                parsingN = false;
            }
        });

        bool IsSymbol(char c)
        {
            return !char.IsDigit(c) && c != '.';
        }
        
        
        foreach (var vk in nPositions)
        {
            var np = vk.Item2;
            var valid = false;
            foreach (var pos in np)
            {
                Extensions.EncloseInTryCatch(() => {
                    if (IsSymbol(gr.NeighborCellDown(pos.X, pos.Y))) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellLeft(pos.X, pos.Y))) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellRight(pos.X, pos.Y)) ) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellUp(pos.X, pos.Y)) ) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellBottomLeft(pos.X, pos.Y)) ) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellBottomRight(pos.X, pos.Y)) ) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellTopLeft(pos.X, pos.Y)) ) { valid = true; }
                });
                Extensions.EncloseInTryCatch(() =>
                {
                    if (IsSymbol(gr.NeighborCellTopRight(pos.X, pos.Y)) ) { valid = true; }
                });
                if (valid)
                {
                    NumSlot+=vk.Item1;
                    break;
                }
            }

        }
        AssignAnswer1(NumSlot, true);
        
        n = string.Empty;
        nP = new List<Pos>();
        parsingN = false;
        NumSlot = 0;
        gr.ForEachCell((i, j) =>
        {
            if (gr[i][j] == '*')
            {
                var found = nPositions.Where(np =>
                {
                    var r = new Pos(i, j + 1);
                    var l = new Pos(i, j - 1);
                    var u = new Pos(i - 1, j);
                    var d = new Pos(i + 1, j);
                    var d1 = new Pos(i + 1, j + 1);
                    var d2 = new Pos(i + 1, j - 1);
                    var d3 = new Pos(i - 1, j + 1);
                    var d4 = new Pos(i - 1, j - 1);
                    return (np.Item2.Contains(r) ||
                        np.Item2.Contains(l) ||
                        np.Item2.Contains(u) ||
                        np.Item2.Contains(d) ||
                        np.Item2.Contains(d1) ||
                        np.Item2.Contains(d2) ||
                        np.Item2.Contains(d3) ||
                        np.Item2.Contains(d4));
                });
                if (found.Count() == 2)
                {
                    NumSlot += found.First().Item1 * found.Last().Item1;
                }
            }
        });
        AssignAnswer2(NumSlot, true);
        
        Console.WriteLine();
        return Answers;
    }
    
    private static void PrintGrid(char[][] grid) {
        for (int i = 0; i < grid.Length; i++) {
            Console.WriteLine();
            for (int j = 0; j < grid[i].Length; j++) {
                Console.WriteLine(grid[i][j]);
            }
        }
    }
}
