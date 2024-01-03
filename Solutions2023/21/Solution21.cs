using System.Numerics;

using Framework;

namespace Solutions2023;

public class Solution21() : SolutionFramework(21)
{
    enum Dir { N, W, S, E };
    public override string[] Solve()
    {
        var grid = InputAsGrid<char>();
        
        var nums = new double[]
        {
            195, 73, 199, 41, 197, 47, 201, 49, 202, 66, 196, 67, 188, 88, 177, 101, 185, 101,
            177, 123, 171, 114, 177, 127, 180, 114, 197, 123, 204, 130, 193, 147, 184, 170,
            166, 171, 156, 186, 148, 209, 141, 231, 147, 238, 144, 217, 153, 218, 175, 223,
            174, 211, 185, 220, 194, 203, 201, 225, 207, 218, 200, 237, 208, 243, 198, 207,
            220, 217, 255, 220, 260, 218, 246, 239, 229, 219, 250, 234, 261, 213, 264, 226,
            295, 200, 309, 186, 328, 177, 355, 175, 369, 155, 358, 184, 340, 212, 321, 260,
            299, 266, 271, 298, 280, 308, 254, 311, 260, 318, 250, 336, 250, 351, 263, 361,
            272, 339, 259, 358, 269, 395, 223, 415, 188, 449, 165, 490, 183, 570, 207, 589, 207
        };

        
        var diffs = new double[]
        {
            386, 138, 386, 102, 344, 98, 330, 110, 314, 135, 302, 160, 280, 177, 253, 192,
            260, 185, 266, 186, 248, 177, 248, 200, 248, 187, 248, 200, 260, 199, 235, 212,
            227, 244, 198, 250, 174, 262, 155, 287, 155, 293, 161, 297, 158, 269, 167, 260,
            194, 256, 188, 235, 203, 247, 209, 222, 214, 240, 213, 234, 203, 245, 211, 247,
            199, 206, 217, 213, 247, 217, 245, 213, 229, 227, 212, 206, 228, 215, 236, 194,
            237, 205, 255, 179, 265, 168, 274, 162, 291, 161, 292, 150, 279, 166, 260, 183,
            249, 218, 229, 227, 201, 251, 200, 255, 186, 247, 187, 249, 184, 254, 186, 263,
            183, 273, 189, 260, 174, 270, 176, 293, 144, 306, 123, 323, 106, 345, 115, 386,
            138, 395, 138
        };


        double s = 3701d;
        for (var i = 0; i < 202300; i++)
        {
            s += nums.Sum();
            for (var j = 0; j < nums.Length; j++)
            {
                nums[j] += diffs[j];
            }

            //Console.WriteLine((s, nums.Sum()));
            //Task.Delay(1000).GetAwaiter().GetResult();
        }
        AssignAnswer2(s);
        return Answers;

        GridPos startPos = null;
        var rocks = new List<GridPos>();
        
        grid.ForEachCell((pos) =>
        {
            var cell = grid.GetCell(pos);
            switch (cell)
            {
                case '#':
                    rocks.Add(pos);
                    break;
                case 'S':
                    startPos = pos;
                    grid.SetCell(pos, '.');
                    break;
            }
        });

        var advancing = new Dictionary<(int WorldR, int WorldC), HashSet<GridPos>>
        {
            {(0,0), [startPos!] }
        };

        var q = new Queue<(int WorldR, int WorldC, GridPos)>();
        q.Enqueue((0,0, startPos!));

        var prevN = 0;

        for (var i = 0; i < 26501365; i++)
        {
            var entriesN = q.Count;
            var added = 0;
            while (entriesN-- > 0)
            {
                var (worldR, worldC, pos) = q.Dequeue();
                var offsets = new[] { (0, 1), (1, 0), (-1, 0), (0, -1) };
                foreach (var offs in offsets)
                {
                    var skip = false;
                    var newWorldR = worldR;
                    var newWorldC = worldC;
                    var newPos = new GridPos(pos.R + offs.Item1, pos.C + offs.Item2);
                    if (newPos.R == -1)
                    {
                        newPos.R = grid.Length - 1;
                        newWorldR--;
                    }
                    else if (newPos.C == -1)
                    {
                        newPos.C = grid[0].Length - 1;
                        newWorldC--;
                    }
                    else if (newPos.R == grid.Length)
                    {
                        newPos.R = 0;
                        newWorldR++;
                    }
                    else if (newPos.C == grid[0].Length)
                    {
                        newPos.C = 0;
                        newWorldC++;
                    }

                    var cell = grid.GetCell(newPos);
                    if (cell is '.')
                    {
                        if (q.Contains((newWorldR, newWorldC, newPos)))
                        {
                            continue;
                        }
                        q.Enqueue((newWorldR, newWorldC, newPos));
                        added++;
                    }
                }
            }

            var nowN = q.Count;
            var diff = 0d;
            if (nowN > 0 && prevN > 0)
            {
                diff = (double)nowN / prevN;
            }
            var hset = new HashSet<(int, int)>();
            foreach (var valueTuple in q.DistinctBy(x => x.WorldC))
            {
                hset.Add((valueTuple.WorldR, valueTuple.WorldC));
            }
            foreach (var valueTuple in q.DistinctBy(x => x.WorldR))
            {
                hset.Add((valueTuple.WorldR, valueTuple.WorldC));
            }
            var distinctWorlds = hset.Count;
            
            Console.WriteLine((i+1, distinctWorlds, nowN, nowN-prevN, diff));
            prevN = nowN;
        }
        
        AssignAnswer1(advancing.Count);
        
        return Answers;
    }
}