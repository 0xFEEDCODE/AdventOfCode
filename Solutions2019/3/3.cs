using Facet.Combinatorics;

using Framework;

namespace Challenges2019;

public class Solution3 : SolutionFramework
{
    public Solution3() : base(3) { }
    
    public override string[] Solve()
    {
        Part1();
        Part2();
        return Answers;
    }

    record Coord(int X, int Y);

    enum Direction
    {
        R,L,U,D
    }

    private Dictionary<Direction, Func<int, Coord, Coord>> Actions = new()
    {
        { Direction.R, (n, c) => c with {X = c.X+n} },
        { Direction.L, (n, c) => c with {X = c.X-n} },
        { Direction.U, (n, c) => c with {Y = c.Y-n} },
        { Direction.D, (n, c) => c with {Y = c.Y+n} },
    };
    
    private void Part1()
    {
        Coord startPos = new Coord(0, 0);
        var calcMDistance = (int x1, int y1, int x2, int y2) => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        var calcMDistanceCoord = (Coord c1, Coord c2) => calcMDistance(c1.X, c1.Y, c2.X, c2.Y);
        
        for (var i = 0; i < InputNlSplit.Length; i+=2)
        {
            var wires1 = new List<(Coord From, Coord To)>();
            var wires2 = new List<(Coord From, Coord To)>();

            for (var j = 0; j < 2; j++)
            {
                var currentPosition = startPos;
                foreach (var s in InputNlSplit[i+j].Split(','))
                {
                    var d = (Direction)Enum.Parse(typeof(Direction), s[0].ToString(), true);
                    var n = s.Skip(1).AsString().ParseInt();
                    (Coord From, Coord To) wire = (currentPosition, null);
                    currentPosition = Actions[d](n, currentPosition);
                    wire.To = currentPosition;
                    if (j == 0)
                    {
                        wires1.Add(wire);
                    } else
                    {
                        wires2.Add(wire);
                    }
                }
            }

            const string w1Tag = "w1", w2Tag = "w2";
            var horizontalLines = wires1.Where(x => x.From.X != x.To.X).Select(x => (w1Tag, x)).Concat(wires2.Where(x => x.From.X != x.To.X).Select(x => (w2Tag, x)));
            var verticalLines =   wires1.Where(x => x.From.Y != x.To.Y).Select(x => (w1Tag, x)).Concat(wires2.Where(x => x.From.Y != x.To.Y).Select(x => (w2Tag, x)));
            
            var lowestDist = int.MaxValue;
            foreach (var hline in horizontalLines)
            {
                var w1 = hline.Item2;
                if (w1.From == startPos)
                {
                    continue;
                }
                var hlineLowPoint = Math.Min(w1.From.X, w1.To.X);
                var hlineHighPoint = Math.Max(w1.From.X, w1.To.X);
                
                if (w1.From.Y != w1.To.Y)
                {
                    new InvalidOperationException();
                }
                var hlineYPoint = w1.From.Y;

                foreach (var vline in verticalLines)
                {
                    var w2 = vline.Item2;
                    if (w2.From == startPos || hline.Item1 == vline.Item1)
                    {
                        continue;
                    }
                    if (w2.From.X != w2.To.Y)
                    {
                        new InvalidOperationException();
                    }
                    
                    var vlineXPoint = w2.From.X;
                    var vlineLowPoint = Math.Min(w2.From.Y, w2.To.Y);
                    var vlineHighPoint = Math.Max(w2.From.Y, w2.To.Y);
                    
                    var overlap = hlineLowPoint <= vlineXPoint && vlineXPoint <= hlineHighPoint && 
                                    vlineLowPoint <= hlineYPoint && hlineYPoint <= vlineHighPoint;

                    if (overlap)
                    {
                        var overlapPoint = new Coord(vlineXPoint, hlineYPoint);
                        var dist = calcMDistanceCoord(startPos, overlapPoint);
                        lowestDist.AssignIfLower(dist);
                    }
                }
            }
            
            AssignAnswer1(lowestDist);
        }
    }

