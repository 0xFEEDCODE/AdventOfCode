using System.Xml.Linq;

using Framework;

namespace Solutions2023;


public class Solution14Alt() : SolutionFramework(14)
{
    public override string[] Solve()
    {
        IEnumerable<IEnumerable<char>> grid = InpGr<char>();
        
        Transpose();
        MoveRocks();
        Transpose();
        int i;
        NSlot = SupportBeamsLoad();
        AssignAnswer1();

        grid = InpGr<char>();
        i = 0;
        
        var flattened = grid.Aggregate(string.Empty, (acc, x) => acc + new string(x.ToArray()));
        var seen = new HashSet<string>(){flattened};
        var arr = new List<string>() {flattened};
        int first;
        while (true)
        {
            i++;
            Cycle();
            
            flattened = grid.Aggregate(string.Empty, (acc, x) => acc + new string(x.ToArray()));
            if (!seen.Add(flattened))
            {
                first = arr.IndexOf(flattened);
                break;
            }
            arr.Add(flattened);
            Console.WriteLine(i);
        }

        grid = arr[(1000000000 - first) % (i - first) + first].Slice(grid.First().Count());
        NSlot = SupportBeamsLoad();
        
        AssignAnswer2();

        return Answers;

        void Transpose()
        {
            grid = grid.Transpose();
            //grid = grid.Select(x => (IEnumerable<char>)new string(x.ToArray()));
        }
        
        void MoveRocks()
        {
            grid = grid
                .Select(s => new string(s.ToArray()).Split('#')
                    .Aggregate(string.Empty, (acc, x) => $"{acc}{new string(x.OrderDescending().ToArray())}#")
                    .SkipLast(1));
            
            //grid = grid.Select(x => (IEnumerable<char>)new string(x.ToArray()));
        }
        
        void Cycle()
        {
            for (var i = 0; i < 4; i++)
            {
                grid = grid.Transpose();
                MoveRocks();
                grid = grid.Select(x => x.Reverse());
                //grid = grid.Select(x => (IEnumerable<char>)new string(x.ToArray()));
            }
        }
        
        int SupportBeamsLoad()
        {
            var total = 0;
            for (i = 0; i < grid.Count(); i++)
            {
                total += grid.ElementAt(i).Count(x => x == 'O') * (grid.Count() - i);
            }
            return total;
        }
    }
}
