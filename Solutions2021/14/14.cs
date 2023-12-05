using System.Text;

using Framework;

namespace Challenges2021;

public class Solution14 : SolutionFramework {
    public Solution14() : base(14) { }

    public record struct Rule(string Pair, string Insert);
    public override string[] Solve() {
        var rules = new Dictionary<string, string>();
        var template = InputNlSplit.First();
        foreach (var line in InputNlSplit.Skip(1)) {
            if (line.Contains("->")) {
                var split = line.Split("->").Select(x=>x.Trim()).ToArray();
                rules.Add(split[0], split[0][0]+split[1]);
            }
        }
        var polymers = template.Distinct().ToList();
        foreach (var r in rules.Keys) {
            foreach (var p in r) {
                if (!polymers.Contains(p)) {
                    polymers.Add(p);
                }
            }
        }

        var chain = template;
        for (var i = 0; i < 10; i++) {
            var nChain = new StringBuilder();
            for (var j = 0; j < chain.Length; j++) {
                if (j + 1 < chain.Length) {
                    nChain.Append(rules[chain.Substring(j, 2)]);
                } else {
                    nChain.Append(chain[j]);
                }
            }

            chain = nChain.ToString();
        }

        var res = SubtractLceFromMce(polymers, chain);
        AssignAnswer1(res);

        //pt 2
        var chains = new Dictionary<string, long>();
        for (var i = 0; i < template.Length; i++) {
            if (i + 1 < template.Length) {
                if (!chains.ContainsKey(template[i].ToString() + template[i + 1])) {
                    chains.Add(template[i].ToString() + template[i + 1], 0);
                }
                chains[template[i].ToString() + template[i + 1]]++;
            }
        }

        var polymerCount = new Dictionary<char, long>();
        foreach (var p in polymers) {
            polymerCount.Add(p, 0);
        }

        foreach (var p in template) {
            polymerCount[p]++;
        }
        
        for (var i = 0; i < 40; i++) {
            var copy = new Dictionary<string, long>(chains);
            foreach (var key in copy.Keys) {
                var newChain = rules[key];
                var newChain2ndPart = newChain.Last().ToString() + key.Last();
                if (!chains.ContainsKey(newChain)) {
                    chains.Add(newChain, 0);
                }
                if (!chains.ContainsKey(newChain2ndPart)) {
                    chains.Add(newChain2ndPart, 0);
                }

                polymerCount[newChain.Last()] += copy[key];
                chains[newChain]+=copy[key];
                chains[newChain2ndPart]+=copy[key];
                chains[key] -= copy[key];
            }
        }
        AssignAnswer2(polymerCount.Values.Max() - polymerCount.Values.Min());
        
        return Answers;
    }

    private static long SubtractLceFromMce(List<char> polymers, string chain) {
        long mce = long.MinValue;
        long lce = long.MaxValue;
        foreach (var p in polymers) {
            var occurrences = chain.Count(x => x == p);
            mce.AssignIfBigger(occurrences);
            lce.AssignIfLower(occurrences);
        }

        return mce-lce;
    }
}
