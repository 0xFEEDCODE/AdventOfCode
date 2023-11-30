using System.Diagnostics;
using Framework;

namespace Challenges2021;

public class Solution11 : SolutionFramework
{
    public Solution11() : base(11) { }

    struct Cell
    {
        public int Energy;
        public bool Flashing;

        public bool IncrementLock;

        public Cell(int energy)
        {
            Energy = energy;
            Flashing = false;
            IncrementLock=false;
        }

        public void IncreaseEnergy()
        {
            Energy++;
            Energy %= 10;
            Flashing = Energy == 0;
        }

        public bool IncreaseEnergyIfNotFlashing()
        {
            if (Flashing)
                return false;

            IncreaseEnergy();
            
            if (Flashing)
                IncrementLock=true;

            return Flashing;
        }
    }

    public override string[] Solve()
    {
        int i = 0, j = 0;
        int W = 10, H = 10;
        var grid = new Cell[H][];
        for (i = 0; i < grid.Length; i++)
        {
            grid[i] = new Cell[W];
        }

        i = 0;
        foreach (var line in RawInput.SplitByNewline())
        {
            j = 0;
            foreach (var octo in line)
            {
                grid[i][j++] = new Cell(int.Parse(octo.ToString()));
            }
            i++;
        }

        var flashes = 0;
        var firstStepWhenAllCellsFlash = -1;
        var stepsTaken = 0;

        var answ1 = 0;

        while (stepsTaken < 1000)
        {
            var allCellsFlash = true;

            grid.ForEachCell((i, j) =>
            {
                if (!grid[i][j].Flashing)
                    grid[i][j].IncreaseEnergy();
                
                if (grid[i][j].Flashing&&!grid[i][j].IncrementLock)
                {
                    IncrementAdjacentCells(grid, i, j);
                    grid[i][j].IncrementLock=true;
                }
            });
            stepsTaken++;
            
            grid.ForEachCell((i, j) =>
                {
                    if (grid[i][j].Flashing)
                        flashes++;
                    else
                        allCellsFlash = false;

                    grid[i][j].Flashing = false;
                    grid[i][j].IncrementLock = false;
                }
            );

            if (allCellsFlash)
            {
                firstStepWhenAllCellsFlash = stepsTaken;
                break;
            }

            if (stepsTaken == 100)
                answ1 = flashes;
        }
        
        
        AssignAnswer1(answ1);
        AssignAnswer2(firstStepWhenAllCellsFlash);

        return Answers;
    }

    private static void IncrementAdjacentCells(Cell[][] grid, int i, int j)
    {
        Extensions.EncloseInTryCatch(() => { if (grid[i+1][j+1].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i+1, j+1); });
        Extensions.EncloseInTryCatch(() => { if (grid[i-1][j-1].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i-1, j-1); });

        Extensions.EncloseInTryCatch(() => { if (grid[i+1][j].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i+1, j); });
        Extensions.EncloseInTryCatch(() => { if (grid[i-1][j].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i-1, j); });

        Extensions.EncloseInTryCatch(() => { if (grid[i][j+1].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i, j+1); });
        Extensions.EncloseInTryCatch(() => { if (grid[i][j-1].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i, j-1); });

        Extensions.EncloseInTryCatch(() => { if (grid[i+1][j-1].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i+1, j-1); });
        Extensions.EncloseInTryCatch(() => { if (grid[i-1][j+1].IncreaseEnergyIfNotFlashing()) IncrementAdjacentCells(grid, i-1, j+1); });
    }

    private void printBoard(Cell[][] board)
    {
        int i = 0, j = 0;
        Debug.WriteLine("--------------------");
        for (i = 0; i < board.Length; i++)
        {
            Debug.WriteLine("");
            for (j = 0; j < board[i].Length; j++)
            {
                Debug.Write(board[i][j].Energy);
            }
        }
    }
}
