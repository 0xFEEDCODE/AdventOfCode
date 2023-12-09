using Framework;

namespace Solutions2023;
    
public class Solution8 : SolutionFramework
{
    public Solution8() : base(8) { }

    enum Direction { L, R };

    class Node(string value, Node? left = null, Node? right = null)
    {
        public readonly string Value = value;
        public Node? Left = left;
        public Node? Right = right;
    }
    
    public override string[] Solve()
    {
        var directions = InputNlSplit.First().Select(x => Enum.Parse<Direction>(x.ToString())).ToArray();
        var map = new Dictionary<string, (string L, string R)>();
        var nodes = new List<Node?>();
        
        foreach (var line in InputNlSplit.Skip(2))
        {
            var spl = line.Split("=");
            
            var origin = spl.First().Trim();
            var repl = spl.Last().Replace(" ", "").Replace("(", "").Replace(")", "");
            spl = repl.Split(",");

            var left = spl.First();
            var right = spl.Last();
            
            map.Add(origin, (left, right));
            nodes.Add(new Node(origin));
        }
        
        foreach (var kv in map)
        {
            var n = nodes.Single(n => n.Value == kv.Key); n.Left = nodes.Single(n => n.Value == kv.Value.L);
            n.Right = nodes.Single(n => n.Value == kv.Value.R);
        }
        
        var aNodes = nodes.Where(a => a.Value.EndsWith('A')).ToArray();
        var stepsToReachZ = new List<(string Value, double Steps)>();

        foreach(var n in aNodes)
        {
            double s = 0;
            var fl = 0;
            var current = n;
            while (fl == 0)
            {
                foreach (var d in directions)
                {
                    s++;
                    current = d == Direction.L ? current.Left : current.Right;
                    if (current.Value.EndsWith("Z"))
                    {
                        fl = 1;
                        stepsToReachZ.Add((current.Value, s));
                        break;
                    }
                }
            }
        }

        AssignAnswer1(stepsToReachZ.First(s => s.Value is "ZZZ").Steps);
        AssignAnswer2(stepsToReachZ.Select(s => s.Steps).Aggregate(LCM));
        
        static double GCD(double a, double b) 
        {
            if (a == 0)
            {
                return b;
            }
            return GCD(b % a, a);  
        } 
        
        static double LCM(double a, double b) => (a / GCD(a, b)) * b;
     
        return Answers;
    }
    
}
