using Framework;

namespace Challenges2019;

public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }

    private enum Field { Empty, Asteroid }
    
    public override string[] Solve()
    {
        var temp = InpGr<char>();

        var gr = (temp.Length, temp[0].Length).CreateGrid<Field>();
        var asteroids = new List<Pos2D>();

        for (var i = 0; i < temp.Length; i++)
        {
            for (var j = 0; j < temp[i].Length; j++)
            {
                gr[i][j] = temp[i][j] == '#' ? Field.Asteroid : Field.Empty;
                if (gr[i][j] == Field.Asteroid)
                {
                    asteroids.Add(new Pos2D(j, i));
                }
            }
        }

        var scores = new Dictionary<Pos2D, double>();
        var angles = new Dictionary<Pos2D, List<(Pos2D Pos, double AngleDegree)>>();

        foreach (var p1 in asteroids)
        {
            foreach (var p2 in asteroids)
            {
                if (p1 == p2) { continue; }

                scores.TryAdd(p1, 0);
                angles.TryAdd(p1, new List<(Pos2D Pos, double AngleDegree)>(0));

                var angle = Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
                var angleDegree = angle > 0 ? angle : (2 * Math.PI + angle) *360 / (2 * Math.PI);

                if (!angles[p1].Any(p => p.AngleDegree == angleDegree))
                {
                    angles[p1].Add((p2, angleDegree));
                    scores[p1]++;
                }
            }
        }

        var bestPlacement = scores.MaxBy(x => x.Value);
        AssignAnswer1(bestPlacement.Value);

        var a = angles[bestPlacement.Key];

        var c = 1;
        foreach (var angle in a.OrderByDescending(x => x.AngleDegree))
        {
            if (c == 200)
            {
                NSlot = angle.Pos.X * 100 + angle.Pos.Y;
                break;
            }
            
            c++;
        }
        
        AssignAnswer2();

        return Answers;
    }
    
}
