using Framework;

namespace Challenges2020;

public class Solution3 : SolutionFramework {
    public Solution3() : base(3) { }

    enum Field {
        Tree,
        Open
    }

    record struct TraverseInstruction(int Right, int Down);

    public override string[] Solve() {
        var patternRepeatCount = 100;
        var grid = (RawInputSplitByNl.Length, RawInputSplitByNl[0].Length * patternRepeatCount).CreateGrid<Field>();
        var row = 0;
        foreach (var line in RawInputSplitByNl) {
            var col = 0;
            foreach (var ch in line) {
                for (var rep = 0; rep < patternRepeatCount; rep++) {
                    grid[row][col+(rep*line.Length)] = ch == '.' ? Field.Open : Field.Tree;
                }
                col++;
            }
            row++;
        }

        var bottomReached = false;
        var pos = (0, 0);
        var treesEncountered = 0;
        while (!bottomReached) {
            if (grid[pos.Item1][pos.Item2] is Field.Tree) {
                treesEncountered++;
            }

            pos = (pos.Item1 + 1, pos.Item2 + 3);
            bottomReached = pos.Item1 >= grid.Length;
        }
        AssignAnswer1(treesEncountered);

        var treesEncounteredTraversing = new List<long>();
        foreach (var traverseInstruction in new TraverseInstruction[] { new(1, 1), new(3, 1), new(5, 1), new(7, 1), new(1, 2)}) {
            bottomReached = false;
            pos = (0, 0);
            treesEncountered = 0;
            while (!bottomReached) {
                if (grid[pos.Item1][pos.Item2] is Field.Tree) {
                    treesEncountered++;
                }

                pos = (pos.Item1 + traverseInstruction.Down, pos.Item2 + traverseInstruction.Right);
                bottomReached = pos.Item1 >= grid.Length;
            }

            treesEncounteredTraversing.Add(treesEncountered);
        }
        AssignAnswer2(treesEncounteredTraversing.Aggregate((x,acc) => x*acc));

        // for (var i = 0; i < grid.Length; i++) {
        //     Console.WriteLine();
        //     for (var j = 0; j < grid[i].Length; j++) {
        //         Console.Write(grid[i][j] is Field.Tree ? '#' : '.');
        //     }
        // }
        
        return Answers;
    }
}
