using Framework;

namespace Challenges2022;

public class Solution14 : SolutionFramework
{
    public Solution14() : base(14) { }

    public class EntityWithCoords
    {
        public Entity Entity;
        public (int, int) Coords;
        public int Row => Coords.Item1;
        public int Col => Coords.Item2;

        public void IncRow()
            => Coords = (Coords.Item1 + 1, Coords.Item2);
        public void DecRow()
            => Coords = (Coords.Item1 - 1, Coords.Item2);
        
        public void IncCol()
            => Coords = (Coords.Item1, Coords.Item2 + 1);
        public void DecCol()
            => Coords = (Coords.Item1, Coords.Item2 - 1);

        public EntityWithCoords(Entity entity, (int, int) coords)
        {
            Coords = coords;
            Entity = entity;
        }
    }
    
    // When in doubt, name it entity
    public struct Entity
    {
        public bool IsRock => Representation == '#';
        public bool IsSand => Representation == '+';
        public bool IsEmpty => Representation == '.';
        public bool IsLandedSand => Representation == 'o';
        public bool IsEdge => Representation == 'X';
        public char Representation;

        public Entity(char representation)
        {
            Representation = representation;
        }
    }

    public char Sand => '+';
    public char Empty => '.';
    public char Rock => '#';
    public char LandedSand => 'o';
    public char Edge => 'X';

    public void Print(Entity[][] grid)
    {
        Console.WriteLine('\n');
        for (int i = 0; i < 0 + 300; i++)
        {
            Console.WriteLine('\n');
            for (int j = 480; j < 480 + 100; j++)
            {
                Console.Write(grid[i][j].Representation);
            }
        }
        
    }
    
    public override string[] Solve()
    {
        var grid = InitGrid();
        PutEdgesAroundRocks(grid, true, true);
        var fallenSand = SimulateSandFall(grid, true);
        AssignAnswer1(fallenSand);
        
        grid = InitGrid();
        PutEdgesAroundRocks(grid, false, true);
        fallenSand = SimulateSandFall(grid, false);
        AssignAnswer2(fallenSand);

        return Answers;
    }

    private Entity[][] InitGrid()
    {
        var grid = (1000, 1000).CreateGrid<Entity>();
        grid.SetAllCellsToValue(new Entity(Empty));
        
        foreach (var line in InputNlSplit)
            PutRocksOnAGrid(line.Split(" -> "), grid);
        return grid;
    }

    private int SimulateSandFall(Entity[][] grid, bool stopWhenEdgeIsHit)
    {
        const int startRow = 0;
        const int startCol = 500;
        
        var fallingSand = new EntityWithCoords(new Entity(Sand), (startRow, startCol));
        var fallenSand = 0;

        var sandOverflow = false;
        while (!sandOverflow)
        {
            if (fallingSand.Row > grid.Length - 1 || fallingSand.Col > grid[0].Length - 1)
                throw new InvalidOperationException();

            grid[fallingSand.Row][fallingSand.Col] = fallingSand.Entity;

            switch (fallingSand.Row, fallingSand.Col)
            {
                case var (r, c) when grid.NeighborCellDown(r, c).IsEmpty:
                    ResetCellAfterFalling(grid, fallingSand);
                    fallingSand.IncRow();
                    break;
                case var (r, c) when grid.NeighborCellBottomLeft(r, c).IsEmpty:
                    ResetCellAfterFalling(grid, fallingSand);
                    fallingSand.IncRow();
                    fallingSand.DecCol();
                    break;
                case var (r, c) when grid.NeighborCellBottomRight(r, c).IsEmpty:
                    ResetCellAfterFalling(grid, fallingSand);
                    fallingSand.IncRow();
                    fallingSand.IncCol();
                    break;
                default:
                    if (stopWhenEdgeIsHit)
                    {
                        if (grid.NeighborCellDown(fallingSand.Row, fallingSand.Col).IsEdge ||
                            grid.NeighborCellBottomLeft(fallingSand.Row, fallingSand.Col).IsEdge ||
                            grid.NeighborCellBottomRight(fallingSand.Row, fallingSand.Col).IsEdge)
                        {
                            sandOverflow = true;
                            break;
                        }
                    }
                    else{
                        if (fallingSand is { Row: startRow, Col: startCol } &&
                            grid.NeighborCellDown(fallingSand.Row, fallingSand.Col).IsLandedSand &&
                            grid.NeighborCellBottomLeft(fallingSand.Row, fallingSand.Col).IsLandedSand &&
                            grid.NeighborCellBottomRight(fallingSand.Row, fallingSand.Col).IsLandedSand)
                        {
                            sandOverflow = true;
                            fallenSand++;
                            break;
                        }
                    }
                    fallenSand++;

                    grid[fallingSand.Row][fallingSand.Col] = new Entity(LandedSand);
                    fallingSand = new EntityWithCoords(new Entity(Sand), (startRow, startCol));
                    break;
            }
        }

        return fallenSand;
    }

    private void PutEdgesAroundRocks(Entity[][] grid, bool putSideEdges, bool putBottomEdge)
    {
        var leftEdge = int.MaxValue;
        var rightEdge = int.MinValue;
        var bottom = int.MinValue;
        grid.ForEachCell((i, j) =>
        {
            if (grid[i][j].IsRock)
            {
                if (j < leftEdge)
                    leftEdge = j;
                if (j > rightEdge)
                    rightEdge = j;
                if (i > bottom)
                    bottom = i;
            }
        });
        leftEdge -= 1;
        rightEdge += 1;
        bottom += 2;
        grid.ForEachCell((i, j) =>
        {
            if (putBottomEdge && i == bottom)
                grid[i][j] = new Entity(Edge);
            if (putSideEdges && (i == leftEdge || i == rightEdge))
                grid[i][j] = new Entity(Edge);
        });
    }

    private void PutRocksOnAGrid(string[] coordinates, Entity[][] grid)
    {
        var previousX = -1;
        var previousY = -1;
        foreach (var entry in coordinates)
        {
            var coord = entry.Split(',').Select(int.Parse).ToArray();
            var x = coord[0];
            var y = coord[1];
            if (previousX != -1 && previousY != -1)
            {
                var verticalPlane = y - previousY;
                var horizontalPlane = x - previousX;
                if (verticalPlane != 0)
                {
                    if (verticalPlane > 0)
                        for (var y2 = 0; y2 <= verticalPlane; y2++)
                            grid[y - y2][x] = new Entity(Rock);
                    if (verticalPlane < 0)
                        for (var y2 = verticalPlane; y2 <= 0; y2++)
                            grid[y - y2][x] = new Entity(Rock);
                }

                if (horizontalPlane != 0)
                {
                    if (horizontalPlane > 0)
                        for (var x2 = 0; x2 <= horizontalPlane; x2++)
                            grid[y][x - x2] = new Entity(Rock);
                    if (horizontalPlane < 0)
                        for (var x2 = horizontalPlane; x2 <= 0; x2++)
                            grid[y][x - x2] = new Entity(Rock);
                }
            }

            previousX = x;
            previousY = y;
        }
    }

    private void ResetCellAfterFalling(Entity[][] grid, EntityWithCoords currentSand) => grid[currentSand.Row][currentSand.Col] = new Entity(Empty);
}
