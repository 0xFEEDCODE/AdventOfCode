using Framework;

namespace Challenges2020;

public enum Equality
{
    LT,
    GT,
    EQ
}

public class Solution9 : SolutionFramework
{
    public Solution9() : base(9) { }


    public override string[] Solve()
    {
        var nums = new List<long>();

        foreach (var line in InputNlSplit)
        {
            nums.Add(long.Parse(line));
        }

        Func<int, int, List<long>> getPrevious = (idx, n) => nums.Skip(idx - n).Take(n).ToList();

        const int preambleLen = 25;
        long? invalidN = null;
        var currentIdx = preambleLen;
        while (!invalidN.HasValue && currentIdx++ < nums.Count())
        {
            var currentN = nums[currentIdx];
            var previousN = getPrevious(currentIdx, preambleLen);
            var localIdx = 0;
            var nFound = false;
            for (var i = 0; i < preambleLen; i++)
            {
                var thisN = previousN[i];
                if (previousN.Skip(i).Any(otherN => (thisN + otherN) == currentN))
                {
                    nFound = true;
                    break;
                }
            }

            if (!nFound)
            {
                invalidN = currentN;
            }
        }
        
        AssignAnswer1(invalidN.Value);

        var slidingWindow = new Queue<long>();

        var windowEq = slidingWindow.GetEq(invalidN.Value);
        currentIdx = 0;
        while (windowEq != Equality.EQ)
        {
            switch (windowEq)
            {
                case Equality.LT:
                    slidingWindow.Enqueue(nums[currentIdx++]);
                    break;
                case Equality.GT:
                    slidingWindow.Dequeue();
                    break;
            }

            windowEq = slidingWindow.GetEq(invalidN.Value);
        }

        AssignAnswer2(slidingWindow.Min() + slidingWindow.Max());
        return Answers;
    }
}

public static partial class Ext
{
    public static Equality GetEq(this Queue<long> window, long comp)
    {
        var windowSum = window.Sum();
        if (windowSum == comp)
        {
            return Equality.EQ;
        }

        return windowSum < comp ? Equality.LT : Equality.GT;
    }
}
