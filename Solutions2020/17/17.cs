using Framework;

namespace Challenges2020;


public static partial class Ext
{
    public static bool IsNeighbour(this Solution17.Cube c1, Solution17.Cube c2) =>
        Math.Abs(c1.Pos.X - c2.Pos.X) <= 1 &&
        Math.Abs(c1.Pos.Y - c2.Pos.Y) <= 1 &&
        Math.Abs(c1.Pos.Z - c2.Pos.Z) <= 1;

    public static bool IsNeighbour(this Solution17.HyperCube c1, Solution17.HyperCube c2) =>
        Math.Abs(c1.Pos.X - c2.Pos.X) <= 1 &&
        Math.Abs(c1.Pos.Y - c2.Pos.Y) <= 1 &&
        Math.Abs(c1.Pos.Z - c2.Pos.Z) <= 1 &&
        Math.Abs(c1.Pos.W - c2.Pos.W) <= 1;
    
    public static void ForEach(this Solution17.Area3D area, Action<int, int, int> action)
    {
        foreach (var x in GetRange(area.RangeX.Start, area.RangeX.End))
        {
            foreach (var y in GetRange(area.RangeY.Start, area.RangeY.End))
            {
                foreach (var z in GetRange(area.RangeZ.Start, area.RangeZ.End))
                {
                    action.Invoke(x,y,z);
                }
            }
        }
    }
    
    public static void ForEach(this Solution17.Area4D area, Action<int, int, int, int> action)
    {
        foreach (var x in GetRange(area.RangeX.Start, area.RangeX.End))
        {
            foreach (var y in GetRange(area.RangeY.Start, area.RangeY.End))
            {
                foreach (var z in GetRange(area.RangeZ.Start, area.RangeZ.End))
                {
                    foreach (var w in GetRange(area.RangeW.Start, area.RangeW.End))
                    {
                        action.Invoke(x,y,z,w);
                    }
                }
            }
        }
    }
    
    private static IEnumerable<int> GetRange(int start, int end) => Enumerable.Range(start, end-start+1);
}

public class Solution17 : SolutionFramework
{
    public Solution17() : base(17) { }

    public class Cube : IEquatable<Cube>
    {
        public bool IsActive;
        
        public bool? PendingActivateStateChange;
        
        public Pos3D Pos;
        
        public Cube(bool isActive, Pos3D pos)
        {
            IsActive = isActive;
            Pos = pos;
        }
        
        public bool Equals(Cube other) => (Pos.X, Pos.Y, Pos.Z) == (other.Pos.X, other.Pos.Y, other.Pos.Z);
        public override int GetHashCode() => HashCode.Combine(Pos.X, Pos.Y, Pos.Z);
    }
    
    public class HyperCube : IEquatable<HyperCube>
    {
        public bool IsActive;
        
        public bool? PendingActivateStateChange;
        
        public Pos4D Pos;
        
        public HyperCube(bool isActive, Pos4D pos)
        {
            IsActive = isActive;
            Pos = pos;
        }
        
        public bool Equals(HyperCube other) => (Pos.X, Pos.Y, Pos.Z, Pos.W) == (other.Pos.X, other.Pos.Y, other.Pos.Z, other.Pos.W);
        public override int GetHashCode() => HashCode.Combine(Pos.X, Pos.Y, Pos.Z, Pos.W);
    }


    public record Pos3D(int X, int Y, int Z);
    public record Pos4D(int X, int Y, int Z, int W);

    public record Range(int Start, int End);
    public record Area3D(Range RangeX, Range RangeY, Range RangeZ);
    public record Area4D(Range RangeX, Range RangeY, Range RangeZ, Range RangeW);
    
    public override string[] Solve()
    {
        var activeCubesCount = Pt1();
        AssignAnswer1(activeCubesCount);
        Console.WriteLine(activeCubesCount);
        
        activeCubesCount = Pt2();
        AssignAnswer2(activeCubesCount);
        
        return Answers;
    }
    
