using System.Text;

using Framework;

namespace Challenges2020;

public class Solution14 : SolutionFramework
{
    public Solution14() : base(14) { }

    record MemoryEntry(int Address, long Value);
    record Entry(string Mask, ICollection<MemoryEntry> Mem);

    public override string[] Solve()
    {
        var entries = new List<Entry>();
        Entry? _entry = null;
        foreach (var line in RawInputSplitByNl)
        {
            var value = line.Split(' ').Last();
            if (line.StartsWith("mask"))
            {
                if (_entry != null)
                {
                    entries.Add(_entry);
                }
                _entry = new Entry(value, new List<MemoryEntry>());
            } else
            {
                var address = line[(line.IndexOf('[')+1)..line.IndexOf(']')];
                _entry.Mem.Add(new MemoryEntry(Convert.ToInt32(address), Convert.ToInt64(value)));
            }
        }

        if (_entry != null)
        {
            entries.Add(_entry);
        }

        var memoryEntries = new Dictionary<long, string>();
        foreach (var entry in entries)
        {
            foreach (var mem in entry.Mem)
            {
                if (!memoryEntries.ContainsKey(mem.Address))
                {
                    memoryEntries.Add(mem.Address, new string(Enumerable.Repeat('0', entry.Mask.Length).ToArray()));
                }
                var valueInBinary = Convert.ToString(mem.Value, 2);
                var valueAfterApplyingMask = ApplyMask(entry.Mask, valueInBinary);
                memoryEntries[mem.Address] = ApplyMask(valueAfterApplyingMask, memoryEntries[mem.Address]);
            }
        }

        var result = memoryEntries.Select(kv => Convert.ToInt64(kv.Value, 2)).Sum();
        AssignAnswer1(result);

        memoryEntries = new Dictionary<long, string>();
        foreach (var entry in entries)
        {
            foreach (var mem in entry.Mem)
            {
                if (!memoryEntries.ContainsKey(mem.Address))
                {
                    memoryEntries.Add(mem.Address, new string(Enumerable.Repeat('0', entry.Mask.Length).ToArray()));
                }

                var memAddressInBinary = Convert.ToString(mem.Address, 2);
                
                var decodedValue = Decode(entry.Mask, memAddressInBinary);
                var possibleAddresses = GetPossibleAddresses(decodedValue);
                
                var valueInBinary = Convert.ToString(mem.Value, 2);
                valueInBinary = valueInBinary.Insert(0, new string(Enumerable.Repeat('0', entry.Mask.Length - valueInBinary.Length).ToArray()));
                foreach (var address in possibleAddresses)
                {
                    memoryEntries[address] = valueInBinary;
                }
            }
        }

        result = memoryEntries.Select(kv => Convert.ToInt64(kv.Value, 2)).Sum();
        AssignAnswer2(result);
        
        Console.WriteLine();
        return Answers;
    }
    
    private static string ApplyMask(string bitmask, string toApply)
    {
        if (toApply.Length < bitmask.Length)
        {
            toApply = toApply.Insert(0, new string(Enumerable.Repeat('0', bitmask.Length - toApply.Length).ToArray()));
        }

        var resultSb = new StringBuilder();
        foreach (var index in Enumerable.Range(0, bitmask.Length))
        {
            resultSb.Append(bitmask[index] is 'X' ? toApply[index] : bitmask[index]);
        }

        return resultSb.ToString();
    }
    
    private static string Decode(string bitmask, string memory)
    {
        if (memory.Length < bitmask.Length)
        {
            memory = memory.Insert(0, new string(Enumerable.Repeat('0', bitmask.Length - memory.Length).ToArray()));
        }
        
        var resultSb = new StringBuilder();
        foreach (var idx in Enumerable.Range(0, bitmask.Length))
        {
            switch (bitmask[idx])
            {
                case '0':
                    resultSb.Append(memory[idx]);
                    break;
                case '1':
                    resultSb.Append('1');
                    break;
                case 'X':
                    resultSb.Append('X');
                    break;
            }
        }

        return resultSb.ToString();
    }

    private static IEnumerable<long> GetPossibleAddresses(string floatingValue)
    {
        var possibleAddresses = new HashSet<string>();
        var value = floatingValue.Replace('X', '0');
        foreach (var idx in Enumerable.Range(0, floatingValue.Length))
        {
            if (floatingValue[idx] is 'X')
            {
                var res = value[..idx] + "1" + value[(idx+1)..];
                possibleAddresses.Add(res);
                res = value[..idx] + "0" + value[(idx+1)..];
                possibleAddresses.Add(res);

                foreach (var address in possibleAddresses.ToArray())
                {
                    res = address[..idx] + "1" + address[(idx+1)..];
                    possibleAddresses.Add(res);
                    res = address[..idx] + "0" + address[(idx+1)..];
                    possibleAddresses.Add(res);
                }
            }
        }

        return possibleAddresses.Select(x => Convert.ToInt64(x, 2)).ToArray();
    }
}
