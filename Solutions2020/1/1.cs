using Framework;

namespace Challenges2020;

public class Solution1 : SolutionFramework
{
    public Solution1() : base(1) { }

    public override string[] Solve() {
        var inputNums = InputNlSplit.Select(int.Parse);
        var numsToProcess = inputNums as int[] ?? inputNums.ToArray();
        var answerFound = false;
        foreach (var n in numsToProcess.ToArray()) {
            foreach (var otherN in numsToProcess.Skip(1)) {
                if (n + otherN == 2020) {
                    AssignAnswer1(n*otherN);
                    answerFound = true;
                    break;
                }
            }
            if (answerFound) {
                break;
            }
            numsToProcess = numsToProcess.Skip(1).ToArray();
        }
        
        
        //Pt 2
        
        inputNums = InputNlSplit.Select(int.Parse);
        numsToProcess = inputNums as int[] ?? inputNums.ToArray();
        answerFound = false;
        foreach (var n in numsToProcess.ToArray()) {
            foreach (var otherN in numsToProcess.Skip(1)) {
                foreach (var otherN2 in numsToProcess.Skip(2)) {
                    if ((n + otherN + otherN2) == 2020) {
                        AssignAnswer2(n * otherN * otherN2);
                        answerFound = true;
                        break;
                    }
                }

                if (answerFound) {
                    break;
                }

            }
            numsToProcess = numsToProcess.Skip(1).ToArray();
        }

        return Answers;
    }
}
