using Facet.Combinatorics;
using Framework;

namespace Solutions2023;

public class Solution24() : SolutionFramework(24)
{
    public record Point3(decimal X, decimal Y, decimal Z);
    public record Point(decimal X, decimal Y);
    public record Hailstone(decimal px, decimal py, decimal pz, decimal vx, decimal vy, decimal vz);

    public record Range(decimal S, decimal E);

    public override string[] Solve()
    {
        var hailstones = new List<Hailstone>();
        foreach (var l in InpNl)
        {
            var spl = l.Split('@');
            var pos = spl.First().Split(',').Select(x => decimal.Parse(x.Trim())).ToArray();
            var vel = spl.Last().Split(',').Select(x => int.Parse(x.Trim())).ToArray();

            hailstones.Add(new Hailstone(pos[0], pos[1], pos[2], vel[0], vel[1], vel[2]));
        }

        var pairs = new Combinations<Hailstone>(hailstones, 2);

        var withinRangeN = 0;
        var testArea = new Range(200000000000000, 400000000000000);

        foreach (var pair in pairs)
        {
            var h1 = pair[0];
            var h2 = pair[1];

            if (h1.vx < 0 && h1.px < testArea.S || h1.vx > 0 && h1.px > testArea.E || h2.vx < 0 && h2.px < testArea.S || h2.vx > 0 && h2.px > testArea.E ||
                h1.vy < 0 && h1.py < testArea.S || h1.vy > 0 && h1.py > testArea.E || h2.vy < 0 && h2.py < testArea.S || h2.vy > 0 && h2.py > testArea.E)
            {
                continue;
            }

            var intersection = h1.GetLineIntersection(h2);
            if (intersection is null ||
                intersection.X < testArea.S || intersection.X > testArea.E || intersection.Y < testArea.S || intersection.Y > testArea.E)
            {
                continue;
            }
            
            if ((h1.vx > 0 ? intersection.X >= h1.px && intersection.X <= testArea.E : intersection.X <= h1.px  && intersection.X >= testArea.S) &&
                (h1.vy > 0 ? intersection.Y >= h1.py && intersection.Y <= testArea.E : intersection.Y <= h1.py && intersection.Y >= testArea.S) &&
                (h2.vx > 0 ? intersection.X >= h2.px && intersection.X <= testArea.E : intersection.X <= h2.px && intersection.X >= testArea.S) &&
                (h2.vy > 0 ? intersection.Y >= h2.py && intersection.Y <= testArea.E : intersection.Y <= h2.py && intersection.Y >= testArea.S))
            {
                withinRangeN++;
            }
        }

        AssignAnswer1(withinRangeN);

        var rockPos = hailstones.Take(3).ToArray().GetSharedIntersection();
        AssignAnswer2(rockPos.X + rockPos.Y + rockPos.Z);

        return Answers;
    }

    static decimal CanSubtractNTimes(decimal starting, decimal subtract, decimal target) => Math.Floor((starting - target) / Math.Abs(subtract));
    static decimal CanAddNTimes(decimal starting, decimal add, decimal target) => Math.Floor((target - starting) / Math.Abs(add));
}


public static partial class Extensions
{
    public static Solution24.Point? GetLineIntersection(this Solution24.Hailstone hs1, Solution24.Hailstone hs2)
    {
        // Line 1 represented as p1S * x + p1E * y = c1 
        var p1S = new Solution24.Point(hs1.px, hs1.py);
        var p1E = new Solution24.Point(hs1.px + hs1.vx, hs1.py + hs1.vy);
        var p2S = new Solution24.Point(hs2.px, hs2.py);
        var p2E = new Solution24.Point(hs2.px + hs2.vx, hs2.py + hs2.vy);
        
        var a1 = p1E.Y - p1S.Y;
        var b1 = p1S.X - p1E.X;
        var c1 = a1 * (p1S.X) + b1 * (p1S.Y);
 
        // Line 2 represented as p2S * x + p2E * y = c2 
        var a2 = p2E.Y - p2S.Y;
        var b2 = p2S.X - p2E.X;
        var c2 = a2 * (p2S.X) + b2 * (p2S.Y);
 
        var determinant = a1 * b2 - a2 * b1;
 
        if (determinant == 0)
        {
            return null;
        }
        
        var x = (b2 * c1 - b1 * c2) / determinant;
        var y = (a1 * c2 - a2 * c1) / determinant;
        return new Solution24.Point(x, y);
    }
    
    public static Solution24.Point3? GetSharedIntersection(this Solution24.Hailstone[] hailstones)
    {
        var vectorRange = Enumerable.Range(-500, 1000).ToArray();
        foreach (var x in vectorRange)
        {
            foreach (var y in vectorRange)
            {
                var adjusted = hailstones.Select(hs => hs with { vx = hs.vx + x, vy = hs.vy + y }).ToArray();
                if (adjusted.Any(v => v.vx == 0 || v.vy == 0))
                {
                    continue;
                }
                
                var h1 = hailstones[1];
                var h2 = hailstones[2];
                var intersection1 = adjusted[0].GetLineIntersection(adjusted[1]);
                var intersection2 = adjusted[0].GetLineIntersection(adjusted[2]);
                if (intersection1 == default || intersection1 != intersection2)
                {
                    continue;
                }
                
                var time1 = (intersection1.X - h1.px) / (h1.vx + x);
                var time2 = (intersection2.X - h2.px) / (h2.vx + x);
                foreach(var z in vectorRange)
                {
                    var z1 = h1.pz + time1 * (h1.vz + z);
                    var z2 = h2.pz + time2 * (h2.vz + z);
                    if (z1 == z2)
                    {
                        return new Solution24.Point3(intersection1.X, intersection1.Y, z1);
                    }
                }
            }
        }
        
        return null;
    }
}
