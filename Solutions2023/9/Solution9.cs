using Framework;

namespace Solutions2023;

public static class Ext
{
    public static double[] GetDiffs(this double[] src)
    {
        var diffs = new List<double>();
        for (var i = 0; i < src.Length-1; i++)
        {
            diffs.Add(src[i+1]-src[i]);
        }
        return diffs.ToArray();
    }

    public static double Extrapolate(this Dictionary<int, double[]> dict)
    {
        var prev = 0d;
        for (var i = dict.Count - 1; i > 1; i--)
        {
            prev = dict[i - 1][^1] + prev;
        }
        return dict[0][^1]+prev;
    }
    
    public static double ExtrapolateRev(this Dictionary<int, double[]> dict)
    {
        var prev = 0d;
        for (var i = dict.Count - 1; i > 1; i--)
        {
            prev = dict[i - 1][0] - prev;
        }
        return dict[0][0]-prev;
    }
}

public class Solution9 : SolutionFramework
{
    public Solution9() : base(9) { }
    
    public override string[] Solve()
    {
        var history = InpNl.Select(x => x.Split(" ").Select(double.Parse).ToArray()).ToArray();
        var ans1 = 0d;
        var ans2 = 0d;
        foreach (var hist in history)
        {
            var dict = new Dictionary<int, double[]>();
            var diffs = hist.GetDiffs();
            
            dict[0] = hist;
            var i = 1;
            dict[i] = diffs;
            while (diffs.Any(d => d != 0))
            {
                i++;
                diffs = diffs.GetDiffs();
                dict[i] = diffs;
            }

            ans1 += dict.Extrapolate();
            ans2 += dict.ExtrapolateRev();
        }
        
        AssignAnswer1(ans1);
        AssignAnswer2(ans2);
        
        return Answers;
    }
}