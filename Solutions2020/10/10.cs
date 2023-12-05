using System.Numerics;

using Framework;

namespace Challenges2020;

public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }


    public override string[] Solve()
    {
        var adapters = InputNlSplit.Select(int.Parse).ToList();
        adapters.Sort();

        var differences = new Dictionary<int, int>{{1,0},{2,0},{3,0}};
        var currentJolt = 0;
        while (currentJolt < adapters.Max())
        {
            var options = adapters.Where(adpt => adpt > currentJolt && adpt <= currentJolt + 3).ToArray();
            if (!options.Any())
            {
                break;
            }
            differences[options.First() - currentJolt]++;
            currentJolt = options.First();
        }

        differences[3]++;
        
        AssignAnswer1(differences[1]*differences[3]);

        long arrangementsFound = 0;
        var arrangements = new Dictionary<int, long>(){{0, 1}};
        while (arrangements.Any(kv=>kv.Value>0))
        {
            foreach (var arrangementJoltage in arrangements.Keys.ToArray())
            {
                var options = adapters.Where(adpt => adpt > arrangementJoltage && adpt <= arrangementJoltage + 3);
                if (!options.Any())
                {
                    arrangementsFound += arrangements[arrangementJoltage];
                    arrangements[arrangementJoltage] = 0;
                }
                
                foreach (var option in options)
                {
                    if (!arrangements.ContainsKey(option))
                    {
                        arrangements.Add(option, 0);
                    }
                    arrangements[option] += arrangements[arrangementJoltage];
                }
                arrangements[arrangementJoltage] = 0;
            }
        }
        
        AssignAnswer2(arrangementsFound);
        return Answers;
    }
}
