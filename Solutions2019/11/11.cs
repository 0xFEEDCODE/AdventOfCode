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
                Direction.U => Direction.L
            };
        }
        else
        {
            return d switch
            {
                Direction.L => Direction.U,
                Direction.D => Direction.L,
                Direction.R => Direction.D,
                Direction.U => Direction.R
            };
        }
    }
}

public class Solution11 : SolutionFramework
{
    public Solution11() : base(11) { }

    record Field(Pos2D Pos, Color Color);

    public double input;

    public override string[] Solve()
    {
        var progInstr = InpR.Split(',').Select(double.Parse).ToArray();
        var memory = new Dictionary<double, double>();
        for (var i = 0; i < progInstr.Length; i++)
        {
            memory[i] = progInstr[i];
        }
        
        var hull = (150, 300).CreateGrid<Color>();
        hull.ForEachCell((x,y) =>
        {
            hull[x][y] = Color.Black;
        });
        var offs = 20;
        
        var painted = new List<Field>();
        
        var startPos = new Field(new Pos2D(offs, offs), Color.White);

        (Field field, Direction dir) robot = (startPos, Direction.U);

        input = robot.field.Color.ToInt();
        painted.Add(robot.field);

        hull[offs][offs] = Color.White;
        
        Console.SetCursorPosition(0,0);
        Console.SetWindowSize(200, 300);
        var prog = ExecuteProgram(0, memory, input);
        var maxX = 0;
        var maxY = 0;
        var minY = 0;
        var minX = 0;
        while (prog.output.Any())
        {
            if (prog.output.Count() != 2)
            {
                throw new InvalidOperationException();
            }
            var paintColor = prog.output.First().ToColor();
            painted.Add(robot.field with { Color = paintColor });
            hull[robot.field.Pos.X][robot.field.Pos.Y] = paintColor;
            
            /*
            Console.SetCursorPosition(robot.field.Pos.X, robot.field.Pos.Y);
            Console.ForegroundColor = paintColor is Color.White ? ConsoleColor.Red : ConsoleColor.Black;
            Console.Write("\u2588");
            */
            
            var turn = prog.output.Last() == 0 ? Direction.L : Direction.R;

            var newDir = robot.dir.Turn(turn == Direction.L);

            var pos = robot.field.Pos;
            var newPos = newDir switch
            {
                Direction.L => pos with { X = pos.X - 1 },
                Direction.R => pos with { X = pos.X + 1 },
                Direction.U => pos with { Y = pos.Y - 1 },
                Direction.D => pos with { Y = pos.Y + 1 }
            };

            var newField = new Field(newPos, hull[newPos.X][newPos.Y]);

            /*
            var paintHistory = painted.Where(p => p.Pos == newPos);
            var lastPaint = paintHistory.LastOrDefault();
            if (lastPaint != null)
            {
                newField = newField with{ Color = lastPaint.Color };
            }
            */
            
            robot = robot with { field = newField, dir = newDir };

            maxX.AssignIfBigger(robot.field.Pos.X);
            maxY.AssignIfBigger(robot.field.Pos.Y);
            minY.AssignIfLower(robot.field.Pos.Y);
            minX.AssignIfLower(robot.field.Pos.X);
            
            input = robot.field.Color.ToInt();
            prog = ExecuteProgram(prog.pointer, prog.memory, input);
        }
        Console.SetCursorPosition(0,0);

        var paintedOnce = painted.DistinctBy(x => x.Pos);
        AssignAnswer1(paintedOnce.Count());

        
        Console.SetCursorPosition(0,0);
        for (var i = offs+minY; i < maxY; i++)
        {
            for (var j = offs+minX; j < maxX; j++)
            {
                if (hull[i][j] is Color.White)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\u2588");
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\u2588");
                }
            }
        }
        
        //gr.PrintGrid();
        
        return Answers;
    }
    
    private (int pointer, Dictionary<double, double> memory, IEnumerable<double> output) ExecuteProgram(int pointer, Dictionary<double, double> memory, double input)
    {
        var relBase = 0d;
        var inputRequested = false;
        var outp = new List<double>();
        while (true)
        {
            var instr = memory[pointer];
            var opCode = instr is 99 ? Halt : (OpCode)(instr % 10);
            if (opCode is Halt)
            {
                return (pointer, memory, outp);
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
                    outp.Add(n1);
                    pointer += 2;
                    break;
                case In:
                    if (inputRequested)
                    {
                        return (pointer, memory, outp);
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
        throw new InvalidOperationException();
    }
}
