using Facet.Combinatorics;
using Framework;

namespace Solutions2023;

public class Solution24() : SolutionFramework(24)
{
    public record Point(double X, double Y);
    public record Hailstone(double px, double py, double pz, int vx, int vy, int vz);
    
    public record Range(double S, double E);

    public override string[] Solve()
    {
        var hailstones = new List<Hailstone>();
        foreach (var l in InpNl)
        {
            var spl = l.Split('@');
            var pos = spl.First().Split(',').Select(x => double.Parse(x.Trim())).ToArray();
            var vel = spl.Last().Split(',').Select(x => int.Parse(x.Trim())).ToArray();

            hailstones.Add(new Hailstone(pos[0], pos[1], pos[2], vel[0], vel[1], vel[2]));
        }

        var pairs = new Combinations<Hailstone>(hailstones, 2);

        var withinRangeN = 0;
        var testArea = new Range(200000000000000, 400000000000000);
        //var testArea = new Range(7, 27);

        foreach (var pair in pairs)
        {
            var h1 = pair[0];
            var h2 = pair[1];

            if (h1.vx < 0 && h1.px < testArea.S || h1.vx > 0 && h1.px > testArea.E || h2.vx < 0 && h2.px < testArea.S || h2.vx > 0 && h2.px > testArea.E || 
                h1.vy < 0 && h1.py < testArea.S || h1.vy > 0 && h1.py > testArea.E || h2.vy < 0 && h2.py < testArea.S || h2.vy > 0 && h2.py > testArea.E)
            {
                continue;
            }

            var startH1 = new Point(h1.px, h1.py);
            var startH2 = new Point(h2.px, h2.py);

            var n1X = h1.vx > 0 ? CanAddNTimes(h1.px, h1.vx, testArea.E) : CanSubtractNTimes(h1.px, h1.vx, testArea.S);
            var n1Y = h1.vy > 0 ? CanAddNTimes(h1.py, h1.vy, testArea.E) : CanSubtractNTimes(h1.py, h1.vy, testArea.S);
            var n2X = h2.vx > 0 ? CanAddNTimes(h2.px, h2.vx, testArea.E) : CanSubtractNTimes(h2.px, h2.vx, testArea.S);
            var n2Y = h2.vy > 0 ? CanAddNTimes(h2.py, h2.vy, testArea.E) : CanSubtractNTimes(h2.py, h2.vy, testArea.S);

            var velocityMul = Math.Min(Math.Min(n1X, n1Y), Math.Min(n2X, n2Y));

            var endH1 = new Point(h1.px + (h1.vx * velocityMul), h1.py + (h1.vy * velocityMul));
            var endH2 = new Point(h2.px + (h2.vx * velocityMul), h2.py + (h2.vy * velocityMul));

            var intersection = LineIntersection(startH1, endH1, startH2, endH2);
            if (intersection is null ||
                intersection.X < testArea.S || intersection.X > testArea.E || intersection.Y < testArea.S || intersection.Y > testArea.E)
            {
                continue;
            }
            
            if ((h1.vx > 0 ? intersection.X >= startH1.X && intersection.X <= testArea.E : intersection.X <= startH1.X && intersection.X >= testArea.S) &&
                (h1.vy > 0 ? intersection.Y >= startH1.Y && intersection.Y <= testArea.E : intersection.Y <= startH1.Y && intersection.Y >= testArea.S) &&
                (h2.vx > 0 ? intersection.X >= startH2.X && intersection.X <= testArea.E : intersection.X <= startH2.X && intersection.X >= testArea.S) &&
                (h2.vy > 0 ? intersection.Y >= startH2.Y && intersection.Y <= testArea.E : intersection.Y <= startH2.Y && intersection.Y >= testArea.S))
            {
                withinRangeN++;
            }
        }
        
        
        AssignAnswer1(withinRangeN);
        return Answers;
    }

    static double CanSubtractNTimes(double starting, double subtract, double target) => Math.Floor((starting - target) / Math.Abs(subtract));
    static double CanAddNTimes(double starting, double add, double target) => Math.Floor((target - starting) / Math.Abs(add));

    static Point? LineIntersection(Point p1S, Point p1E, Point p2S, Point p2E)
    {
        // Line 1 represented as p1S * x + p1E * y = c1 
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
        return new Point(x, y);
    }
    
    static (double SlopeM, double InterceptB) GetSlope(Point p1, Point p2)
    {
        double x1 = p1.X, y1 = p2.Y;
        double x2 = p2.X, y2 = p2.Y;

        var slopeM = (y2 - y1) / (x2 - x1);
        var interceptB = y1 - (slopeM * x1);

        var minX = Math.Min(x1, x2);
        var maxX = Math.Max(x1, x2);

        return (slopeM, interceptB);
    }
}