    private int Pt1()
    {
        var cubes =
            (from x in Enumerable.Range(0, InputNlSplit.Length)
                from y in Enumerable.Range(0, InputNlSplit[x].Length)
                select new Cube(InputNlSplit[x][y] is '#', new Pos3D(x, y, 0))).ToHashSet();

        var area = new Area3D(
            new Range(cubes.Min(c => c.Pos.X) - 1, cubes.Max(c => c.Pos.X) + 1),
            new Range(cubes.Min(c => c.Pos.Y) - 1, cubes.Max(c => c.Pos.Y) + 1),
            new Range(cubes.Min(c => c.Pos.Z) - 1, cubes.Max(c => c.Pos.Z) + 1));


        foreach (var cube in cubes.ToArray())
        {
            foreach (var n in CreateNeighbours(cube))
            {
                cubes.Add(n);
            }
        }

        var cycle = 1;
        while (cycle++ <= 6)
        {
            
            foreach(var cube in cubes.ToArray())
            {
                cube.PendingActivateStateChange = DetermineActiveState(cubes, cube);
                
                var neighbours = CreateNeighbours(cube).ToArray();
                foreach (var n in neighbours)
                {
                    cubes.Add(n);
                }
            };

            foreach (var cube in cubes.Where(cube => cube is { PendingActivateStateChange: not null }))
            {
                cube.IsActive = cube.PendingActivateStateChange.Value;
                cube.PendingActivateStateChange = null;
            }
            // area.ForEach((x, y, z) =>
            // {
            //     var cube = cubes.SingleOrDefault(c => c.Pos.X == x && c.Pos.Y == y && c.Pos.Z == z);
            //     if (cube == default)
            //     {
            //         cube = new Cube(false, new Pos3D(x, y, z));
            //         cubes.Add(cube);
            //     }
            //
            //     cube.PendingActivateStateChange = DetermineActiveState(cubes, cube);
            // });
            //
            // area = new Area3D(
            //     new Range(cubes.Min(c => c.Pos.X) - 1, cubes.Max(c => c.Pos.X) + 1),
            //     new Range(cubes.Min(c => c.Pos.Y) - 1, cubes.Max(c => c.Pos.Y) + 1),
            //     new Range(cubes.Min(c => c.Pos.Z) - 1, cubes.Max(c => c.Pos.Z) + 1));
            //
            // area.ForEach((x, y, z) =>
            // {
            //     var cube = cubes.SingleOrDefault(c => c.Pos.X == x && c.Pos.Y == y && c.Pos.Z == z);
            //     if (cube is { PendingActivateStateChange: not null })
            //     {
            //         cube.IsActive = cube.PendingActivateStateChange.Value;
            //         cube.PendingActivateStateChange = null;
            //     }
            // });
        }


        var activeCubesCount = cubes.Count(c => c.IsActive);
        return activeCubesCount;
    }
    
    private int Pt2()
    {
        var cubes = (from x in Enumerable.Range(0, InputNlSplit.Length)
                from y in Enumerable.Range(0, InputNlSplit[x].Length)
                select new HyperCube(InputNlSplit[x][y] is '#', new Pos4D(x, y, 0, 0))).ToHashSet();
        
        foreach (var cube in cubes.ToArray())
        {
            foreach (var n in CreateNeighbours(cube))
            {
                cubes.Add(n);
            }
        }

        var cycle = 1;
        while (cycle++ <= 6)
        {
            foreach(var cube in cubes.ToArray())
            {
                cube.PendingActivateStateChange = DetermineActiveState(cubes, cube);
                
                var neighbours = CreateNeighbours(cube).ToArray();
                foreach (var n in neighbours)
                {
                    cubes.Add(n);
                }
            };

            foreach (var cube in cubes.Where(cube => cube is { PendingActivateStateChange: not null }))
            {
                cube.IsActive = cube.PendingActivateStateChange.Value;
                cube.PendingActivateStateChange = null;
            }
        }


        var activeCubesCount = cubes.Count(c => c.IsActive);
        return activeCubesCount;
    }

    private static bool DetermineActiveState(IEnumerable<Cube> cubes, Cube cube)
    {
        var neighbours = cubes.Where(c => c != cube && cube.IsNeighbour(c)).ToArray();
        var activeNeighbours = neighbours.Where(n => n.IsActive).ToArray();

        if (cube.IsActive)
        {
            return activeNeighbours.Length is 2 or 3;
        } 
        return activeNeighbours.Length is 3;
        
    }
    
    private static ICollection<Cube> CreateNeighbours(Cube cube)
    {
        var neighbours = new List<Cube>();
        var area = new Area3D(
            new Range(cube.Pos.X - 1, cube.Pos.X + 1),
            new Range(cube.Pos.Y - 1, cube.Pos.Y + 1),
            new Range(cube.Pos.Z - 1, cube.Pos.Z + 1));
        area.ForEach((x, y, z) =>
        {
            if ((x, y, z) != (cube.Pos.X, cube.Pos.Y, cube.Pos.Z))
            {
                var cube = new Cube(false, new Pos3D(x, y, z));
                neighbours.Add(cube);
            }
        });
        return neighbours;
    }
    private static ICollection<HyperCube> CreateNeighbours(HyperCube cube)
    {
        var neighbours = new List<HyperCube>();
        var area = new Area4D(
            new Range(cube.Pos.X-1, cube.Pos.X+1),
            new Range(cube.Pos.Y-1, cube.Pos.Y+1),
            new Range(cube.Pos.Z-1, cube.Pos.Z+1),
            new Range(cube.Pos.W-1, cube.Pos.W+1));
        area.ForEach((x, y, z, w) =>
        {
            if ((x, y, z, w) != (cube.Pos.X, cube.Pos.Y, cube.Pos.Z, cube.Pos.W))
            {
                var cube = new HyperCube(false, new Pos4D(x, y, z, w));
                neighbours.Add(cube);
            }
        });
        return neighbours;
    }

    private static bool DetermineActiveState(IEnumerable<HyperCube> cubes, HyperCube cube)
    {
        var neighbours = cubes.Where(c => c != cube && cube.IsNeighbour(c)).ToArray();
        var activeNeighbours = neighbours.Where(n => n.IsActive).ToArray();

        if (cube.IsActive)
        {
            return activeNeighbours.Length is 2 or 3;
        } 
        return activeNeighbours.Length is 3;
        
    }
    
    private static IEnumerable<int> GetRange(int start, int end) => Enumerable.Range(start, end-start+1);

}

