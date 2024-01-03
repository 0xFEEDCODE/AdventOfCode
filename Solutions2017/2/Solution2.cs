using Framework;

namespace Solutions2017;

public class Solution2() : SolutionFramework(2)
{
    public override string[] Solve()
    {
        foreach (var l in InpNl)
        {
            var nums = l.Split(new[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x.Trim())).ToArray();
            NSlot += nums.Max() - nums.Min();
        }
        AssignAnswer1();

        foreach (var l in InpNl)
        {
            var nums = l.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x.Trim())).ToArray();
            var matchFound = nums.SelectMany(n1 => nums.Where(n2 => n2 != n1), (n1, n2) => new { n1, n2 })
                .Single(pair => pair.n1 % pair.n2 == 0);

            NSlot += Math.Max(matchFound.n1, matchFound.n2) / Math.Min(matchFound.n1, matchFound.n2);
        }
        AssignAnswer2();
        
        return Answers;
    }
}