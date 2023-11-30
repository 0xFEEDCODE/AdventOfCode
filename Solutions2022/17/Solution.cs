using System.Diagnostics;

using Framework;

using static Challenges2022.Solution15;

namespace Challenges2022;

public partial class Solution17 : SolutionFramework {
    public Solution17() : base(17) {
    }

    private readonly char flatShape = '-';
    private readonly char plusShape = '+';
    private readonly char lShape = 'l';
    private readonly char iShape = 'i';
    private readonly char boxShape = 'b';
    public long FirstThreeSteps = 3;
    public Dictionary<int, List<int>> TraversedRows = new(){{0, new List<int>()}};

    public override string[] Solve() {
        var parsed = ParseInput();
        var pushes = parsed.Item1;
        var shapes = parsed.Item2;

        var shapeChoice = new Dictionary<int, Func<Shape>>() {
            { 0, () => new Shape(shapes[flatShape].ToList()) },
            { 1, () => new Shape(shapes[plusShape].ToList()) },
            { 2, () => new Shape(shapes[lShape].ToList()) },
            { 3, () => new Shape(shapes[iShape].ToList()) },
            { 4, () => new Shape(shapes[boxShape].ToList()) },
        };
        var shapeCounter = 0;

        var leftWall = 0;
        var rightWall = 6;
        var floor = -4;
        
        var pushIdx = 0;
        
        int LeftMost(Shape shape) => shape.Coords.MinBy(x => x.Col).Col;
        int RightMost(Shape shape) => shape.Coords.MaxBy(x => x.Col).Col; 
        Shape MoveRight(Shape shape, Shape floorShape) {
            if (RightMost(shape) >= 7)
                return shape;
            var updatedShape = new Shape(new List<Coord>());
            updatedShape.Coords.AddRange(shape.Coords.Select(c => new Coord(c.Row, c.Col + 1)));
            if (!CollisionOfShapes(floorShape, updatedShape))
                return updatedShape;
            return shape;
        }
        
        Shape MoveLeft(Shape shape, Shape floorShape) {
            if (LeftMost(shape) <= 1)
                return shape;
            var updatedShape = new Shape(new List<Coord>());
            updatedShape.Coords.AddRange(shape.Coords.Select(c => new Coord(c.Row, c.Col - 1)));
            if (!CollisionOfShapes(floorShape, updatedShape))
                return updatedShape;
            return shape;
        }
        (Shape, bool) TryMoveDown(Shape shape, Shape floor) {
            var updatedShape = new Shape(new List<Coord>());
            updatedShape.Coords.AddRange(shape.Coords.Select(c => new Coord(c.Row + 1, c.Col)));
            return !CollisionOfShapes(floor, updatedShape) ? (updatedShape, false) : (shape, true);
        }
        
        bool CollisionOfCoord(Coord coord) {
            if (FirstThreeSteps >= 0)
                return false;
            if (TraversedRows.ContainsKey(coord.Row)) {
                if (TraversedRows[coord.Row].Contains(coord.Col)) {
                    return true;
                }
            }
            return false;
        }
        
        bool CollisionOfShapes(Shape floorShape, Shape shape) {
            if (FirstThreeSteps >= 0)
                return false;
            foreach (var sp in shape.Coords) {
                if (CollisionOfCoord(sp))
                    return true;
            }

            return false;
        }

        long rocksFallen = 0;
        var floorDist = 3;
        var collision = false;
        var shape = shapeChoice[shapeCounter++ % shapeChoice.Count].Invoke();
        for (var i = 0; i < shape.Coords.Count; i++) {
            shape.Coords[i] = new Coord(shape.Coords[i].Row-4, shape.Coords[i].Col+3);
        }
        var floorShape = CreateFloorShape();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        floorShape.Coords.ForEach(x=>TraversedRows[x.Row].Add(x.Col));
        
        var alt = 0;
        var highestFloorPoint = 0;
        long ex1 = 2023;
        long ex2 = 1000000000000;
        long test = 0;
        var rfBetween = 0;
        bool finding = false;
        long lenPrev = 0;
        
        var prevTx = floorShape.Coords.MinBy(y => y.Row);
        while (rocksFallen != ex2-1) {
            test++;
            
            if (collision) {
                rocksFallen++;
                rfBetween++;
                shape = shapeChoice[shapeCounter++ % shapeChoice.Count].Invoke();
                for (var i = 0; i < shape.Coords.Count; i++) {
                    shape.Coords[i] = new Coord(highestFloorPoint - 4 - shape.Coords[i].Row , shape.Coords[i].Col + 3);
                }

                collision = false;
                FirstThreeSteps = 3;
            }

            var nums = new List<int>() { 11, 0, 2, 2, 1, 12 };
            var print = false;
            for(int i = 1; i < 7; i++){
                var highestPointAtCol = floorShape.Coords.Where(y => y.Col == i).MinBy(x => x.Row);
                var highestPointAtPrevCol = floorShape.Coords.Where(y => y.Col == i+1).MinBy(x => x.Row);

                print = (Math.Abs(highestPointAtCol.Row - highestPointAtPrevCol.Row)) == nums[i-1];
                if (!print)
                    break;
            }

            if (print) {
                Console.WriteLine();
                var t = floorShape.Coords.MinBy(y => y.Row);
                Console.WriteLine("\nRF : " + rfBetween + " and tall " + t.Row);
                rfBetween = 0;
            }

            if (rfBetween is 747) {
                var t = floorShape.Coords.MinBy(y => y.Row);
                Console.WriteLine("\nRF : " + rfBetween + " and tall " + t.Row);
                Console.WriteLine();
            }
            

            
            
            if (alt == 0) {
                //Push rock
                shape = pushes[pushIdx++] == Direction.Right ? MoveRight(shape, floorShape) : MoveLeft(shape, floorShape);
                if (pushIdx == pushes.Count() ) {
                    pushIdx = 0;
                }


                alt = 1;
            } else {
                //Rock goes down
                var shapeWithCollisionResult = TryMoveDown(shape, floorShape);
                shape = shapeWithCollisionResult.Item1;
                collision = shapeWithCollisionResult.Item2;
                if (collision) {
                    floorShape.Coords.AddRange(shape.Coords);
                    
                    foreach (var coord in shape.Coords) {
                        if (!TraversedRows.ContainsKey(coord.Row)) {
                            TraversedRows.Add(coord.Row, new List<int>());
                        }
                        TraversedRows[coord.Row].Add(coord.Col);
                        highestFloorPoint.AssignIfLower(coord.Row);
                    }
                }

                alt = 0;
                //Visualize(shape, floorShape);
            }

            FirstThreeSteps--;

            if (rocksFallen == 10000000) {
                Console.WriteLine(rocksFallen);
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
        }

        var tall = floorShape.Coords.MinBy(y => y.Row);
        
        Console.WriteLine(tall.Coords);

         return Answers;
    }

    private void Visualize(Shape shape, Shape floorShape) {
        Console.WriteLine();
        int h = 1000;
        var grid = (h, 9).CreateGrid<char>();
        for (int i = grid.Length/2; i < grid.Length; i++) {
            Console.WriteLine();
            for (int j = 0; j < grid[0].Length; j++) {
                if (j == 0 || j == 8)
                    Console.Write('|');
                else {
                    if (floorShape.Coords.Any(x => x.Row == i-h+5 && x.Col == j)) {
                        Console.Write("#");
                    } else if (shape.Coords.Any(x => x.Row == i-h+5 && x.Col == j)) {
                        Console.Write('@');
                    } else {
                        Console.Write('.');
                    }
                }
            }
        }
    }

    private static Shape CreateFloorShape() => new Shape(new List<Coord>(){ new(0,0), new(0,1), new(0,2), new(0,3), new(0,4), new(0,5), new(0,6), new(0,7)});
}
