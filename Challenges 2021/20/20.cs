using System.Text;

using Framework;

namespace Challenges2021;

public static partial class Ext {
    public static string PixelsAround(this int[][] grid, int i, int j) {
        var pixels = new StringBuilder();

        for (var v = -1; v <= 1; v++) {
            for (var h = -1; h <= 1; h++) {
                try { pixels.Append(grid[i + v][j + h].ToString()); } catch { pixels.Append('0'); }
            }
        }

        return pixels.ToString();
    }

    public static int BinToDec(this string bits) {
        var mul = 1;
        var n = 0;
        foreach(var bit in bits.Reverse()) {
            n += mul * int.Parse(bit.ToString());
            mul *= 2;
        }
        return n;
    }
    
}

public class Solution20 : SolutionFramework {
    public Solution20() : base(20) { }
    
    public override string[] Solve() {
        var alg = RawInputSplitByNl.First().Select(x=> {
            return x switch {
                '.' => 0,
                '#' => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
            };
        }).ToArray();
        const int h = 400,w = 400;
        var inputImage = (h, w).CreateGrid<int>();
        var outputImage = (h, w).CreateGrid<int>();
        inputImage.SetAllCellsToValue(0);
        outputImage.SetAllCellsToValue(0);

        var i = 150;
        foreach (var line in RawInputSplitByNl.Skip(2)) {
            var j = 150;
            foreach (var ch in line) {
                inputImage[i][j] = ch switch {
                    '.' => 0,
                    '#' => 1,
                    _ => throw new ArgumentOutOfRangeException()
                };
                j++;
            }
            i++;
        }

        var litAfter2 = 0;
        var litAfter50 = 0;
        for (var r = 1; r < 100; r++) {
            var lit = 0;
            for (i = 0; i < inputImage.Length; i++) {
                for (var j = 0; j < inputImage[i].Length; j++) {
                    outputImage[i][j] = alg[inputImage.PixelsAround(i, j).BinToDec()];
                    if (outputImage[i][j] == 1 && i > 80 && j > 80) {
                        lit++;
                    }
                }
            }

            if (r == 2) {
                litAfter2 = lit;
            }

            if (r == 50) {
                litAfter50 = lit;
                break;
            }

            outputImage.ForEachCell((i,j)=>inputImage[i][j] = outputImage[i][j]);
        }

        AssignAnswer1(litAfter2);
        AssignAnswer2(litAfter50);

        return Answers;
    }
}
