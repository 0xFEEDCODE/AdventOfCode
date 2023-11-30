using Framework;

namespace Challenges2021;

public class Solution9 : SolutionFramework {
    public Solution9() : base(9) { }

    public enum Direction { Up, Down, Right, Left };

    public override string[] Solve() {
        var H = RawInputSplitByNl.Length;
        var W = RawInputSplitByNl[0].Length;
        var heightMap = (H, W).CreateGrid<int>();

        var i = 0;
        foreach (var line in RawInputSplitByNl) {
            for (var j = 0; j < line.Length; j++) {
                heightMap[i][j] = int.Parse(line[j].ToString());
            }
            i++;
        }

        var lowestPoints = new List<(int, int)>();
        
        var riskLevelOfAllLowPoints = 0;
        heightMap.ForEachCell((i, j) => {
            var lowestPoint = true;
            var v = heightMap[i][j];
            try { lowestPoint = lowestPoint && v < heightMap.NeighborCellDown(i, j);} catch (Exception e) { }
            try { lowestPoint = lowestPoint && v < heightMap.NeighborCellLeft(i, j); } catch (Exception e) { }
            try { lowestPoint = lowestPoint && v < heightMap.NeighborCellRight(i, j); } catch (Exception e) { }
            try { lowestPoint = lowestPoint && v < heightMap.NeighborCellUp(i, j); } catch (Exception e) { }

            if (lowestPoint) {
                lowestPoints.Add((i,j));
                riskLevelOfAllLowPoints += heightMap[i][j] + 1;
            }
        });
        AssignAnswer1(riskLevelOfAllLowPoints);
        
        //pt 2
        var basinSizes = new List<int>();
        // foreach (var lp in lowestPoints) {
        //     var basinSize = GetBasinSize(heightMap, lp.Item1, lp.Item2, new List<(int, int)>());
        //     if (basinSize > 0) {
        //         basinSizes.Add(basinSize);
        //     }
        // }
        
        foreach (var lp in lowestPoints) {
            var basinSize = GetBasinSize(heightMap, lp.Item1, lp.Item2, new List<(int, int)>());
            if (basinSize > 0) {
                basinSizes.Add(basinSize);
            }
        }
        
        AssignAnswer2(basinSizes.OrderByDescending(x=>x).Take(3).Aggregate((x, acc) => x*acc));

        for (i = 0; i < heightMap.Length; i++) {
            Console.WriteLine();
            for (var j = 0; j < heightMap[i].Length; j++) {
                if (Basins.Contains((i, j))) {
                    Console.ForegroundColor = ConsoleColor.Red;
                } else {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(heightMap[i][j]);
            }
        }
        
        return Answers;
    }

    public static List<(int, int)> Basins = new List<(int, int)>();

    private static int GetBasinSize(int[][] heightMap, int i, int j, ICollection<(int, int)> basin) {
        int v;
        try {
            v = heightMap[i][j];
        } catch { return basin.Count;}

        if (v == 9)
            return basin.Count;

        if (basin.Contains((i, j))) {
            return basin.Count;
        }

        var containsUp = basin.Contains((i - 1, j));
        var containsDown = basin.Contains((i + 1, j));
        var containsLeft = basin.Contains((i, j - 1));
        var containsRight = basin.Contains((i, j + 1));
    
        var lowestPoint = true;
        if (!containsDown) { try { lowestPoint = lowestPoint && v <= heightMap.NeighborCellDown(i, j); } catch (Exception e) { } }
        if (!containsLeft) { try { lowestPoint = lowestPoint && v <= heightMap.NeighborCellLeft(i, j); } catch (Exception e) { } }
        if (!containsRight) { try { lowestPoint = lowestPoint && v <= heightMap.NeighborCellRight(i, j); } catch (Exception e) { } }
        if (!containsUp) { try { lowestPoint = lowestPoint && v <= heightMap.NeighborCellUp(i, j); } catch (Exception e) { } }

        if (lowestPoint) {
            basin.Add((i,j));
            { GetBasinSize(heightMap, i, j - 1, basin); }
            { GetBasinSize(heightMap, i + 1, j, basin); }
            { GetBasinSize(heightMap, i, j + 1, basin); }
            { GetBasinSize(heightMap, i - 1, j, basin); }
        }

        Basins.AddRange(basin);
        return basin.Count();
    }
}
