using System.Text.RegularExpressions;

using Framework;

using static Challenges2021.Solution4;

namespace Challenges2021;


public static partial class Ext{
    public static MarkingCheck AnyColAllMarked(this bool[][] bingoBoard) {
        for (var j = 0; j < bingoBoard[0].Length; j++) {
            var allMarked = true;
            for (var i = 0; i < bingoBoard.Length; i++) {
                allMarked = bingoBoard[i][j];
                if (!allMarked)
                    break;
            }

            if (allMarked)
                return new MarkingCheck(null, j, true);
        }
        return new MarkingCheck();
    }
    public static MarkingCheck AnyRowAllMarked(this bool[][] bingoBoard) {
        for (var i = 0; i < bingoBoard.Length; i++) {
            var allMarked = true;
            for (var j = 0; j < bingoBoard[i].Length; j++) {
                allMarked = bingoBoard[i][j];
                if (!allMarked)
                    break;
            }

            if (allMarked)
                return new MarkingCheck(i, null, true);
        }
        return new MarkingCheck();
    }
}

public class Solution4 : SolutionFramework {

    public Solution4() : base(4) { }

    public int SumOfAllUnmarked(BingoBoard board) {
        var sum = 0;
        board.Numbers.ForEachCell((i, j) => {
            if (board.MarkedMappings[i][j] == false) {
                sum += board.Numbers[i][j];
            }
        });

        return sum;
    }

    public record struct BingoBoard(int[][] Numbers, bool[][] MarkedMappings, int Nr);
    public record struct MarkingCheck(int? MarkedRow = null, int? MarkedCol = null, bool AnyMarked = false, int? BoardNr = null);

    public override string[] Solve() {

        const string pattern = @"\b(\d+)\b";

        var bingoBoards = new List<BingoBoard>();

        var numbersToDraw = Regex.Matches(RawInputSplitByNl.First(), pattern).Select(x => int.Parse(x.Value)).ToArray();

        var row = 0;
        var bingoBoard = (5, 5).CreateGrid<int>();
        var boardMarked = (5, 5).CreateGrid<bool>();
        var boardNr = 0;
        foreach (var line in RawInputSplitByNl.Skip(1)) {
            var matches = Regex.Matches(line, pattern);
            if (matches.Count == 0)
                continue;
            var bingoN = matches.Select(x => int.Parse(x.Value)).ToArray();
            boardMarked[row] = new[] { false, false, false, false, false };
            bingoBoard[row++] = new[] { bingoN[0], bingoN[1], bingoN[2], bingoN[3], bingoN[4] };

            if (row > bingoBoard.Length - 1) {
                row = 0;
                bingoBoards.Add(new BingoBoard(bingoBoard, boardMarked, boardNr));
                bingoBoard = (5, 5).CreateGrid<int>();
                boardMarked = (5, 5).CreateGrid<bool>();
                boardNr++;
            }
        }

        int? score = null;
        var previousNDrawn = 0;
        var nToDraw = 5;
        while (previousNDrawn < numbersToDraw.Length && !score.HasValue) {
            var drawn = numbersToDraw.Skip(previousNDrawn).Take(nToDraw).ToArray();
            foreach (var n in drawn) {
                foreach (var board in bingoBoards) {
                    board.Numbers.ForEachCell((i, j) => {
                        if (!score.HasValue) {
                            if (board.Numbers[i][j] == n) {
                                board.MarkedMappings[i][j] = true;
                            }

                            var colMarkedCheck = board.MarkedMappings.AnyColAllMarked();
                            var rowMarkedCheck = board.MarkedMappings.AnyRowAllMarked();
                            if (colMarkedCheck.AnyMarked || rowMarkedCheck.AnyMarked) {
                                score = SumOfAllUnmarked(board) * n;
                            }
                        }
                    });
                    if (score.HasValue) break;
                }
            }

            previousNDrawn += nToDraw;
            nToDraw++;
        }
        
        AssignAnswer1(score.Value);
        
        //pt 2
        score = null;
        
        previousNDrawn = 0;
        nToDraw = 5;
        var winningBoards = new List<BingoBoard>();
        int? lastN = null;
        while (previousNDrawn < numbersToDraw.Length) {
            var drawn = numbersToDraw.Skip(previousNDrawn).Take(nToDraw).ToArray();
            foreach (var n in drawn) {
                boardNr = 0;
                foreach (var board in bingoBoards) {
                    var isWinningBoard = false;
                    if (winningBoards.Any(b => b.Nr == boardNr)) {
                        boardNr++;
                        continue;
                    }

                    board.Numbers.ForEachCell((i, j) => {
                        if (!isWinningBoard) {
                            if (board.Numbers[i][j] == n) {
                                board.MarkedMappings[i][j] = true;
                            }

                            var colMarkedCheck = board.MarkedMappings.AnyColAllMarked();
                            var rowMarkedCheck = board.MarkedMappings.AnyRowAllMarked();
                            if (colMarkedCheck.AnyMarked || rowMarkedCheck.AnyMarked) {
                                winningBoards.Add(board with { Nr = boardNr });
                                isWinningBoard = true;
                                lastN = n;
                            }
                        }
                    });
                    boardNr++;
                }
            }

            previousNDrawn += nToDraw;
            nToDraw++;
        }
        
        score = SumOfAllUnmarked(winningBoards.Last()) * lastN.Value;
        
        AssignAnswer2(score.Value);

        return Answers;
    }
}
