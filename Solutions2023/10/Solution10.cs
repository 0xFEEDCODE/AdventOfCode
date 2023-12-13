using System.Collections;

using Framework;

using Gee.External.Capstone.M68K;

namespace Solutions2023;

public enum FieldT { Pipe, Ground, Start }
public enum Dir { Vert, Hor, NE, NW, SE, SW };
public enum GD { N, S, W, E, TL, TR, BL, BR };

public class Field(Pos2D pos, FieldT type, char symbol, Dir? dir = null)
{
    public Pos2D Pos = pos;
    public FieldT Type = type;
    public Dir? Dir = dir;
    public bool Loop;
    public bool Inner;
    public char Symbol = symbol;
    public GD? GD;
}

        
public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }
    
     List<Field> loop = new();

    public override string[] Solve()
    {
         var pipes = new Dictionary<char, Dir>()
         {

             {'|', Dir.Vert},
             {'-', Dir.Hor},
             {'L', Dir.NE},
             {'J', Dir.NW},
             {'7', Dir.SW},
             {'F', Dir.SE},
         };

         var gr = (InpNl.Length, InpNl[0].Length).CreateGrid<Field>();
         var startPos = new Pos2D(0,0);
         for (var i = 0; i < InpNl.Length; i++)
         {
             for (var j = 0; j < InpNl[0].Length; j++)
             {
                 var pos = new Pos2D(j,i);
                 var symbol = InpNl[i][j];
                 if (pipes.ContainsKey(symbol))
                 {
                     gr[i][j] = new Field(pos, FieldT.Pipe, symbol)
                     {
                         Dir = pipes[symbol]
                     };
                 }
                 else
                 {
                     if (symbol == 'S')
                     {
                         startPos = pos;
                         gr[i][j] = new Field(startPos, FieldT.Start, symbol);
                     }
                     else
                     {
                         gr[i][j] = new Field(pos, FieldT.Ground, symbol);
                     }
                 }
             }
         }

         var st = new Stack<Pos2D>();
         st.Push(startPos);

         var c = 0;
         var p = startPos;
         gr.GetCell(startPos).Symbol = '|';
         var adj = GetConnections(startPos).Skip(1).First();
         GD gd;
         if (adj.pos.Y == startPos.Y)
         {
             gd = adj.pos.X > startPos.X ? GD.E : GD.W;
         }
         else
         {
             gd = adj.pos.Y > startPos.Y ? GD.S : GD.N;
         }
         p = adj.pos;
         
         while (true)
         {
             var cell = gr.GetCell(p);
             var dir = cell.Dir;
             
             switch (gd)
             {
                 case GD.E when dir is Dir.SW or Dir.NW:
                     gd = dir is Dir.SW ? GD.S : GD.N;
                     break;
                 case GD.W when dir is Dir.NE or Dir.SE:
                     gd = dir is Dir.NE ? GD.N : GD.S;
                     break;
                 case GD.N when dir is Dir.SE or Dir.SW:
                     gd = dir is Dir.SE ? GD.E : GD.W;
                     break;
                 case GD.S when dir is Dir.NE or Dir.NW:
                     gd = dir is Dir.NE ? GD.E : GD.W;
                     break;
             }

             p = (gd) switch
             {
                 GD.N => p with { Y = p.Y - 1},
                 GD.S => p with { Y = p.Y + 1},
                 GD.W => p with { X = p.X - 1},
                 GD.E => p with { X = p.X + 1},
             };
             
             cell.Loop = true;
             loop.Add(cell);
             c++;

             if (cell.Pos == startPos)
             {
                 break;
             }
         }
         AssignAnswer1(c/2);

         var outer = 0;
         var inner = 0;
         
         for (var i = 0; i < gr.Length; i++)
         {
             var openings = new Stack<char>();
             var nEdges = 0;
                 
             for (var j = 0; j < gr[i].Length; j++)
             {
                 var pos = new Pos2D(j, i);
                 var cell = gr.GetCell(pos);
                 
                 if (!cell.Loop)
                 {
                     if (nEdges % 2 == 0)
                     {
                         outer++;
                         Console.Write('.');
                     }
                     else
                     {
                         Console.Write('I');
                         inner++;
                     }
                 }
                 else
                 {
                     Console.Write(cell.Symbol);
                     if (cell.Symbol is 'L' or 'F' or 'S')
                     {
                         openings.Push(cell.Symbol);
                     }
                     else
                     {
                         openings.TryPeek(out var lastOpening);
                         if (lastOpening is 'L' && cell.Symbol is '7' or 'S' ||
                             lastOpening is 'F' && cell.Symbol is 'J' or 'S')
                         {
                             nEdges++;
                             openings.Pop();
                         }
                         if (cell.Symbol is '|')
                         {
                             nEdges++;
                         }
                     }
                 }
             }
             Console.WriteLine();
         }

        
         Console.WriteLine("Dim " + (InpNl.Length, InpNl[0].Length));
         Console.WriteLine("PL " + loop.Count);
         Console.WriteLine((InpNl.Length*InpNl[0].Length-loop.Count-inner-outer));
         Console.WriteLine(outer);
         Console.WriteLine(inner);
         Console.WriteLine();
         AssignAnswer2(inner);
         
         return Answers;
         
         IEnumerable<(Pos2D pos, Field val)> GetConnections(Pos2D p) =>gr.GetAllAdjacentCells(p, false).Where(c => c.val.Type is FieldT.Pipe);
    }
}
