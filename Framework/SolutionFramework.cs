namespace Framework;

public abstract class SolutionFramework
{
    protected string[] Answers;
    protected string RawInput;

    protected SolutionFramework(int challengeNo)
    {
        Answers = new string[2];
        RawInput = File.ReadAllText($"{challengeNo}/input.txt");
    }

    protected string[] RawInputSplitByNl => RawInput.SplitByNewline();
    protected int[] IntInputSplitByNl => RawInput.SplitByNewline().Select(int.Parse).ToArray();
    protected double[] DoubleInputSplitByNl => RawInput.SplitByNewline().Select(double.Parse).ToArray();
    protected long[] LongInputSplitByNl => RawInput.SplitByNewline().Select(long.Parse).ToArray();

    protected bool StringContainsSubstring(string str, string sub)
    {
        var acc = string.Empty;
        foreach (var ch in str)
        {
            acc += ch;
            if (acc.Contains(sub))
            {
                return true;
            }
        }
        return false;
    }
    
    protected ICollection<string> GetSubstringsContainedByString(string str, string[] subs)
    {
        var occurrences = new List<string>();

        str.ForEach(i =>
        {
            var sk = str.Skip(i).AsString();
            str.Skip(i).AsString().ForEach(j =>
            {
                var substring = str.Substring(i, j + 1);
                if (subs.Contains(substring))
                {
                    occurrences.Add(substring);
                }
            });
        });
        return occurrences;
    }
    
    protected void ForEachInputLine(Action<string> action) => RawInput.ForEachInputLine(action);

    protected void AssignAnswer1(int answer)
    {
        Answers[0] = answer.ToString();
    }
    protected void AssignAnswer1(double answer)
    {
        Answers[0] = answer.ToString();
    }
    protected void AssignAnswer1(long answer)
    {
        Answers[0] = answer.ToString();
    }
    protected void AssignAnswer1(ulong answer)
    {
        Answers[0] = answer.ToString();
    }

    protected void AssignAnswer2(int answer)
    {
        Answers[1] = answer.ToString();
    }
    protected void AssignAnswer2(double answer)
    {
        Answers[1] = answer.ToString();
    }
    protected void AssignAnswer2(long answer)
    {
        Answers[1] = answer.ToString();
    }
    protected void AssignAnswer2(ulong answer)
    {
        Answers[1] = answer.ToString();
    }

    public abstract string[] Solve();
}
