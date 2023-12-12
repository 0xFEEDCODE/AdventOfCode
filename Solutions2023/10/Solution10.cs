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
    public bool Considered = false;
    public bool IsOutside;
    public bool Inner;
    public bool Opening;
    public char Symbol = symbol;
    public GD? GD;
}

        
public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }
    
     List<Field> loop = new();

     record ExpectedPipeRecord(Pos2D P1, char P1S, Pos2D P2, char P2S);
    
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
         Console.WriteLine(gd.ToString());
         
         
         while (true)
         {
             var cell = gr.GetCell(p);
             var pos = cell.Pos;
             var dir = cell.Dir;
             var s = cell.Symbol;
             
             switch (s)
             {
                 case 'L':
                 {
                     switch (gd)
                     {
                         case GD.N:
                             try { gr.GetCell(pos with { X = pos.X + 1, Y = pos.Y + 1 }).Inner = true; }
                             catch { }
                             break;

                         case GD.W:
                             try { gr.GetCell(pos with { X = pos.X - 1, Y = pos.Y - 1 }).Inner = true; }
                             catch { }
                             break;
                     }
                     break;
                 }

                 case 'J':
                     switch (gd)
                     {
                         case GD.E:
                             try { gr.GetCell(pos with { X = pos.X - 1, Y = pos.Y - 1 }).Inner = true; }
                             catch { }
                             break;
                         case GD.N:
                             try { gr.GetCell(pos with { X = pos.X + 1, Y = pos.Y + 1 }).Inner = true; }
                             catch { }
                             break;
                     }
                     break;
                 case '7':
                     switch (gd)
                     {
                         case GD.E:
                             try { gr.GetCell(pos with { X = pos.X + 1, Y = pos.Y + 1 }).Inner = true; }
                             catch { }
                             break;
                         case GD.S:
                             try { gr.GetCell(pos with { X = pos.X - 1, Y = pos.Y - 1 }).Inner = true; }
                             catch { }
                             break;
                     }
                     break;
                 case 'F':
                     switch (gd)
                     {
                         case GD.N:
                             try { gr.GetCell(pos with { X = pos.X - 1, Y = pos.Y - 1 }).Inner = true; }
                             catch { }
                             break;
                         case GD.W:
                             try { gr.GetCell(pos with { X = pos.X + 1, Y = pos.Y + 1 }).Inner = true; }
                             catch { }
                             break;
                     }
                     break;
             }


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

         var nonInner = 0;
         var inner = 0;
         
         //DetermineOpenings();

         for (var i = 0; i < gr.Length; i++)
         {
             for (var j = 0; j < gr[i].Length; j++)
             {
                 var pos = new Pos2D(j, i);
                 var cell = gr.GetCell(pos);

                 if (cell is { Loop: false, Considered: false })
                 {
                     var area = GetSeparatedGroundArea(pos);
                     var isInner = !area.Items.Any(x => x.Inner);
                     if (isInner)
                     {
                         inner += area.Items.Length;
                     }
                     
                     foreach (var kv in area.Items)
                     {
                         kv.Inner = !kv.Loop && isInner;
                     }
                 }
                 Console.Write(cell.Inner ? "I" : cell.Loop ? cell.Symbol : 'O');
             }
             Console.WriteLine();
         }

        
         Console.WriteLine("Dim " + (InpNl.Length, InpNl[0].Length));
         Console.WriteLine("PL " + loop.Count);
         Console.WriteLine((InpNl.Length*InpNl[0].Length-loop.Count-inner-nonInner));
         Console.WriteLine(nonInner);
         Console.WriteLine(inner);
         Console.WriteLine();
         AssignAnswer2(inner);
         
         return Answers;
         
         
         IEnumerable<(Pos2D pos, Field val)> GetConnections(Pos2D p) =>gr.GetAllAdjacentCells(p, false).Where(c => c.val.Type is FieldT.Pipe);

         void DetermineOpenings()
         {
                     //VERT
             for (var i = 0; i < gr.Length-1; i++)
             {
                 for (var j = 0; j < gr[i].Length; j++)
                 {
                     var pos = new Pos2D(j, i);
                     var c1 = gr.GetCell(pos);
                     var c2 = gr.GetCell(pos with{Y = pos.Y+1});
                     if (!(c1.Loop && c2.Loop))
                     {
                         continue;
                     }
                     var joint = c1.Symbol.ToString() + c2.Symbol;
                     if (joint is "--" or "JF" or "L7" or "-F" or "-7" or "L-" or "J-" or "||" or "|L" or "|J" or "FL" or "7|" or "7L" or "7J")
                     {
                         c1.Opening = true;
                         c2.Opening = true;
                     }
                 }
             }
                     //HOR
             for (var i = 0; i < gr.Length; i++)
             {
                 for (var j = 0; j < gr[i].Length-1; j++)
                 {
                     var pos = new Pos2D(j, i);
                     var c1 = gr.GetCell(pos);
                     var c2 = gr.GetCell(pos with{X = pos.X+1});
                     if (!(c1.Loop && c2.Loop))
                     {
                         continue;
                     }
                     var joint = c1.Symbol.ToString() + c2.Symbol;
                     if (joint is "||" or "7F" or "JL" or "J|" or "|F" or "7|" or "J|" or "|L" or "S-" or "-7" or "F-" or "LJ" or "7L" or "-7" or "--" or "F7" or "S7")
                         
                     {
                         c1.Opening = true;
                         c2.Opening = true;
                     }
                 }
             }
                     
         }

         (Field[] Items, bool hasOpening) GetSeparatedGroundArea(Pos2D pos)
         {
             var cell = gr.GetCell(pos);
             var area = new List<Field>() {};
             var st = new Stack<Field>();
             
             st.Push(cell);
             var open = false;

             while (st.Any())
             {
                 cell = st.Pop();
                 area.Add(cell);
                 cell.Considered = true;

                 var adj1 = gr.GetAllAdjacentCells(cell.Pos, false).Where(kv=>kv.val is { Loop: false, Considered: false }); 
                 foreach (var kv in adj1)
                 {
                     if (!area.Contains(kv.val))
                     {
                         st.Push(kv.val);
                     }
                 }
             }
             
             return (area.ToArray(), area.Any(x => x.Pos.X == 0 || x.Pos.Y == 0 || x.Pos.X == gr[0].Length || x.Pos.Y == gr.Length));
         }
         
         bool CanPass(GD gd, Pos2D p1, Pos2D p2)
         {
             Field? c1, c2;
             try
             {
                 c1 = gr.GetCell(p1);
                 c2 = gr.GetCell(p2);
             }
             catch
             {
                 return false;
             }
             var pipe = c1.Symbol.ToString() + c2.Symbol;

             switch (gd)
             {
                 case GD.N:
                     return pipe is "||" or "7F" or "|F" or "7|";
                 case GD.S:
                     return pipe is "||" or "JL" or "|L" or "J|";
                 case GD.W:
                     return pipe is "--" or "L7" or "L-" or "-7";
                 case GD.E:
                     return pipe is "--" or "JF" or "J-" or "-F";
                 case GD.BL:
                     return pipe is "JF";
                 case GD.BR:
                     return pipe is "7L";
                 case GD.TL:
                     return pipe is "L7";
                 case GD.TR:
                     return pipe is "JF";
             }
             return false;
         }
         
    }
}
