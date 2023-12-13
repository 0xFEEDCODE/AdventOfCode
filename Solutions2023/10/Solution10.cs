using Framework;

namespace Solutions2023;

public class Solution10() : SolutionFramework(10)
{
    public override string[] Solve()
    {
         var gr = InputAsGrid<char>();
         var startPos = new GridPos(0,0);
         for (var i = 0; i < gr.Length; i++)
         {
             for (var j = 0; j < gr[0].Length; j++)
             {
                 if (gr[i][j] == 'S')
                 {
                     startPos = new GridPos(i, j);
                     break;
                 }
             }
             if (startPos != new GridPos(0, 0))
             {
                 break;
             }
         }

         var st = new Stack<GridPos>();
         var loop = new Dictionary<GridPos, int>();
         var seen = new Dictionary<GridPos, int>();
         
         st.Push(startPos);
         
         while (st.Any())
         {
             var pos = st.Pop();
             loop.Add(pos, 0);
             
             var ch = gr.GetCell(pos);
             var toAdd = new List<GridPos>();
             
             if (gr.TryGetNeighborCellRight(pos, out var r) && "S-LF".Contains(ch) && "-7J".Contains(r))
             {
                 toAdd.Add(pos with{ C = pos.C + 1});
             }
             if (gr.TryGetNeighborCellLeft(pos, out var l) && "S-7J".Contains(ch) && "-LF".Contains(l))
             {
                 toAdd.Add(pos with{ C = pos.C - 1});
             }
             if (gr.TryGetNeighborCellDown(pos, out var d) && "S|F7".Contains(ch) && "|JL".Contains(d))
             {
                 toAdd.Add(pos with { R = pos.R + 1});
             }
             if (gr.TryGetNeighborCellUp(pos, out var u) && "S|JL".Contains(ch) && "|F7".Contains(u))
             {
                 toAdd.Add(pos with { R = pos.R - 1});
             }
             
             foreach (var p in toAdd.Where(p => !seen.ContainsKey(p)))
             {
                 seen.Add(p, 0);
                 st.Push(p);
             }
         }
         
         AssignAnswer1(loop.Count/2);
         
         gr.SetCell(startPos, '|');
         
         var inner = 0;
         
         for (var i = 0; i < gr.Length; i++)
         {
             var openings = new Stack<char>();
             var nEdges = 0;
                 
             for (var j = 0; j < gr[i].Length; j++)
             {
                 var pos = new GridPos(i, j);
                 var cell = gr.GetCell(pos);
                 
                 if (!loop.ContainsKey(pos))
                 {
                     if (nEdges % 2 != 0)
                     {
                         inner++;
                     }
                 }
                 else
                 {
                     if ("LFS".Contains(cell))
                     {
                         openings.Push(cell);
                     }
                     else
                     {
                         openings.TryPeek(out var lastOpening);
                         if (lastOpening is 'L' && cell is '7' or 'S' ||
                             lastOpening is 'F' && cell is 'J' or 'S')
                         {
                             nEdges++;
                             openings.Pop();
                         }
                         if (cell is '|')
                         {
                             nEdges++;
                         }
                     }
                 }
             }
         }

         AssignAnswer2(inner);
         
         return Answers;
    }
}
