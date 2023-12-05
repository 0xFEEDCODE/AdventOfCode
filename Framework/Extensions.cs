namespace Framework;

public static class Extensions
{
    public static void EncloseInTryCatch(Action action, Action? catchAction = null)
    {
        try
        {
            action.Invoke();
        } 
        catch
        {
            catchAction?.Invoke();
        }
    }

    public static T[][] CreateGrid<T>(this (int, int) size) => CreateGrid<T>(size.Item1, size.Item2);

    public static T[][] CreateGrid<T>(int H, int W)
    {
        var grid = new T[H][];
        grid.InitRows(W);
        return grid;
    }

    public static void AssignIfBigger(this ref long n1, long n2) => n1 = n2 > n1 ? n2 : n1;
    public static void AssignIfBigger(this ref int n1, int n2) => n1 = n2 > n1 ? n2 : n1;
    public static void AssignIfLower(this ref long n1, long n2) => n1 = n1 > n2 ? n2 : n1;
    public static void AssignIfLower(this ref int n1, int n2) => n1 = n1 > n2 ? n2 : n1;

    public static T NeighborCellUp<T>(this T[][] grid, int i, int j) => grid[i - 1][j];
    public static T NeighborCellLeft<T>(this T[][] grid, int i, int j) => grid[i][j - 1];
    public static T NeighborCellRight<T>(this T[][] grid, int i, int j) => grid[i][j + 1];
    public static T NeighborCellDown<T>(this T[][] grid, int i, int j) => grid[i + 1][j];
    public static T NeighborCellUp<T>(this T[][] grid, Pos2D pos) => grid[pos.X - 1][pos.Y];
    public static T NeighborCellLeft<T>(this T[][] grid, Pos2D pos) => grid[pos.X][pos.Y - 1];
    public static T NeighborCellRight<T>(this T[][] grid, Pos2D pos) => grid[pos.X][pos.Y + 1];
    public static T NeighborCellDown<T>(this T[][] grid, Pos2D pos) => grid[pos.X + 1][pos.Y];

    //Clockwise diagonals
    public static T NeighborCellTopLeft<T>(this T[][] grid, int i, int j) => grid[i - 1][j - 1];
    public static T NeighborCellTopRight<T>(this T[][] grid, int i, int j) => grid[i - 1][j + 1];
    public static T NeighborCellBottomRight<T>(this T[][] grid, int i, int j) => grid[i + 1][j + 1];
    public static T NeighborCellBottomLeft<T>(this T[][] grid, int i, int j) => grid[i + 1][j - 1];
    public static T NeighborCellTopLeft<T>(this T[][] grid, Pos2D pos) => grid[pos.X - 1][pos.Y - 1];
    public static T NeighborCellTopRight<T>(this T[][] grid, Pos2D pos) => grid[pos.X - 1][pos.Y + 1];
    public static T NeighborCellBottomRight<T>(this T[][] grid, Pos2D pos) => grid[pos.X + 1][pos.Y + 1];
    public static T NeighborCellBottomLeft<T>(this T[][] grid, Pos2D pos) => grid[pos.X + 1][pos.Y - 1];


    public static bool IsWithinBounds<T>(this T[][] grid, int i, int j) => i >= 0 && i < grid.Length && j >= 0 && j < grid[0].Length;

