using Framework;

namespace Solutions2023;

public class Solution15() : SolutionFramework(15)
{
    private enum Op { Min, Eq }

    public override string[] Solve()
    {
        foreach (var s in RawInput.Split(','))
        {
            var v = s.Aggregate(0, (current, c) => ((current + c) * 17) % 256);
            NSlot += v;
        }
        AssignAnswer1();

        var boxes = new Dictionary<int, List<(string, int)>>();
        
        foreach (var s in RawInput.Split(','))
        {
            var op = s.Contains('=') ? Op.Eq : Op.Min;
            var label = op is Op.Eq ? s.Split('=').First() : s.Split('-').First();
            var focalLength = op is Op.Eq ? s[^1].ParseInt() : 0;
            
            var box = label.Aggregate(0, (current, c) => ((current + c) * 17) % 256);
            if (op is Op.Eq)
            {
                if (!boxes.ContainsKey(box))
                {
                    boxes.Add(box, []);
                }
                var entry = boxes[box].SingleOrDefault(e => e.Item1 == label);
                if (entry != default)
                {
                    var idx = boxes[box].IndexOf(entry);
                    entry.Item2 = focalLength;
                    boxes[box][idx] = entry;
                }
                else
                {
                    boxes[box].Add((label, focalLength));
                }
            }
            else
            {
                if (boxes.ContainsKey(box))
                {
                    var entry = boxes[box].SingleOrDefault(e => e.Item1 == label);
                    if (entry != default)
                    {
                        boxes[box].Remove(entry);
                    }
                }
            }
        }
        
        NSlot = 0;
        foreach (var kv in boxes)
        {
            var s = 0;
            var i = 1;
            var b = kv.Key;
            foreach (var e in kv.Value)
            {
                var (l, fl) = e;
                s += (b + 1) * i++ * fl;
            }
            NSlot += s;
        }
        AssignAnswer2();
        
        return Answers;
    }
}