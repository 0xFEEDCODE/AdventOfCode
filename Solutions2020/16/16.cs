using System.Text.RegularExpressions;

using Framework;

namespace Challenges2020;

public class Solution16 : SolutionFramework
{
    public Solution16() : base(16) { }

    enum Scanning { Ranges, MyTicket, OtherTickets };

    record Range(int Start, int End);
    
    
    public override string[] Solve()
    {
        var allRanges = new List<Range>();
        var ranges = new Dictionary<string, (Range, Range)>();
        var myTicket = new Dictionary<int, List<int>>();
        var otherTickets = new Dictionary<int, List<int>>();

        var rangePattern = @"(\d+)-(\d+)";
        var scanning = Scanning.Ranges;
        var _row = 0;
        foreach (var line in RawInputSplitByNl)
        {
            if (line.Contains("your ticket"))
            {
                _row = 0;
                scanning = Scanning.MyTicket;
                continue;
            }
            if (line.Contains("nearby tickets"))
            {
                _row = 0;
                scanning = Scanning.OtherTickets;
                continue;
            }

            if (line == string.Empty)
            {
                continue;
            }
            switch (scanning)
            {
                case Scanning.Ranges:
                {
                    var name = line.Split(':').First();
                    var _ranges = new List<Range>();
                    foreach (Match match in Regex.Matches(line, rangePattern))
                    {
                        var start = int.Parse(match.Groups[1].Value);
                        var end = int.Parse(match.Groups[2].Value);
                        _ranges.Add(new Range(start, end));
                        allRanges.Add(new Range(start, end));
                    }
                    ranges.Add(name, (_ranges[0], _ranges[1]));
                    

                    break;
                }
                case Scanning.MyTicket or Scanning.OtherTickets:
                    var collection = scanning is Scanning.MyTicket ? myTicket : otherTickets;
                    collection.Add(_row, new List<int>());
                    foreach (var entry in line.Split(','))
                    {
                        if (int.TryParse(entry, out var value))
                        {
                            collection[_row].Add(value);
                        }
                    }

                    _row++;
                    break;
            }
        }

        var orderedRanges = allRanges.OrderBy(r => r.Start).ThenBy(r => r.End).ToList();
        var mergedRanges = new List<Range>();

        int? mergeStart = null;
        int? mergeEnd = null;
        foreach (var idx in Enumerable.Range(0, orderedRanges.Count-1))
        {
            mergeStart ??= orderedRanges[idx].Start;
            mergeEnd ??= orderedRanges[idx].End;
            
            var nextRange = orderedRanges[idx + 1];

            if (nextRange.Start < mergeEnd)
            {
                mergeEnd = nextRange.End;
            } 
            else
            {
                mergedRanges.Add(new Range(mergeStart.Value, mergeEnd.Value));

                mergeStart = null;
                mergeEnd = null;
            }
        }

        if (mergeStart.HasValue && mergeEnd.HasValue)
        {
            mergedRanges.Add(new Range(mergeStart.Value, mergeEnd.Value));
        }
        mergedRanges.Add(orderedRanges.Last());

        var invalidNumbers = (from entry in otherTickets.Values
            from num in entry
            let isValid = mergedRanges.Any(range => IsInRange(range, num))
            where !isValid
            select num).ToArray();
        
        AssignAnswer1(invalidNumbers.Sum());

        const int invalidNumber = int.MaxValue;
        
        foreach (var num in invalidNumbers)
        {
            foreach (var entry in otherTickets.Values.Where(entry => entry.Contains(num)))
            {
                entry[entry.IndexOf(num)] = invalidNumber;
            }
        }

        var allTickets = Merge(myTicket, otherTickets);
        var columnToCategoryMap = new Dictionary<int, string>();
        var matchesFound = new Dictionary<int, ICollection<KeyValuePair<string, (Range, Range)>>>();
        foreach (var col in Enumerable.Range(0, myTicket.Values.First().Count))
        {
            var matching = (from kv in ranges
                     let r1 = kv.Value.Item1
                     let r2 = kv.Value.Item2
                     let isValid = Enumerable.Range(0, allTickets.Count)
                         .All(row => 
                             allTickets[row][col] == invalidNumber ||
                             IsInRange(r1, allTickets[row][col]) || IsInRange(r2, allTickets[row][col]))
                     where isValid 
                     select kv);
            matchesFound.Add(col, matching.ToArray());
        }

        foreach (var matches in matchesFound.OrderBy(m => m.Value.Count))
        {
            var filtered = matches.Value.Where(kv => !columnToCategoryMap.ContainsValue(kv.Key));
            if (filtered.Count() is 1)
            {
                columnToCategoryMap[matches.Key] = filtered.Single().Key;
            }
        }

        long? result = null;
        foreach (var category in columnToCategoryMap.Where(kv => kv.Value.Contains("departure")))
        {
            if (!result.HasValue)
            {
                result = myTicket[0][category.Key];
            } 
            else
            {
                result *= myTicket[0][category.Key];
            }
        }
        AssignAnswer2(result.Value);
        
        return Answers;
    }
    
    public static Dictionary<TKey, TValue>
        Merge<TKey,TValue>(Dictionary<TKey, TValue> d1, Dictionary<TKey, TValue> d2)
    {
        var result = new Dictionary<TKey, TValue>();
        foreach (var x in d1)
        {
            result[x.Key] = x.Value;
        }
        foreach (var x in d2)
        {
            result[x.Key] = x.Value;
        }

        return result;
    }
    
    private static bool IsInRange(Range range, int num) => range.Start <= num && num <= range.End;
}
