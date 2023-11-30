using static Challenges2022.Solution15;

namespace Challenges2022; 

public partial class Solution23 {
    public enum Direction {
        N, S, W, E, NW, SW, NE, SE,
    }

    public enum Tile {
        Elf, Empty
    }

    public record Elf(Coord Coord, int Id) {
        public Coord Coord { get; set; } = Coord;
    };
    public record struct Rectangle(Coord TopLeftEdge, Coord TopRightEdge, Coord BotLeftEdge, Coord BotRightEdge);

    public Rectangle GetRectangleRegion(Tile[][] grid) {
        int leftMostCol = -1;
        var rightMostCol = -1;
        var topMostRow = -1;
        var botMostRow = -1;
        for (var i = 0; i < grid.Length; i++) {
            for (var j = 0; j < grid[0].Length; j++) {
                if (grid[i][j] == Tile.Elf) {
                    if (topMostRow == -1) {
                        topMostRow = i;
                    } else if (leftMostCol == -1 || j < leftMostCol) {
                        leftMostCol = j;
                    } else if (rightMostCol == -1 || j > rightMostCol) {
                        rightMostCol = j;
                    } else if (botMostRow == -1 || i > botMostRow) {
                        botMostRow = i;
                    }
                }
            }
        }

        var topLeft = new Coord(topMostRow, leftMostCol);
        var topRight = new Coord(topMostRow, rightMostCol);
        var botLeft = new Coord(botMostRow, leftMostCol);
        var botRight = new Coord(botMostRow, rightMostCol);

        return new Rectangle(topLeft, topRight, botLeft, botRight);
    }

    public record struct Priority(Direction Dir1, Direction Dir2, Direction Dir3);
    public record struct PriorityWithValidationOffsets(Priority Priority, List<Coord> ValidationOffsets);

    public static Priority Priority1 = new(Direction.N, Direction.NE, Direction.NW);
    public static Priority Priority2 = new(Direction.S, Direction.SE, Direction.SW);
    public static Priority Priority3 = new(Direction.W, Direction.NW, Direction.SW);
    public static Priority Priority4 = new(Direction.E, Direction.NE, Direction.SE);

    public class PriorityManager {
        private static List<Priority> priorities = new() { Priority1, Priority2, Priority3, Priority4 };
        public Queue<Priority> PrioritiesQueue = new(priorities);
        
        public Priority GetPriority() => PrioritiesQueue.Dequeue();

        public void ShiftBy(int n) {
            for (int i = 0; i < n; i++) {
                PrioritiesQueue.Enqueue(PrioritiesQueue.Dequeue());
            }
        }
        public PriorityWithValidationOffsets DequeuePrioWithValidationOffsets() {
            var prio = PrioritiesQueue.Dequeue();
            var offs = GetOffsetsForPrio(prio);
            return new PriorityWithValidationOffsets(prio, offs);
        }
    }

    public static List<Coord> GetOffsetsForPrio(Priority priority) {
        var directions = new[] { priority.Dir1, priority.Dir2, priority.Dir3};
        var offsets = new List<Coord>() { };
        foreach (var dir in directions) {
            offsets.Add(dir switch {
                Direction.N => new Coord(-1, 0),
                Direction.S => new Coord(1, 0),
                Direction.W => new Coord(0, -1),
                Direction.E => new Coord(0, 1),
                Direction.NE => new Coord(-1, 1),
                Direction.SE => new Coord(1, 1),
                Direction.SW => new Coord(1, -1),
                Direction.NW => new Coord(-1, -1),
            });
        }
        return offsets;
    }
}
