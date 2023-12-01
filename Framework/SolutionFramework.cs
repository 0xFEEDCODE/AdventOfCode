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
