using Framework;

namespace Solutions2023;

public class Solution4 : SolutionFramework
{
    public Solution4() : base(4) { }

    record Numbers(ICollection<int> Winning, ICollection<int> Yours);
    
    public override string[] Solve()
    {
        var c = new Dictionary<int, Numbers>();
        foreach (var s in RawInputSplitByNl)
        {
            var s1 = s.Split(':');
            var n = s1.First().Split(' ').Last().ParseInt();
            var spl = s1.Last().Split(" | ");
            var w = spl.First().Split(' ').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
            var y = spl.Last().Split(' ').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
            var n1 = new Numbers(w, y);
            c[n] = new Numbers(w, y);
        }
        
        foreach (var kv in c)
        {
            var n = kv.Key;

            var count = kv.Value.Winning.Count(c => kv.Value.Yours.Contains(c));

            if (count >= 1)
            {
                var points = 1;
                for (var i = 1; i < count; i++)
                {
                    points *= 2;
                }
                NumSlot += points;
            }

        }
        AssignAnswer1();
        
        c = new Dictionary<int, Numbers>();
        foreach (var s in RawInputSplitByNl)
        {
            var s1 = s.Split(':');
            var n = s1.First().Split(' ').Last().ParseInt();
            var spl = s1.Last().Split(" | ");
            var w = spl.First().Split(' ').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
            var y = spl.Last().Split(' ').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
            var n1 = new Numbers(w, y);
            c[n] = new Numbers(w, y);
        }

        var cards = c;

        var cs = cards.Select(kv => (kv.Key, kv.Value)).ToList();

        while (cs.Count != 0)
        {
            var newCards = new Dictionary<int, (int q, Numbers numbers)>();

            foreach (var (cardNr, numbers) in cs)
            {
                NumSlot++;
                    
                var count = numbers.Winning.Count(c => numbers.Yours.Contains(c));

                if (count >= 1)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var idx = cardNr + i + 1;
                        var key = newCards.Keys.SingleOrDefault(k => k == idx);
                        if (key != default)
                        {
                            newCards[key] = newCards[key] with{q = newCards[key].q + 1};
                        } else
                        {
                            newCards[idx] = (1, cards[idx]);
                        }
                    }
                }
            }
            cs.Clear();
            foreach (var kv in newCards)
            {
                for (var i = 0; i < kv.Value.q; i++)
                {
                    cs.Add((kv.Key, kv.Value.numbers));
                }
            }
        }
        AssignAnswer2();

        return Answers;
    }
}