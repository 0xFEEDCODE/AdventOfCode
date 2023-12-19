using Framework;

namespace Solutions2023;

public class Solution17() : SolutionFramework(17)
{
    public override string[] Solve()
    {
        var gr = InputAsGrid<int>();
        gr.ForEachCell((i, j) =>
        {
            gr[i][j] = InputNlSplit[i][j].ParseInt();
        });
        
        Search2(gr, new (0,0), new(gr.Length-1, gr[0].Length-1));
        return Answers;
    }

    enum Dir { N, S, W, E, X };

    record Node(GridPos Pos, Dir Dir, int NDir);
    
    void Search2(int[][] grid, GridPos startPos, GridPos endPos)
    {
        var startNode = new Node(startPos, Dir.X, 0);
        
        var pq = new PriorityQueue<Node, int>();
        pq.Enqueue(startNode, 0);

        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, int>();

        var path = new List<(GridPos, Dir d)>();
        var totalCost = 0;

        costSoFar[startNode] = 0;

        while (pq.Count > 0)
        {
            var current = pq.Dequeue();
            var currentDir = current.Dir;
            if (current.Pos == endPos && current.NDir >= 4)
            {
                var temp = current;
                while (cameFrom.ContainsKey(temp))
                {
                    path.Add((temp.Pos, temp.Dir));
                    temp = cameFrom[temp];
                }
                totalCost = costSoFar[current];
                break;
            }

            if (currentDir != Dir.X && current.NDir < 10)
            {
                var newPos = currentDir switch
                {
                    Dir.N => new GridPos(current.Pos.R-1, current.Pos.C),
                    Dir.S => new GridPos(current.Pos.R+1, current.Pos.C),
                    Dir.E => new GridPos(current.Pos.R, current.Pos.C+1),
                    Dir.W => new GridPos(current.Pos.R, current.Pos.C-1),
                };
                if (!IsOutOfBounds(newPos.R, newPos.C))
                {
                    var cost = costSoFar[current] + grid[newPos.R][newPos.C];
                    var newNode = new Node(newPos, currentDir, current.NDir + 1);
                    pq.Enqueue(newNode, cost);
                    costSoFar[newNode] = cost;
                    cameFrom[newNode] = current;
                }
            }

            if (current.Dir != Dir.X && current.NDir < 4)
            {
                continue;
            }
            
            foreach (var (nextPos, nextCost) in grid.GetAllAdjacentCells(current.Pos))
            {
                var nextDir = (nextPos.R - current.Pos.R, nextPos.C - current.Pos.C) switch
                {
                    (1, 0) => Dir.S,
                    (-1, 0) => Dir.N,
                    (0, 1) => Dir.E,
                    (0, -1) => Dir.W,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                switch (nextDir)
                {
                    case Dir.N when current.Dir is Dir.S:
                    case Dir.S when current.Dir is Dir.N:
                    case Dir.W when current.Dir is Dir.E:
                    case Dir.E when current.Dir is Dir.W:
                        continue;
                }
                
                if (nextDir == current.Dir)
                {
                    continue;
                }
                
                var nextN = new Node(nextPos, nextDir, 1);

                var newCost = costSoFar[current] + nextCost;
                if (!costSoFar.ContainsKey(nextN) || newCost < costSoFar[nextN])
                {
                    pq.Enqueue(nextN, newCost);
                    
                    costSoFar[nextN] = newCost;
                    
                    cameFrom[nextN] = current;
                }
            }
        }
        Console.WriteLine(totalCost);
        AssignAnswer2(totalCost);
        
        /*
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[0].Length; j++)
            {
                if (path.Any(x => (x.Item1.R, x.Item1.C) == (i, j)))
                {
                    var d = path.Single(x => (x.Item1.R, x.Item1.C) == (i, j)).d;
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
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
        */

        bool IsOutOfBounds(int r, int c) => r < 0 || c < 0 || r > grid.Length-1 || c > grid[0].Length-1;
        int Heuristic(GridPos x, GridPos y) => Math.Abs(x.R - y.R) + Math.Abs(x.C - y.C);
    }

    void Search(int[][] grid, GridPos startPos, GridPos endPos)
    {
        var startNode = new Node(startPos, Dir.X, 0);
        
        var pq = new PriorityQueue<Node, int>();
        pq.Enqueue(startNode, 0);

        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, int>();

        var path = new List<(GridPos, Dir d)>();
        var totalCost = 0;

        costSoFar[startNode] = 0;

        while (pq.Count > 0)
        {
            var current = pq.Dequeue();
            var currentDir = current.Dir;
            if (current.Pos == endPos)
            {
                var temp = current;
                while (cameFrom.ContainsKey(temp))
                {
                    path.Add((temp.Pos, temp.Dir));
                    temp = cameFrom[temp];
                }
                totalCost = costSoFar[current];
                break;
            }

            if (currentDir != Dir.X && current.NDir is > 0 and < 3)
            {
                var newPos = currentDir switch
                {
                    Dir.N => new GridPos(current.Pos.R-1, current.Pos.C),
                    Dir.S => new GridPos(current.Pos.R+1, current.Pos.C),
                    Dir.E => new GridPos(current.Pos.R, current.Pos.C+1),
                    Dir.W => new GridPos(current.Pos.R, current.Pos.C-1),
                };
                if (!IsOutOfBounds(newPos.R, newPos.C))
                {
                    var cost = costSoFar[current] + grid[newPos.R][newPos.C];
                    var newNode = new Node(newPos, currentDir, current.NDir + 1);
                    pq.Enqueue(newNode, cost);
                    costSoFar[newNode] = cost;
                    cameFrom[newNode] = current;
                }
            }

            foreach (var (nextPos, nextCost) in grid.GetAllAdjacentCells(current.Pos))
            {
                var nextDir = (nextPos.R - current.Pos.R, nextPos.C - current.Pos.C) switch
                {
                    (1, 0) => Dir.S,
                    (-1, 0) => Dir.N,
                    (0, 1) => Dir.E,
                    (0, -1) => Dir.W,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                switch (nextDir)
                {
                    case Dir.N when current.Dir is Dir.S:
                    case Dir.S when current.Dir is Dir.N:
                    case Dir.W when current.Dir is Dir.E:
                    case Dir.E when current.Dir is Dir.W:
                        continue;
                }
                
                if (nextDir == current.Dir)
                {
                    continue;
                }
                
                var nextN = new Node(nextPos, nextDir, 1);

                var newCost = costSoFar[current] + nextCost;
                if (!costSoFar.ContainsKey(nextN) || newCost < costSoFar[nextN])
                {
                    pq.Enqueue(nextN, newCost);
                    
                    costSoFar[nextN] = newCost;
                    
                    cameFrom[nextN] = current;
                }
            }
        }
        Console.WriteLine(totalCost);
        
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[0].Length; j++)
            {
                if (path.Any(x => (x.Item1.R, x.Item1.C) == (i, j)))
                {
                    var d = path.Single(x => (x.Item1.R, x.Item1.C) == (i, j)).d;
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
                    Console.Write(grid[i][j]);
                */
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }

        bool IsOutOfBounds(int r, int c) => r < 0 || c < 0 || r > grid.Length-1 || c > grid[0].Length-1;
        int Heuristic(GridPos x, GridPos y) => Math.Abs(x.R - y.R) + Math.Abs(x.C - y.C);
    }
}