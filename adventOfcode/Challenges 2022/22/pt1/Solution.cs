using System.Text;

using Challenges2022;

using Framework;

namespace Archive;

public static partial class Ext
{
}

public partial class Solution22 : SolutionFramework {
    public Solution22() : base(22) {
    }

    public override string[] Solve() {
        var (grid, instructions) = ParseInput();

        var pos = new Solution15.Coord(0,0);
        var dir = Direction.Right;
        SetStartPos(grid, pos);
        grid[pos.Row][pos.Col] = Tile.Player;
        
        foreach (var instr in instructions) {
            if (instr.IsSteps) {
                for (var i = 0; i < instr.Steps; i++) {
                    if (dir == Direction.Right) {
                        if (!TryStepRight(grid, pos)) {
                            break;
                        }
                    }
                    else if (dir == Direction.Left) {
                        if (!TryStepLeft(grid, pos)) {
                            break;
                        }
                    }
                    else if (dir == Direction.Up) {
                        if (!TryStepUp(grid, pos)) {
                            break;
                        }
                    }
                    else if (dir == Direction.Down) {
                        if (!TryStepDown(grid, pos)) {
                            break;
                        }
                    }
                }
            }
            else if (instr.IsRotation) {
                if (instr.Rotation == Rotation.Left) 
                    dir = RotateLeft(dir);
                else if (instr.Rotation == Rotation.Right)
                    dir = RotateRight(dir);
            }
            PrintGrid(grid);
            
        }

        var facingValue = dir switch {
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

    static bool TryStepLeft(Tile[][] grid, Solution15.Coord pos) {
        switch (grid[pos.Row][pos.Col-1]) {
            case Tile.Player:
            case Tile.Open:
                pos.Col--;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                for (var temp = pos.Col; temp <= grid[pos.Row].Length-1; temp++) {
                    if (temp == grid[pos.Row].Length-1 || grid[pos.Row][temp + 1] == Tile.BoundaryWall) {
                        if (grid[pos.Row][temp] != Tile.Wall) {
                            pos.Col = temp;
                            break;
                        } 
                        return false;
                    }
                }
                break;
        }
        
        grid[pos.Row][pos.Col] = Tile.Player;

        return true;
    }

    static bool TryStepUp(Tile[][] grid, Solution15.Coord pos) {
        switch (grid[pos.Row - 1][pos.Col]) {
            case Tile.Player:
            case Tile.Open:
                pos.Row--;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                for (var temp = pos.Row; temp <= grid.Length-1; temp++) {
                    if (temp == grid.Length-1 || grid[temp + 1][pos.Col] == Tile.BoundaryWall) {
                        if (grid[temp][pos.Col] != Tile.Wall) {
                            pos.Row = temp;
                            break;
                        } 
                        return false;
                    }
                }
                break;
        }
        
        grid[pos.Row][pos.Col] = Tile.Player;
        
        return true;
    }

    static bool TryStepDown(Tile[][] grid, Solution15.Coord pos) {
        switch (grid[pos.Row+1][pos.Col]) {
            case Tile.Player:
            case Tile.Open:
                pos.Row++;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall:
                for (var temp = pos.Row; temp >= 0; temp--) {
                    if (temp == 0 || grid[temp-1][pos.Col] == Tile.BoundaryWall) {
                        if (grid[temp][pos.Col] != Tile.Wall) {
                            pos.Row = temp;
                            break;
                        } 
                        return false;
                    }
                }
                break;
        }
        
        grid[pos.Row][pos.Col] = Tile.Player;

        return true;
    }

    static bool TryStepRight(Tile[][] grid, Solution15.Coord pos) {
        switch (grid[pos.Row][pos.Col+1]) {
            case Tile.Player:
            case Tile.Open:
                pos.Col++;
                break;
            case Tile.Wall:
                return false;
            case Tile.BoundaryWall: 
                for (var temp = pos.Col; temp >= 0; temp--) {
                    if (temp == 0 || grid[pos.Row][temp - 1] == Tile.BoundaryWall) {
                        if (grid[pos.Row][temp] != Tile.Wall) {
                            pos.Col = temp;
                            break;
                        } 
                        return false;
                    }
                }
                break;
        }

        grid[pos.Row][pos.Col] = Tile.Player;

        return true;
    }
}
