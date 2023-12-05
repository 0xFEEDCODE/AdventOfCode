using Framework;

using static Challenges2019.OpCode;

namespace Challenges2019;

public class Solution9 : SolutionFramework
{
    public Solution9() : base(9) { }

    public override string[] Solve()
    {

        var prog = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
        ExecuteProgram(0, prog.Split(',').Select(double.Parse).ToArray());
        Console.WriteLine(NSlot.ToString().Length);
        return Answers;
    }
    
    private void ExecuteProgram(int pointer, double[] memory)
    {
        var relBase = 0;
        while (pointer < memory.Length)
        {
            var firstInstruction = memory[pointer];
            var opCode = firstInstruction is 99 ? Halt : (OpCode)(firstInstruction % 10);
            if (opCode is Halt)
            {
                break;
            }

            double p1 = 0, p2 = 0, p3 = 0;

            var m1 = firstInstruction >= 100 ? (Mode)(firstInstruction / 100 % 10) : Mode.Position;
            var m2 = firstInstruction >= 1000 ? (Mode)(firstInstruction / 1000 % 10) : Mode.Position;
            var m3 = firstInstruction >= 10000 ? (Mode)(firstInstruction / 10000 % 10) : Mode.Position;

            Extensions.EncloseInTryCatch(() =>
            {
                p1 = memory[pointer + 1];
                p2 = memory[pointer + 2];
                p3 = memory[pointer + 3];
            });

            var ip1 = (int)p1;
            var ip2 = (int)p2;
            var ip3 = (int)p3;

            double n1 = 0, n2 = 0;
            if (opCode is not (Out or RelBaseOffset))
            {
                n1 = m1 is Mode.Position ? memory[ip1] : m1 is Mode.Relative ? memory[ip1+relBase] : p1;
                n2 = m2 is Mode.Position ? memory[ip2] : m2 is Mode.Relative ? memory[ip2+relBase] : p2;
            }

            switch (opCode)
            {
                case Add or Mul when m3 is Mode.Immediate:
                    throw new InvalidOperationException();
                case Add or Mul:
                    var opResult = opCode is Add ? n1 + n2 : n1 * n2;
                    memory[ip3] = opResult;
                    pointer += 4;
                    break;
                case JumpT or JumpF:
                    pointer = (int)(opCode is JumpT && n1 != 0 || opCode is JumpF && n1 == 0 ? n2 : pointer + 3);
                    break;
                case Lt or Eq:
                    memory[ip3] = (opCode is Lt && n1 < n2 || opCode is Eq && n1 == n2) ? 1 : 0;
                    pointer += 4;
                    break;
                case Out:
                    NSlot = m1 is Mode.Position ? memory[ip1] : m1 is Mode.Relative ? memory[ip1+relBase] : p1;
                    pointer += 2;
                    break;
                case RelBaseOffset:
                    relBase += (int)(m1 is Mode.Position ? memory[ip1] : m1 is Mode.Relative ? memory[ip1+relBase] : p1);
                    pointer += 2;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

