using System.Numerics;

using Framework;

namespace Challenges2020;

public class Solution11 : SolutionFramework
{
    public Solution11() : base(11) { }


    public enum SeatLayoutCell
    {
        Occupied,
        Empty,
        Floor
    }

    public override string[] Solve()
    {
        var originalSeating = (InputNlSplit.Length, InputNlSplit.First().Length).CreateGrid<SeatLayoutCell>();
        var row = 0;
        foreach (var line in InputNlSplit)
        {
            var col = 0;
            foreach (var item in line)
            {
                originalSeating[row][col] = item switch
                {
                    'L' => SeatLayoutCell.Empty,
                    '#' => SeatLayoutCell.Occupied,
                    '.' => SeatLayoutCell.Floor
                };
                col++;
            }
            row++;
        }

        var stabilized = false;
        var lastSeating = originalSeating;
        while (!stabilized)
        {
            stabilized = true;
            var newSeating = (InputNlSplit.Length, InputNlSplit.First().Length).CreateGrid<SeatLayoutCell>();
            newSeating.ForEachCell((i, j) =>
            {
                if (lastSeating[i][j] == SeatLayoutCell.Floor)
                {
                    newSeating[i][j] = SeatLayoutCell.Floor;
                } 
                else
                {
                    var occupiedAdjacentSeats = 0;
                    try { if (lastSeating.NeighborCellDown(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellLeft(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellRight(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellUp(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellBottomLeft(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellBottomRight(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellTopLeft(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }
                    try { if (lastSeating.NeighborCellTopRight(i, j) == SeatLayoutCell.Occupied) occupiedAdjacentSeats++; } catch { }

                    newSeating[i][j] = lastSeating[i][j] switch
                    {
                        SeatLayoutCell.Empty => occupiedAdjacentSeats == 0 ? SeatLayoutCell.Occupied : SeatLayoutCell.Empty,
                        SeatLayoutCell.Occupied => occupiedAdjacentSeats >= 4 ? SeatLayoutCell.Empty : SeatLayoutCell.Occupied
                    };
                }

                if (lastSeating[i][j] != newSeating[i][j])
                {
                    stabilized = false;
                }
            });
            lastSeating = newSeating;
        }

        var occupiedSeats = 0;
        lastSeating.ForEachCell((i, j) =>
        {
            if (lastSeating[i][j] == SeatLayoutCell.Occupied)
            {
                occupiedSeats++;
            }
        });
        
        AssignAnswer1(occupiedSeats);
        
        stabilized = false;
        lastSeating = originalSeating;
        while (!stabilized)
        {
            stabilized = true;
            var newSeating = (InputNlSplit.Length, InputNlSplit.First().Length).CreateGrid<SeatLayoutCell>();
            newSeating.ForEachCell((i, j) =>
            {
                if (lastSeating[i][j] == SeatLayoutCell.Floor)
                {
                    newSeating[i][j] = SeatLayoutCell.Floor;
                } 
                else
                {
                    var occupiedSeatsInDirections = 0;

                    bool isSeatInDirectionOccupied(int startI, int startJ, int iIncr, int jIncr)
                    {
                        var _i = startI+iIncr;
                        var _j = startJ+jIncr;
                        while (_i >= 0 && _j >= 0 && _i < lastSeating.Length && _j < lastSeating.First().Length)
                        {
                            switch (lastSeating[_i][_j])
                            {
                                case SeatLayoutCell.Empty:
                                    return false;
                                case SeatLayoutCell.Occupied:
                                    return true;
                                default:
                                    _i += iIncr;
                                    _j += jIncr;
                                    break;
                            }
                        }
                        return false;
                    }

                    var directionIncrements = new[] { (0, 1), (0, -1), (1, 0), (-1, 0), (1, 1), (-1, -1), (1, -1), (-1, 1) };
                    occupiedSeatsInDirections = directionIncrements.Count(x => isSeatInDirectionOccupied(i, j, x.Item1, x.Item2));

                    newSeating[i][j] = lastSeating[i][j] switch
                    {
                        SeatLayoutCell.Empty => occupiedSeatsInDirections == 0 ? SeatLayoutCell.Occupied : SeatLayoutCell.Empty,
                        SeatLayoutCell.Occupied => occupiedSeatsInDirections >= 5 ? SeatLayoutCell.Empty : SeatLayoutCell.Occupied
                    };
                }

                if (lastSeating[i][j] != newSeating[i][j])
                {
                    stabilized = false;
                }
            });
            lastSeating = newSeating;
        }
        
        occupiedSeats = 0;
        lastSeating.ForEachCell((i, j) =>
        {
            if (lastSeating[i][j] == SeatLayoutCell.Occupied)
            {
                occupiedSeats++;
            }
        });
        
        AssignAnswer2(occupiedSeats);

        return Answers;
    }
}