    public static bool IsAnyNeighbor<T>(this T[][] grid, Pos2D pos, Func<T, bool> eval, bool includeDiagonal = true) => grid.IsAnyNeighbor(pos.X, pos.Y, eval, includeDiagonal);
    public static bool IsAnyNeighbor<T>(this T[][] grid, int i, int j, Func<T, bool> eval, bool includeDiagonal = true)
    {
        var found = false;
        EncloseInTryCatch(() => { found = eval(grid.NeighborCellUp(i, j));});
        EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellDown(i, j));});
        EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellLeft(i, j));});
        EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellRight(i, j));});
        if (includeDiagonal)
        {
            EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellTopLeft(i, j)); });
            EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellTopRight(i, j)); });
            EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellBottomLeft(i, j)); });
            EncloseInTryCatch(() => { found = found || eval(grid.NeighborCellBottomRight(i, j)); });
        }
        return found;
    }
    
    public static ICollection<(Pos2D pos, T val)> GetAllAdjacentCells<T>(this T[][] grid, Pos2D pos, bool includeDiagonal = true) 
        => grid.GetAllAdjacentCells(pos.X, pos.Y, includeDiagonal).Select(x => (new Pos2D(x.i, x.j), x.val)).ToArray();
    
    public static ICollection<(int i, int j, T val)> GetAllAdjacentCells<T>(this T[][] grid, int i, int j, bool includeDiagonal = true)
    {
        var adjacent = new List<(int i, int j, T val)>();
        if (grid.IsWithinBounds(i - 1, j)) { adjacent.Add((i - 1, j, grid[i - 1][j]));}
        if (grid.IsWithinBounds(i + 1, j)) { adjacent.Add((i + 1, j, grid[i + 1][j]));}
        if (grid.IsWithinBounds(i, j - 1)) { adjacent.Add((i, j - 1, grid[i][j - 1]));}
        if (grid.IsWithinBounds(i, j + 1)) { adjacent.Add((i, j + 1, grid[i][j + 1]));}
        if (includeDiagonal)
        {
            if (grid.IsWithinBounds(i - 1, j - 1)) { adjacent.Add((i - 1, j - 1, grid[i - 1][j - 1])); }
            if (grid.IsWithinBounds(i - 1, j + 1)) { adjacent.Add((i - 1, j + 1, grid[i - 1][j + 1])); }
            if (grid.IsWithinBounds(i + 1, j + 1)) { adjacent.Add((i + 1, j + 1, grid[i + 1][j + 1])); }
            if (grid.IsWithinBounds(i + 1, j - 1)) { adjacent.Add((i + 1, j - 1, grid[i + 1][j - 1])); }
        }
        return adjacent;
    }

    public static T GetCell<T>(this T[][] grid, Pos2D pos) => grid[pos.X][pos.Y];
    public static void SetCell<T>(this T[][] grid, Pos2D pos, T val) => grid[pos.X][pos.Y] = val;

    public static void InitRows<T>(this T[][] grid, int rowLen)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            grid[i] = new T[rowLen];
        }
    }

    public static void ForEachCell<T>(this T[][] grid, Action<int, int> action)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                action.Invoke(i, j);
            }
        }
    }
    
    public static void ForEachCell<T>(this T[][] grid, Action<Pos2D> action)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                action.Invoke(new Pos2D(i, j));
            }
        }
    }

    public static void SetAllCellsToValue<T>(this T[][] grid, T value)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                grid[i][j] = value;
            }
        }
    }

    public static void ForEachCell<T>(this T[][] grid, Action action)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                action.Invoke();
            }
        }
    }

    // String
    public static string[] SplitByNewline(this string text)
    {
        var lines = text.Split('\n').Select(l => l.Trim('\r'));
        if (lines.Last().Length == 0)
        {
            return lines.Take(lines.Count() - 1).ToArray();
        }

        return lines.ToArray();
    }

    public static string[] SplitByGroup(this string text) =>
        text.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

    public static void ForEachInputLine(this string text, Action<string> action)
    {
        foreach (var l in SplitByNewline(text))
        {
            action(l);
        }
    }
    
    public static void ForEach(this string str, Action<int> action)
    {
        for (var i = 0; i < str.Length; i++)
        {
            action(i);
        }
    }

    public static int ParseInt(this string s) => int.Parse(s);
    public static int ParseInt(this char c) => int.Parse(c.ToString());
    public static double ParseDouble(this string s) => double.Parse(s);
    public static double ParseLong(this string s) => long.Parse(s);

    // Stack
    public static void InsertToBottom<T>(this Stack<T> stack, T item)
    {
        var temp = new Stack<T>();

        while (stack.Count != 0)
        {
            temp.Push(stack.Peek());
            stack.Pop();
        }

        stack.Push(item);
        while (temp.Count != 0)
        {
            stack.Push(temp.Peek());
            temp.Pop();
        }
    }

    //ICollection
    public static bool HasNumberOfDistinct<T>(this ICollection<T> collection, int number) => collection.Distinct().Count() == number;
    
    //Array
    public static int? FindIndexOfItem(this string[] arr, string item) => Array.FindIndex(arr, i => i == item);
    public static int? FindIndexOfItem(this int[] arr, int item) => Array.FindIndex(arr, i => i == item);
    public static int? FindIndexOfItem(this double[] arr, double item) => Array.FindIndex(arr, i => i == item);
    
    //IEnumerable
    public static string AsString(this IEnumerable<char> arr) => new(arr.ToArray());
    
    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
    {
        if (sequence == null)
        {
            yield break;
        }

        var list = sequence.ToList();

        if (!list.Any())
        {
            yield return Enumerable.Empty<T>();
        }
        else
        {
            var startingElementIndex = 0;
            foreach (var startingElement in list)
            {
                var remainingItems = list.Where((e, i) => i != startingElementIndex);

                foreach (var permutationOfRemainder in remainingItems.Permute())
                {
                    yield return startingElement.Concat(permutationOfRemainder);
                }

                startingElementIndex++;
            }
        }
    }

    private static IEnumerable<T> Concat<T>(this T firstElement, IEnumerable<T> secondSequence)
    {
        yield return firstElement;
        if (secondSequence == null)
        {
            yield break;
        }

        foreach (var item in secondSequence)
        {
            yield return item;
        }
    }
    
    public static IEnumerable<IEnumerable<T>> Slice<T>(this IEnumerable<T> source, int sliceLen) =>
        source.Where((_, i) => i % sliceLen == 0).Select((_, i) => source.Skip(i * sliceLen).Take(sliceLen));

    //Numeric
    public static IEnumerable<int> GetDigits(this int number)
    {
        var individualFactor = 0;
        var tennerFactor = Convert.ToInt32(Math.Pow(10, number.ToString().Length));
        do
        {
            number -= tennerFactor * individualFactor;
            tennerFactor /= 10;
            individualFactor = number / tennerFactor;

            yield return individualFactor;
        } while (tennerFactor > 1);
    }
    
    public static int ReplaceNthDigit(this int value, int digitPosition, int digitToReplace, bool fromLeft = true)
    {
        if (fromLeft) {
            digitPosition = 1 + (int)Math.Ceiling(Math.Log10(value)) - digitPosition;
        }

        var divisor = (int)Math.Pow(10, digitPosition - 1);
        var quotient = value / divisor;
        var remainder = value % divisor;
        var digitAtPosition = quotient % 10;
        return (quotient - digitAtPosition + digitToReplace) * divisor + remainder;
    }

    public static int GetNthDigit(this int value, int n) => (value / (int)Math.Pow(10,n-1)) % 10;
    public static double GetNthDigit(this double value, int n) => (value / (int)Math.Pow(10,n-1)) % 10;
}
