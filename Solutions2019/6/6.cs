using Framework;

namespace Challenges2019;

public class Solution6 : SolutionFramework
{
    public Solution6() : base(6) { }

    record Pair(string E1, string E2);
    record Node(Node? Parent, string Value);

    public override string[] Solve()
    {
        var pairs = new List<Pair>();
        var nodes = new List<Node>();
        
        int Distance(string o1, string o2)
        {
            var d = 0;
            var c = o1;
            var pred1 = GetPredecessors(c, pairs);
            c = o2;
            var pred2 = GetPredecessors(c, pairs);

            var firstCommonPred = pred1.Intersect(pred2).First();
            var dist1 = pred1.IndexOf(firstCommonPred);
            var dist2 = pred2.IndexOf(firstCommonPred);
            
            return dist1+dist2;
        }

        var memoized = new Dictionary<string, int>();

        foreach (var s in InputNlSplit)
        {
            var s1 = s.Split(')');
            var obj1 = s1.First();
            var obj2 = s1.Last();
            var pair = new Pair(obj1, obj2);

            var parent = nodes.SingleOrDefault(x => x.Value == obj1);
            nodes.Add(parent is null ? new Node(new Node(null, obj1), obj2) : new Node(parent, obj2));

            pairs.Add(pair);
        }

        foreach (var p in pairs)
        {
            NSlot += PredecessorsCount(p);
        }
        
        AssignAnswer1();
        
        NSlot = Distance("SAN", "YOU");
        AssignAnswer2();
        
        return Answers;

        int PredecessorsCount(Pair pair)
        {
            var pred = pair.E1;
            var c = 1;
            
            while (!string.IsNullOrEmpty(pred))
            {
                if (memoized.TryGetValue(pred, out var count))
                {
                    return count + c ;
                }
                
                var o = pairs.SingleOrDefault(p => p.E2 == pred);
                if (o != null)
                {
                    c++;
                    pred = o.E1;
                    
                } else
                {
                    pred = null;
                }
            }
            memoized[pair.E2] = c;
            return c;
        }
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

