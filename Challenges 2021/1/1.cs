using Framework;

namespace Challenges2021;

public class Solution1 : SolutionFramework
{
    public Solution1() : base(1) { }

    public override string[] Solve() {
        int? previousN = null;
        var biggerCount = 0;
        foreach (var item in RawInputSplitByNl) {
            var n = int.Parse(item);
            if (n > previousN)
                biggerCount++;
            previousN = n;
        }
        AssignAnswer1(biggerCount);
        
        // pt 2
        var window = new Queue<int>();
        biggerCount = 0;
        foreach (var item in RawInputSplitByNl) {
            var n = int.Parse(item);
            window.Enqueue(n);
            if (window.Count() == 4) {
                if (window.Take(3).Sum() < window.Skip(1).Take(3).Sum()) {
                    biggerCount++;
                }

                window.Dequeue();
            }
        }
        AssignAnswer2(biggerCount);

        return Answers;
    }
}
