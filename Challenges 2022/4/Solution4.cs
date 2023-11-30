using Framework;

namespace Challenges2022;

public class Solution4 : SolutionFramework
{
    public Solution4() : base(4) { }

    struct SetOfPairs
    {
        public (int, int) P1;
        public (int, int) P2;
    }

    public override string[] Solve()
    {
        var setOfPairs = new List<SetOfPairs>();

        foreach (var line in RawInput.SplitByNewline())
        {
            var sections = line.Split(',').Select(x => x.Split('-').Select(int.Parse).ToArray()).ToArray();
            setOfPairs.Add(new SetOfPairs { P1 = (sections[0][0], sections[0][1]), P2 = (sections[1][0], sections[1][1]) });
        }

        var overlapCount = 0;
        foreach (var pair in setOfPairs)
        {
            if ((pair.P1.Item1 >= pair.P2.Item1 && pair.P1.Item2 <= pair.P2.Item2)
                || (pair.P2.Item1 >= pair.P1.Item1 && pair.P2.Item2 <= pair.P1.Item2))
            {
                overlapCount++;
            }
        }
        AssignAnswer1(overlapCount);

        overlapCount = 0;
        foreach (var pair in setOfPairs)
        {
            if ((pair.P2.Item1 >= pair.P1.Item1 && pair.P2.Item1 <= pair.P1.Item2)
                ||  (pair.P1.Item1 >= pair.P2.Item1 && pair.P1.Item1 <= pair.P2.Item2))
            {
                overlapCount++;
            }
        }
        AssignAnswer2(overlapCount);

        return Answers;
    }
}
