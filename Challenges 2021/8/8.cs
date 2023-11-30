using System.Text.RegularExpressions;

using Framework;

namespace Challenges2021;

public class Solution8 : SolutionFramework {
    public Solution8() : base(8) { }

    public override string[] Solve() {
        var c = 0;
        foreach (var line in RawInputSplitByNl) {
            var split = line.Split('|');
            var outputValueDigits = Regex.Matches(split[1], @"\w+").Select(x=>x.Value).ToList();
            c += outputValueDigits.Count(d => d.Length is 2 or 3 or 4 or 7);
        }
        AssignAnswer1(c);
        var digits = new Dictionary<int, List<char>>() {
            { 0, new List<char>() { 'a', 'b', 'c', 'e', 'f', 'g' } },
            { 1, new List<char>() { 'c', 'f' } },
            { 2, new List<char>() { 'a', 'c', 'd', 'e', 'g' } },
            { 3, new List<char>() { 'a', 'c', 'd', 'f', 'g' } },
            { 4, new List<char>() { 'b', 'c', 'd', 'f' } },
            { 5, new List<char>() { 'a', 'b', 'd', 'f', 'g' } },
            { 6, new List<char>() { 'a', 'b', 'd', 'e', 'f', 'g' } },
            { 7, new List<char>() { 'a', 'c', 'f' } },
            { 8, new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' } },
            { 9, new List<char>() { 'a', 'b', 'c', 'd', 'f', 'g' } },
        };
        
        //pt 2
        long sum = 0;
        foreach (var line in RawInputSplitByNl) {
            var signalMapping = new Dictionary<char, char>();
            var split = line.Split('|');
            var signalPatterns = Regex.Matches(split[0], @"\w+").Select(x=>x.Value).ToList();
            var outputValueDigits = Regex.Matches(split[1], @"\w+").Select(x=>x.Value).ToList();
            var allSignals = string.Join("", signalPatterns);
            
            /* Occurrences
            * A 8
            * B 6
            * C 8
            * D 7
            * E 4
            * F 9
            * G 7
            */
            var allSignalsDistinct = allSignals.Distinct().ToArray();
            foreach (var signal in allSignalsDistinct) {
                switch (allSignals.Count(s => s==signal)) {
                    case 4:
                        signalMapping.Add('e', signal);
                        break;
                    case 6:
                        signalMapping.Add('b', signal);
                        break;
                    case 9:
                        signalMapping.Add('f', signal);
                        break;
                }
            }

            signalMapping.Add('c', signalPatterns.Single(x => x.Length == 2).Single(s => !signalMapping.ContainsValue(s)));
            signalMapping.Add('d', signalPatterns.Single(x => x.Length == 4).Single(s => !signalMapping.ContainsValue(s)));
            signalMapping.Add('a', signalPatterns.Single(x => x.Length == 3).Single(s => !signalMapping.ContainsValue(s)));
            signalMapping.Add('g', allSignals.First(x => !signalMapping.ContainsValue(x)));

            var outputDigits = new List<int>();
            foreach (var d in outputValueDigits) {
                var convertedD = d.Select(x => signalMapping.Single(kv => kv.Value == x).Key).ToArray();
                outputDigits.Add(digits.Single(x => x.Value.Count() == convertedD.Length && x.Value.All(y => convertedD.Contains(y))).Key);
            }

            sum += long.Parse(string.Join("", outputDigits.Select(x => x.ToString())));
        }
        AssignAnswer2(sum);

        return Answers;
    }
}
