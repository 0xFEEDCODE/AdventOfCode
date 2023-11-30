using System.Diagnostics.Tracing;

using Framework;

using static Challenges2022.Solution15;
using static Challenges2022.Solution23;

namespace Challenges2022;

public static partial class Ext
{
    public static bool AnyoneAround(this Tile[][] grid, Coord elfPos) =>
        grid.NeighborCellDown(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellLeft(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellUp(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellRight(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellTopLeft(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellTopRight(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellBottomLeft(elfPos.Row, elfPos.Col) == Tile.Elf ||
        grid.NeighborCellBottomRight(elfPos.Row, elfPos.Col) == Tile.Elf;

    public static void PrintGrid(this Tile[][] grid) {
        return;
        var offsetI = 2495;
        var offsetJ = 2495;
        Console.WriteLine();
        for (var i = offsetI; i < offsetI+20; i++) {
            Console.WriteLine();
            for (var j = offsetJ; j < offsetJ+25; j++) {
                if(grid[i][j] == Tile.Elf)
                    Console.Write("#");
                else
                    Console.Write(".");
            }
        }
    }
    public static void PrintGrid(this Tile[][] grid, List<Elf> elves) {
        var offsetI = 2495;
        var offsetJ = 2495;
        Console.WriteLine();
        for (var i = offsetI; i < offsetI+25; i++) {
            Console.WriteLine();
            for (var j = offsetJ; j < offsetJ+25; j++) {
                /*
                if(grid[i][j] == Tile.Elf)
                    Console.Write("#");
                */
                if(grid[i][j] == Tile.Elf)
                    Console.Write(elves.First(x=>x.Coord.Coords == (i,j)).Id);
                else
                    Console.Write(".");
            }
        }
    }
}

public partial class Solution23 : SolutionFramework {
    public Solution23() : base(23) { }


    public override string[] Solve() {
        var (grid, elves) = ParseInput();
        grid.ForEachCell((i, j) => {
            if (grid[i][j] == Tile.Elf) {
                Console.WriteLine();
            }
        });

        var shiftCounter = 0;
        var rounds = 0;
        while (true) {
            rounds++;
            
            var proposals = new Dictionary<Coord?, List<int>>();
            var priorities = new Dictionary<int, Queue<(Coord?, int)>>();
            
            foreach (var elf in elves) {
                if (!grid.AnyoneAround(elf.Coord)) continue;
                
                var pm = new PriorityManager();
                pm.ShiftBy(shiftCounter);
                if (shiftCounter == 4)
                    shiftCounter = 0;
                priorities.Add(elf.Id, new Queue<(Coord?, int)>());
                var prioNr = -1;
                while (pm.PrioritiesQueue.TryPeek(out _) && pm.DequeuePrioWithValidationOffsets() is { } prioWithOffset) {
                    prioNr++;
                    
                    var proposal = prioWithOffset.ValidationOffsets.Select(x => new Coord(elf.Coord.Row + x.Row, elf.Coord.Col + x.Col)).ToArray();
                    
                    //Another elf at validation position, this choice of priority is not valid
                    if (proposal.Any(p => elves.Any( e => p == e.Coord)))
                        continue;

                    var coordForPrio = new List<Coord>() {

                        new(elf.Coord.Row - 1, elf.Coord.Col),
                        new(elf.Coord.Row + 1, elf.Coord.Col),
                        new(elf.Coord.Row, elf.Coord.Col - 1),
                        new(elf.Coord.Row, elf.Coord.Col + 1),
                    };
                    var proposedPos = coordForPrio[(prioNr + shiftCounter)%4];

                    //Add to list of proposals, where we detect if there are any other elves attempting the same position
                    if (proposals.ContainsKey(proposedPos)) {
                        proposals[proposedPos].Add(elf.Id);
                    }
                    else
                        proposals.Add(proposedPos, new List<int> {elf.Id});
                    
                    //Add to priorities where we keep the mapping between proposal
                    priorities[elf.Id].Enqueue((proposedPos, prioNr));
                    break;
                }
            }

            ElvesMoved = 0;
            foreach (var elf in elves)
                Do(priorities, elf, proposals, grid);

            grid.PrintGrid();
            var a = 5;
            shiftCounter++;

            if (ElvesMoved == 0) {
                Console.WriteLine();
                Console.WriteLine(rounds);
                break;
            }
        }


        var empties = 0;
        var rg = GetRectangleRegion(grid);
        grid.ForEachCell((i, j) => {
            if (i >= rg.TopLeftEdge.Row && i <= rg.BotLeftEdge.Row) {
                if (j >= rg.TopLeftEdge.Col && j <= rg.TopRightEdge.Col) {
                    if (grid[i][j] == Tile.Empty) {
                        empties++;
                    }
                }
            }
        });
        Console.WriteLine(empties);
        
        
        return Answers;
    }

    public static int ElvesMoved = 0;

    private static void Do(Dictionary<int, Queue<(Coord?, int)>> priorities, Elf elf, Dictionary<Coord?, List<int>> proposals, Tile[][] grid) {
        Coord? validProposal = null;
        if(!priorities.ContainsKey(elf.Id))
            return;

        while (validProposal == null && priorities[elf.Id].Any()) {
            var proposal = priorities[elf.Id].Dequeue().Item1;
            if (proposals[proposal].Count == 1) {
                validProposal = proposal;
                proposals.Remove(proposal);
            } 
        }

        if (validProposal is null)
            return;

        ElvesMoved++;

        grid[elf.Coord.Row][elf.Coord.Col] = Tile.Empty;
        elf.Coord = new Coord(validProposal.Row, validProposal.Col);
        grid[elf.Coord.Row][elf.Coord.Col] = Tile.Elf;
    }
}
