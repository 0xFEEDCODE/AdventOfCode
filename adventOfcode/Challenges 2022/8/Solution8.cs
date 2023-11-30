using Framework;

namespace Challenges2022;

public class Solution8 : SolutionFramework
{
    public Solution8() : base(8)
    {
    }

    public override string[] Solve()
    {
        var H = 99;
        var W = "113003322412033102023303501444545044215232525401341546163452453404402234201151432242402140110220101".Length;
        int FirstRow, FirstCol;
        FirstRow = FirstCol = 0;
        var LastRow = H - 1;
        var LastCol = W - 1;

        var grid = (H, W).CreateGrid<Tree>();

        int i = 0;
        foreach (var line in RawInput.SplitByNewline())
        {
            int j = 0;
            foreach (var entry in line) grid[i][j++] = new Tree { Height = int.Parse(entry.ToString()) };
            i++;
        }

        var edgeVisible = 0;
        var interiorVisible = 0;
        grid.ForEachCell((i, j) =>
        {
            if (i == FirstRow || j == LastCol || i == LastRow || j == FirstCol)
            {
                edgeVisible++;
            }
            else
            {
                var cTree = grid[i][j];

                if (Validate(H, W, cTree, grid, i, j)) interiorVisible++;
            }
        });

        var allVisible = interiorVisible + edgeVisible;
        AssignAnswer1(allVisible);


        long highestScenicScore = 0;

        grid.ForEachCell((i, j) =>
        {
            if (i == FirstRow || j == LastCol || i == LastRow || j == FirstCol)
            {
            }
            else
            {
                var cTree = grid[i][j];

                var score = GetScenicScore(H, cTree, grid, W, i, j);
                highestScenicScore.AssignIfBigger(score);
            }

        });

        AssignAnswer2(highestScenicScore);

        return Answers;
    }

    private long GetScenicScore(int H, Tree cTree, Tree[][] grid, int W, int i, int j)
    {
        long s1, s2, s3, s4;
        s1 = s2 = s3 = s4 = 0;

        Extensions.EncloseInTryCatch(() => {
            var x = i + 1;
            while (x < H)
            {
                if (cTree.Height <= grid[x][j].Height)
                {
                    s1++;
                    break;
                }

                s1++;
                x++;
            } });


        Extensions.EncloseInTryCatch(() => {
            var x = i - 1;
            while (x > -1)
            {
                if (cTree.Height <= grid[x][j].Height)
                {
                    s2++;
                    break;
                }

                s2++;
                x--;
            } });

        Extensions.EncloseInTryCatch(() => {
            var x = j + 1;
            while (x < W)
            {
                if (cTree.Height <= grid[i][x].Height)
                {
                    s3++;
                    break;
                }

                s3++;
                x++;
            } });

        Extensions.EncloseInTryCatch(() => {
            var x = j - 1;
            while (x > -1)
            {
                if (cTree.Height <= grid[i][x].Height)
                {
                    s4++;
                    break;
                }

                s4++;
                x--;
            } });

        long total = 0;
        if (s1 > 0)
            total += s1;
        if (s2 > 0)
            total *= s2;
        if (s3 > 0)
            total *= s3;
        if (s4 > 0)
            total *= s4;

        return total;
    }

    private bool Validate(int H, int W, Tree cTree, Tree[][] grid, int i, int j)
    {
        bool isVisible = false;
        Extensions.EncloseInTryCatch(() => {
            var x = i + 1;
            while (x < H)
            {
                if (cTree.Height <= grid[x][j].Height) break;

                x++;
            }

            isVisible = x == H;
        });
        if (isVisible) return isVisible;

        Extensions.EncloseInTryCatch(() => {
            var x = i - 1;
            while (x > -1)
            {
                if (cTree.Height <= grid[x][j].Height) break;

                x--;
            }

            isVisible = x  == - 1;
        });
        if (isVisible) return isVisible;

        Extensions.EncloseInTryCatch(() => {
            var x = j + 1;
            while (x < W)
            {
                if (cTree.Height <= grid[i][x].Height) break;

                x++;
            }

            isVisible = x == W;
        });
        if (isVisible) return isVisible;

        Extensions.EncloseInTryCatch(() => {
            var x = j - 1;
            while (x > -1)
            {
                if (cTree.Height <= grid[i][x].Height) break;

                x--;
            }

            isVisible = x == -1;
        });

        return isVisible;
    }

    private struct Tree
    {
        public int Height;
    }
}
