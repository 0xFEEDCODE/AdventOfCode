using System.Text.RegularExpressions;

using Framework;

namespace Challenges2021;

public class Solution13 : SolutionFramework {
    public Solution13() : base(13) { }

    public record struct Coord(int Row, int Col);
    public record struct FoldInstr(int? X, int? Y);

    public override string[] Solve() {
        var coords = new List<Coord>();
        var folds = new List<FoldInstr>();
        foreach (var line in RawInputSplitByNl) {
            var matches = Regex.Matches(line, @"\d+");
            if (!matches.Any()) {
                continue;
            }
            
            if (line.Contains("Fold", StringComparison.InvariantCultureIgnoreCase)) {
                if (line.Contains("y")) {
                    folds.Add(new FoldInstr(null, int.Parse(matches[0].Value)));
                } else {
                    folds.Add(new FoldInstr(int.Parse(matches[0].Value), null));
                }
            }
            else{
                coords.Add(new Coord(int.Parse(matches[1].Value), int.Parse(matches[0].Value)));
            }
        }

        var grid = (coords.MaxBy(c => c.Row).Row+1, coords.MaxBy(c => c.Col).Col+1).CreateGrid<bool>();
        foreach (var c in coords) {
            grid[c.Row][c.Col] = true;
        }

        Fold(folds.Take(1).ToArray(), grid);
        AssignAnswer2(grid.Sum(x=>x.Count(y=>y)));
        
        //pt2
        Fold(folds.Skip(1).ToArray(), grid);
        PrintGrid(grid);
        //Answer 2 -> JRZBLGKH

        return Answers;
    }

    public static List<int> FoldedCols = new();
    public static List<int> FoldedRows = new();

    private static void Fold(ICollection<FoldInstr> folds, bool[][] grid) {
        foreach (var fold in folds) {
            switch (fold.Y.HasValue, fold.X.HasValue) {
                case (true, false):
                    for (var i = grid.Length - 1; i >= fold.Y!.Value; i--) {
                        var rowWhenFolded = fold.Y.Value - (i - fold.Y.Value);
                        for (var j = 0; j < grid[i].Length; j++) {
                            if (!grid[i][j]) {
                                continue;
                            }

                            grid[rowWhenFolded][j] = true;
                            grid[i][j] = false;
                        }
                        FoldedRows.Add(i);
                    }

                    break;
                case (false, true):
                    for (var j = grid[0].Length - 1; j >= fold.X!.Value; j--) {
                        var colWhenFolded = fold.X!.Value - (j - fold.X.Value);
                        for (var i = 0; i < grid.Length; i++) {
                            if (!grid[i][j]) {
                                continue;
                            }

                            grid[i][colWhenFolded] = true;
                            grid[i][j] = false;
                        }
                        FoldedCols.Add(j);
                    }

                    break;
            }
        }
    }

    private static void PrintGrid(bool[][] grid) {
        for (var i = 0; i < grid.Length; i++) {
            if (FoldedRows.Any(r => r == i)) {
                continue;
            }
            Console.WriteLine();
            for (var j = 0; j < grid[0].Length; j++) {
                if (FoldedCols.Any(c => c == j)) {
                    continue;
                }
                if (grid[i][j]) {
                    Console.Write("#");
                }
                else {
                    Console.Write(" ");
                }
            }
        }
    }
}
