using System.Text;

using Framework;

namespace Challenges2022;

public static partial class Ext
{
}

public partial class Solution22 : SolutionFramework {
    public Solution22() : base(22) {
    }

    public override string[] Solve() {
        var (grid, instructions, faces) = ParseInput();

        var pos = new Solution15.Coord(0,0);
        Dir = Direction.Right;
        SetStartPos(grid, pos);
        grid[pos.Row][pos.Col] = Tile.Player;
        
        foreach (var instr in instructions) {
            if (instr.IsSteps) {
                for (var i = 0; i < instr.Steps; i++) {
                    if (Dir == Direction.Right) {
                        if (!TryStepRight(grid, pos, faces)) {
                            break;
                        }
                        faces.UpdateFace(pos);
                        PrintGrid(grid);
                    }
                    else if (Dir == Direction.Left) {
                        if (!TryStepLeft(grid, pos, faces)) {
                            break;
                        }
                        faces.UpdateFace(pos);
                        PrintGrid(grid);
                    }
                    else if (Dir == Direction.Up) {
                        if (!TryStepUp(grid, pos, faces)) {
                            break;
                        }
                        faces.UpdateFace(pos);
                        PrintGrid(grid);
                    }
                    else if (Dir == Direction.Down) {
                        if (!TryStepDown(grid, pos, faces)) {
                            break;
                        }
                        faces.UpdateFace(pos);
                        PrintGrid(grid);
                    }
                }
            }
            else if (instr.IsRotation) {
                if (instr.Rotation == Rotation.Left) 
                    Dir = RotateLeft(Dir);
                else if (instr.Rotation == Rotation.Right)
                    Dir = RotateRight(Dir);
            }
        }

        var facingValue = Dir switch {
            Direction.Right => 0,
            Direction.Down => 1,
            Direction.Left => 2,
            Direction.Up => 3
        };
        long ans = (1000 * pos.Row) + (4 * pos.Col) + facingValue;
        var p = pos;
        Console.WriteLine(ans);
    
        PrintGrid(grid);
        Console.WriteLine();
        
        return Answers;
    }

    private static Direction Dir;
    private static Direction RotateRight(Direction dir) {
        dir = dir switch {
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.Up => Direction.Right,
            _ => dir
        };
        return dir;
    }
    private static Direction RotateLeft(Direction dir) {
        dir = dir switch {
            Direction.Right => Direction.Up,
            Direction.Up => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Down => Direction.Right,
            _ => dir
        };
        return dir;
    }

    static bool TryStepLeft(Tile[][] grid, Solution15.Coord pos, Faces faces) {
        switch (grid[pos.Row][pos.Col-1]) {
            case Tile.Player:
            case Tile.Open:
                pos.Col--;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                var newPos = faces.WrapAround(pos, Direction.Left);
                if (grid[newPos.Item1.Row][newPos.Item1.Col] == Tile.Wall) {
                    faces.ReverseWrap();
                    return false;
                }

                pos.Row = newPos.Item1.Row;
                pos.Col = newPos.Item1.Col;
                Dir = newPos.Item2;
                grid[pos.Row][pos.Col] = Tile.Player;
                PrintGrid(grid);
                break;
        }
        
        grid[pos.Row][pos.Col] = Tile.Player;

        return true;
    }

    static bool TryStepUp(Tile[][] grid, Solution15.Coord pos, Faces faces) {
        switch (grid[pos.Row - 1][pos.Col]) {
            case Tile.Player:
            case Tile.Open:
                pos.Row--;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                var newPos = faces.WrapAround(pos, Direction.Up);
                if (grid[newPos.Item1.Row][newPos.Item1.Col] == Tile.Wall) {
                    faces.ReverseWrap();
                    return false;
                }

                pos.Row = newPos.Item1.Row;
                pos.Col = newPos.Item1.Col;
                Dir = newPos.Item2;
                grid[pos.Row][pos.Col] = Tile.Player;
                PrintGrid(grid);
                break;
        }
        
        grid[pos.Row][pos.Col] = Tile.Player;
        
        return true;
    }

    static bool TryStepDown(Tile[][] grid, Solution15.Coord pos, Faces faces) {
        switch (grid[pos.Row+1][pos.Col]) {
            case Tile.Player:
            case Tile.Open:
                pos.Row++;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                var newPos = faces.WrapAround(pos, Direction.Down);
                if (grid[newPos.Item1.Row][newPos.Item1.Col] == Tile.Wall) {
                    faces.ReverseWrap();
                    return false;
                }

                pos.Row = newPos.Item1.Row;
                pos.Col = newPos.Item1.Col;
                Dir = newPos.Item2;
                grid[pos.Row][pos.Col] = Tile.Player;
                PrintGrid(grid);
                break;
        }
        
        grid[pos.Row][pos.Col] = Tile.Player;

        return true;
    }

    static bool TryStepRight(Tile[][] grid, Solution15.Coord pos, Faces faces) {
        switch (grid[pos.Row][pos.Col+1]) {
            case Tile.Player:
            case Tile.Open:
                pos.Col++;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                var newPos = faces.WrapAround(pos, Direction.Right);
                if (grid[newPos.Item1.Row][newPos.Item1.Col] == Tile.Wall) {
                    faces.ReverseWrap();
                    return false;
                }

                pos.Row = newPos.Item1.Row;
                pos.Col = newPos.Item1.Col;
                Dir = newPos.Item2;
                grid[pos.Row][pos.Col] = Tile.Player;
                PrintGrid(grid);
                break;
        }

        grid[pos.Row][pos.Col] = Tile.Player;

        return true;
    }
}
