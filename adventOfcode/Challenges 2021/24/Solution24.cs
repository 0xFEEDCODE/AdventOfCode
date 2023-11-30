using System.Text;
using Framework;

namespace Challenges2021;

public class Solution24 : SolutionFramework
{
    public Solution24() : base(24) { }

    public enum Instruction
    {
        inp,
        add,
        mul,
        div,
        mod,
        eql
    }

    public string[] SplitByNl;
    public List<(Instruction, char, string)> instructions;

    public override string[] Solve()
    {

        SplitByNl = this.RawInput.SplitByNewline();

        var inputNums = new List<int>()
        {
            1
        };

        int i;

        var nums = new List<string>();
        var sb = new StringBuilder();
        for (i = 0; i < 14; i++)
            sb.Append((i%10) == 0 ? 1 : i%10);
        nums.Add(sb.ToString());

        var num = "VV1VVV775199V";

        long m = -99999999999999;
        long f = -11111111111111;

        long biggestFound = 0;

        var variables = new Dictionary<char, long>()
        {
            { 'x', 0 }, { 'y', 0 }, { 'z', 0 }, { 'w', 0 },
        };

        Console.WriteLine();

        num =   "99196985775XXX";
        num =   "99196987975942";
        num =   "9919698XXXXXXX";
        //num = "99196985775942"

        var results = new Dictionary<string, long>();

        long biggestEver = 99196987975942;
        var hSet = new HashSet<string>();
        Random r = new Random();
        var lowestZ = long.MaxValue;
        var lowestZV = string.Empty;
        var added = true;
        while (added)
        {
            added = false;
            for (long fkl = 0; fkl < 10000; fkl++)
            {
                var numArr = num.ToArray();
                for (int xi = 0; xi < numArr.Length; xi++)
                {
                    if (numArr[xi] == 'X')
                    {
                        int rn = r.Next(1, 10);
                        numArr[xi] = rn.ToString().First();
                    }
                }

                var N = new string(numArr);
                if(hSet.Contains(N))
                    continue;

               var gv = GetValue(variables, N);

                results.Add(N, variables['z']);
                if (gv || Math.Abs(variables['z']) == 0)
                {
                    biggestFound = long.Parse(N) > biggestFound ? long.Parse(N) : biggestFound;
                    if (lowestZ == 0 && biggestFound > biggestEver)
                    {
                        Console.WriteLine();
                    }
                    var ch = false;
                    if (variables['z'] < lowestZ)
                    {
                        lowestZ = variables['z'];
                        lowestZV = N;
                    }
                    Console.WriteLine();
                }

                if (added == false)
                {
                    added = hSet.Add(N);
                }
                else
                {
                    hSet.Add(N);
                }
            }
        }


        if (lowestZ == 0 && biggestFound > biggestEver)
        {
            Console.WriteLine();
        }

        return Answers;
    }

    private bool GetValue(Dictionary<char, long> variables, string nums)
    {
        var addInstr = false;
        if (instructions == null || !instructions.Any())
        {
            instructions = new List<(Instruction, char, string)>();
            addInstr = true;
        }

        var i = 0;
        var instrI = 0;
        var visualized = new List<string>();

        variables['w'] = 0;
        variables['x'] = 0;
        variables['y'] = 0;
        variables['z'] = 0;

        foreach (var rawLine in SplitByNl)
        {
            var instrParts = addInstr ? rawLine.Split(' ').Select(x => x.TrimEnd('\r')) : null;
            var instrName = addInstr ? instrParts.First() : String.Empty ;
            var instrP1 = addInstr ? Char.Parse(instrParts.Skip(1).First()) : instructions[instrI].Item2;
            var instrP2 = addInstr ? instrParts.Count() > 2 ? instrParts.Skip(2).First() : null : instructions[instrI].Item3;
            var instrP2IsVariable = instrP2 != null ? char.IsLetter(instrP2.First()) : false;

            var instr = Instruction.inp;
            if (addInstr)
            {
                Enum.TryParse(instrName, out instr);

                if (addInstr)
                {
                    instructions.Add((instr, instrP1, instrP2)!);
                }
            }
            else
            {
                instr = instructions[instrI++].Item1;
            }

            switch (instr)
            {
                case Instruction.inp:
                    variables[instrP1] = Int32.Parse(nums[i++].ToString());
                    //
                    // if (i == 14)
                    // {
                    //     Debug.WriteLine($"{i - 1}--------------------");
                    //     foreach (var l in visualized)
                    //     {
                    //         Debug.WriteLine(l);
                    //     }
                    //
                    //     Debug.WriteLine($"END{i - 1}--------------------");
                    // }
                    //
                    // visualized.Clear();
                    break;
                case Instruction.add:
                    if (instrP2IsVariable)
                        variables[instrP1] += variables[Char.Parse(instrP2)];
                    else
                        variables[instrP1] += int.Parse(instrP2);
                    break;
                case Instruction.mul:
                    if (instrP2IsVariable)
                        variables[instrP1] *= variables[Char.Parse(instrP2)];
                    else
                        variables[instrP1] *= int.Parse(instrP2);
                    break;
                case Instruction.div:
                    if (instrP2IsVariable)
                    {
                        if (variables[Char.Parse(instrP2)] == 0)
                            throw new SystemException();

                        variables[instrP1] /= variables[Char.Parse(instrP2)];
                    }
                    else
                    {
                        var n = int.Parse(instrP2);
                        if (n == 0)
                            throw new SystemException();

                        variables[instrP1] /= int.Parse(instrP2);
                    }
                    break;
                case Instruction.mod:
                    if(variables[instrP1]<0)
                        throw new SystemException();
                    if (int.Parse(instrP2) <= 0)
                        throw new SystemException();

                    if (instrP2IsVariable)
                        variables[instrP1] %= variables[Char.Parse(instrP2)];
                    else
                        variables[instrP1] %= int.Parse(instrP2);
                    break;
                case Instruction.eql:
                    if (instrP2IsVariable)
                        variables[instrP1] = variables[instrP1] == variables[Char.Parse(instrP2)] ? 1 : 0;
                    else
                        variables[instrP1] = variables[instrP1] == int.Parse(instrP2) ? 1 : 0;
                    break;
            }

            //visualized.Add($"{instr.ToString().PadRight(5-instr.ToString().Length)} {instrP1} {(instrP2?.PadLeft(4) ?? nums[i-1].ToString().PadLeft(4))} || {("X:"+variables['x']).PadLeft(10)} {("Y:"+variables['y']).PadLeft(10)} {("Z:"+variables['z']).PadLeft(10)} {("W:"+variables['w']).PadLeft(10)}");
        }

        var result = variables['z'] == 0;

        return result;
    }
}
