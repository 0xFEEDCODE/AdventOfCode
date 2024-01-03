using Framework;

namespace Solutions2023;

public class Solution17Alt() : SolutionFramework(17)
{
    public override string[] Solve()
    {
        var gr = InputAsGrid<int>();
        gr.ForEachCell((i, j) =>
        {
            gr[i][j] = InputNlSplit[i][j].ParseInt();
        });
        
        var res = AStar(gr, (0, 0), (gr.Length-1, gr[0].Length-1));
        Console.WriteLine(res.SkipLast(1).Sum(x => x.Item2));
        
        for (var i = 0; i < gr.Length; i++)
        {
            for (var j = 0; j < gr[0].Length; j++)
            {
                if (res.Any(x => x.Item1 == (i, j)))
                {
                    var d = res.Single(x=>x.Item1==(i,j)).d;
                    switch (d)
                    {
                        case Dir.E:
                            Console.Write(">");
                            break;
                        case Dir.W:
                            Console.Write("<");
                            break;
                        case Dir.N:
                            Console.Write("^");
                            break;
                        case Dir.S:
                            Console.Write("v");
                            break;
                        case Dir.X:
                            Console.Write("x");
                            break;
                    }
                }
                else
                {
                    /*
                    Console.Write(gr[i][j]);
                */
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
        NSlot = res.SkipLast(1).Sum(x => x.Item2);
        AssignAnswer1();
        Console.WriteLine();
        
        res = AStar2(gr, (0, 0), (gr.Length-1, gr[0].Length-1));
        
        for (var i = 0; i < gr.Length; i++)
        {
            for (var j = 0; j < gr[0].Length; j++)
            {
                if (res.Any(x => x.Item1 == (i, j)))
                {
                    var d = res.Single(x=>x.Item1==(i,j)).d;
                    switch (d)
                    {
                        case Dir.E:
                            Console.Write(">");
                            break;
                        case Dir.W:
                            Console.Write("<");
                            break;
                        case Dir.N:
                            Console.Write("^");
                            break;
                        case Dir.S:
                            Console.Write("v");
                            break;
                        case Dir.X:
                            Console.Write("x");
                            break;
                    }
                }
                else
                {
                    /*
                    Console.Write(gr[i][j]);
                */
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
        AssignAnswer2(res.SkipLast(1).Sum(x => x.Item2));
        
        return Answers;
    }

    enum Dir { N, S, W, E, X };
    Dir CDir;
    
    class Node((int R, int C) coords)
    {
        public readonly (int R, int C) Coords = coords;
        public int Row => Coords.Item1;
        public int Col => Coords.Item2;
        public int G;
        public Dir? Dir;
        public Node? Parent;
    }

    List<((int, int), int, Dir d)>? AStar(int[][] grid, (int, int) startCoords, (int, int) endCoords)
    {
        if (grid.Length == 0)
        {
            throw new InvalidOperationException();
        }

        var openSet = new Dictionary<string, Node>();
        var closeSet = new Dictionary<string, Node>();

        var origin = new Node(startCoords) { Dir =  Dir.X };

        var neighboursOffsets = new[] { (-1, 0), (0, 1), (1, 0), (0, -1) };
        openSet.Add($"{origin.Row}_{origin.Col}", origin);

        var colEdge = grid.GetLength(0);
        var rowEdge = grid[0].Length;

        var found = false;
        KeyValuePair<string, Node> minNode = default;
        while (openSet.Any())
        {
            minNode = openSet.MinBy(n => n.Value.G);

            if (minNode.Value.Coords == endCoords)
            {
                found = true;
                break;
            }

            openSet.Remove(minNode.Key);
            closeSet.Add(minNode.Key, minNode.Value);

            var currentRow = minNode.Value.Coords.R;
            var currentCol = minNode.Value.Coords.C;

            foreach (var offset in neighboursOffsets)
            {
                var newRow = currentRow + offset.Item1;
                var newCol = currentCol + offset.Item2;

                if (IsOutOfBounds(newRow, newCol))
                {
                    continue;
                }

                var dir = (newRow - currentRow, newCol - currentCol) switch
                {
                    (1, 0) => Dir.S,
                    (-1, 0) => Dir.N,
                    (0, 1) => Dir.E,
                    (0, -1) => Dir.W,
                    _ => throw new ArgumentOutOfRangeException()
                };

                minNode.Value.Dir ??= dir;
                
                var p1 = minNode.Value;
                var p2 = p1.Parent;
                var p3 = p2?.Parent;

                switch (dir)
                {
                    case Dir.N when p1.Dir is Dir.S:
                    case Dir.S when p1.Dir is Dir.N:
                    case Dir.W when p1.Dir is Dir.E:
                    case Dir.E when p1.Dir is Dir.W:
                        continue;
                }

                if(
                    p1.Dir != Dir.X && p2?.Dir != Dir.X && p3?.Dir != Dir.X &&
                    p1.Dir == dir && p2?.Dir == dir && p3?.Dir == dir)
                {
                    continue;
                }

                var childNodeKey = $"{newRow}_{newCol}_{dir}_{p1.Dir}_{p2?.Dir}";

                var childNodeG = minNode.Value.G + grid[newRow][newCol];
                var childNode = new Node((newRow, newCol))
                {
                    G = childNodeG,
                    Dir = dir
                };

                if (!closeSet.ContainsKey(childNodeKey) &&
                    !openSet.ContainsKey(childNodeKey))
                {
                    childNode.Parent = minNode.Value;
                    openSet.Add(childNodeKey, childNode);
                }
            }
        }

        var path = new List<((int, int), int, Dir)>();

        if (found)
        {
            var curNode = openSet.Where(n => n.Value.Coords == minNode.Value.Coords).MinBy(x=>x.Value.G).Value;
            while (curNode != null && curNode.Coords != startCoords)
            {
                path.Add(((curNode.Coords), grid[curNode.Row][curNode.Col], curNode.Dir ?? Dir.X));
                curNode = curNode.Parent;
            }

            path.Add(((curNode.Coords), grid[curNode.Row][curNode.Col], curNode.Dir ?? Dir.X));
        }

        return path;

        bool IsOutOfBounds(int attemptCellRow, int attemptCellCol) => attemptCellRow < 0 || attemptCellCol < 0 || attemptCellRow >= colEdge || attemptCellCol >= rowEdge;
    }
    
    List<((int, int), int, Dir d)>? AStar2(int[][] grid, (int, int) startCoords, (int, int) endCoords)
    {
        if (grid.Length == 0)
        {
            throw new InvalidOperationException();
        }
        
        var endRow = endCoords.Item1;
        var endCol = endCoords.Item2;

        var openSet = new Dictionary<string, Node>();
        var closeSet = new Dictionary<string, Node>();

        var origin = new Node(startCoords) { Dir =  Dir.X };

        var neighboursOffsets = new[] { (-1, 0), (0, 1), (1, 0), (0, -1) };
        openSet.Add($"{origin.Row}_{origin.Col}", origin);

        var colEdge = grid.GetLength(0);
        var rowEdge = grid[0].Length;

        var found = false;
        KeyValuePair<string, Node> minNode = default;
        while (openSet.Any())
        {
            minNode = openSet.MinBy(n => n.Value.G);

            if (minNode.Value.Coords == endCoords)
            {

                var d = minNode.Value.Dir;
                var cr = minNode.Value.Parent;
                var sameDirCount = 0;
                var lastDir = d;
                while (cr != null)
                {
                    if (cr.Dir == d || cr.Dir is Dir.X)
                    {
                        sameDirCount++;
                    }
                    else
                    {
                        break;
                    }
                    cr = cr.Parent;
                }
                if (sameDirCount < 3)
                {
                    openSet.Remove(minNode.Key);
                    continue;
                }
                found = true;
                break;
            }

            openSet.Remove(minNode.Key);
            closeSet.Add(minNode.Key, minNode.Value);

            var currentRow = minNode.Value.Coords.R;
            var currentCol = minNode.Value.Coords.C;

            foreach (var offset in neighboursOffsets)
            {
                var newRow = currentRow + offset.Item1;
                var newCol = currentCol + offset.Item2;

                if (IsOutOfBounds(newRow, newCol))
                {
                    continue;
                }

                var dir = (newRow - currentRow, newCol - currentCol) switch
                {
                    (1, 0) => Dir.S,
                    (-1, 0) => Dir.N,
                    (0, 1) => Dir.E,
                    (0, -1) => Dir.W,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var p1 = minNode.Value;
                var p2 = p1.Parent;
                var p3 = p2?.Parent;
                var p4 = p3?.Parent;
                
                switch (dir)
                {
                    case Dir.N when p1.Dir is Dir.S:
                    case Dir.S when p1.Dir is Dir.N:
                    case Dir.W when p1.Dir is Dir.E:
                    case Dir.E when p1.Dir is Dir.W:
                        continue;
                }

                var d = p1.Dir;
                var cr = minNode.Value.Parent;
                var sameDirCount = 0;
                var lastDir = d;
                while (cr != null)
                {
                    lastDir = cr.Dir;
                    if (cr.Dir == d || cr.Dir is Dir.X)
                    {
                        sameDirCount++;
                    }
                    else
                    {
                        break;
                    }
                    cr = cr.Parent;
                }

                if (p1.Dir is not Dir.X)
                {
                    if (dir == p1.Dir && sameDirCount+1 > 8)
                    {
                        continue;
                    }
                    if (dir != p1.Dir && sameDirCount is >= 0 and < 3)
                    {
                        continue;
                    }
                }

                var childNodeKey = $"{newRow}_{newCol}_{dir}_{lastDir}";

                var childNodeG = minNode.Value.G + grid[newRow][newCol];
                var childNode = new Node((newRow, newCol))
                {
                    G = childNodeG,
                    Dir = dir
                };

                if (!closeSet.ContainsKey(childNodeKey) &&
                    !openSet.ContainsKey(childNodeKey))
                {
                    childNode.Parent = minNode.Value;
                    openSet.Add(childNodeKey, childNode);
                }
            }
        }

        var path = new List<((int, int), int, Dir)>();

        if (found)
        {
            var curNode = openSet.Where(n => n.Value.Coords == minNode.Value.Coords).MinBy(x=>x.Value.G).Value;
            while (curNode != null && curNode.Coords != startCoords)
            {
                path.Add(((curNode.Coords), grid[curNode.Row][curNode.Col], curNode.Dir ?? Dir.X));
                curNode = curNode.Parent;
            }

            path.Add(((curNode.Coords), grid[curNode.Row][curNode.Col], curNode.Dir ?? Dir.X));
        }

        return path;

        bool IsOutOfBounds(int attemptCellRow, int attemptCellCol) => attemptCellRow < 0 || attemptCellCol < 0 || attemptCellRow >= colEdge || attemptCellCol >= rowEdge;
    }
}