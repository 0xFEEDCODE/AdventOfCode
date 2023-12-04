using Framework;

namespace Solutions2023;

public class Solution4 : SolutionFramework
{
    public Solution4() : base(4) { }

    public override string[] Solve()
    {
        var points = 0;
        var nCards = 0;
        
        var processedCards = new Dictionary<int, int>();
        
        foreach (var s in RawInputSplitByNl)
        {
            var s1 = s.Split(':');
            var cardN = s1.First().Split(' ').Last().ParseInt();
            var spl = s1.Last().Split(" | ");
            var w = spl.First().Split(' ').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
            var y = spl.Last().Split(' ').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();

            var c = w.Count(x => y.Contains(x));
            points += CountPoints(c);

            var cardNQuantity = ProcessCard(processedCards, cardN, 1);

            for (var i = cardN + 1; i < cardN + 1 + c; i++)
            {
                ProcessCard(processedCards, i, cardNQuantity);
            }
        }
        
        AssignAnswer1(points);
        AssignAnswer2(processedCards.Values.Sum());

        return Answers;
    }
    
    private static int CountPoints(int c)
    {
        var points = 1;
        for (var i = 0; i < c; i++)
        {
            points <<= 1;
        }
        return points >> 1;
    }

    private static int ProcessCard(Dictionary<int, int> processedCards, int idx, int quantity)
    {
        if (processedCards.ContainsKey(idx))
        {
            processedCards[idx] += quantity;
        }
        else
        {
            processedCards[idx] = quantity;
        }
        return processedCards[idx];
    }
}