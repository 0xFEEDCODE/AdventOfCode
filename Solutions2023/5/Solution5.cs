using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Headers;

using Framework;

namespace Solutions2023;

public class Solution5 : SolutionFramework
{
    public Solution5() : base(5) { }

    enum Category { water, light, fertilizer, soil, seed, temperature, humidity, location };
    public record Entry(double src, double dst, double len);
    record Map(Category from, Category to, List<Entry> entries);

    public override string[] Solve()
    {
        var seeds = "seeds: 929142010 467769747 2497466808 210166838 3768123711 33216796 1609270159 86969850 199555506 378609832 1840685500 314009711 1740069852 36868255 2161129344 170490105 2869967743 265455365 3984276455 31190888"
                .Split(' ').Skip(1).Select(double.Parse).ToArray();
        
        var allMaps = new List<Map>();
        
        for (var i = 0; i < InpNl.Length; i++)
        {
            if (!InpNl[i].Contains("map:"))
            {
                continue;
            }

            var categories = InpNl[i].Split(" ").First().Split('-').ToArray();
            Enum.TryParse<Category>(categories[0], out var c1);
            Enum.TryParse<Category>(categories[2], out var c2);
            
            var entries = new List<Entry>();
            var map = new Map(c1, c2, entries);
            i++;

            while (i < InpNl.Length && !InpNl[i].Contains("map:"))
            {
                var l = InpNl[i].Split(" ").ToArray();
                if (l.Length != 3)
                {
                    break;
                }

                var dst = l[0].ParseDouble();
                var src = l[1].ParseDouble();
                var len = l[2].ParseDouble();
                
                entries.Add(new Entry(src, dst, len));
                i++;
            }
            allMaps.Add(map);
        }

        var lowest = seeds.Select(SeedCalc).MinBy(x=>x);
        AssignAnswer1(lowest);

        var lowestFound = double.MaxValue;

        var seedRanges = seeds.Slice(2).Select(x => x.ToArray());
        var closest = double.MaxValue;
        
        Parallel.ForEach(seedRanges, (seedRange) =>
        {
            var len = seedRange[0] + seedRange[1];
            for (var i = seedRange[0];  i < len; i+=1)
            {
                var v = SeedCalc(i);
                if (v <= lowestFound)
                {
                    lowestFound = v;
                }
            }
        });

        AssignAnswer2(lowestFound);

        return Answers;
        
        double SeedCalc(double seed)
        {
            var from = Category.seed;
            var nr = seed;
            while (from != Category.location)
            {
                var m = allMaps.Single(x => x.from == from);
                var matchingRange = m.entries.SingleOrDefault(x => nr >= x.src && nr < x.src + x.len);
                if (matchingRange != null)
                {
                    nr = mapfn(matchingRange.src, matchingRange.dst, nr);
                }
                from = m.to;
            }
            return nr;
        }

        double mapfn(double from, double to, double v)
        {
            var off = v - from;
            if (off < 0)
            {
                throw new InvalidOperationException();
            }
            
            var rstart = from > to ? from - to : to - from;
            return from > to ? v - rstart : v + rstart;
        }
    }
}