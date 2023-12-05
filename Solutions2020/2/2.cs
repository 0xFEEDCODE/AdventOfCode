using System.Text.RegularExpressions;

using Framework;

namespace Challenges2020;

public class Solution2 : SolutionFramework
{
    public Solution2() : base(2) { }


    record struct ValidationRule(int Min, int Max, char Symbol);
    record struct ValidationEntry(ValidationRule Rule, string ToValidate);
    
    public override string[] Solve() {
        var entriesToValidate = new List<ValidationEntry>();

        foreach (var line in InputNlSplit) {
            const string pattern = @"(?<x>\d+)-(?<y>\d+) (?<z>\w): (?<xx>\w+)";
            var groups = Regex.Match(line, pattern).Groups;
            entriesToValidate.Add(
                new ValidationEntry(new ValidationRule(int.Parse(groups[1].Value), int.Parse(groups[2].Value), groups[3].Value.Single()), groups[4].Value));
        }

        var validCount = 0;
        foreach (var validationEntry in entriesToValidate) {
            var symbolOccurrences = validationEntry.ToValidate.Count(ch => ch == validationEntry.Rule.Symbol);
            if (symbolOccurrences >= validationEntry.Rule.Min && symbolOccurrences <= validationEntry.Rule.Max) {
                validCount++;
            }
        }
        AssignAnswer1(validCount);
        
        validCount = 0;
        foreach (var validationEntry in entriesToValidate) {
            var matchesFirst = validationEntry.ToValidate.ElementAt(validationEntry.Rule.Min-1) == validationEntry.Rule.Symbol;
            var matchesSecond = validationEntry.ToValidate.ElementAt(validationEntry.Rule.Max-1) == validationEntry.Rule.Symbol;
            if ((matchesFirst && !matchesSecond) || (!matchesFirst && matchesSecond)) {
                validCount++;
            }
        }
        AssignAnswer2(validCount);
        
        return Answers;
    }
}
