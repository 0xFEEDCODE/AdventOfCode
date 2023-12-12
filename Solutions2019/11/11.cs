using System.Diagnostics;
using System.Xml.XPath;

using Framework;
using static Challenges2019.OpCode;

using Extensions = Framework.Extensions;

namespace Challenges2019;

public enum Color { White, Black }
public enum Direction { L, U, R, D };

public static class Ext
{
    public static Color ToColor(this double v) => v == 0 ? Color.Black : Color.White;
    public static int ToInt(this Color v) => v is Color.Black ? 0 : 1;
    
    public static Direction Turn(this Direction d, bool left)
    {
        if (left)
        {
            return d switch
            {
                Direction.L => Direction.D,
                Direction.D => Direction.R,
                Direction.R => Direction.U,
                Direction.U => Direction.L,
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
        }
        return d switch
        {
            Direction.L => Direction.U,
            Direction.D => Direction.L,
            Direction.R => Direction.D,
            Direction.U => Direction.R,
            _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
        };
    }
}

public class Solution11 : SolutionFramework
{
    public Solution11() : base(11) { }

    record Field(Pos2D Pos, Color Color);

    public override string[] Solve()
    {
        var progInstr = InpR.Split(',').Select(double.Parse).ToArray();
        var memory = new Dictionary<double, double>();
        for (var i = 0; i < progInstr.Length; i++)
        {
            memory[i] = progInstr[i];
        }
        
        var painted = new List<Field>();
        
        var startPos = new Field(new Pos2D(0, 0), Color.White);
        (Field field, Direction dir) robot = (startPos, Direction.U);
        
        var pointer = 0;
        var relBase = 0d;
        while (true)
        {
            var program = ExecuteProgram(pointer, memory, relBase, robot.field.Color.ToInt());
            
            pointer = program.pointer;
            memory = program.memory;
            relBase = program.relBase;

            var color = program.output.First().ToColor();

            var turn = program.output.Last() == 0 ? Direction.L : Direction.R;
            
            robot = Paint(robot, color, painted);
            robot = TurnAndAdvance(turn, robot, painted);
            
            if (program.halt)
            {
                break;
            }
        }
        if (painted.Contains(robot.field))
        {
            throw new InvalidOperationException();
        }
        painted.Add(robot.field);

        var paintedOnce = painted.DistinctBy(x => x.Pos);
        var bl = paintedOnce.Count(x => x.Color is Color.Black);
        var wh = paintedOnce.Count(x => x.Color is Color.White);
        Console.WriteLine((bl, wh));
        Console.WriteLine(bl+wh);
        Console.WriteLine(painted.Count());

        for (var j = painted.MinBy(x=>x.Pos.Y)!.Pos.Y; j <= painted.MaxBy(x=>x.Pos.Y)!.Pos.Y; j++)
        {
            for (var i = painted.MinBy(x=>x.Pos.X)!.Pos.X; i <= painted.MaxBy(x=>x.Pos.X)!.Pos.X; i++)
            {
                    Console.Write(painted.Any(p => p.Color is Color.White && p.Pos.X == i && p.Pos.Y == j) ? "#" : " ");
            }
        }
        return Answers;
    }
    private static (Field field, Direction dir) Paint((Field field, Direction dir) robot, Color color, List<Field> painted)
    {
        robot = robot with { field =  robot.field with{Color = color} };
        painted.Add(robot.field);
        return robot;
    }

    private static (Field field, Direction dir) TurnAndAdvance(Direction dir, (Field field, Direction dir) robot, List<Field> painted)
    {
        var newDir = robot.dir.Turn(dir == Direction.L);

        var pos = robot.field.Pos;
        var newPos = newDir switch
        {
            Direction.L => pos with { X = pos.X - 1 },
            Direction.R => pos with { X = pos.X + 1 },
            Direction.U => pos with { X = pos.Y - 1 },
            Direction.D => pos with { X = pos.Y + 1 },
            _ => throw new ArgumentOutOfRangeException()
        };

        var newField = new Field(newPos, Color.Black);
        
        var paintHistory = painted.Where(p => p.Pos == newPos);
        var lastPaint = paintHistory.SingleOrDefault();
        if (lastPaint != null)
        {
            newField = newField with{ Color = lastPaint.Color };
            painted.Remove(lastPaint);
        }
            
        return (newField, newDir);
    }

    private (int pointer, Dictionary<double, double> memory, IEnumerable<double> output, double relBase, bool halt) ExecuteProgram(int pointer, Dictionary<double, double> memory, double relBase, double input)
    {
        var inputRequested = false;
        var output = new List<double>();
        while (true)
        {
            var instr = memory[pointer];
            var opCode = instr is 99 ? Halt : (OpCode)(instr % 10);
            if (opCode is Halt)
            {
                return (pointer, memory, output, relBase, true);
            }

            double p1 = 0, p2 = 0, p3 = 0;

            var m1 = instr >= 100 ? (Mode)(instr / 100 % 10) : Mode.Position;
            var m2 = instr >= 1000 ? (Mode)(instr / 1000 % 10) : Mode.Position;
            var m3 = instr >= 10000 ? (Mode)(instr / 10000 % 10) : Mode.Position;

            Extensions.EncloseInTryCatch(() =>
            {
                p1 = memory[pointer + 1];
                p2 = memory[pointer + 2];
                p3 = memory[pointer + 3];
            });

            double n1 = 0, n2 = 0;
            memory.TryAdd(p1, 0);
            memory.TryAdd(p2, 0);
            n1 = m1 is Mode.Position ? memory[p1] : m1 is Mode.Relative ? memory[p1+relBase] : p1;
            if (opCode is not (Out or RelBaseOffset))
            {
                n2 = m2 is Mode.Position ? memory[p2] : m2 is Mode.Relative ? memory[p2 + relBase] : p2;
            }

            switch (opCode)
            {
                case Add or Mul when m3 is Mode.Immediate:
                    throw new InvalidOperationException();
                case Add or Mul:
                    var opResult = opCode is Add ? n1 + n2 : n1 * n2;
                    memory[m3 is Mode.Relative ? p3 + relBase : p3] = opResult;
                    pointer += 4;
                    break;
                case JumpT or JumpF:
                    pointer = (int)(opCode is JumpT && n1 != 0 || opCode is JumpF && n1 == 0 ? n2 : pointer + 3);
                    break;
                case Lt or Eq:
                    memory[m3 is Mode.Relative ? p3 + relBase : p3] = opCode is Lt && n1 < n2 || opCode is Eq && n1 == n2 ? 1 : 0;
                    pointer += 4;
                    break;
                case Out:
                    output.Add(n1);
                    pointer += 2;
                    break;
                case In:
                    if (inputRequested)
                    {
                        return (pointer, memory, output, relBase, false);
                    }
                    if (input is -1)
                    {
                        throw new InvalidOperationException();
                    }
                    inputRequested = true;
                    memory[m1 is Mode.Relative ? p1 + relBase : p1] = input;
                    pointer += 2;
                    break;
                case RelBaseOffset:
                    relBase += n1;
                    pointer += 2;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
