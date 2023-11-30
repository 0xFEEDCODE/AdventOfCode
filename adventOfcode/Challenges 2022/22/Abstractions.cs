using System.Diagnostics;

using static Challenges2022.Solution15;

namespace Challenges2022; 

public partial class Solution22 {
    
    public enum Rotation { Right, Left };
    public enum Direction { Up, Right, Down, Left };
    public enum Tile { Wall, Open, BoundaryWall, Player }

    public record struct DiceFace(int Number, int StartRow, int StartCol, int Len = 50) {
        public int EndRow => StartRow + Len;
        public int EndCol => StartCol + Len;
        public int RotationFromFace1 { get; set; } = 0;
        public int RotationFromFace2 { get; set; } = 0;
        public int RotationFromFace3 { get; set; } = 0;
        public int RotationFromFace4 { get; set; } = 0;
        public int RotationFromFace5 { get; set; } = 0;
        public int RotationFromFace6 { get; set; } = 0;
    };

    public record struct Faces(DiceFace Face1, DiceFace Face2, DiceFace Face3, DiceFace Face4, DiceFace Face5, DiceFace Face6) {
        public List<DiceFace> AllFaces => new() { Face1, Face6, Face4, Face5, Face3, Face2 };

        public DiceFace PreviousFace = Face1;
        public DiceFace CurrentFace = Face1;

        public void ReverseWrap() => CurrentFace = PreviousFace;

        public void UpdateFace(Coord pos) => CurrentFace = AllFaces.Single(x => x.StartRow <= pos.Row && x.EndRow > pos.Row && x.StartCol <= pos.Col && x.EndCol > pos.Col);

        public (Coord, Direction) WrapAround(Coord pos, Direction dir) {
            PreviousFace = CurrentFace;
            CurrentFace = (CurrentFace.Number, dir) switch {
                (1, Direction.Right) => Face6, (1, Direction.Left) => Face3, (1, Direction.Up) => Face2, (1, Direction.Down) => Face4,
                (5, Direction.Right) => Face6, (5, Direction.Left) => Face3, (5, Direction.Up) => Face4, (5, Direction.Down) => Face2,
                
                (2, Direction.Right) => Face5, (2, Direction.Left) => Face1, (2, Direction.Up) => Face3, (2, Direction.Down) => Face6,
                (4, Direction.Right) => Face6, (4, Direction.Left) => Face3, (4, Direction.Up) => Face1, (4, Direction.Down) => Face5,
                
                (3, Direction.Right) => Face5, (3, Direction.Left) => Face1, (3, Direction.Up) => Face4, (3, Direction.Down) => Face2,
                (6, Direction.Right) => Face5, (6, Direction.Left) => Face1, (6, Direction.Up) => Face2, (6, Direction.Down) => Face4,
                _ => throw new ArgumentOutOfRangeException()
            };

            var atCol = (pos.Col - PreviousFace.StartCol);
            var atRow = (pos.Row - PreviousFace.StartRow);
            if (pos.Col == 50 && pos.Row == 175) {
                Console.WriteLine();
            }

            var updatedPos = GetUpdatedPosForRotation(dir, atRow, atCol);

            return (new Coord(updatedPos.Item1, updatedPos.Item2), updatedPos.Item3);
        }

