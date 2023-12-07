using Framework;

namespace Challenges2019;

public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }


    private enum Field { Empty, Asteroid }
    private enum Direction { Up, Down, Left, Right }
    
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
        foreach (var p1 in asteroids)
        {
            var angles = new List<double>();
            var sameLines = new List<Direction>();

            scores.Add(p1, 0);
            foreach (var p2 in asteroids)
            {
                if (p1 == p2)
                {
                    continue;
                }
                
                var angle = Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
                //var m = dy / dx;
                if (!angles.Contains(angle))
                {
                    angles.Add(angle);
                    scores[p1]++;
                }
            }
        }

        AssignAnswer1(scores.MaxBy(x => x.Value).Value);

        Console.WriteLine();

        return Answers;
    }
    
}
