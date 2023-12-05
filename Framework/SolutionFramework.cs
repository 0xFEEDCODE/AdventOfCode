using System.Globalization;
using System.Runtime.InteropServices;

namespace Framework;

public abstract class SolutionFramework
{
    protected string Answer1;
    protected string Answer2;
    protected string[] Answers => new[] { Answer1, Answer2 };
    protected string RawInput;
    
    protected string InpR => RawInput;
    protected string[] InpNl => InputNlSplit;
    protected T[][] InpGr<T>() => InputAsGrid<T>();
    
    protected double NSlot;
    protected string StrSlot;
    
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
    
    protected string[] InputNlSplit => RawInput.SplitByNewline();
    protected int[] IntInputSplitNl => RawInput.SplitByNewline().Select(int.Parse).ToArray();
    protected double[] DoubleInputSplitNl => RawInput.SplitByNewline().Select(double.Parse).ToArray();
    protected long[] LongInputSplitNl => RawInput.SplitByNewline().Select(long.Parse).ToArray();
    
    protected T[][] InputAsGrid<T>()
    {
        var grid = (InputNlSplit.Length, InputNlSplit[0].Length).CreateGrid<T>();
        grid.ForEachCell((i, j) =>
        {
            if (InputNlSplit[i][j] is T v)
            {
                grid[i][j] = v;
            }
        });
        return grid;
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

    protected void AssignAnswer1(bool resetNumSlot = true)
    {
        AssignAnswer1(NSlot);
        NSlot = resetNumSlot ? 0 : NSlot;
    }
    protected void AssignAnswer2(bool resetNumSlot = true)
    {
        AssignAnswer2(NSlot);
        NSlot = resetNumSlot ? 0 : NSlot;
    }
    
    protected void AssignAnswer1<T>(T answer, bool resetNumSlot = true)
    {
        Answer1 = answer.ToString();
        NSlot = resetNumSlot ? 0 : NSlot;
    }
    protected void AssignAnswer2<T>(T answer, bool resetNumSlot = true)
    {
        Answer2 = answer.ToString();
        NSlot = resetNumSlot ? 0 : NSlot;
    }

    public abstract string[] Solve();
}
