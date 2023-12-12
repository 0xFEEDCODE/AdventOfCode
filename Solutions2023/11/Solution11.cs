using System.Collections.Concurrent;
using System.Numerics;

using Framework;

namespace Solutions2023;

public class Solution11 : SolutionFramework
{
    public Solution11() : base(11) { }

    class Field(Pos2D Pos)
    {
        public Pos2D Pos = Pos;
        public int? GalaxyN;
    }

    Field[][] g;
    
    public override string[] Solve()
    {
        var galaxies = new List<Field>();
        var tempG = InputAsGrid<char>();

        var height = 0;

        g = (tempG.Length, tempG[0].Length).CreateGrid<Field>();
        var ag = (tempG.Length, tempG[0].Length).CreateGrid<int>();
        var gi = 1;

        for (var i = 0; i < g.Length; i++)
        {
            for (var j = 0; j < g[0].Length; j++)
            {
                var pos = new Pos2D(j, i);
                var entry = new Field(pos);
                
                ag.SetCell(pos, 1);
                
                if (tempG[i][j] == '#')
                {
                    entry.GalaxyN = gi++;
                    galaxies.Add(entry);
                }
                
                g.SetCell(pos, entry);
            }
        }
        
        for (var i = 0; i < g.Length; i++)
        {
            var c = false;
            for (var j = 0; j < g[0].Length; j++)
            {
                if (g[i][j].GalaxyN.HasValue)
                {
                    c = true;
                }
            }
            if (c is false)
            {
                for (var j = 0; j < g[0].Length; j++)
                {
                    var pos = new Pos2D(j, i);
                    ag.SetCell(pos, 1000000);
                }
            }
        }
        for (var j = 0; j < g[0].Length; j++)
        {
            var c = false;
            for (var i = 0; i < g.Length; i++)
            {
                if (g[i][j].GalaxyN.HasValue)
                {
                    c = true;
                }
            }
            if (c is false)
            {
                for (var i = 0; i < g.Length; i++)
                {
                    var pos = new Pos2D(j, i);
                    ag.SetCell(pos, 1000000);
                }
            }
        }
        

        var l = new object();
        Console.WriteLine(galaxies.Count);
        
        var distances = new ConcurrentDictionary<(int Start, int End), double>();
        var pairs = galaxies.SelectMany((x, i) => galaxies.Skip(i + 1), Tuple.Create);
        /*foreach (var tuple in pairs)
        {

            var r = AStar(GetCopyOfGrid(ag), (tuple.Item1.Pos.Y, tuple.Item1.Pos.X), (tuple.Item2.Pos.Y, tuple.Item2.Pos.X));
            if (tuple.Item1.GalaxyN == 3 && tuple.Item2.GalaxyN == 6)
            {
                Console.WriteLine();
            }
            distances.TryAdd((tuple.Item1.GalaxyN.Value, tuple.Item2.GalaxyN.Value), r.Count-1);
        }
        */
        
        var left = pairs.Count();
        
        Parallel.ForEach(new[] { (0, 25), (25, 50), (50, 75), (75, 100), (100, 125), (125, 150), (150, 175), (175, 225), (225, 250), (250, 275), (275, 300), (300, 325), (325, 350), (350, 375), (375, 400), (400, 500) }, r =>
        {
            var start = r.Item1;
            var end = r.Item2;

            for (var i = start; i <= end; i++)
            {
                var p = pairs.Where(x => x.Item1.GalaxyN == i);
                foreach (var g in p)
                {
                    var galaxy = g.Item1;
                    var otherG = g.Item2;
                    if (distances.ContainsKey((galaxy.GalaxyN.Value, otherG.GalaxyN.Value)) || distances.ContainsKey((otherG.GalaxyN.Value, galaxy.GalaxyN.Value)))
                    {
                        continue;
                    }
                    var path = AStar(GetCopyOfGrid(ag), (galaxy.Pos.Y, galaxy.Pos.X), (otherG.Pos.Y, otherG.Pos.X));
                    lock (l)
                    {
                        if (distances.TryAdd((galaxy.GalaxyN.Value, otherG.GalaxyN.Value), path.Sum(x=>x.Item2)-1))
                        {
                            left--;
                        }
                    }

                }
                Console.WriteLine((i, left));
            }
        });

        BigInteger s = 0;
        foreach (var keyValuePair in distances)
        {
            s += (BigInteger)keyValuePair.Value;
        }
        AssignAnswer1(s);
        
        return Answers;
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
    private int[][] GetCopyOfGrid(int[][] grid) {
        var copy = (grid.Length, grid[0].Length).CreateGrid<int>();
        for(var i = 0; i < grid.Length; i++){
            for(var j = 0; j < grid[i].Length; j++) {
                copy[i][j] = grid[i][j];
            }
        }

        return copy;
    }
}