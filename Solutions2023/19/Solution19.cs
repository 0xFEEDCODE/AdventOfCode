using Framework;

namespace Solutions2023;

public class Solution19() : SolutionFramework(19)
{

    enum Category { x, m, a, s };

    enum Operation { LT, GT };

    record Rule(Category Cat, Operation Op, int Val, string WFlow);
    
    public override string[] Solve()
    {
        var workflows = new Dictionary<string, (Rule[] Rules, string targetWFlow)>();
        var parts = new List<(int x, int m, int a, int s)>();

        var parsingParts = false;
        foreach (var line in InpNl)
        {
            if (line == string.Empty)
            {
                parsingParts = true;
                continue;
            }

            if (!parsingParts)
            {
                var spl = line.Split('{');
                var workflow = spl[0];
                spl = spl[1].Split(',');
                var rules = new List<Rule>();
                
                foreach (var substr in spl.SkipLast(1))
                {
                    var cat = (Category)Enum.Parse(typeof(Category), substr[0].ToString(), true);
                    var op = substr[1] == '<' ? Operation.LT : Operation.GT;
                    var val = new string(substr.Where(char.IsDigit).ToArray()).ParseInt();
                    var targetWFlow = substr.Split(':').Last();
                    var rule = new Rule(cat, op, val, targetWFlow);
                    rules.Add(rule);
                }
                
                workflows[workflow] = (rules.ToArray(), spl.Last().Substring(0, spl.Last().Length-1));
            }
            else
            {
                var spl = line.Split(',');
                var values = spl.Select(substr => new string(substr.Where(char.IsDigit).ToArray()).ParseInt()).ToList();
                parts.Add((values[0], values[1], values[2], values[3]));
            }
        }

        (int x, int m, int a, int s) result = (0,0,0,0);
        foreach (var (x, m, a, s) in parts)
        {
            var wFlowName = "in";
            while (wFlowName is not ("R" or "A"))
            {
                var wFlow = workflows[wFlowName];

                var processedAllRules = true;
                foreach (var (cat, op, val, targetWFlow) in wFlow.Rules)
                {
                    var compareTo = cat switch
                    {
                        Category.x => x,
                        Category.m => m,
                        Category.a => a,
                        Category.s => s,
                    };

                    switch (op)
                    {
                        case Operation.GT when val > compareTo:
                        case Operation.LT when val < compareTo:
                            continue;
                    }

                    processedAllRules = false;
                    wFlowName = targetWFlow;
                    break;
                }

                if (processedAllRules)
                {
                    wFlowName = wFlow.targetWFlow;
                }
            }
            
            if (wFlowName is "A")
            {
                result.x += x;
                result.m += m;
                result.a += a;
                result.s += s;
            }
        }
        
        AssignAnswer1(result.x+result.m+result.a+result.s);

        var fullRange = new Range(1, 4000);

        AssignAnswer1(167409079868000);
        AssignAnswer2(Accepted("in" ,fullRange, fullRange, fullRange, fullRange));
        
        return Answers;
        
        double Accepted(string wfName, Range x, Range m, Range a, Range s)
        {
            var result = 0d;
            
            switch (wfName)
            {
                case "R":
                    return 0;
                case "A":
                {
                    return new[] { x, m, a, s }.
                        Aggregate<Range, double>(1, (current, range) => current * ((range.End.Value - range.Start.Value) + 1));
                }
            }

            var wf = workflows[wfName];
            
            foreach (var (cat, op, rangeSplit, wFlow) in wf.Rules)
            {
                var lX = cat is Category.x ? new Range(x.Start, rangeSplit) : x;
                var lM = cat is Category.m ? new Range(m.Start, rangeSplit) : m;
                var lA = cat is Category.a ? new Range(a.Start, rangeSplit) : a;
                var lS = cat is Category.s ? new Range(s.Start, rangeSplit) : s;
                
                var hX = cat is Category.x ? new Range(rangeSplit, x.End) : x;
                var hM = cat is Category.m ? new Range(rangeSplit, m.End) : m;
                var hA = cat is Category.a ? new Range(rangeSplit, a.End) : a;
                var hS = cat is Category.s ? new Range(rangeSplit, s.End) : s;

                if (op is Operation.GT)
                {
                    result += Accepted(wFlow,
                        cat is Category.x ? hX.IncStart(1) : hX,
                        cat is Category.m ? hM.IncStart(1) : hM,
                        cat is Category.a ? hA.IncStart(1) : hA,
                        cat is Category.s ? hS.IncStart(1) : hS);
                    x = lX; 
                    m = lM; 
                    a = lA; 
                    s = lS;
                }
                else if (op is Operation.LT)
                {
                    result += Accepted(wFlow, 
                        cat is Category.x ? lX.DecEnd(1) : lX,
                        cat is Category.m ? lM.DecEnd(1) : lM,
                        cat is Category.a ? lA.DecEnd(1) : lA,
                        cat is Category.s ? lS.DecEnd(1) : lS);
                    x = hX; 
                    m = hM; 
                    a = hA; 
                    s = hS;
                }
            }

            result += Accepted(wf.targetWFlow, x, m, a, s);
            return result;
        }
    }
}
static partial class Ext
{
    public static Range IncStart(this Range range, int inc) => new(range.Start.Value + inc, range.End);
    public static Range DecEnd(this Range range, int dec) => new(range.Start, range.End.Value - dec);
}
