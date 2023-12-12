using System.Collections;

using Framework;

namespace Challenges2019;


    
public enum FieldT { Pipe, Ground, Start }
public enum Dir { Vert, Hor, TR, TL, BR, BL };

public class Field(Pos2D pos, FieldT type, Dir? dir = null, bool visited = false, int distS = 0)
{
    public Pos2D Pos = pos;
    public FieldT Type = type;
    public Dir? Dir = dir;
    public bool Visited = visited;
    public int DistS = distS;
}

public class Solution12 : SolutionFramework
{
    public Solution12() : base(12) { }

    public override string[] Solve()
    {
         var pipes = new Dictionary<char, Dir>()
         {

             {'|', Dir.Vert},
             {'-', Dir.Hor},
             {'L', Dir.TR},
             {'J', Dir.TL},
             {'7', Dir.BR},
             {'F', Dir.BL},
         };

         var gr = (InpNl.Length, InpNl[0].Length).CreateGrid<Field>();
         var startPos = new Pos2D(0,0);
         for (var i = 0; i < InpNl.Length; i++)
         {
             for (var j = 0; j < InpNl[0].Length; j++)
             {
                 var pos = new Pos2D(i, j);
                 if (pipes.ContainsKey(InpNl[i][j]))
                 {
                     gr[i][j] = new Field(pos, FieldT.Pipe, pipes[InpNl[i][j]]);
                 }
                 else
                 {
                     if (InpNl[i][j] == 'S')
                     {
                         startPos = new Pos2D(i, j);
                         gr[i][j] = new Field(pos, FieldT.Start);
                     }
                     else
                     {
                         gr[i][j] = new Field(pos, FieldT.Ground);
                     }
                 }
             }
         }

         var st = new Stack<Field>();
         PushConnections(GetConnections());
         
         while (st.Any())
         {
             var c1 = st.Pop();
             var cell = gr.GetCell(c1.Pos);
             cell.Visited = true;
             cell.DistS++;

             var p1 = c1.Pos;
             var d1 = c1.Dir;

             var connections = GetConnections();
             foreach (var kv in connections)
             {
                 var d2 = kv.val.Dir;
                 var p2 = kv.val.Pos;
                 switch (d2)
                 {
                     case Dir.Hor when d1 != Dir.Vert:
                     case Dir.Vert when d1 != Dir.Hor:
                         st.Push(kv.val);
                         break;
                     case Dir.TL:
                         switch (d1)
                         {
                             case Dir.Vert when p1.Y < p2.Y:
                             case Dir.Hor when p1.X < p2.X:
                                 st.Push(kv.val);
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException();
                         }
                         break;
                     case Dir.TR:
                         switch (d1)
                         {
                             case Dir.Vert when p1.Y < p2.Y:
                             case Dir.Hor when p1.X > p2.X:
                                 st.Push(kv.val);
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException();
                         }
                         break;
                     case Dir.BL:
                         switch (d1)
                         {
                             case Dir.Vert when p1.Y > p2.Y:
                             case Dir.Hor when p1.X < p2.X:
                                 st.Push(kv.val);
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException();
                         }
                         break;
                     case Dir.BR:
                         switch (d1)
                         {
                             case Dir.Vert when p1.Y > p2.Y:
                             case Dir.Hor when p1.X > p2.X:
                                 st.Push(kv.val);
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException();
                         }
                         break;
                 }
             }

             var farthest = 0d;
             gr.ForEachCell((c) =>
             {
                 var dist = gr.GetCell(c).DistS;
                 if (farthest < dist)
                 {
                     farthest = dist;
                 }
             });
             AssignAnswer1(farthest);
         }
         
         
         return Answers;
        
         void PushConnections(IEnumerable<(Pos2D, Field)> connections)
         {
             foreach (var kv in connections)
             {
                 st.Push(kv.Item2);
             }
         }
         
         IEnumerable<(Pos2D pos, Field val)> GetConnections() =>
             gr.GetAllAdjacentCells(startPos).Where(c =>
             {
                 if (c.val.Type != FieldT.Pipe)
                 {
                     return false;
                 }
             
                 return c.val.Type is FieldT.Pipe && !gr.GetCell(c.pos).Visited;
             });
    }
}
