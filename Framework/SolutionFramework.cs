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

    protected void AssignAnswer1(int answer)
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
