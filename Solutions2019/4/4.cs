using Framework;

namespace Challenges2019;

public class Solution4 : SolutionFramework
{
    public Solution4() : base(4) { }

    public override string[] Solve()
    {
        int rangeStart = 264360, rangeEnd = 746325;

        var nPossiblePw = 0;
        var nPossiblePw2 = 0;

        var range = Enumerable.Range(rangeStart, rangeEnd - rangeStart + 1);

        foreach (var n in range)
        {
            if (matchesCriteria(n))
            {
                nPossiblePw++;
            }
            if (matchesCriteria2(n))
            {
                nPossiblePw2++;
            }
        }
        
        AssignAnswer1(nPossiblePw);
        AssignAnswer2(nPossiblePw2);
        return Answers;

        bool matchesCriteria(int pw)
        {
            var prev = int.MinValue;
            var foundAdjacent = false;

            foreach (var c in pw.ToString())
            {
                var d = c.ParseInt();

                if (d < prev)
                {
                    return false;
                }
                if (d == prev)
                {
                    foundAdjacent = true;
                }

                prev = d;
            }
            return foundAdjacent;
        }

        bool matchesCriteria2(int pw)
        {
            var prev = int.MinValue;
            var adjacents = new List<int>();
            var adjStreak = 0;

            foreach (var c in pw.ToString())
            {
                var d = c.ParseInt();

                if (d < prev)
                {
                    return false;
                }
                if (d == prev)
                {
                    adjStreak++;
                    if (adjStreak is 1)
                    {
                        adjacents.Add(2);
                    } else
                    {
                        adjacents[^1]++;
                    }
                } else
                {
                    adjStreak = 0;
                }

                prev = d;
            }
            return adjacents.Count(adj => adj == 2) >= 1;
        }
    }
}

