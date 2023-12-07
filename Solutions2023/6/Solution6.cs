using Framework;

namespace Solutions2023;

public class Solution6 : SolutionFramework
{
    public Solution6() : base(6) { }

    public override string[] Solve()
    {
        var times = InpNl[0].Split(" ").Where(x => x.Length > 0 && Char.IsDigit(x[0])).Select(double.Parse).ToArray();
        var distances = InpNl[1].Split(" ").Where(x => x.Length > 0 && Char.IsDigit(x[0])).Select(double.Parse).ToArray();

        AssignAnswer1(GetNWays(times, distances));
        
        var mergedTime = times.Aggregate((acc, x) => double.Parse(acc.ToString() + x));
        var mergedDist = distances.Aggregate((acc, x) => double.Parse(acc.ToString() + x));

        AssignAnswer2(GetNWays(mergedTime, mergedDist));
        return Answers;
    }
    
    double CalcDistanceTraveled(double velocity, double timeTaken, double totalTime) => velocity * (totalTime - timeTaken);
    
    double GetNWays(double times, double distances) => GetNWays(new[] { times }, new[] { distances });
    double GetNWays(double[] times, double[] distances) =>
        times.Zip(distances).Select(r =>
        { var count = 0; for (double i = 0; i < r.First; i++) { if (CalcDistanceTraveled(i, i, r.First) > r.Second) { count++; } } return count; })
            .Aggregate((acc, x) => acc * x);
}