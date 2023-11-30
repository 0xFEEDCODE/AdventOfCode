using Framework;

using static Challenges2022.Solution15;

namespace Challenges2022;

public partial class Solution24 {
    (Tile[][], List<Blizzard>, Player) ParseInput() {
        //var grid = (6, 8).CreateGrid<Tile>();
        var grid = (22, 152).CreateGrid<Tile>();
        var i = 0;
        var blizzards = new List<Blizzard>() { };
        var player = new Player(new Coord(0, 1));
        foreach (var line in RawInputSplitByNl) {
            var j = 0;
            foreach (var ch in line) {
                var tile = ch switch {
                    '#' => Tile.Wall,
                    '.' => Tile.Empty,
                    '<' => Tile.BL,
                    'v' => Tile.BD,
                    '>' => Tile.BR,
                    '^' => Tile.BU
                };
                
                if (tile is Tile.BR or Tile.BL or Tile.BD or Tile.BU)
                    blizzards.Add(new Blizzard(new Coord(i, j), tile.GetBlizzardDirection()));
                else
                    grid[i][j] = tile;

                j++;
            }

            i++;
        }

        return (grid, blizzards, player);
    }
}
