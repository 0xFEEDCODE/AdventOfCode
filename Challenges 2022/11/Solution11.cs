using System.Numerics;
using System.Text.RegularExpressions;
using Framework;

namespace Challenges2022;
public class Solution11 : SolutionFramework
{
    public Solution11() : base(11) { }
    
    public class Monkey
    {
        public int No;
        public List<PairNode> Items;
        public long TimesInspected;
        public Action<PairNode> Operation;
        public long Divisor;
        public long ActionTrueMonkeyNo;
        public long ActionFalseMonkeyNo;
    }

    public enum Operation { Mul,Add }

    public class PairNode
    {
        public PairNode Head;
        public PairNode Prev;
        public PairNode Next;
        
        public long Value;
        public Operation Operation;
        
        public bool IsHead;
        
        public long MulEverythingBeforeTimesN = 0;
        public long AddEverythingBeforeTimesN = 0;
        
        public long CachedRem = -1;

        public PairNode()
        {
        }

        public PairNode(long v, bool startingNode)
        {
            if (!startingNode)
                throw new InvalidOperationException();
            this.Value = v;
            this.Head = this;
            this.IsHead = true;
        }

        public void AddNextNode(long v, Operation op)
        {
            if (Next != null)
                throw new InvalidOperationException();

            Next = new PairNode(){Head = this.Head, Prev = this, Operation = op, Value = v};
        }

        public long GetRemainder(long N)
        {
            var curr = Head;
            var rem = curr.Value % N;
            while (curr != null)
            {
                for (int i = 0; i < curr.AddEverythingBeforeTimesN; i++)
                    rem += rem%N;
                for (int i = 0; i < curr.MulEverythingBeforeTimesN; i++)
                    rem *= rem%N;
                rem %= N;

                if (curr.Next == null)
                    break;
                
                var next = curr.Next;
                var op = next.Operation;
                if (op == Operation.Add)
                    rem += next.Value%N;
                else 
                    rem *= next.Value%N;
                curr = next;
            }

            return rem % N;
        }

        public BigInteger GetTotal()
        {
            var curr = Head;
            BigInteger val = curr.Value;
            while (curr != null)
            {
                for (int i = 0; i < curr.AddEverythingBeforeTimesN; i++)
                    val += val;
                for (int i = 0; i < curr.MulEverythingBeforeTimesN; i++)
                    val *= val;
                
                if (curr.Next == null)
                    break;
                
                var next = curr.Next;
                var op = next.Operation;
                if (op == Operation.Add)
                    val += next.Value;
                else
                    val *= next.Value;
                curr = next;
            }

            return val;
        }
    }
    
    public override string[] Solve()
    {
        var monkeys = new List<Monkey>();
        var parsedMonkey = new Monkey();
        foreach (var line in RawInputSplitByNl)
        {
            if (line.StartsWith("Monkey"))
            {
                var no = Regex.Match(line, @"\d+").Value;
                parsedMonkey = new Monkey(){Items = new List<PairNode>(), No = int.Parse(no), TimesInspected = 0};
            }

            if (line.StartsWith("  Starting items"))
            {
                var nums = Regex.Matches(line, @"\d+").ToList().Select(x=>x.Value);
                foreach (string num in nums)
                {
                    var cop = new PairNode(int.Parse(num), true);
                    parsedMonkey.Items.Add(cop);
                }
            }
            else if (line.StartsWith("  Operation"))
            {
                var isMul = line.Contains('*');
                var isPlus = line.Contains('+');
                var num = Regex.Match(line, @"\d+");
                    
                if (num.Success)
                {
                    var n = int.Parse(num.Value);
                    parsedMonkey.Operation = (incompletePair) =>
                    {
                        var op = isMul switch {
                            true => Operation.Mul,
                            false => Operation.Add };
                        incompletePair.AddNextNode(n,op);
                    };
                }
                else
                {
                    parsedMonkey.Operation = (incompletePair) =>
                    {
                        if (isMul) incompletePair.MulEverythingBeforeTimesN++;
                        else incompletePair.AddEverythingBeforeTimesN++;
                    };
                }
            }
            else if (line.StartsWith("  Test"))
            {
                parsedMonkey.Divisor = int.Parse(Regex.Match(line, @"\d+").Value);
            }
            else if (line.StartsWith("    If true"))
                parsedMonkey.ActionTrueMonkeyNo = int.Parse(Regex.Match(line, @"\d+").Value);
            else if (line.StartsWith("    If false"))
            {
                parsedMonkey.ActionFalseMonkeyNo = int.Parse(Regex.Match(line, @"\d+").Value);
                monkeys.Add(parsedMonkey);
            }
        }

        var roundsLeft = 10000;
        var rounds = 0;
        while (roundsLeft > 0)
        {
            Console.WriteLine(roundsLeft);
            foreach (var monkey in monkeys)
            {
                for(var i = 0; i < monkey.Items.Count; i++)
                {
                    var chainOfPairs = monkey.Items[i];
                    monkey.TimesInspected++;
                    
                    monkey.Operation(chainOfPairs);
                    if (chainOfPairs.Next != null)
                        chainOfPairs = chainOfPairs.Next;

                    Monkey target;
                    if (chainOfPairs.GetRemainder(monkey.Divisor) == 0)
                        target = monkeys.Single(m => m.No == monkey.ActionTrueMonkeyNo);
                    else
                        target = monkeys.Single(m => m.No == monkey.ActionFalseMonkeyNo);
                    
                    target.Items.Add(chainOfPairs);
                }
                monkey.Items.Clear();
            }
            
            rounds++;
            roundsLeft--;
        }

        var temp = monkeys.OrderByDescending(x => x.TimesInspected).Take(2).Select(x=>x.TimesInspected).ToArray();
        long ans = temp[0] * temp[1];
        Console.Write(ans);
        
        return Answers;
    }
}
