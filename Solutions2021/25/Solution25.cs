using System.Diagnostics;
using Framework;

namespace Challenges2021;

public class Solution25 : SolutionFramework
{
    public Solution25() : base(25) { }

    public enum Field
    {
        Empty,
        CSouth,
        CEast
    }

    public override string[] Solve()
    {
        var inputSplitByNl = this.RawInput.SplitByNewline();
        int H = inputSplitByNl.Length;
        // -1 for \r at end
        int W = inputSplitByNl.First().Length-1;

        //To prevent error by one
        int LastRow = H - 1;
        int LastCol = W - 1;

        var sea = new Field[H][];
        int i; int j;
        for (i = 0; i < H; i++)
        {
            sea[i] = new Field[W];
        }

        i = 0;
        foreach (var line in inputSplitByNl)
        {
            j = 0;
            foreach (var ch in line.TrimEnd('\r'))
            {
                var field = ch switch
                {
                    '.' => Field.Empty,
                    '>' => Field.CEast,
                    'v' => Field.CSouth,
                    _   => throw new Exception()
                };

                sea[i][j] = field;
                j++;
            }
            i++;
        }

        int moves = 1;
        bool hasMoved = true;

        while (hasMoved)
        {
            hasMoved = false;

            for (i = 0; i < H; i++)
            {
                bool movedFromLast = false;
                for (j = 0; j < LastCol; j++)
                {
                    if (j == 1)
                    {
                        //Edge hit -> move to the first position on left
                        if (sea[i][LastCol] == Field.CEast && sea[i][0] == Field.Empty)
                        {
                            sea[i][LastCol] = Field.Empty;
                            sea[i][0] = Field.CEast;
                            hasMoved = true;
                            movedFromLast = true;
                        }
                    }

                    var field = sea[i][j];
                    if(field == Field.CEast)
                    {
                        if (sea[i][j+1] == Field.Empty)
                        {
                            if (j == LastCol - 1 && movedFromLast)
                            {
                                break;
                            }

                            sea[i][j+1] = field;
                            sea[i][j] = Field.Empty;
                            j++;
                            hasMoved = true;
                        }
                    }
                }
            }

            for (j = 0; j < W; j++)
            {
                bool movedFromLast = false;
                for (i = 0; i < LastRow; i++)
                {
                    if (i == 1)
                    {
                        //Edge hit -> move to the first position on top
                        if (sea[LastRow][j] == Field.CSouth && sea[0][j] == Field.Empty)
                        {
                            sea[H - 1][j] = Field.Empty;
                            sea[0][j] = Field.CSouth;
                            movedFromLast = true;
                            hasMoved = true;
                        }
                    }

                    var field = sea[i][j];
                    if(field == Field.CSouth)
                    {
                        if(sea[i+1][j] == Field.Empty)
                        {
                            if (i == LastRow - 1 && movedFromLast)
                            {
                                break;
                            }

                            sea[i+1][j] = field;
                            sea[i][j] = Field.Empty;
                            i++;
                            hasMoved = true;
                        }
                    }
                }
            }

            if (hasMoved)
            {
                moves++;
            }
        }

        AssignAnswer1(moves);

        return Answers;
    }
}
