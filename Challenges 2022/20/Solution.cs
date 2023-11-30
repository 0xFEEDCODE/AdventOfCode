using System.Collections;

using Framework;

namespace Challenges2022;

public static partial class Ext
{
    public static string ToKey(this long value, Dictionary<string, long> dict) {
        var existing = dict.Where(kv => long.Parse(kv.Key.Split("_")[0]) == value);
        if (existing.Any()) {
            var highestNum = long.Parse(existing.MaxBy(kv => long.Parse(kv.Key.Split("_")[1])).Key.Split("_")[1]);
            return $"{value.ToString()}_{highestNum + 1}";
        } 
        return $"{value.ToString()}_{0}";
    }

    public static Dictionary<long, long> Accessed = new();
    public static string GetKey(this long value, Dictionary<string, long> dict) {
        var ret = $"{value}_{0}";
        if (Accessed.ContainsKey(value)) {
            ret = $"{value}_{Accessed[value]}";
        } else {
            Accessed.Add(value, 0);
        }
        Accessed[value]++;
        return ret;
    }
}

public partial class Solution20 : SolutionFramework {
    public Solution20() : base(20) {
        var originalOrder = new Queue<long>();
        var shiftedMappings = new Dictionary<string, long>();
        var originals = new List<long>();
        
        var decryptKey = 811589153;
        var idx = 0;
        foreach (var line in RawInputSplitByNl) {
            var n = long.Parse(line) * decryptKey;
            originalOrder.Enqueue(n);
            originals.Add(n);
            shiftedMappings.Add(n.ToKey(shiftedMappings), idx++);
        }

        for (long i = 0; i < 10; i++) {
            foreach (var n in originals) {
                originalOrder.Enqueue(n);
            }
        }
        
        

        var nextRound = false;

        for (long x = 0; x < 10; x++) {
            Ext.Accessed = new Dictionary<long, long>();
            for (long i = 0; i < shiftedMappings.Count; i++) {

                var n = originalOrder.Dequeue();

                var key = n.GetKey(shiftedMappings);
                var oldPos = shiftedMappings[key];
                var newPos = (oldPos + n) % (shiftedMappings.Count - 1);
                if (newPos <= 0) {
                    newPos = shiftedMappings.Count - 1 + newPos;
                }

                if (newPos > oldPos) {
                    foreach (var kv in shiftedMappings.Where(kv => kv.Value > oldPos && kv.Value <= newPos)) {
                        shiftedMappings[kv.Key]--;
                    }
                } else if (newPos < oldPos) {
                    foreach (var kv in shiftedMappings.Where(kv => kv.Value >= newPos && kv.Value < oldPos)) {
                        shiftedMappings[kv.Key]++;
                    }
                }

                shiftedMappings[key] = newPos;

            }
        }

        var zeroPos = shiftedMappings["0_0"];
        var count = shiftedMappings.Count();
        var val1 = long.Parse(shiftedMappings.Single(kv => kv.Value == ((zeroPos + 1000) % count)).Key.Split("_")[0]);
        var val2 = long.Parse(shiftedMappings.Single(kv => kv.Value == ((zeroPos + 2000) % count)).Key.Split("_")[0]);
        var val3 = long.Parse(shiftedMappings.Single(kv => kv.Value == ((zeroPos + 3000) % count)).Key.Split("_")[0]);
        var ans = val1 + val2 + val3;
        Console.WriteLine(ans);

    }

    public override string[] Solve() {
        return Answers;
    }
}
