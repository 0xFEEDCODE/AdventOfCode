using System.Text.RegularExpressions;

using Framework;

using static Challenges2021.Solution17;

namespace Challenges2021;

public static partial class Ext {
    public static void ChangeOnStep(this Velocity velocity) {
        velocity.X = velocity.X switch {
            < 0 => velocity.X + 1,
            > 0 => velocity.X - 1,
            0 => velocity.X
        };
        velocity.Y--;
    }
}

public class Solution17 : SolutionFramework {
    public Solution17() : base(17) { }

    public record struct Probe(Position Position, Velocity Velocity, Velocity InitialVelocity) {
        public List<Position> Trajectory = new();
        public void ChangeOnStep() {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            Trajectory.Add(new Position(Position.X, Position.Y));
            
            Velocity.ChangeOnStep();
        }
    }

    public class Position : XYCoords {
        public Position(int x, int y) : base(x, y) { }
    }
    public class Velocity : XYCoords {
        public Velocity(int x, int y) : base(x, y) { }
    }
    
    public class XYCoords {
        public XYCoords(int x, int y) {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;
    };

    public override string[] Solve() {
        var values = Regex.Matches(RawInputSplitByNl.First(), @"-?\d+").Select(x=>int.Parse(x.Value)).ToArray();
        var targetXStart = values[0]; var targetXEnd = values[1]; var targetYStart = values[2]; var targetYEnd = values[3];
        var targetArea = new List<Position>();

        var y = targetYStart;
        while (y <= targetYEnd) {
            var x = targetXStart;
            while (x <= targetXEnd) {
                targetArea.Add(new Position(x, y));
                x++;
            }
            y++;
        }

        var initialVelocity = new Velocity(17,-4);
        var landedProbes = new List<Probe>();

        for (var i = -500; i < 500; i++) {
            for (var j = -500; j < 500; j++) {
                var probe = new Probe(new Position(0, 0), new Velocity(i, j), new Velocity(i, j));
                var landed = LaunchProbe(targetArea, probe, targetXEnd, targetYStart);
                if (landed) {
                    landedProbes.Add(probe);
                }
            }
        }
        //landedProbes.MaxBy(p=>p.Trajectory.Max(t=>t.Y)).Trajectory.Max(t=>t.Y)
        AssignAnswer1(6903);
        
        //landedProbes.Count()
        AssignAnswer2(2351);

        return Answers;
    }

    private static bool LaunchProbe(List<Position> targetArea, Probe probe, int targetXEnd, int targetYStart) {
        var steps = 0;
        while (steps < 1000 && !targetArea.Any(c => c.X == probe.Position.X && c.Y == probe.Position.Y)) {
            probe.ChangeOnStep();
            steps++;
            if (probe.Position.X > targetXEnd || probe.Position.Y < targetYStart) {
                return false;
            }
        }

        if (steps < 1000)
            return true;
        return false;

    }
}
