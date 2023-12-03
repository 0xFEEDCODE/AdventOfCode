
using Framework;

namespace Challenges2019;


enum OpCode { Add = 1, Mul = 2, X = 3, Y = 4, N = 99 }
public class Solution5 : SolutionFramework
{
    public Solution5() : base(5) { }
    
    public override string[] Solve()
    {
        
        return Answers;
    }
}


public static class Ext
{
    public static void ExecuteAction(this int[] arr, int pos)
    {
        Dictionary<OpCode, Func<int, int, int>> actions2Param = new()
        {
            { OpCode.Add, (i, i1) => i+i1 },
            { OpCode.Mul, (i, i1) => i*i1 },
            { OpCode.N, (i, i1) => 0 }
        };

        var op = (OpCode)arr[pos];
        if (op is OpCode.N)
        {
            return;
        }

        if (op is OpCode.Add or OpCode.Mul)
        {
            var n1 = arr[pos + 1];
            var n2 = arr[pos + 2];
            var n3 = arr[pos + 3];
            arr[n3] = actions2Param[op](arr[n1], arr[n2]);
        } 
        else
        {
            var n1 = arr[pos + 1];
            var n2 = arr[pos + 2];
        }
    }
}
