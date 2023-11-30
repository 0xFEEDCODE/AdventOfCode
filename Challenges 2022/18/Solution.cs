using Framework;

namespace Challenges2022;

public partial class Solution18 : SolutionFramework {
    public Solution18() : base(18) {
    }

    public override string[] Solve() {
        var cubes = ParseInput();
        
        var solidObj = new HashSet<Cube>(cubes);
        
        foreach (var x in Enumerable.Range(cubes.MinBy(x=>x.X).X+3, cubes.MaxBy(x=>x.X).X+1)) {
            foreach (var y in Enumerable.Range(cubes.MinBy(x=>x.Y).Y+3, cubes.MaxBy(x=>x.Y).Y+1)) {
                foreach (var z in Enumerable.Range(cubes.MinBy(x=>x.Z).Z+3, cubes.MaxBy(x=>x.Z).Z+1)) {
                    if ((x, y, z) == (11, 2, 14)) {
                        Console.WriteLine();
                    }
                    var cube = new Cube(x, y, z);
                    var neighb = GetNeighbors(cubes, cube);
                    if (neighb.Item2.Any()) {
                        foreach (var neighC in neighb.Item2) {
                            if (!IsOuter(cubes, neighC)) {
                                solidObj.Add(neighC);
                            }
                        }
                    }
                }
            }
        }
        
        var n = 0;
        foreach(var cube in solidObj) {
            var neigh = GetNeighbors(solidObj.ToList(), cube);
            if (cube.Y == 0) {
                Console.WriteLine();
            }
            n += 6 - neigh.Item1.Count;
        }

        var outer = new List<Cube>();

        /*
        var overlaps = new List<Cube>();
        foreach (var cube in solidObj) {
            foreach(var anotherCube in solidObj.Where(x => x != cube)) {
                var xDiff = Math.Abs(cube.X - anotherCube.X);
                var yDiff = Math.Abs(cube.Y - anotherCube.Y);
                var zDiff = Math.Abs(cube.Z - anotherCube.Z);
                if (xDiff + yDiff + zDiff <= 1) {
                    overlaps.Add(cube);
                }
            }
            */
            /*
            var neigh = GetNeighbors(solidObj.ToList(), cube);
            if(neigh.Item1.Count!=6)
                outer.Add(cube);
        }
        */

        var n3 = 0;
        /*
        overlaps = new List<Cube>();
        foreach (var cube in outer) {
            var neigh = GetNeighbors(cubes, cube);
            n3 += 6 - neigh.Item1.Count;
            foreach (var ne in neigh.Item2) {
                if ((ne.X,ne.Y,ne.Z) == (2, 2, 5)) {
                    Console.WriteLine();
                }
                if (!IsOuter(cubes, ne)) {
                    n3--;
                }
            }
        }
        */


        using StreamWriter file = new("o.txt");
        foreach (var o in solidObj) {
            file.WriteLine($"{o.X},{o.Y},{o.Z}");
        }
        file.Dispose();
        Console.WriteLine();
        
        //var n2 = (solidObj.Count * 6) - overlaps.Count;
        var n2 = 1;
        Console.WriteLine(n);
        Console.WriteLine(n2);
        return Answers;
    }


    private static bool CheckForTrapped(IReadOnlyCollection<Cube> cubes, Cube cube) {
        var trappedOffsets = new List<int[]> { 
            new[] { 1, 0, 0 }, new[] { -1, 0, 0 }, 
            new[] { 0, 1, 0 }, new[] { 0, -1, 0 }, 
            new[] { 0, 0, 1 },  new[] { 0, 0, -1 } };

        if (cubes.Any(c => c.X == cube.X && c.Y == cube.Y && c.Z == cube.Z))
            return false;

        var found = 0;
        foreach (var offset in trappedOffsets) {
            if (!IsTrapped(cubes, cube, offset)) 
                return false;
        }

        return true;
    }
    
        static (List<Cube>, List<Cube>) GetNeighbors(List<Cube> cubes, Cube cube) {
            var neighbors = new List<Cube>();
            var emptyNeigh = new List<Cube>();
            var n1 = cubes.FirstOrDefault(c => c.X == cube.X + 1 && c.Y == cube.Y && c.Z == cube.Z);
            if(n1!=default)
                neighbors.Add(n1);
            else
                emptyNeigh.Add(new Cube(cube.X+1, cube.Y, cube.Z));
            
            var n2 = cubes.FirstOrDefault(c => c.X == cube.X - 1 && c.Y == cube.Y && c.Z == cube.Z);
            if(n2!=default)
                neighbors.Add(n2);
            else
                emptyNeigh.Add(new Cube(cube.X-1, cube.Y, cube.Z));
            
            var n3 = cubes.FirstOrDefault(c => c.X == cube.X  && c.Y == cube.Y+1 && c.Z == cube.Z);
            if(n3!=default)
                neighbors.Add(n3);
            else
                emptyNeigh.Add(new Cube(cube.X, cube.Y+1, cube.Z));
            
            var n4 = cubes.FirstOrDefault(c => c.X == cube.X  && c.Y == cube.Y-1 && c.Z == cube.Z);
            if(n4!=default)
                neighbors.Add(n4);
            else
                emptyNeigh.Add(new Cube(cube.X, cube.Y-1, cube.Z));
            
            var n5 = cubes.FirstOrDefault(c => c.X == cube.X  && c.Y == cube.Y && c.Z == cube.Z+1);
            if(n5!=default)
                neighbors.Add(n5);
            else
                emptyNeigh.Add(new Cube(cube.X, cube.Y, cube.Z+1));
            
            var n6 = cubes.FirstOrDefault(c => c.X == cube.X  && c.Y == cube.Y && c.Z == cube.Z-1);
            if(n6!=default)
                neighbors.Add(n6);
            else
                emptyNeigh.Add(new Cube(cube.X, cube.Y, cube.Z-1));
            
            return (neighbors, emptyNeigh);
        }

        static bool IsOuter(List<Cube> cubes, Cube cube) {
            /*
            var mx = cubes.MaxBy(x => x.X).X+1;
            var my = cubes.MaxBy(x => x.Y).Y+1;
            var mz = cubes.MaxBy(x => x.Z).Z+1;
            return !(
                -1 <= cube.X && cube.X <= mx && 
                -1 <= cube.Y && cube.Y <= my && 
                -1 <= cube.Z && cube.Z <= mz);
            */
            var xUp = cubes.Any(c => c.X < cube.X && c.Y == cube.Y && c.Z == cube.Z);
            var xDown = cubes.Any(c => c.X > cube.X && c.Y == cube.Y && c.Z == cube.Z);
            var yUp = cubes.Any(c => c.X == cube.X && c.Y < cube.Y && c.Z == cube.Z);
            var yDown = cubes.Any(c => c.X == cube.X && c.Y > cube.Y && c.Z == cube.Z);
            var zUp = cubes.Any(c => c.X == cube.X && c.Y == cube.Y && c.Z < cube.Z);
            var zDown = cubes.Any(c => c.X == cube.X && c.Y == cube.Y && c.Z > cube.Z);
            
            return !xUp || !xDown || !yUp || !yDown || !zUp || !zDown;
        }
    
    private static bool IsTrapped(IReadOnlyCollection<Cube> cubes, Cube cube, int[] offset) => 
        cubes.Any(c => c.X == cube.X + offset[0] && c.Y == cube.Y + offset[1] && c.Z == cube.Z + offset[2]);
}
