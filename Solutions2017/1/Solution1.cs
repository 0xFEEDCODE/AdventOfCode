using Framework;

namespace Solutions2017;

public class Solution1() : SolutionFramework(1)
{
    public override string[] Solve()
    {
        var s = 9;
        for (var i = 0; i < InpR.Length-1; i++)
        {
            if (InpR[i] == InpR[i + 1])
            {
                s += InpR[i].ParseInt();
            }
        }
        AssignAnswer1(s);
        
        s = 0;
        for (var i = 0; i < InpR.Length; i++)
        {
            if (InpR[i] == InpR[(i + InpR.Length/2) % InpR.Length])
            {
                s += InpR[i].ParseInt();
            }
        }
        AssignAnswer2(s);
        return Answers;
    }
}