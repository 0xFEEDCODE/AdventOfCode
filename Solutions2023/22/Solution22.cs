using Framework;

namespace Solutions2023;

public class Solution22() : SolutionFramework(22)
{
    public record Pos3D(int X, int Y, int Z);
    public record Brick(Pos3D Start, Pos3D End);
    
    public override string[] Solve()
    {
        var bricks = (from l in InpNl select l.Split("~") into spl let startCoords = spl[0].Split(',').Select(int.Parse).ToArray() let endCoords = spl[1].Split(',').Select(int.Parse).ToArray() select new Brick(new Pos3D(startCoords[0], startCoords[1], startCoords[2]), new Pos3D(endCoords[0], endCoords[1], endCoords[2])))
            .OrderByDescending(x=>x.Start.Z).ToList();
        
        SettleBricks();

        var supported = new Dictionary<Brick, ICollection<Brick>>();

        foreach (var b1 in bricks.OrderBy(b => b.Start.Z))
        {
            var above = bricks.OrderBy(b2 => b2.Start.Z).Where(b2 => b2!=b1 && b1.Supports(b2)).ToArray();
            supported.Add(b1, above);
        }

        /*
        var cannotBeDisintegrated = new List<Brick>();
        var disintegrated = 0;
        foreach (var (br, supportedBr) in supported.ToDictionary())
        {
            if (!supportedBr.Any())
            {
                disintegrated++;
                continue;
            }

            var canDisintegrate = supportedBr.All(supB => supported.Any(kv => kv.Key != br && kv.Key.End.Z <=  br.End.Z && kv.Value.Contains(supB)));
            if (canDisintegrate)
            {
                disintegrated++;
            }
            else
            {
                cannotBeDisintegrated.Add(br);
            }
        }
        */
        
        var s = 0;
        var disintegrated = 0;
        
        foreach (var (br, supportedBr) in supported)
        {
            var chain = 0;

            var considered = new HashSet<Brick> { br };
            var canDisintegrate = supportedBr.All(supB => supported.Any(kv => kv.Key != br && kv.Key.End.Z <= br.End.Z && kv.Value.Contains(supB)));
            if (supportedBr.Any() && !canDisintegrate)
            {
                var supportedBricks = supportedBr;

                while (supportedBricks.Any())
                {
                    var newBricks = new HashSet<Brick>();
                    foreach (var (brick, sb) in supportedBricks.Where(brick => supported.Where(sp => sp.Value.Contains(brick)).Select(x => x.Key).All(considered.Contains)).Select(brick => (brick, supported[brick])))
                    {
                        if (considered.Add(brick))
                        {
                            chain++;
                            foreach (var sbr in sb)
                            {
                                newBricks.Add(sbr);
                            }
                        }
                    }
                    supportedBricks = newBricks;
                }
                
                s += chain;
                Console.WriteLine((br.End.Z, chain));
            }
            else
            {
                disintegrated++;
            }
        }
        
        AssignAnswer2(s);
        
        AssignAnswer1(disintegrated);
        
        return Answers;
        
        int SettleBricks()
        {
            var nSettled = 0;
            var settled = false;
            while (!settled)
            {
                settled = true;
                for (var i = 0; i < bricks.Count; i++)
                {
                    var b1 = bricks[i];
                    var lowerBricks = bricks.Where(b2 => b2 != b1 && b1.Start.Z > b2.End.Z).ToArray();
                    if (!lowerBricks.Any())
                    {
                        continue;
                    }
                    
                    var z = lowerBricks.MaxBy(x => x.End.Z)!.End.Z;
                    var bricksAtZ = lowerBricks.Where(b => b.End.Z == z).ToArray();
                    
                    var diff = b1.Start.Z - z;

                    var anyOverlaps = bricksAtZ.Any(b => b1.Overlaps(b));
                    
                    if (anyOverlaps)
                    {
                        if (diff > 1)
                        {
                            bricks[i] = new Brick(Start: b1.Start with { Z = b1.Start.Z - diff + 1 }, End: b1.End with { Z = b1.End.Z - diff + 1 });
                            nSettled++;
                            settled = false;
                        }
                    }
                    else
                    {
                        bricks[i] = new Brick(Start: b1.Start with { Z = b1.Start.Z - diff }, End: b1.End with { Z = b1.End.Z - diff });
                        nSettled++;
                        settled = false;
                    }
                }
            }
            return nSettled;
        }
    }
    
}

public static partial class Ext
{
    public static bool Overlaps(this Solution22.Brick b1, Solution22.Brick b2)
    {
        var minX1 = Math.Min(b1.Start.X, b1.End.X);
        var minX2 = Math.Min(b2.Start.X, b2.End.X);
        var min = minX1 < minX2 ? b1 : b2;
        var max = min == b1 ? b2 : b1;
        
        return (min.Start.X <= max.Start.X && min.End.X >= max.Start.X) &&
               ((min.Start.Y <= max.Start.Y && min.End.Y >= max.Start.Y) || (max.Start.Y <= min.Start.Y && max.End.Y >= min.Start.Y));
    }
    public static bool Supports(this Solution22.Brick b1, Solution22.Brick b2)
    {
        var z1 = b1.End.Z;
        var z2 = b2.Start.Z;
        
        return z2-z1 == 1 && b1.Overlaps(b2);
    }
}
