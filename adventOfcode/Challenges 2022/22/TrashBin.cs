using System.Text;

using Framework;

namespace Challenges2022;

public partial class Solution22 {
    private (Tile[][], List<Instruction>, Faces) ParseInput() {
        var colL = "                                                  ...........#....#.........##............###.......#..............#..........#.#......#..............".Length + 50;
        var rowL = 202;
        var grid = (rowL, colL).CreateGrid<Tile>();
        grid.SetAllCellsToValue(Tile.BoundaryWall);
        var instructions = new List<Instruction>();

        var startRow = -1;
        var startCol = -1;

        var instructionsPart = false;
        var numSb = new StringBuilder();
        var i = 1;
        foreach (var line in RawInputSplitByNl) {
            if (!instructionsPart && (line.Contains('.') || line.Contains("#"))) {
                var j = 1;
                foreach (var ch in line) {
                    grid[i][j++] = ch switch {
                        ' ' => Tile.BoundaryWall,
                        '.' => Tile.Open,
                        '#' => Tile.Wall,
                        _ => throw new InvalidOperationException()
                    };
                    if (startRow == -1 && startCol == -1 && grid[i][j - 1] == Tile.Open) {
                        startRow = i;
                        startCol = j - 1;
                    }
                }
            }

            if (instructionsPart) {
                foreach (var ch in line) {
                    if (char.IsDigit(ch)) {
                        numSb.Append(ch);
                    } else if (char.IsLetter(ch)) {
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
            } else if (line.Trim().Length == 0) {
                instructionsPart = true;
            }

            i++;
        }

        if (numSb.Length > 0) {
            instructions.Add(new Instruction(Steps: int.Parse(numSb.ToString())));
            numSb.Clear();
        }

        /*
        var face1 = new DiceFace(1, startRow, startCol, 4){RotationFromFace6 = 2};
        var face6 = new DiceFace(6, startRow + 8, startCol + 4, 4){RotationFromFace2 = 3, RotationFromFace1 = 2};

        var face4 = new DiceFace(4, startRow + 4, startCol, 4){RotationFromFace6 = 1};
        var face5 = new DiceFace(5, startRow + 8, startCol, 4){RotationFromFace3 = 1, RotationFromFace2 = 2};

        var face3 = new DiceFace(3, startRow + 4, startCol - 4, 4){RotationFromFace3 = 1, RotationFromFace1 = 1, RotationFromFace5 = 3};

        var face2 = new DiceFace(2, startRow+4, startCol-8, 4){RotationFromFace5 = 2, RotationFromFace1 = 2, RotationFromFace6 = 3};
        */
        var face1 = new DiceFace(1, startRow, startCol) { RotationFromFace2 = 1, RotationFromFace3 = 2 };
        var face6 = new DiceFace(6, startRow, startCol+50) { RotationFromFace4 = 1, RotationFromFace5 = 2};
        var face4 = new DiceFace(4, startRow+50, startCol) { RotationFromFace3 = 3, RotationFromFace6 = 3};
        var face5 = new DiceFace(5, startRow+100, startCol) {  RotationFromFace2 = 1, RotationFromFace6 = 2};
        var face3 = new DiceFace(3, startRow+100, startCol-50) { RotationFromFace1 = 2,  RotationFromFace4 = 1};
        var face2 = new DiceFace(2, startRow+150, startCol-50) { RotationFromFace1 = 3, RotationFromFace5 = 3};

        var faces = new Faces(face1, face2, face3, face4, face5, face6);

        return (grid, instructions, faces);
    }


    private static void PrintFace(Tile[][] grid, DiceFace face) {
        for (int i = face.StartRow; i < face.EndRow; i++) {
            Console.WriteLine();
            for (int j = face.StartCol; j < face.EndCol; j++) {
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
    
    private static void PrintGrid(Tile[][] grid) {
        return;
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
    
    /*
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
    */
    
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
