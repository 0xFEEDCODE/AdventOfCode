using Framework;

namespace Solutions2017;

public class Solution3() : SolutionFramework(3)
{
    public override string[] Solve()
    {
        var start = 2;
        var len = 8;
        var isEven = true;
        var nums = new Dictionary<int, (int X, int Y)>();
        var count = 0;
        var x = 1;
        var y = 0;
        var num = 1;
        var target = 361527;
        //var target = 1024;
        while (true)
        {
            var frameLen = len / 4;
            x++;
            y++;
            start += len;
            len += 8;
            isEven = !isEven;
            if (target >= start && target < start + len)
            {
                var dist = target - start;
                var movements = new int[][] { [0, -1], [-1, 0], [0, 1], [1, 0] };
                foreach (var movement in movements)
                {
                    for (var i = 0; i < len / 4; i++)
                    {
                        x += movement[0];
                        y += movement[1];
                        dist--;
                        
                        if (dist <= 0)
                        {
                            break;
                        }
                    }
                    if (dist <= 0)
                    {
                        break;
                    }
                }
                Console.WriteLine(Math.Abs(x) + Math.Abs(y));
                break;
            }
        }
        return Answers;
    }
}