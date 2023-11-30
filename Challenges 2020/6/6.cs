using Framework;

namespace Challenges2020;

public class Solution6 : SolutionFramework {
    public Solution6() : base(6) { }

    public override string[] Solve() {
        var allGroupsAnswers = new List<(int, Dictionary<char, int>)>();
        var groupAnswers = new Dictionary<char, int>();
        var groupMemberCount = 0;
        
        foreach (var line in RawInputSplitByNl.Append(string.Empty)) {
            if (string.IsNullOrEmpty(line)) {
                allGroupsAnswers.Add((groupMemberCount, groupAnswers));
                groupAnswers = new Dictionary<char, int>();
                groupMemberCount = 0;
            } else {
                groupMemberCount++;
                foreach (var ch in line) {
                    if (!groupAnswers.ContainsKey(ch)) {
                        groupAnswers.Add(ch, 0);
                    }
                    groupAnswers[ch]++;
                }
            }
        }
        AssignAnswer1(allGroupsAnswers.Sum(g=>g.Item2.Keys.Count));
        AssignAnswer2(allGroupsAnswers.Sum(g=>g.Item2.Count(nAnswers => nAnswers.Value == g.Item1)));
        
        return Answers;
    }
}
