namespace Challenges2022; 

public partial class Solution18 {
    
    private List<Cube> ParseInput() {
        var cubes = new List<Cube>();
        foreach (var line in InputNlSplit) {
            var nums = line.Split(',').Select(int.Parse).ToList();
            var c = new Cube(nums[0], nums[1], nums[2], 1, 1);
            cubes.Add(c);
        }

        return cubes;
    }
    
    public static int IntersectingArea(Cube c1, Cube c2) {
        var areaX = (c1.X + c1.Width - c1.X) * (c2.X + c2.Width - c2.X);
        var areaY = (c1.Y + c1.Width - c1.Y) * (c2.Y + c2.Width - c2.Y);
        var areaZ = (c1.Z + c1.Height - c1.Z) * (c2.Z + c2.Height - c2.Z);
        var xIntersection = 0;
        var yIntersection = 0;
        var zIntersection = 0;

        if (Math.Min(c1.X + c1.Width, c2.X + c2.Width) - Math.Max(c1.X, c2.X) > 0) 
            xIntersection = Math.Min(c1.X + c1.Width, c2.X + c2.Width) - Math.Max(c1.X, c2.X);
        if (Math.Min(c1.Y + c1.Width, c2.Y + c2.Width) - Math.Max(c1.Y, c2.Y) > 0) 
            yIntersection = Math.Min(c1.Y + c1.Width, c2.Y + c2.Width) - Math.Max(c1.Y, c2.Y);
        if (Math.Min(c1.Z + c1.Height, c2.Z + c2.Height) - Math.Max(c1.Z, c2.Z) > 0) 
            zIntersection = Math.Min(c1.Z + c1.Height, c2.Z + c2.Height) - Math.Max(c1.Z, c2.Z);
        
        var areaIntersection = 0;

        if ((xIntersection == 0) && (yIntersection == 0) && (zIntersection == 0)) {
            areaIntersection = 0;
        } else {
            if (xIntersection > 0)
                areaIntersection = xIntersection;
            if (yIntersection > 0)
                areaIntersection *= yIntersection;
            if (zIntersection > 0)
                areaIntersection *= zIntersection;
        }

        return (areaX + areaY + areaZ - areaIntersection);
    }
}

    /*
    private (bool, int) CheckForTrapped(IReadOnlyCollection<Cube> cubes, Cube cube, List<int[]> forbiddenOffset = default) {
        var trappedOffsets = new List<int[]> { 
            new[] { 1, 0, 0 }, new[] { -1, 0, 0 }, 
            new[] { 0, 1, 0 }, new[] { 0, -1, 0 }, 
            new[] { 0, 0, 1 },  new[] { 0, 0, -1 } };

        if (forbiddenOffset != default) {
            foreach(var fOffset in forbiddenOffset)
                trappedOffsets = trappedOffsets.Where(o=>o[0] ==fOffset[0] && o[1] ==fOffset[1] && o[2] ==fOffset[2]).ToList();
        }
        
        if (Trapped.Any(x => x.X == cube.X && x.Y == cube.Y && x.Z == cube.Z)) {
            return (false, 0);
        }

        if (cube.X < 0 || cube.Y < 0 || cube.Z < 0)
            return (false, 0);
        if (cube.X > cubes.MaxBy(c => c.X).X || cube.Y > cubes.MaxBy(c => c.Y).Y || cube.Z > cubes.Max(c => c.Z)) {
            return (false, 0);
        }
        
        if (cubes.Any(c => c.X == cube.X && c.Y == cube.Y && c.Z == cube.Z))
            return (false, 0);

        var offsetsFound = 0;
        int[] offsetNotFound = { };
        foreach (var offset in trappedOffsets) {
            if (cubes.Any(c => c.X == cube.X + offset[0] && c.Y == cube.Y + offset[1] && c.Z == cube.Z + offset[2]))
                offsetsFound++;
            else 
                offsetNotFound = offset;
        }
        if (offsetsFound == trappedOffsets.Count - 1) {
            if (forbiddenOffset == default) {
                forbiddenOffset = new List<int[]>(){new[] {offsetNotFound[0]*-1, offsetNotFound[1]*-1, offsetNotFound[2]*-1}};
            }
            var result = CheckForTrapped(cubes, new Cube(cube.X+offsetNotFound[0], cube.Y+offsetNotFound[1], cube.Z+offsetNotFound[2]), forbiddenOffset);
            if (result.Item1 == false)
                return result;
            return (true, result.Item2 + 6);
        }

        if (offsetsFound == trappedOffsets.Count) {
            Trapped.Add(cube);
        }
        return (offsetsFound == trappedOffsets.Count) ? (true, 6) : (false, 0);
    }
    */