        private (int, int, Direction) GetUpdatedPosForRotation(Direction dir, int atRow, int atCol) {
            switch (CurrentFace.Number) {
                case 1 when PreviousFace.Number == 2:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace2, dir);
                case 1 when PreviousFace.Number == 3:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace3, dir);
                case 2 when PreviousFace.Number == 1:
                    return  GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace1, dir);
                case 2 when PreviousFace.Number == 5:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace5, dir);
                case 3 when PreviousFace.Number == 1:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace1, dir);
                case 3 when PreviousFace.Number == 4:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace4, dir);
                case 4 when PreviousFace.Number == 3:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace3, dir);
                case 4 when PreviousFace.Number == 6:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace6, dir);
                case 5 when PreviousFace.Number == 2:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace2, dir);
                case 5 when PreviousFace.Number == 6:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace6, dir);
                case 6 when PreviousFace.Number == 4:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace4, dir);
                case 6 when PreviousFace.Number == 5:
                    return GetPosForRotationBasedOnDir(atRow, atCol, CurrentFace.RotationFromFace5, dir);
                default:
                    return GetPosWithoutRotation(atRow, atCol, dir);
            }
        }

        private (int, int, Direction) GetPosWithoutRotation(int atRow, int atCol, Direction dir) =>
            dir switch {
                Direction.Down => (CurrentFace.StartRow, CurrentFace.StartCol + atCol, Direction.Down),
                Direction.Right => (CurrentFace.StartRow + atRow, CurrentFace.StartCol, Direction.Right),
                Direction.Left => (CurrentFace.StartRow + atRow, CurrentFace.EndCol - 1, Direction.Left),
                Direction.Up => (CurrentFace.EndRow - 1, CurrentFace.StartCol + atCol, Direction.Up),
            };

        private (int, int, Direction) GetPosForRotationBasedOnDir(int atRow, int atCol, int rotation, Direction dir) =>
            dir switch {
                Direction.Down => GetPosForDownRotations(atRow, atCol, rotation),
                Direction.Right => GetPosForRightRotations(atRow, atCol, rotation),
                Direction.Left => GetPosForLeftRotations(atRow, atCol, rotation),
                Direction.Up => GetPosForUpRotations(atCol, atCol, rotation),
            };

        private (int, int, Direction) GetPosForDownRotations(int atRow, int atCol, int rotation) {
            var down_rot3_row = CurrentFace.StartRow + atCol;
            var down_rot3_col = CurrentFace.EndCol - 1;
            return rotation switch {
                3 => (down_rot3_row, down_rot3_col, Direction.Left),
                _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
            };
        }
        private (int, int, Direction) GetPosForLeftRotations(int atRow, int atCol, int rotation) {
            var left_rot1_row = CurrentFace.StartRow;
            var left_rot1_col = CurrentFace.StartCol + atRow;
            var left_rot2_row = CurrentFace.EndRow - 1 - atRow;
            var left_rot2_col = CurrentFace.StartCol;
            return rotation switch {
                1 => (left_rot1_row, left_rot1_col, Direction.Down),
                2 => (left_rot2_row, left_rot2_col, Direction.Right),
                _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
            };
        }
        private (int, int, Direction) GetPosForUpRotations(int atRow, int atCol, int rotation) {
            var up_rot3_row = CurrentFace.StartRow + atCol;
            var up_rot3_col = CurrentFace.StartCol;
            return rotation switch {
                3 => (up_rot3_row, up_rot3_col, Direction.Right),
                _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
            };
        }
        private (int, int, Direction) GetPosForRightRotations(int atRow, int atCol, int rotation) {
            var right_rot1_row = CurrentFace.EndRow - 1;
            var right_rot1_col = CurrentFace.StartCol + atRow;
            var right_rot2_row = CurrentFace.EndRow - 1 - atRow;
            var right_rot2_col = CurrentFace.EndCol - 1;
            return rotation switch {
                1 => (right_rot1_row, right_rot1_col, Direction.Up),
                2 => (right_rot2_row, right_rot2_col, Direction.Left),
                _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
            };
        }
    }
    
            /*
            var comingFrom = (CurrentFace.Number, PreviousFace.Number) switch {
                (1, 3) => Direction.Left, (1, 6) => Direction.Right, (1, 4) => Direction.Down, (1, 2) => Direction.Up,
                (2, 6) => Direction.Left, (2, 3) => Direction.Right, (2, 1) => Direction.Down, (2, 5) => Direction.Up,
                (3, 2) => Direction.Left, (3, 4) => Direction.Right, (3, 5) => Direction.Down, (3, 1) => Direction.Up,
                (4, 3) => Direction.Left, (4, 6) => Direction.Right, (4, 5) => Direction.Down, (4, 1) => Direction.Up,
                (5, 3) => Direction.Left, (5, 6) => Direction.Right, (5, 2) => Direction.Down, (5, 4) => Direction.Up,
                (6, 4) => Direction.Left, (6, 2) => Direction.Right, (6, 5) => Direction.Down, (6, 1) => Direction.Up,
            };
            
            var newPos = comingFrom switch {
                Direction.Up => (CurrentFace.StartRow, (CurrentFace.EndCol - 1-(pos.Col - PreviousFace.StartCol)) - 1),
                Direction.Down => (CurrentFace.EndRow - 1-1, (CurrentFace.StartCol+pos.Col - PreviousFace.StartCol)),
                Direction.Right => (CurrentFace.StartRow + pos.Row - PreviousFace.StartRow, CurrentFace.EndCol - 1-1),
                Direction.Left => (CurrentFace.StartRow + pos.Row - PreviousFace.StartRow, (CurrentFace.EndCol - 1-(pos.Row - PreviousFace.StartRow)) - 1),
            };
            */
            
            /*
            PreviousFace = CurrentFace;
            CurrentFace = (CurrentFace.Number, dir) switch {
                (1, Direction.Right) => Face6, (1, Direction.Left) => Face3, (1, Direction.Up) => Face2, (1, Direction.Down) => Face4,
                (5, Direction.Right) => Face6, (5, Direction.Left) => Face3, (5, Direction.Up) => Face4, (5, Direction.Down) => Face2,
                
                (2, Direction.Right) => Face5, (2, Direction.Left) => Face1, (2, Direction.Up) => Face3, (2, Direction.Down) => Face6,
                (4, Direction.Right) => Face6, (4, Direction.Left) => Face3, (4, Direction.Up) => Face1, (4, Direction.Down) => Face5,
                
                (3, Direction.Right) => Face5, (3, Direction.Left) => Face1, (3, Direction.Up) => Face4, (3, Direction.Down) => Face2,
                (6, Direction.Right) => Face5, (6, Direction.Left) => Face1, (6, Direction.Up) => Face2, (6, Direction.Down) => Face4,
            };
            var row = CurrentFace.StartRow;
            var col = CurrentFace.StartCol;
            
            var row_ec_sc = CurrentFace.StartRow + PreviousFace.StartRow - pos.Row;
            var row_sc_invSc = CurrentFace.EndRow - 1 - 1 -  pos.Row - PreviousFace.StartRow;
            var row_sr_sc = CurrentFace.StartRow;
            var row_er_sr = CurrentFace.EndRow - 1-1;
            var col_ec_sc = CurrentFace.EndCol - 1 - 1;
            var col_sc_invSc = CurrentFace.StartCol;
            var col_sr_sc = CurrentFace.EndRow - 1 - 1 - pos.Row - PreviousFace.StartRow;
            var col_er_sr = CurrentFace.StartCol + pos.Col - PreviousFace.StartCol;
            
            var row_sc_sr = CurrentFace.StartRow + pos.Col - PreviousFace.StartCol;
            var row_er_invEr = CurrentFace.EndRow - 1 - 1 - pos.Col - PreviousFace.StartCol;
            var row_sr_er = CurrentFace.StartRow;
            var col_sc_sr = CurrentFace.StartCol;
            var col_er_invEr = CurrentFace.EndCol - 1 - 1;
            var col_sr_er = CurrentFace.StartCol + pos.Col - PreviousFace.StartCol;
            
            var row_er_sc = CurrentFace.StartRow + pos.Row - PreviousFace.StartRow;
            var row_sr_sc45 = CurrentFace.StartRow;
            var col_sr_sc45 = CurrentFace.StartRow + pos.Col - PreviousFace.StartCol;
            var col_er_sc = CurrentFace.EndRow - 1 - 1;
            
            var row_sc_sr45 = CurrentFace.StartRow + pos.Col - PreviousFace.StartCol;
            var col_sc_sr45 = CurrentFace.StartCol;

            var row_sc_er = CurrentFace.StartRow + pos.Row - PreviousFace.StartRow;
            var row_er_invEc = CurrentFace.EndRow - 1 - 1 - pos.Row - PreviousFace.StartRow;
            var row_er_er45 = CurrentFace.EndRow - 1 - 1;
            var col_sc_er = CurrentFace.StartRow;
            var col_er_invEc = CurrentFace.EndRow - 1 - 1;
            var col_er_er45 = CurrentFace.StartCol + pos.Row - PreviousFace.StartRow;

           var row_sc_ec = CurrentFace.StartRow + pos.Row - PreviousFace.StartRow;
           var row_ec_invEr = CurrentFace.EndRow - 1 - 1 - pos.Row - PreviousFace.StartRow;
           var col_sc_ec = CurrentFace.StartCol;
           var col_ec_invEr = CurrentFace.EndCol - 1-1;
            
            
            if (CurrentFace.Number == 1) {
                row = PreviousFace.Number switch {
                    6 => row_ec_sc, 4 => row_er_sr, 3 => row_sc_invSc, 2 => row_sr_sc,
                };
                col = PreviousFace.Number switch {
                    6 => col_ec_sc, 4 => col_er_sr, 3 => col_sc_invSc, 2 => col_sr_sc,
                };
            }
            else if (CurrentFace.Number == 2) {
                row = PreviousFace.Number switch {
                    1 => row_sc_sr, 6 => row_er_sr, 5 => row_er_invEr, 3 => row_sr_er,
                };
                col = PreviousFace.Number switch {
                    1 => col_sc_sr, 6 => col_er_sr, 5 => col_er_invEr, 3 => col_sr_er,
                };
            }
            else if (CurrentFace.Number == 3) {
                row = PreviousFace.Number switch {
                    1 => row_sr_sc45, 5 => row_er_sc, 2 => row_er_sr, 4 => row_sr_sc,
                };
                col = PreviousFace.Number switch {
                    1 => col_sr_sc45, 5 => col_er_sc, 2 => col_er_sr, 4 => col_sc_invSc,
                };
            }
            else if (CurrentFace.Number == 4) {
                row = PreviousFace.Number switch {
                    1 => row_sr_er, 5 => row_er_sr, 3 => row_sc_sr45, 4 => row_er_invEr,
                };
                col = PreviousFace.Number switch {
                    1 => col_sr_er, 5 => col_er_sr, 3 => col_sc_sr45, 4 => col_er_invEr,
                };
            }
            else if (CurrentFace.Number == 5) {
                row = PreviousFace.Number switch {
                    4 => row_sr_er, 3 => row_sc_er, 6 => row_er_invEc, 2 => row_er_er45,
                };
                col = PreviousFace.Number switch {
                    4 => col_sr_er, 3 => col_sc_er, 6 => col_er_invEc, 2 => col_er_er45,
                };
            }
            else if (CurrentFace.Number == 6) {
                row = PreviousFace.Number switch {
                    1 => row_sc_ec, 5 => row_ec_invEr, 2 => row_sr_er, 4 => row_er_er45,
                };
                col = PreviousFace.Number switch {
                    1 => col_sc_ec, 5 => col_ec_invEr, 2 => col_sr_er, 4 => col_er_er45,
                };
            }
            */
            

    public record struct Instruction(Rotation? Rotation = null, int? Steps = null) {
        public bool IsRotation => Rotation.HasValue;
        public bool IsSteps => Steps.HasValue;
        public Rotation GetRotation => Rotation!.Value;
        public int GetSteps => Steps!.Value;
    }

}
