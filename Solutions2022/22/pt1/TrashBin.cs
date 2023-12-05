using System.Text;

using Challenges2022;

using Framework;

namespace Archive;

public partial class Solution22 {
    private (Tile[][], List<Instruction>) ParseInput() {
        var colL = "                                                  ...........#....#.........##............###.......#..............#..........#.#......#..............".Length + 50;
        var rowL = 500;
        var grid = (rowL, colL).CreateGrid<Tile>();
        grid.SetAllCellsToValue(Tile.BoundaryWall);
        var instructions = new List<Instruction>();
        
        var instructionsPart = false;
        var numSb = new StringBuilder();
        var i = 1;
        foreach (var line in InputNlSplit) {
            if (!instructionsPart && (line.Contains('.') || line.Contains("#"))) {
                var j = 1;
                foreach (var ch in line) {
                    grid[i][j++] = ch switch {
                        ' ' => Tile.BoundaryWall,
                        '.' => Tile.Open,
                        '#' => Tile.Wall,
                        _ => throw new InvalidOperationException()
                    };
                }
            }

            if (instructionsPart) {
                foreach (var ch in line) {
                    if (char.IsDigit(ch)) {
                        numSb.Append(ch);
                    }
                    else if (char.IsLetter(ch)) {
                        if (numSb.Length > 0) {
                            instructions.Add(new Instruction(Steps: int.Parse(numSb.ToString())));
                            numSb.Clear();
                        }

                        switch (ch) {
                            case 'L':
                                instructions.Add(new Instruction(Rotation.Left));
                                break;
                            case 'R':
                                instructions.Add(new Instruction(Rotation.Right));
                                break;
                        }
                    }
                }
            }
            else if (line.Trim().Length == 0) {
                instructionsPart = true;
            }

            i++;
        }
        if (numSb.Length > 0) {
            instructions.Add(new Instruction(Steps: int.Parse(numSb.ToString())));
            numSb.Clear();
        }

        return (grid, instructions);
    }
    
    private static void PrintGrid(Tile[][] grid) {
        for (int i = 0; i < grid.Length; i++) {
            Console.WriteLine();
            for (int j = 0; j < grid[i].Length; j++) {
                if (grid[i][j] == Tile.BoundaryWall) {
                    Console.Write(' ');
                }
                else if (grid[i][j] == Tile.Open) {
                    Console.Write('.');
                }
                else if (grid[i][j] == Tile.Wall) {
                    Console.Write('#');
                }
                else if (grid[i][j] == Tile.Player) {
                    Console.Write('X');
                }
            }
        }
    }
    
    private static void SetStartPos(Tile[][] grid, Solution15.Coord pos) {
        pos.Row = 1;
        var startPosFound = false;
        for (int j = 0; j < grid.First().Length; j++) {
            if (grid[pos.Row][j] == Tile.Open) {
                pos.Col = j;
                startPosFound = true;
                break;
            }
        }

        if (!startPosFound)
            throw new InvalidOperationException();
    }
}
