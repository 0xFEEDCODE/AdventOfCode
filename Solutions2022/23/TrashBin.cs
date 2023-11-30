using Framework;

using static Challenges2022.Solution15;

namespace Challenges2022;

public partial class Solution23 {
    public (Tile[][], List<Elf>) ParseInput() {
        var H = 5000;
        var W = 5000;
        var grid = (H, W).CreateGrid<Tile>();
        grid.SetAllCellsToValue(Tile.Empty);
        
        var startOffsetI = H / 2;
        var startOffsetJ = W / 2;

        var elves = new List<Elf>() { };

        var i = 0;
        var id = 1;
        foreach (var line in RawInputSplitByNl) {
            for (var j = 0; j < line.Length; j++) {
                if (line[j] == '#') {
                    grid[i + startOffsetI][j + startOffsetJ] = Tile.Elf;
                    elves.Add(new Elf(new Coord(i+startOffsetI, j+startOffsetJ), id++));
                }
            }
            i++;
        }
        
        return (grid, elves);
    }
}
