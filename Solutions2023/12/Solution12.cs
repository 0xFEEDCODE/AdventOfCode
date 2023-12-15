using Framework;

namespace Solutions2023;

public class Solution12() : SolutionFramework(12)
{
    sealed record Set(string Cfg, int[] Nums)
    {
        public bool Equals(Set? other) => Cfg == other.Cfg && Nums.SequenceEqual(other.Nums);
        public override int GetHashCode() => Cfg.GetHashCode() + Nums.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())); 
    }
    
    public override string[] Solve()
    {
        IEnumerable<Set> inp = InpNl.Select(x =>
        {
            var l = x.Split(' ');
            var c = Enumerable.Repeat(l[0], 5).Aggregate((acc, x) => acc + "?" + x);
            var n = Enumerable.Repeat(l[1].Split(',').Select(int.Parse).ToArray(), 5).Aggregate((acc, x) => acc.Concat(x).ToArray());
            return new Set(c, n);
        }).ToArray();

        AssignAnswer1(inp.Select(Count).Sum());
        
        return Answers;
    }
    
    static Dictionary<Set, double> memo = new();
    private static double Count(Set set)
    {
        var result = 0d;
        var (cfg, nums) = set;

        if (string.IsNullOrEmpty(cfg))
        {
            return !nums.Any() ? 1 : 0;
        }
        if (!nums.Any())
        {
            return !cfg.Contains('#') ? 1 : 0;
        }

        if (memo.TryGetValue(set, out var value))
        {
            return value;
        }

        if (cfg.Any() && nums.Any())
        {
            if (".?".Contains(cfg[0]))
            {
                result += Count(new Set(new string(cfg.Skip(1).ToArray()), nums));
            }
            if ("#?".Contains(cfg[0]))
            {
                var n = nums[0];
                if (n <= cfg.Length && !cfg[..n].Contains(".") && (n == cfg.Length || cfg[n] != '#'))
                {
                    result += Count(new Set(cfg[Math.Min(n + 1, cfg.Length)..], nums.Skip(1).ToArray()));
                }
            }
        }
        
        memo.Add(set, result);
        return result;
    }
}