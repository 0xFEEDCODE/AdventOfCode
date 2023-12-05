using Framework;

namespace Challenges2020;

public class Solution13 : SolutionFramework
{
    public Solution13() : base(13) { }

    public override string[] Solve()
    {
        var departEstimate = int.Parse(InputNlSplit.First());
        var buses = InputNlSplit.Last().Split(',')?.Where(b => b.All(Char.IsDigit)).Select(int.Parse).ToArray();

        var departTime = departEstimate;
        int? earliestBus = null;
        int? waitTime = null;
        while (earliestBus is null)
        {
            departTime++;
            foreach (var bus in buses)
            {
                if ((departTime % bus) == 0)
                {
                    earliestBus = bus;
                    waitTime = departTime - departEstimate;
                }
            }
        }
        AssignAnswer1(earliestBus.Value * waitTime!.Value);

        var busesNr = InputNlSplit.Last().Split(',')?.Select(b => b == "x" ? 0 : (uint)(int.Parse(b))).ToArray();
        //Found 101498759702480
        //var timestampEstimate = 101338657319961;
        //var timestampEstimate = 101498759702480;
        var timestampEstimate = 101338657339130;
        long? timestamp = null;
        
        var busesWithNr = new List<(int, int)>();
        for (var i = 0; i < busesNr.Length; i++)
        {
            if (busesNr[i] != 0)
            {
                busesWithNr.Add(((int)busesNr[i], i));
            }
        }

        for (var i = 0; i < busesWithNr.Count; i++)
        {
            for (var j = i+1; j < busesWithNr.Count; j++)
            {
                if (((busesWithNr[i].Item1) % busesWithNr[j].Item2) == 0)
                {
                    Console.WriteLine();
                }
            }
        }

        //busesWithNr = busesWithNr.OrderByDescending(x=>x.Item1).ToList();
        
        while (timestamp is null)
        {
            var matchFound = true;
            foreach (var bus in busesWithNr)
            {
                var rem = (timestampEstimate + bus.Item2) % bus.Item1;
                if (rem != 0)
                {
                    timestampEstimate += (661*29);
                    matchFound = false;
                    break;
                }
            }

            if (matchFound)
            {
                timestamp = timestampEstimate;
            }
        }

        AssignAnswer2(timestamp.Value);

        return Answers;
    }
    
    static long GCF(long[] numbers) => numbers.Aggregate(gcf);
    static long LCM(long[] numbers) => numbers.Aggregate(lcm);

    static long gcf(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long lcm(long a, long b) => (a / gcf(a, b)) * b;
}
