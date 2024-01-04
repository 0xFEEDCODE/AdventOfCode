using System.Drawing;
using System.Globalization;

using BenchmarkDotNet.Columns;

using Framework;

namespace Solutions2023;

public class Solution18Alt() : SolutionFramework(18)
{
    enum Dir { R, L, U, D };

    record Range(int S, int E);

    public override string[] Solve()
    {
        var instructions = InputNlSplit.Select(l =>
        {
            var spl = l.Split(' ');
            var hexV = spl[2].Substring(2, 6);
            var d = hexV[^1];
            var n = int.Parse("0" + hexV.Substring(0, hexV.Length - 1), NumberStyles.HexNumber);
            var dir = d switch
            {
                '0' => Dir.R,
                '1' => Dir.D,
                '2' => Dir.L,
                '3' => Dir.U,
                _ => throw new ArgumentOutOfRangeException()
            };

            //return new { dir = (Dir)Enum.Parse(typeof(Dir), spl[0]), amount = int.Parse(spl[1]), hex = hexV };
            return new { dir = dir, amount = n, hex = hexV };
        }).ToArray();


        var pos = new Point(0, 0);
        var points = new List<(Point From, Point To, Dir Dir)>();

        foreach (var instruction in instructions)
        {
            var dir = instruction.dir;
            var n = instruction.amount;
            var newPos = dir switch
            {
                Dir.D => new Point(pos.X, pos.Y + n),
                Dir.U => new Point(pos.X, pos.Y - n),
                Dir.R => new Point(pos.X + n, pos.Y),
                Dir.L => new Point(pos.X - n, pos.Y),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            points.Add((pos, newPos, dir));
            pos = newPos;
        }

        var vLines = points.Where(p => p.Dir is Dir.D or Dir.U).ToArray();
        var hLines = points.Where(p => p.Dir is Dir.L or Dir.R).Select(x => x.From.Y).Distinct().Order().ToArray();

        var area = 0d;

        IEnumerable<(int vLineLeft, int vLineRight)> previousVLinePairs = Array.Empty<(int, int)>();
        for (var i = 0; i < hLines.Length - 1; i++)
        {
            var hLineTop = hLines[i];
            var hLineBot = hLines[i+1];

            var intersectingVLines = vLines.Where(vl =>
            {
                var min = Math.Min(vl.From.Y, vl.To.Y);
                var max = Math.Max(vl.From.Y, vl.To.Y);
                return min <= hLineTop && max >= hLineBot;
            }).Select(vl => vl.From.X).Order().ToArray();

            IEnumerable<(int vLineLeft, int vLineRight)> vLinePairs = intersectingVLines.Zip(intersectingVLines.Skip(1), Tuple.Create)
                .Select(p => (p.Item1, p.Item2))
                .Where((_, idx) => idx % 2 ==0)
                .ToArray();

            long vertDist = hLineBot - hLineTop + 1;
            
            foreach (var pair in vLinePairs)
            {
                long horDist = pair.vLineRight - pair.vLineLeft + 1;

                area += horDist * vertDist;

                area -= previousVLinePairs.GetOverlappingArea(pair);
            }

            previousVLinePairs = vLinePairs;
        }

        AssignAnswer1(area);
        return Answers;
    }
}

static partial class Extensions
{
    public static double GetOverlappingArea(this IEnumerable<(int vLineLeft, int vLineRight)> vLines, (int start, int end) vLine) => vLines.GetOverlappingArea(vLine.start, vLine.end);
    public static double GetOverlappingArea(this IEnumerable<(int vLineLeft, int vLineRight)> vLines, int start, int end)
    {
        var overlap = 0d;
        foreach (var vl in vLines)
        {
            if (vl.vLineLeft < end && vl.vLineRight > start)
            {
                overlap += Math.Min(vl.vLineRight, end) - Math.Max(vl.vLineLeft, start) + 1;
            }
        }
        return overlap;
    }
}