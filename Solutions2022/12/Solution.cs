using Framework;

namespace Challenges2022;

public class Solution12 : SolutionFramework
{
    public Solution12() : base(12) { }

    public override string[] Solve()
    {
        var grid = (InputNlSplit.Length, InputNlSplit.First().Length).CreateGrid<char>();
        var results = (InputNlSplit.Length, InputNlSplit.First().Length).CreateGrid<int>();
        results.ForEachCell((i, j) => results[i][j] = 0);

        var startcoords = (0, 0);
        var endCoords = (0, 0);
        int i = 0;
        foreach (var line in InputNlSplit)
        {
            int j = 0;
            foreach (var ch in line)
            {
                grid[i][j] = ch;
                if (ch == 'S')
                    startcoords = (i, j);
                if (ch == 'E')
                    endCoords = (i, j);
                j++;
            }

            i++;
        }

        Func<(int, int), (int, int), bool> isStepValid = (currentPos, attemptPos) 
            => grid[attemptPos.Item1][attemptPos.Item2] <= grid[currentPos.Item1][currentPos.Item2] + 1;

        grid[startcoords.Item1][startcoords.Item2] = 'a';

        var ans = AStar(grid, startcoords, endCoords, isStepValid);
        AssignAnswer1(ans.StepsTaken);

        var lowestFound = int.MaxValue;
        grid.ForEachCell((i, j) =>
        {
            if (grid[i][j] == 'a')
            {
                var res = AStar(grid, (i, j), endCoords, isStepValid);
                if (res != null && res.StepsTaken < lowestFound)
                    lowestFound = res.StepsTaken;
            }
        });
        AssignAnswer2(lowestFound);

        return Answers;
    }

    public class GridNode
    {
        public int From = 0;
        public int To = 0;
        public int StepsTaken = 0;
        public (int, int) Coords;
        public int Row => Coords.Item1;
        public int Col => Coords.Item2;
        public GridNode? Parent;
    }

    public static GridNode? AStar(char[][] grid, (int, int) startCoords, (int, int) endCoords, Func<(int, int),(int, int), bool> stepValidation = null)
    {
        if (grid.Length == 0)
            throw new InvalidOperationException();

        var availablePaths = new Dictionary<string, GridNode>();
        var visitedPaths = new Dictionary<string, GridNode>();

        var startGridNode = new GridNode { Coords = startCoords, StepsTaken = 0 };
        var key = startCoords.ToString();
        availablePaths.Add(key, startGridNode);

        KeyValuePair<string, GridNode> GetSmallestAvailablePath()
        {
            if (!availablePaths.Any())
                throw new System.InvalidOperationException();

            var smallest = availablePaths.ElementAt(0);

            foreach (var item in availablePaths)
            {
                if (item.Value.StepsTaken < smallest.Value.StepsTaken)
                    smallest = item;
                else if (item.Value.StepsTaken == smallest.Value.StepsTaken && item.Value.To < smallest.Value.To)
                    smallest = item;
            }

            return smallest;
        }

        var neighboursOffsets = new List<(int, int)> { (-1, 0), (0, 1), (1, 0), (0, -1) };

        var colEdge = grid.GetLength(0);
        var rowEdge = grid[0].Length;

        bool IsOutOfBounds(int attemptCellRow, int attemptCellCol)
            => attemptCellRow < 0 || attemptCellCol < 0 || attemptCellRow >= colEdge || attemptCellCol >= rowEdge;

        while (true)
        {
            //No path found
            if (availablePaths.Count == 0)
                return null;

            var current = GetSmallestAvailablePath();

            if (current.Value.Coords == endCoords)
                return current.Value;

            availablePaths.Remove(current.Key);
            visitedPaths.Add(current.Key, current.Value);

            foreach (var offset in neighboursOffsets)
            {
                var attemptCellRow = current.Value.Coords.Item1 + offset.Item1;
                var attemptCellCol = current.Value.Coords.Item2 + offset.Item2;
                var attemptCellKey = attemptCellRow.ToString() + attemptCellCol;

                if (IsOutOfBounds(attemptCellRow, attemptCellCol) || visitedPaths.ContainsKey(attemptCellKey))
                    continue;

                if (stepValidation != null && !stepValidation.Invoke(current.Value.Coords, (attemptCellRow, attemptCellCol)))
                    continue;

                if (availablePaths.ContainsKey(attemptCellKey))
                {
                    var node = availablePaths[attemptCellKey];
                    var from = Math.Abs(attemptCellRow - startCoords.Item1) + Math.Abs(attemptCellCol - startCoords.Item2);
                    if (from < node.From)
                    {
                        node.From = from;
                        node.StepsTaken = current.Value.StepsTaken + 1;
                        node.Parent = current.Value;
                    }
                }
                else
                {
                    var newNode = new GridNode
                    {
                        Coords = (attemptCellRow, attemptCellCol),
                        From = Math.Abs(attemptCellRow - startCoords.Item1) + Math.Abs(attemptCellCol - startCoords.Item2),
                        To = Math.Abs(attemptCellRow - endCoords.Item1) + Math.Abs(attemptCellCol - endCoords.Item2),
                        StepsTaken = current.Value.StepsTaken + 1,
                        Parent = current.Value
                    };
                    availablePaths.Add(attemptCellKey, newNode);
                }
            }
        }
    }
}
