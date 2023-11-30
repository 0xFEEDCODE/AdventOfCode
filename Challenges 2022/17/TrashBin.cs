namespace Challenges2022; 

public partial class Solution17 {
    
    private (List<Direction>, Dictionary<char, List<Solution15.Coord>>) ParseInput() {
        var pushes = new List<Direction>();
        foreach (var ch in RawInputSplitByNl.SelectMany(x => x)) {
            if (ch == '<')
                pushes.Add(Direction.Left);
            if (ch == '>')
                pushes.Add(Direction.Right);
        }

        var shapes = new Dictionary<char, List<Solution15.Coord>>() {
            { flatShape, new List<Solution15.Coord>() { new(0, 0), new(0, 1), new(0, 2), new(0, 3) } },
            { plusShape, new List<Solution15.Coord>() { new(0, 1), new(1, 0), new(1, 1), new(1, 2), new(2, 1) } },
            { lShape, new List<Solution15.Coord>() { new(2, 2), new(1, 2), new(0, 2), new(0, 1), new(0, 0) } },
            { iShape, new List<Solution15.Coord>() { new(0, 0), new(1, 0), new(2, 0), new(3, 0) } },
            { boxShape, new List<Solution15.Coord>() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
        };
        return (pushes, shapes);
    }
}
