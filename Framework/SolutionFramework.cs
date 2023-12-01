using System.Globalization;
using System.Runtime.InteropServices;

namespace Framework;

public abstract class SolutionFramework
{
    protected string Answer1;
    protected string Answer2;
    protected string[] Answers => new[] { Answer1, Answer2 };
    protected string RawInput;
    
    protected double VarA;
    protected double VarB;
    
    [DllImport("user32.dll")]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);
    [DllImport("user32.dll")]
    internal static extern bool CloseClipboard();
    [DllImport("user32.dll")]
    internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

    [STAThread]
    protected void ClipboardAnswer1() => CopyToClipboard(Answer1);
    protected void ClipboardAnswer2() => CopyToClipboard(Answer2);
    
    private void CopyToClipboard(string str)
    {
        OpenClipboard(IntPtr.Zero);
        var ptr = Marshal.StringToHGlobalUni(str);
        SetClipboardData(13, ptr);
        CloseClipboard();
        Marshal.FreeHGlobal(ptr);
    }

    protected SolutionFramework(int challengeNo)
    {
        RawInput = File.ReadAllText($"{challengeNo}/input.txt");
    }

    protected string[] RawInputSplitByNl => RawInput.SplitByNewline();
    protected int[] IntInputSplitByNl => RawInput.SplitByNewline().Select(int.Parse).ToArray();
    protected double[] DoubleInputSplitByNl => RawInput.SplitByNewline().Select(double.Parse).ToArray();
    protected long[] LongInputSplitByNl => RawInput.SplitByNewline().Select(long.Parse).ToArray();

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

    protected void AssignAnswer1<T>(T answer, bool copyToClipboard = false)
    {
        Answer1 = answer.ToString();
        if (copyToClipboard)
        {
            ClipboardAnswer1();
        }
    }
    protected void AssignAnswer2<T>(T answer, bool copyToClipboard = false)
    {
        Answer2 = answer.ToString();
        if (copyToClipboard)
        {
            ClipboardAnswer2();
        }
    }

    public abstract string[] Solve();
}
