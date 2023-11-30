using System.Text.RegularExpressions;
using Framework;

namespace Challenges2022;

public class Solution5 : SolutionFramework
{
    public Solution5() : base(5)
    {
    }

    struct MoveInstruction
    {
        public int QUANTITY;
        public int FROM;
        public int TO;
    }

    public override string[] Solve()
    {
        var stacks1 = new Dictionary<int, Stack<char>>();
        var stacks2 = new Dictionary<int, Stack<char>>();
        for (var i = 0; i <= 9; i++)
        {
            stacks1.Add(i, new Stack<char>());
            stacks2.Add(i, new Stack<char>());
        }

        var instructions = new List<MoveInstruction>();

        foreach (var line in RawInput.SplitByNewline())
        {
            if (!line.StartsWith("move"))
            {
                var offset = 1;
                var slot = line.Take(3).ToArray();
                var idx = 1;
                while (slot.Any() && slot[0] != '\r')
                {
                    if (char.IsLetter(slot[1]))
                    {
                        stacks1[idx].InsertToBottom(slot[1]);
                        stacks2[idx].InsertToBottom(slot[1]);
                    }

                    slot = line.Skip(4 * offset++).Take(3).ToArray();
                    idx++;
                }
            }
            else if (line.StartsWith("move"))
            {
                var re = new Regex(@"\D+");
                var numbers = re.Split(line).Where(x => !string.IsNullOrEmpty(x) && char.IsDigit(x[0])).Select(int.Parse).ToArray();
                if (numbers.Length == 3) instructions.Add(new MoveInstruction { QUANTITY = numbers[0], FROM = numbers[1], TO = numbers[2] });
            }
        }

        foreach (var instr in instructions)
            for (var i = 0; i < instr.QUANTITY; i++)
                stacks1[instr.TO].Push(stacks1[instr.FROM].Pop());

        Answers[0] = GetTopValuesFromStacks(stacks1);

        foreach (var instr in instructions)
        {
            var temp = new List<char>();
            for (var i = 0; i < instr.QUANTITY; i++)
                temp.Insert(0, stacks2[instr.FROM].Pop());

            foreach (var entry in temp)
                stacks2[instr.TO].Push(entry);
        }

        Answers[1] = GetTopValuesFromStacks(stacks2);

        return Answers;
    }

    private static string GetTopValuesFromStacks(Dictionary<int, Stack<char>> stacks)
    {
        return new string(stacks.Values.Select(x => x.Count > 0 ? x.Peek() : ' ').ToArray());
    }
}
