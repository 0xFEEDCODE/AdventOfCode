using Framework;

namespace Challenges2022;

public class Solution1 : SolutionFramework
{
    public Solution1() : base(1) { }

    public override string[] Solve()
    {
        var splitByGroups = RawInput.SplitByGroup();

        var allSums = new List<int>();

        var biggestSum = 0;
        foreach (var group in splitByGroups)
        {
            var localSum = 0;
            foreach (var entry in group.Split("\r\n"))
            {
                if (int.TryParse(entry, out var num))
                {
                    localSum += num;
                }
            }

            allSums.Add(localSum);
            biggestSum = biggestSum > localSum ? biggestSum : localSum;
        }

        Answers[0] = biggestSum.ToString();
        Answers[1] = allSums.OrderByDescending(x => x).Take(3).Sum().ToString();

        return Answers;
    }
}
