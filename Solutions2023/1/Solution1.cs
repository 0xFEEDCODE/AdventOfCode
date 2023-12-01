using Framework;

namespace Solutions2023;

public class Solution1 : SolutionFramework
{
    public Solution1() : base(1) { }

    private string[] valid = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "edkqweqwejs", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    public override string[] Solve()
    {
        int? n1 = null;
        int? n2 = null;
        double s = 0;
        
        ForEachInputLine(l =>
        {
            var acc = string.Empty;
            foreach (var ch in l)
            {
                acc += ch;
                var i = 1;
                foreach (var v in valid)
                {
                    if (acc.Contains(v))
                    {
                        n1 ??= i % 10;
                        n2 = i % 10;
                        acc = acc.Remove(acc.Length - v.Length, 1);
                        acc = acc.Insert(acc.Length - v.Length + 1, "0");
                    }

                    i++;
                }
            }

            var n = (n1.HasValue ? n1.Value.ToString() : string.Empty) + (n2.HasValue ? n2.Value : string.Empty);
            s += (n1.ToString() + n2).ParseInt();
            n1 = null;
            n2 = null;
        });

        AssignAnswer1(s);
        return Answers;
    }
}
