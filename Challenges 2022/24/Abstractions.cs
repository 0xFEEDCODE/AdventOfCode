using Framework;

using static Challenges2022.Solution15;
using static Challenges2022.Solution24;

namespace Challenges2022;

public static partial class Ext {
    public static Tile NeighborUp(this Coord tileObject, Tile[][] grid) => grid.NeighborCellUp(tileObject.Row, tileObject.Col);
    public static Tile NeighborDown(this Coord tileObject, Tile[][] grid) => grid.NeighborCellDown(tileObject.Row, tileObject.Col);
    public static Tile NeighborRight(this Coord tileObject, Tile[][] grid) => grid.NeighborCellRight(tileObject.Row, tileObject.Col);
    public static Tile NeighborLeft(this Coord tileObject, Tile[][] grid) => grid.NeighborCellLeft(tileObject.Row, tileObject.Col);

    public static bool IsBlizzard(this Tile tile) =>
        tile switch {
            Tile.BL or Tile.BU or Tile.BD or Tile.BR => true,
            _ => false
        };
        

    public static char GetTileRepresentation(this Tile tile) =>
        tile switch {
            Tile.Empty => '.',
            Tile.Player => 'E',
            Tile.Wall => '#',
            Tile.BL => '<',
            Tile.BD => 'v',
            Tile.BR => '>',
            Tile.BU => '^',
            _ => throw new ArgumentOutOfRangeException(nameof(tile), tile, null)
        };
    public static Direction GetBlizzardDirection(this Tile tile) =>
        tile switch {
            Tile.BL => Direction.Left,
            Tile.BD => Direction.Down,
            Tile.BR => Direction.Right,
            Tile.BU => Direction.Up,
            _ => throw new ArgumentOutOfRangeException(nameof(tile), tile, null)
        };

    public static (int, int) ExitCoordCached = (-1, -1);

    public static (int, int) ExitCoord(this Tile[][] grid) {
        if (ExitCoordCached == (-1, -1)) {
            ExitCoordCached = (grid.Length - 1, grid[0].Length - 2);
        }

        return ExitCoordCached;
    }
}

public partial class Solution24 {
    public enum Direction { Up, Down, Right, Left }
    
    public class Node 
    {
        public (int, int) Coords;
        public int Row => Coords.Item1;
        public int Col => Coords.Item2;
        public int F;
        public int G;
        public int Time;
        public Node? Prev;
    }

    public class TileObject {
        public TileObject(Coord coord) {
            Coord = coord;
        }
        public Coord Coord;
    }
    
    public class Player : TileObject {
        public Player(Coord coord) : base(coord){ }
    }

    public class Blizzard : TileObject {
        public Direction Direction;
        
        public Blizzard(Coord coord, Direction direction) : base(coord) {
            Direction = direction;
        }

        public Blizzard Copy() => new(Coord.Copy(), Direction);

        public Tile GetTile() =>
            Direction switch {
                Direction.Up => Tile.BU,
                Direction.Down => Tile.BD,
                Direction.Left => Tile.BL,
                Direction.Right => Tile.BR
            };
    }
    
    public enum Tile {
        Empty, BU, BD, BR, BL, Player, Wall
    }

    public static void MoveBlizzards(Tile[][] grid, List<Blizzard> blizzards) {
        foreach (var bl in blizzards) {
            grid[bl.Coord.Row][bl.Coord.Col] = Tile.Empty;
            switch (bl.Direction) {
                case Direction.Up:
                    var nbUp = bl.Coord.NeighborUp(grid);
                    if (nbUp is Tile.Empty || nbUp.IsBlizzard()) {
                        bl.Coord.Row--;
                    }
                    else if (nbUp == Tile.Wall) {
                        bl.Coord.Row = grid.Length-2;
                    }
                    grid[bl.Coord.Row][bl.Coord.Col] = Tile.BU;
                    break;
                case Direction.Down:
                    var nbDown = bl.Coord.NeighborDown(grid);
                    if (nbDown == Tile.Empty || nbDown.IsBlizzard()) {
                        bl.Coord.Row++;
                    }
                    else if (nbDown == Tile.Wall) {
                        bl.Coord.Row = 1;
                    }
                    grid[bl.Coord.Row][bl.Coord.Col] = Tile.BD;
                    break;
                case Direction.Right:
                    var nbRight = bl.Coord.NeighborRight(grid);
                    if (nbRight == Tile.Empty || nbRight.IsBlizzard()) {
                        bl.Coord.Col++;
                    }
                    else if (nbRight == Tile.Wall) {
                        bl.Coord.Col = 1;
                    }
                    grid[bl.Coord.Row][bl.Coord.Col] = Tile.BR;
                    break;
                case Direction.Left:
                    var nbLeft = bl.Coord.NeighborLeft(grid);
                    if (nbLeft == Tile.Empty || nbLeft.IsBlizzard()) {
                        bl.Coord.Col--;
                    }
                    else if (nbLeft == Tile.Wall) {
                        bl.Coord.Col = grid[0].Length-2;
                    }
                    grid[bl.Coord.Row][bl.Coord.Col] = Tile.BL;
                    break;
            }
        }
    }
}
