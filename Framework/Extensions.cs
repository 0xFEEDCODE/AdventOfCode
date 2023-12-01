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

    //Clockwise diagonals
    public static T NeighborCellTopLeft<T>(this T[][] grid, int i, int j) => grid[i - 1][j - 1];
    public static T NeighborCellTopRight<T>(this T[][] grid, int i, int j) => grid[i - 1][j + 1];
    public static T NeighborCellBottomRight<T>(this T[][] grid, int i, int j) => grid[i + 1][j + 1];
    public static T NeighborCellBottomLeft<T>(this T[][] grid, int i, int j) => grid[i + 1][j - 1];

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
}
