using Framework;

namespace Solutions2023;

public class Solution1 : SolutionFramework
{
    public Solution1() : base(1) { }

    private static string[] digits = Enumerable.Range(1, 9).Select(x => x.ToString()).ToArray();
    private static string[] nonDigits = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "_" };
    private string[] allNumbers = nonDigits.Concat(digits).ToArray();
        
    public override string[] Solve()
    {
        int? n1 = null;
        int? n2 = null;
        double sum = 0;
        
        ForEachInputLine(l =>
        {
            var occurrences = GetSubstringsContainedByString(l, digits);
            n1 ??= 1 + allNumbers.FindIndexOfItem(occurrences.First()) % 10;
            n2 = 1 + allNumbers.FindIndexOfItem(occurrences.Last()) % 10;

            sum += (n1.ToString() + n2.Value).ParseInt();
            n1 = null;
            n2 = null;
        });

        AssignAnswer1(sum);
        
        n1 = null;
        n2 = null;
        sum = 0;
        ForEachInputLine(l =>
        {
            var occurrences = GetSubstringsContainedByString(l, allNumbers);
            n1 ??= 1 + allNumbers.FindIndexOfItem(occurrences.First()) % 10;
            n2 = 1 + allNumbers.FindIndexOfItem(occurrences.Last()) % 10;

            sum += (n1.ToString() + n2.Value).ParseInt();
            n1 = null;
            n2 = null;
        });
        
        AssignAnswer2(sum);
        return Answers;
    }
}
