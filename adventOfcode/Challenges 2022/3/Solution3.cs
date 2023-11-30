using Framework;

namespace Challenges2022;

public class Solution3 : SolutionFramework
{
    public Solution3() : base(3) { }

    public override string[] Solve()
    {
        var sum = 0;
        foreach (var line in RawInput.SplitByNewline())
        {
            var firstHalf = line.Take(line.Length / 2);
            var secondHalf = line.Skip(line.Length / 2);

            var commonLetter = firstHalf.Intersect(secondHalf).Single();

            sum += GetValueForLetter(commonLetter);

            Console.WriteLine();
        }

        AssignAnswer1(sum);

        var groups = new Dictionary<int, IEnumerable<string>>();

        var idx = 0;
        var i = 0;
        var lines = RawInput.SplitByNewline();
        while (i < lines.Length)
        {
            var formattedLine = lines[i].TrimEnd('\r');

            if (!groups.ContainsKey(idx))
            {
                groups[idx] = new List<string>();
            }

            if (groups[idx].Count() >= 3)
            {
                idx++;
                continue;
            }

            groups[idx] = groups[idx].Append(formattedLine);

            i++;
        }

        sum = 0;
        foreach (var group in groups)
        {
            var vals = group.Value.ToArray();
            var commonLetter = vals[0].Intersect(vals[1]).Intersect(vals[2]);
            if (commonLetter.Any())
            {
                sum += GetValueForLetter(commonLetter.Single());
            }
        }

        AssignAnswer2(sum);

        return Answers;
    }

    private static int GetValueForLetter(char letter)
    {
        if (letter is >= 'a' and <= 'z')
        {
            return letter - 'a' + 1;
        }

        if (letter is >= 'A' and <= 'Z')
        {
            return letter - 'A' + 27;
        }

        throw new ArgumentException("Not a letter.");
    }
}