    private void Part2()
    {
        var startPos = new Coord(0, 0);

        for (var i = 0; i < InputNlSplit.Length; i+=2)
        {
            var wires1 = new List<(Coord From, Coord To, int NStepsToReach, Direction dir)>();
            var wires2 = new List<(Coord From, Coord To, int NStepsToReach, Direction dir)>();
            var nsteps1 = 0;
            var nsteps2 = 0;

            for (var j = 0; j < 2; j++)
            {
                var currentPosition = startPos;
                foreach (var s in InputNlSplit[i+j].Split(','))
                {
                    var d = (Direction)Enum.Parse(typeof(Direction), s[0].ToString(), true);
                    var n = s.Skip(1).AsString().ParseInt();
                    (Coord From, Coord To) wire = (currentPosition, null);
                    currentPosition = Actions[d](n, currentPosition);
                    wire.To = currentPosition;
                    var nsteps = Math.Max(Math.Abs(wire.From.X - wire.To.X), Math.Abs(wire.From.Y - wire.To.Y));
                    if (j == 0)
                    {
                        wires1.Add((wire.Item1, wire.Item2, nsteps1 += nsteps, d));
                    } else
                    {
                        wires2.Add((wire.Item1, wire.Item2, nsteps2 += nsteps, d));
                    }
                }
            }

            const string w1Tag = "w1", w2Tag = "w2";
            var horizontalLines = wires1.Where(x => x.From.X != x.To.X).Select(x => (w1Tag, x)).Concat(wires2.Where(x => x.From.X != x.To.X).Select(x => (w2Tag, x)));
            var verticalLines =   wires1.Where(x => x.From.Y != x.To.Y).Select(x => (w1Tag, x)).Concat(wires2.Where(x => x.From.Y != x.To.Y).Select(x => (w2Tag, x)));
            
            var lowestNSteps = int.MaxValue;
            foreach (var hline in horizontalLines)
            {
                var w1 = hline.Item2;
                if (w1.From == startPos)
                {
                    continue;
                }
                var hlineLowPoint = Math.Min(w1.From.X, w1.To.X);
                var hlineHighPoint = Math.Max(w1.From.X, w1.To.X);
                
                if (w1.From.Y != w1.To.Y)
                {
                    new InvalidOperationException();
                }
                var hlineYPoint = w1.From.Y;

                foreach (var vline in verticalLines)
                {
                    var w2 = vline.Item2;
                    if (w2.From == startPos || hline.Item1 == vline.Item1)
                    {
                        continue;
                    }
                    if (w2.From.X != w2.To.Y)
                    {
                        new InvalidOperationException();
                    }
                    
                    var vlineXPoint = w2.From.X;
                    var vlineLowPoint = Math.Min(w2.From.Y, w2.To.Y);
                    var vlineHighPoint = Math.Max(w2.From.Y, w2.To.Y);
                    
                    var overlap = hlineLowPoint <= vlineXPoint && vlineXPoint <= hlineHighPoint && 
                                    vlineLowPoint <= hlineYPoint && hlineYPoint <= vlineHighPoint;

                    if (overlap)
                    {
                        var overlapPoint = new Coord(vlineXPoint, hlineYPoint);
                        var nsteps1Minus = CalcNStepMinus(hline.x.dir, hlineHighPoint, overlapPoint.X, hlineLowPoint, vlineLowPoint, vlineHighPoint);
                        var nsteps2Minus = CalcNStepMinus(vline.x.dir, hlineHighPoint, overlapPoint.Y, hlineLowPoint, vlineLowPoint, vlineHighPoint);
                        lowestNSteps.AssignIfLower(hline.x.NStepsToReach + vline.x.NStepsToReach - nsteps1Minus - nsteps2Minus);
                    }
                }
            }
            
            AssignAnswer2(lowestNSteps);
        }
        return;

        int CalcNStepMinus(Direction dir, int hlineHighPoint, int overlapPoint, int hlineLowPoint, int vlineLowPoint, int vlineHighPoint) =>
            Math.Abs((dir) switch
            {
                Direction.R => hlineHighPoint - overlapPoint,
                Direction.L => overlapPoint - hlineLowPoint,
                Direction.D => overlapPoint - vlineHighPoint,
                Direction.U => vlineLowPoint - overlapPoint
            });
    }
}
