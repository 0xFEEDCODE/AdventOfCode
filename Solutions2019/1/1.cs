using Framework;

namespace Challenges2019;

public class Solution1 : SolutionFramework
{
    public Solution1() : base(1) { }

    public override string[] Solve()
    {
        double Fn(double x) => Math.Floor(x / 3) - 2;
        
        double sum = 0;
        foreach (var mass in DoubleInputSplitByNl)
        {
            sum += Fn(mass);
        }

        AssignAnswer1(sum);
        sum = 0;

        foreach (var mass in DoubleInputSplitByNl)
        {
            var temp = mass;
            while ((temp = Fn(temp)) > 0)
            {
                sum += temp;
            }
        }
        
        AssignAnswer2(sum);

        return Answers;
    }
}
