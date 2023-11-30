using Framework;

namespace Challenges2020;

public class Solution8 : SolutionFramework
{
    public Solution8() : base(8) { }

    private enum InstructionType
    {
        Nop, Acc, Jmp
    }

    private record struct Instruction(InstructionType Type, int Value);

    private class InstructionWithExecutionHistory
    {
        public InstructionWithExecutionHistory(Instruction instruction, bool wasExecuted = false)
        {
            Instruction = instruction;
            WasExecuted = wasExecuted;
        }

        public Instruction Instruction;
        public bool WasExecuted;
    }

    public override string[] Solve()
    {
        var instructions = new List<InstructionWithExecutionHistory>() { };
        foreach (var line in RawInputSplitByNl)
        {
            var spl = line.Split(' ');
            var instrType = spl.First() switch
            {
                "jmp" => InstructionType.Jmp,
                "acc" => InstructionType.Acc,
                "nop" => InstructionType.Nop
            };
            var instrVal = int.Parse(spl.Last().Skip(1).ToArray());
            instrVal = spl.Last().First() switch
            {
                '+' => instrVal,
                '-' => -instrVal,
                _ => throw new ArgumentOutOfRangeException()
            };

            instructions.Add(new InstructionWithExecutionHistory(new Instruction(instrType, instrVal)));
        }
        
        var acc = 0;
        var cInstr = instructions.First();
        var instrPos = 0;
        while (!cInstr.WasExecuted)
        {
            cInstr.WasExecuted = true;
            switch (cInstr.Instruction.Type)
            {
                case InstructionType.Nop:
                    instrPos++;
                    break;
                case InstructionType.Acc:
                    acc += cInstr.Instruction.Value;
                    instrPos++;
                    break;
                case InstructionType.Jmp:
                    instrPos += cInstr.Instruction.Value;
                    break;
            }

            cInstr = instructions.ElementAt(instrPos);
        }
        
        AssignAnswer1(acc);
        
        foreach (var inst in instructions)
        {
            inst.WasExecuted = false;
        }

        acc = 0;
        cInstr = instructions.First();
        instrPos = 0;
        int? accVal = null;
        while (!accVal.HasValue)
        {
            if (cInstr.Instruction.Type == InstructionType.Acc)
            {
                acc += cInstr.Instruction.Value;
            } 
            else
            {
                accVal = AccUntilEnd(
                    new InstructionWithExecutionHistory(cInstr.Instruction with { Type = InstructionType.Nop }, cInstr.WasExecuted),
                    instrPos, acc,
                    instructions.Select(inst => new InstructionWithExecutionHistory(inst.Instruction, inst.WasExecuted)).ToList());
                if (accVal.HasValue)
                {
                    acc = accVal.Value;
                    break;
                }

                accVal = AccUntilEnd(
                    new InstructionWithExecutionHistory(cInstr.Instruction with { Type = InstructionType.Jmp }, cInstr.WasExecuted),
                    instrPos, acc,
                    instructions.Select(inst => new InstructionWithExecutionHistory(inst.Instruction, inst.WasExecuted)).ToList());
                if (accVal.HasValue)
                {
                    acc = accVal.Value;
                    break;
                }
            }

            if (cInstr.Instruction.Type == InstructionType.Jmp)
            {
                instrPos += cInstr.Instruction.Value;
            } 
            else
            {
                instrPos++;
            }
            
            cInstr.WasExecuted = true;
            cInstr = instructions.ElementAt(instrPos);
        }

        AssignAnswer2(acc);

        Console.WriteLine();

        return Answers;
    }

    private static int? AccUntilEnd(InstructionWithExecutionHistory cInstr, int instrPos, int acc, List<InstructionWithExecutionHistory> instructions)
    {
        var executed = 0;
        while (!cInstr.WasExecuted)
        {
            cInstr.WasExecuted = true;
            executed++;
            switch (cInstr.Instruction.Type)
            {
                case InstructionType.Nop:
                    instrPos++;
                    break;
                case InstructionType.Acc:
                    acc += cInstr.Instruction.Value;
                    instrPos++;
                    break;
                case InstructionType.Jmp:
                    instrPos += cInstr.Instruction.Value;
                    break;
            }

            if (instrPos == instructions.Count)
            {
                return acc;
            }
            if (instrPos < 0 || instrPos >= instructions.Count)
            {
                return null;
            }
            cInstr = instructions.ElementAt(instrPos);
            if (cInstr.WasExecuted)
            {
                return null;
            }
        }
        
        return null;
    }
}
