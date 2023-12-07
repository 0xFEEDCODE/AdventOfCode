using Facet.Combinatorics;

using Framework;

namespace Solutions2023;

public class Solution7 : SolutionFramework
{
    public Solution7() : base(7) { }
    
    enum Cards {A=13, K=12, Q=10, T=9, C9=8, C8=7, C7=6, C6=5, C5=4, C4=3, C3=2, C2=1, J = 0}
    record Hand(IEnumerable<Cards> Cards);
    
    public override string[] Solve()
    {
        var hands = InputNlSplit.Select(l => new Hand(l.Split(" ").First().Select(c => char.IsDigit(c) ? Enum.Parse<Cards>("C" + c) : Enum.Parse<Cards>(c.ToString())).ToList())).ToList();
        var bids = InputNlSplit.Select(l => double.Parse(l.Split(" ").Last())).ToArray();

        var scores = new Dictionary<Hand, int>();
        var timesW = new Dictionary<Hand, int>();
        foreach (var h1 in hands)
        {
            timesW.TryAdd(h1, 0);
            foreach (var h2 in hands.Where(h => h != h1))
            {
                timesW.TryAdd(h2, 0);

                if (!scores.TryGetValue(h1, out var r1))
                {
                    r1 = HandRanking(h1, true);
                    scores[h1] = r1;
                }
                if (!scores.TryGetValue(h2, out var r2))
                {
                    r2 = HandRanking(h2, true);
                    scores[h2] = r2;
                }
                
                if (r1==r2)
                {
                    timesW[ResolveTie(h1, h2)]++;
                } else
                {
                    timesW[r1 > r2 ? h1 : h2]++;
                }
            }
        }

        var ordered = timesW.OrderBy(x => x.Value).Distinct().ToArray();
        if (ordered.Count() != hands.Count() && hands.Count() != bids.Length)
        {
            throw new InvalidOperationException();
        }
        
        for (var i = 1; i <= ordered.Length; i++)
        {
            var idx = hands.IndexOf(ordered[i-1].Key);
            
            PrintRes(i, idx);
            NSlot += bids[idx] * i;
        }
        AssignAnswer1();
        return Answers;

        void PrintRes(int i, int idx)
        {
            Console.Write(i + " :");
            foreach (var keyCard in ordered[i - 1].Key.Cards)
            {
                Console.Write(keyCard.ToString().Replace("C", string.Empty));
            }
            Console.Write(" " + bids[idx] * i + " " + bids[idx]);
            Console.WriteLine();
        }
    }

    Hand ResolveTie(Hand h1, Hand h2)
    {
        var c1 = h1.Cards.ToArray();
        var c2 = h2.Cards.ToArray();
        if (c1.Length != c2.Length)
        {
            throw new InvalidOperationException();
        }
        for (var i = 0; i < c1.Length; i++)
        {
            if (c1[i] == c2[i])
            {
                continue;
            }
            return (int)c1[i] > (int)c2[i] ? h1 : h2;
        }
        throw new InvalidOperationException();
    }

    int HandRanking(Hand hand, bool joker = false)
    {
        var dist = hand.Cards.Distinct();

        if (joker && hand.Cards.Any(c => c is Cards.J))
        {
            var rankings = new List<int>();
            var idx = 0;
            for (var i = 0; i < hand.Cards.Count(); i++)
            {
                if (hand.Cards.ElementAt(i) == Cards.J)
                {
                    foreach (var c in Enum.GetNames(typeof(Cards)))
                    {
                        if (c is "J")
                        {
                            continue;
                        }
                        var newHand = hand.Cards.ToArray();
                        newHand[i] = Enum.Parse<Cards>(c);
                        rankings.Add(HandRanking(new Hand(newHand), true));
                    }
                }
            }
            return rankings.Max();
        }
        
        switch (dist.Count())
        {
            //High card
            case 5:
                return 0;
            //Ace
            case 1:
                return 6;
        }
        
        //FourK
        if (hand.Cards.Any(x => hand.Cards.Count(y => x == y) == 4))
        {
            return 5;
        }
        if (hand.Cards.Any(x => hand.Cards.Count(y => x == y) == 3))
        {
            return dist.Count() == 2 ? 4 : 3;
        }
        
        var samePairs = dist.Count(x => hand.Cards.Count(y => x == y) == 2);
        return samePairs switch
        {
            //2Pair
            2 => 2,
            //1Pair
            1 => 1,
            _ => throw new InvalidOperationException()
        };
    }
}