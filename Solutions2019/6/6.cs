using Framework;

namespace Challenges2019;

public class Solution6 : SolutionFramework
{
    public Solution6() : base(6) { }

    record Pair(string E1, string E2);

    public override string[] Solve()
    {
        var pairs = new List<Pair>();
        
        int Distance(string o1, string o2)
        {
            var d = 0;
            var c = o1;
            var pred1 = GetPredecessors(c, pairs);
            c = o2;
            var pred2 = GetPredecessors(c, pairs);


            Console.WriteLine();
            return 0;
        }

        int PredecessorsCount(Pair pair)
        {
            var c = 0;
            var e = pair.E1;
            while (!string.IsNullOrEmpty(e))
            {
                var o = pairs.SingleOrDefault(p => p.E2 == e);
                if (o != null)
                {
                    c++;
                    e = o.E1;
                } else
                {
                    e = null;
                }
            }
            return c;
        }
        
        foreach (var s in RawInputSplitByNl)
        {
            var s1 = s.Split(')');
            var obj1 = s1.First();
            var obj2 = s1.Last();
            var pair = new Pair(obj1, obj2);
            pairs.Add(pair);
        }

        var d = Distance("SAN", "YOU");

        foreach (var p in pairs)
        {
            var c = (p, PredecessorsCount(p) + 1);
            NumSlot += PredecessorsCount(p) + 1;
        }
        
        AssignAnswer1();
        
        
        return Answers;
    }
    
    private static List<string> GetPredecessors(string c, List<Pair>? pairs)
    {
        var pred = new List<string>();
        while (!string.IsNullOrEmpty(c))
        {
            var o = pairs.SingleOrDefault(p => p.E2 == c);
            if (o != null)
            {
                pred.Add(o.E1);
                c = o.E1;
            }
            else
            {
                c = null;
            }
        }
        return pred;
    }

}

