using Framework;

namespace Challenges2020;

public class Solution5 : SolutionFramework {
    public Solution5() : base(5) { }

    public override string[] Solve() {
        var seatIds = new List<long>();
        foreach (var line in InputNlSplit) {
            var rowLower = 0;
            var rowUpper = 127;
            
            var colLower = 0;
            var colUpper = 7;
            
            foreach (var ch in line) {
                switch (ch) {
                    case 'F':
                        rowUpper = TakeLowerHalf(rowLower, rowUpper);
                        break;
                    case 'B':
                        rowLower = TakeUpperHalf(rowLower, rowUpper);
                        break;
                    case 'L':
                        colUpper = TakeLowerHalf(colLower, colUpper);
                        break;
                    case 'R':
                        colLower = TakeUpperHalf(colLower, colUpper);
                        break;
                }
            }

            int? seatRow = (rowLower == rowUpper) ? rowLower : null;
            int? seatCol = (colLower == colUpper) ? colLower : null;
            if (seatRow is null || seatCol is null) {
                throw new InvalidOperationException();
            }
            
            seatIds.Add((seatRow.Value * 8) + seatCol.Value);
        }
        AssignAnswer1(seatIds.Max());

        var dict = seatIds.ToDictionary(id => id);
        for (var i = seatIds.Min(); i < seatIds.Max(); i++) {
            if (!dict.ContainsKey(i + 1)) {
                AssignAnswer2(i+1);
                break;
            }
        }
        
        return Answers;
    }

    private static int TakeUpperHalf(int rowLower, int rowUpper) => rowLower + ((rowUpper - rowLower) / 2) + 1;
    private static int TakeLowerHalf(int rowLower, int rowUpper) => rowLower + ((rowUpper - rowLower) / 2);
}
