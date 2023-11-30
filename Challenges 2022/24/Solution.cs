using Framework;

using static Challenges2022.Solution24;

namespace Challenges2022;

public static partial class Ext {
    public static void PrintGrid(this Tile[][] grid, Player player, List<Blizzard> blizzards) {
        for (var i = 0; i < grid.Length; i++) {
            Console.WriteLine();
            for (var j = 0; j < grid[0].Length; j++) {
                var blizzardsAtPos = blizzards.Where(x => x.Coord.Coords == (i, j)).ToArray();
                Console.ForegroundColor = ConsoleColor.Black;
                if ((i, j) == ExitCoord(grid)) {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                
                if (blizzardsAtPos.Count() > 1) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(blizzardsAtPos.Count());
                } else if (blizzardsAtPos.Count() == 1) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(blizzardsAtPos.First().GetTile().GetTileRepresentation());
                } else if (player.Coord.Coords == (i, j)) {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write('E');
                } else
                    Console.Write(grid[i][j].GetTileRepresentation());
            }
        }
    }
    public static List<Blizzard> CopyBlizzards(this List<Blizzard> blizzards) => blizzards.Select(bl => bl.Copy()).ToList();
}

public partial class Solution24 : SolutionFramework {
    public Solution24() : base(24) { }

    public static Dictionary<int, List<Blizzard>> BlizzardsAtTime = new();
    
    public override string[] Solve() {
        var (grid, blizzards, player) = ParseInput();
        var minute = 0;

        var originalBlizzards = blizzards.CopyBlizzards();
        var g = 0;
        BlizzardsAtTime.Add(g++, blizzards.CopyBlizzards());
        MoveBlizzards(grid, blizzards);
        
        while (!AreSame(blizzards, originalBlizzards)) {
            BlizzardsAtTime.Add(g++, blizzards.CopyBlizzards());
            MoveBlizzards(grid, blizzards);
        }

        var res = AStarSpaceTime(grid, player.Coord.Coords, grid.ExitCoord());
        var res2 = AStarSpaceTime(grid, grid.ExitCoord(), player.Coord.Coords, res.MaxBy(x=>x.Item2).Item2+1);
        var res3 = AStarSpaceTime(grid, player.Coord.Coords, grid.ExitCoord(), res2.MaxBy(x=>x.Item2).Item2+1);
        
        Console.WriteLine(res.MaxBy(x=>x.Item2).Item2);
        Console.WriteLine(res3.MaxBy(x=>x.Item2).Item2 );

        return Answers;
    }

    int GetManhattanDistance(int xOri, int yOri, int xDes, int yDes) => Math.Abs(xDes - xOri) + Math.Abs(yDes - yOri);
    
    public List<((int, int), int)>? AStarSpaceTime(Tile[][] grid, (int, int) startCoords, (int, int) desCoords, int time = 0) {
        if (grid.Length == 0)
            throw new InvalidOperationException();

        var openSet = new Dictionary<string, Node>();
        var closeSet = new Dictionary<string, Node>();

        var origin = new Node
            { Coords = startCoords, F = GetManhattanDistance(startCoords.Item1, startCoords.Item2, desCoords.Item1, desCoords.Item2), G = 0, Time = time};

        var key = $"{origin.Row}_{origin.Col}_{origin.Time}";
        openSet.Add(key, origin);

        var neighboursOffsets = new List<(int, int)> { (-1, 0), (0, 1), (1, 0), (0, -1), (0, 0)};

        var colEdge = grid.GetLength(0);
        var rowEdge = grid[0].Length;

        var found = false;
        
        bool IsOutOfBounds(int attemptCellRow, int attemptCellCol)
            => attemptCellRow < 0 || attemptCellCol < 0 || attemptCellRow > colEdge-1 || attemptCellCol > rowEdge-1;

        KeyValuePair<string, Node> minNode = default;
        while (openSet.Any()) {
            if (openSet.Count == 0)
                return null;

            minNode = openSet.First();
            foreach (var node in openSet){
                if (node.Value.F < minNode.Value.F){
                    minNode = node;
                }
            }

            if (minNode.Value.Coords == desCoords) {
                found = true;
                break;
            }

            openSet.Remove(minNode.Key);
            closeSet.Add(minNode.Key, minNode.Value);

            var epoch = minNode.Value.Time + 1;
            foreach (var offset in neighboursOffsets)
            {
                var newRow = minNode.Value.Coords.Item1 + offset.Item1;
                var newCol = minNode.Value.Coords.Item2 + offset.Item2;

                if (IsOutOfBounds(newRow, newCol) || grid[newRow][newCol] == Tile.Wall)
                    continue;

                var childKey = $"{newRow}_{newCol}_{epoch}";
                var childNode = new Node() {
                    Coords = (newRow, newCol), G = minNode.Value.G + 1, Time = epoch,
                    F = minNode.Value.G + 1 + GetManhattanDistance(newRow, newCol, desCoords.Item1, desCoords.Item2)
                };
                
                if (!closeSet.ContainsKey(childKey))
                {
                    if (BlizzardsAtTime[epoch % BlizzardsAtTime.Count].Any(obst => obst.Coord.Coords == (newRow, newCol))) {
                        continue;
                    }

                    if (!openSet.ContainsKey(childKey)) {
                        childNode.Prev = minNode.Value;
                        openSet.Add(childKey, childNode);
                    }
                }
            }
        }
        
        var path = new List<((int, int), int)>();

        if (found){
            var curNode = openSet.Single(n => n.Value.Coords == minNode.Value.Coords).Value;
            while (curNode != null && curNode.Coords != startCoords){
                path.Add(((curNode.Coords), curNode.Time));
                curNode = curNode.Prev;
            }
            path.Add(((curNode.Coords), curNode.Time));
        }

        return path;
    }

    private static bool AreSame(List<Blizzard> oBlizzards, List<Blizzard> blizzards) {
        for (var i = 0; i < oBlizzards.Count; i++) {
            if(blizzards[i].Coord.Coords != oBlizzards[i].Coord.Coords)
                return false;
        }
        return true;
    }
    
}
