using Framework;

namespace Challenges2019;

enum OpCode { _, Add, Mul, In, Out, JumpT, JumpF, Lt, Eq, Halt }
public enum Mode { Position, Immediate};

public class Solution5 : SolutionFramework
{
    public Solution5() : base(5) { }
    
    public override string[] Solve()
    {
        var memory = RawInput.Split(',').Select(int.Parse).ToArray();
        memory[225] = 1;
        ExecuteProgram(2, memory, false);
        AssignAnswer1();
        
        memory = RawInput.Split(',').Select(int.Parse).ToArray();
        memory[225] = 5;
        ExecuteProgram(2, memory, true);
        AssignAnswer2();
        
        return Answers;
    }
    
    private void ExecuteProgram(int pointer, int[] memory, bool usePart2Instructions)
    {
        while (pointer < memory.Length)
        {
            var firstInstruction = memory[pointer];
            var opCode = firstInstruction is 99 ? OpCode.Halt : (OpCode)(firstInstruction % 10);
            if (opCode is OpCode.Halt)
            {
                break;
            }

            int p1 = memory[pointer + 1], p2 = memory[pointer + 2], p3 = memory[pointer + 3];

            var m1 = firstInstruction >= 100 ? (Mode)(firstInstruction / 100 % 10) : Mode.Position;
            var m2 = firstInstruction >= 1000 ? (Mode)(firstInstruction / 1000 % 10) : Mode.Position;
            var m3 = firstInstruction >= 10000 ? (Mode)(firstInstruction / 10000 % 10) : Mode.Position;

            int n1 = 0, n2 = 0;
            if (opCode != OpCode.Out)
            {
                n1 = m1 is Mode.Position ? memory[p1] : p1;
                n2 = m2 is Mode.Position ? memory[p2] : p2;
            }

            switch (opCode)
            {
                case OpCode.Add or OpCode.Mul when m3 is Mode.Immediate:
                    throw new InvalidOperationException();
                case OpCode.Add or OpCode.Mul:
                    var opResult = opCode is OpCode.Add ? n1 + n2 : n1 * n2;
                    memory[p3] = opResult;
                    pointer += 4;
                    break;
                case OpCode.JumpT or OpCode.JumpF:
                    if (!usePart2Instructions) break;
                    pointer = opCode is OpCode.JumpT && n1 != 0 || opCode is OpCode.JumpF && n1 == 0 ? n2 : pointer + 3;
                    break;
                case OpCode.Lt or OpCode.Eq:
                    if (!usePart2Instructions) break;
                    memory[p3] = (opCode is OpCode.Lt && n1 < n2 || opCode is OpCode.Eq && n1 == n2) ? 1 : 0;
                    pointer += 4;
                    break;
                case OpCode.Out:
                    NumSlot = m1 is Mode.Position ? memory[p1] : p1;
                    pointer += 2;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

