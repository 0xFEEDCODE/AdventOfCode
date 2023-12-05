using Framework;

namespace Challenges2021;

public class Solution15 : SolutionFramework {
    public Solution15() : base(15) { }

    public override string[] Solve() {
        var H = InputNlSplit.Length;
        var W = InputNlSplit.First().Length;
        var grid = (H,W).CreateGrid<int>();
        var i = 0;
        foreach (var line in InputNlSplit) {
            var j = 0;
            foreach (var n in line) {
                grid[i][j] = int.Parse(n.ToString());
                j++;
            }
            i++;
        }

        var path = AStar(grid, (0, 0), (grid.Length - 1, grid[0].Length - 1));
        var riskLevelTotal = path!.SkipLast(1).Sum(p=>p.Item2);
        AssignAnswer1(riskLevelTotal);
        
        //Pt2
        var fullGrid = (H*5,W*5).CreateGrid<int>();
        var copy = GetCopyOfGrid(grid);
        var x = 0;
        for (var colRepeat = 0; colRepeat < 5; colRepeat++) {
            for (var rowRepeat = 0; rowRepeat < 5; rowRepeat++) {
                var fullGridRow = grid.Length * rowRepeat;
                var fullGridCol = grid[0].Length * colRepeat;
                for (i = 0; i < copy.Length; i++) {
                    for (var j = 0; j < copy[i].Length; j++) {
                        fullGrid[fullGridRow + i][fullGridCol + j] = copy[i][j];
                        copy[i][j] = (((copy[i][j]) % 9) + 1);
                    }
                }
            }

            copy = GetCopyOfGrid(grid);
            copy.ForEachCell((_i, j) => {
                copy[_i][j] =(((copy[_i][j]+x) % 9) + 1);
            });
            x++;
        }


        path = AStar(fullGrid, (0, 0), (fullGrid.Length - 1, fullGrid[0].Length - 1));
        AssignAnswer2(path!.SkipLast(1).Sum(p=>p.Item2));
        
        return Answers;
    }

    private int[][] GetCopyOfGrid(int[][] grid) {
        var copy = (grid.Length, grid[0].Length).CreateGrid<int>();
        for(var i = 0; i < grid.Length; i++){
            for(var j = 0; j < grid[i].Length; j++) {
                copy[i][j] = grid[i][j];
            }
        }

        return copy;
    }

    private static void PrintPath(int[][] grid, List<((int, int), int)> path) {
        for (var i = 0; i < grid.Length; i++) {
            Console.WriteLine();
            for (var j = 0; j < grid[0].Length; j++) {
                if (path.Any(p => (p.Item1.Item1, p.Item1.Item2) == (i, j))) {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write(grid[i][j]);
            }
        }
    }

    public class Node {
        public int G = 0;
        public int F = 0;
        public (int, int) Coords;
        public int Row => Coords.Item1;
        public int Col => Coords.Item2;
        public Node? Parent;
    }

    public static List<((int, int), int)>? AStar(int[][] grid, (int, int) startCoords, (int, int) endCoords) {
        if (grid.Length == 0)
            throw new InvalidOperationException();

        var openSet = new Dictionary<string, Node>();
        var closeSet = new Dictionary<string, Node>();

        var origin = new Node { Coords = startCoords, G = 0 };
        openSet.Add($"{origin.Row}_{origin.Col}", origin);

        var neighboursOffsets = new List<(int, int)> { (-1, 0), (0, 1), (1, 0), (0, -1) };

        var colEdge = grid.GetLength(0);
        var rowEdge = grid[0].Length;

        bool IsOutOfBounds(int attemptCellRow, int attemptCellCol)
            => attemptCellRow < 0 || attemptCellCol < 0 || attemptCellRow >= colEdge || attemptCellCol >= rowEdge;
        
        var found = false;
        KeyValuePair<string, Node> minNode = default;
        while (openSet.Any()) {
            if (openSet.Count == 0)
                return null;

            minNode = openSet.MinBy(n => n.Value.G);
            
            if (minNode.Value.Coords == endCoords) {
                found = true;
                break;
            }

            openSet.Remove(minNode.Key);
            closeSet.Add(minNode.Key, minNode.Value);

            foreach (var offset in neighboursOffsets) {
                var newRow = minNode.Value.Coords.Item1 + offset.Item1;
                var newCol = minNode.Value.Coords.Item2 + offset.Item2;

                if (IsOutOfBounds(newRow, newCol))
                    continue;
                var childNodeKey = $"{newRow}_{newCol}";

                var childNodeG = minNode.Value.G + grid[newRow][newCol];
                var childNode = new Node() {
                    Coords = (newRow, newCol),
                    G = childNodeG,
                };

                if (!closeSet.ContainsKey(childNodeKey)) {
                    if (!openSet.ContainsKey(childNodeKey)) {
                        childNode.Parent = minNode.Value;
                        openSet.Add(childNodeKey, childNode);
                    }
                }
            }
        }

        var path = new List<((int, int), int)>();

        if (found) {
            var curNode = openSet.Single(n => n.Value.Coords == minNode.Value.Coords).Value;
            while (curNode != null && curNode.Coords != startCoords) {
                path.Add(((curNode.Coords), grid[curNode.Row][curNode.Col]));
                curNode = curNode.Parent;
            }

            path.Add(((curNode.Coords), grid[curNode.Row][curNode.Col]));
        }

        return path;
    }
}
