using Facet.Combinatorics;

using Framework;

namespace Challenges2019;

public class Solution2 : SolutionFramework
{
    public Solution2() : base(2) { }

    
    enum OpCode { Add = 1, Mul = 2, N = 99 }
    private Dictionary<OpCode, Func<int, int, int>> Actions => new()
    {
        { OpCode.Add, (i, i1) => i+i1 },
        { OpCode.Mul, (i, i1) => i*i1 },
        { OpCode.N, (i, i1) => 0 }
    };
        
    public override string[] Solve()
    {
        Part1();
        Part2();

        return Answers;
    }
    
    private void Part1()
    {
        var inp = RawInput.Split(',').Select(int.Parse).ToArray();
        inp[1] = 12;
        inp[2] = 2;

        for (var i = 0; i < inp.Length && i + 4 <= inp.Length; i += 4)
        {
            var op = (OpCode)inp[i];
            if (op is OpCode.N)
            {
                continue;
            }
            var n1 = inp[i + 1];
            var n2 = inp[i + 2];
            var n3 = inp[i + 3];
            inp[n3] = Actions[op](inp[n1], inp[n2]);
        }

        AssignAnswer1(inp[0]);
    }
    
    private void Part2()
    {
        var inp = RawInput.Split(',').Select(int.Parse).ToArray();

        var range = Enumerable.Range(1, 99);
        var combinations = new Combinations<int>(range.ToList(), 2);

        foreach (var combination in combinations)
        {
            inp = RawInput.Split(',').Select(int.Parse).ToArray();
            inp[1] = combination[0];
            inp[2] = combination[1];

            for (var i = 0; i < inp.Length && i + 4 <= inp.Length; i += 4)
            {
                var op = (OpCode)inp[i];
                if (op is OpCode.N)
                {
                    break;
                }
                var n1 = inp[i + 1];
                var n2 = inp[i + 2];
                var n3 = inp[i + 3];
                inp[n3] = Actions[op](inp[n1], inp[n2]);
            }

            if (inp[0] == 19690720)
            {
                break;
            }
        }

        AssignAnswer2(100*inp[1]+inp[2]);
    }
}
