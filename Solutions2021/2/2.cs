using Framework;

namespace Challenges2021;

public class Solution2 : SolutionFramework
{
    public Solution2() : base(2) { }

    public override string[] Solve() {
        var x = 0;
        var y = 0;
        foreach (var line in InputNlSplit) {
            var split = line.Split(' ');
            var n = int.Parse(split[1]);
            switch (split[0]) {
                case "forward":
                    x+=n;
                    break;
                case "down":
                    y+=n;
                    break;
                case "up":
                    y-=n;
                    break;
            }
        }
        AssignAnswer1(x*y);
        
        //pt 2
        x = 0;
        y = 0;
        var aim = 0;
        foreach (var line in InputNlSplit) {
            var split = line.Split(' ');
            var n = int.Parse(split[1]);
            switch (split[0]) {
                case "forward":
                    x+=n;
                    y += (aim*n);
                    break;
                case "down":
                    aim += n;
                    break;
                case "up":
                    aim -= n;
                    break;
            }
        }
        AssignAnswer2(x*y);
        
        return Answers;
    }
}
