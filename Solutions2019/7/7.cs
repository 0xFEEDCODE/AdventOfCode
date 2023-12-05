using System.Security.Authentication;

using Facet.Combinatorics;

using Framework;

namespace Challenges2019;

public class Solution7 : SolutionFramework
{
    public Solution7() : base(7) { }

    List<int> Phase;

    public override string[] Solve()
    {
        var highestFound = int.MinValue;
        
        var lastOut = 0;
        foreach (var phaseSettings in new Permutations<int>(Enumerable.Range(0, 5).ToArray()).ToArray())
        {
            lastOut = 0;
            foreach (var phase in phaseSettings)
            {
                lastOut = ExecuteProgram(0, RawInput.Split(',').Select(int.Parse).ToArray(), true, phase, lastOut);
            }
            highestFound.AssignIfBigger(lastOut);
        }

        AssignAnswer1(highestFound);

        highestFound = int.MinValue;
        var initialMem = RawInput.Split(',').Select(int.Parse).ToArray();
        foreach (var phaseSettings in new Permutations<int>(Enumerable.Range(5, 5).ToArray()).ToArray())
        {
            var state = new (int Output, int Pointer, int[] Memory, int PhaseSetting)[5];
            for (var i = 0; i < phaseSettings.Count; i++)
            {
                state[i] = (0, 0, initialMem.ToArray(), phaseSettings[i]);
            }
            
            lastOut = 0;
            var haltReceived = false;
            while (!haltReceived)
            {
                for (var i = 0; i < phaseSettings.Count; i++)
                {
                    var result = ExecuteProgram2(state[i].Pointer, state[i].Memory, state[i].PhaseSetting, lastOut);
                    if (result.Output != -99)
                    {
                        state[i] = state[i] with { Output = result.Output, Memory = result.Memory, Pointer = result.Pointer };
                        lastOut = state[i].Output;
                    } else
                    {
                        haltReceived = true;
                        state[i] = state[i] with { Pointer = result.Pointer, Memory = result.Memory };
                    }

                    if (lastOut > highestFound)
                    {
                        highestFound = lastOut;
                    }
                }
            }
        }
        
        AssignAnswer2(highestFound);
        
        return Answers;
    }
    
    private (int Output, int Pointer, int[] Memory) ExecuteProgram2(int pointer, int[] memory, int inp1, int inp2)
    {
        var inpIdx = 0;
        var flag = pointer == 0 ? 0 : 1;
        while (pointer < memory.Length)
        {
            var firstInstruction = memory[pointer];
            var opCode = firstInstruction is 99 ? OpCode.Halt : (OpCode)(firstInstruction % 10);
            if (opCode is OpCode.Halt)
            {
                return (-99, pointer, memory.ToArray());
            }

            int p1 = 0, p2 = 0, p3 = 0;
            Extensions.EncloseInTryCatch(() =>
            {
                p1 = memory[pointer + 1];
                p2 = memory[pointer + 2];
                p3 = memory[pointer + 3];
            });

            var m1 = firstInstruction >= 100 ? (Mode)(firstInstruction / 100 % 10) : Mode.Position;
            var m2 = firstInstruction >= 1000 ? (Mode)(firstInstruction / 1000 % 10) : Mode.Position;
            var m3 = firstInstruction >= 10000 ? (Mode)(firstInstruction / 10000 % 10) : Mode.Position;

            int n1 = 0, n2 = 0;
            if (opCode is not OpCode.Out and not OpCode.In)
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
                    pointer = opCode is OpCode.JumpT && n1 != 0 || opCode is OpCode.JumpF && n1 == 0 ? n2 : pointer + 3;
                    break;
                case OpCode.Lt or OpCode.Eq:
                    memory[p3] = (opCode is OpCode.Lt && n1 < n2 || opCode is OpCode.Eq && n1 == n2) ? 1 : 0;
                    pointer += 4;
                    break;
                case OpCode.Out:
                    NSlot = m1 is Mode.Position ? memory[p1] : p1;
                    var outp = m1 is Mode.Position ? memory[p1] : p1;
                    pointer += 2;
                    return (outp, pointer, memory.ToArray());
                    break;
                case OpCode.In:
                    memory[p1] = (int)(flag++ == 0 ? inp1 : inp2);
                    pointer += 2;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
        throw new InvalidOperationException();
    }

    private int ExecuteProgram(int pointer, int[] memory, bool usePart2Instructions, int inp1, int inp2)
    {
        var inpIdx = 0;
        var flag = 0;
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
            if (opCode is not OpCode.Out and not OpCode.In)
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
                    return m1 is Mode.Position ? memory[p1] : p1;
                    pointer += 2;
                    break;
                case OpCode.In:
                    memory[p1] = flag++ == 0 ? inp1 : inp2;
                    pointer += 2;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
        return 0;
    }
}

