using Framework;

namespace Challenges2020;

public enum Direction { N, E, S, W };
public enum Rotation { L, R };

public class Action
{
    public Direction? direction;
    internal Rotation? rotation;
    internal bool? forward;
    
    public Direction GetDirection() => direction.Value;
    public Rotation GetRotation() => rotation.Value;

    public bool IsDirection() => direction.HasValue;
    public bool IsRotation() => rotation.HasValue;
    public bool IsForward() => forward.HasValue;
    
    public Action(string action)
    {
        if (action == "F")
        {
            forward = true;
        }
        else if (Enum.TryParse<Direction>(action, out var directionVal))
        {
            direction = directionVal;
        }
        else if (Enum.TryParse<Rotation>(action, out var rotationVal))
        {
            rotation = rotationVal;
        } 
        else
        {
            throw new ArgumentException();
        }
    }
}

public class Solution12 : SolutionFramework
{
    public Solution12() : base(12) { }

    public record struct Ship(Direction Direction, (int HorizontalPos, int VerticalPos) Position);

    public record struct Waypoint(int HorizontalPos, int VerticalPos);

    public record struct ShipMoveInstruction(Action Action, int Units);

    public override string[] Solve()
    {
        var instructions = RawInputSplitByNl.Select(line => new ShipMoveInstruction(new Action(line[..1]), int.Parse(line[1..]))).ToList();
        var ship = new Ship(Direction.E, (0, 0));

        foreach (var instruction in instructions)
        {
            var newDirection = ship.Direction;
            var newPosition = ship.Position;

            if (instruction.Action.IsForward() || instruction.Action.IsDirection())
            {
                var direction = instruction.Action.IsForward() ? ship.Direction : instruction.Action.GetDirection();
                switch (direction)
                {
                    case Direction.E or Direction.W:
                        newPosition.HorizontalPos += (direction is Direction.W ? -instruction.Units : instruction.Units);
                        break;
                    case Direction.N or Direction.S:
                        newPosition.VerticalPos += (direction is Direction.N ? -instruction.Units : instruction.Units);
                        break;
                }
            } else if (instruction.Action.IsRotation())
            {
                var rotationSteps = (instruction.Units / 90) % 4;
                var prevDir = newDirection;
                newDirection = (Direction)(instruction.Action.GetRotation() switch
                {
                    Rotation.L => (int)ship.Direction >= rotationSteps
                        ? (int)ship.Direction - rotationSteps
                        : Enum.GetNames(typeof(Direction)).Length - rotationSteps + (int)ship.Direction,
                    Rotation.R => (((int)ship.Direction + rotationSteps) % Enum.GetNames(typeof(Direction)).Length),
                });
                //Console.WriteLine($"{instruction.Action.GetRotation()}, [{rotationSteps}, {prevDir}->{newDirection}]");
            }

            ship = new Ship(newDirection, newPosition);
        }

        AssignAnswer1(Math.Abs(ship.Position.HorizontalPos) + Math.Abs(ship.Position.VerticalPos));

        ship = new Ship(Direction.E, (0, 0));
        var waypoint = new Waypoint(10, -1);

        foreach (var instruction in instructions)
        {
            var shipNewDirection = ship.Direction;
            var shipNewPosition = ship.Position;
            var waypointNewHorizontalPos = waypoint.HorizontalPos;
            var waypointNewVerticalPos = waypoint.VerticalPos;

            switch (isForward: instruction.Action.IsForward(), 
                isDirection: instruction.Action.IsDirection(), 
                isRotation: instruction.Action.IsRotation())
            {
                case (isForward: true, _, _):
                    shipNewPosition.HorizontalPos += waypoint.HorizontalPos * instruction.Units;
                    shipNewPosition.VerticalPos += waypoint.VerticalPos * instruction.Units;
                    break;
                case (_, isDirection: true, _):
                    var direction = instruction.Action.GetDirection();
                    switch (direction)
                    {
                        case Direction.E or Direction.W:
                            waypointNewHorizontalPos += (direction is Direction.W ? -instruction.Units : instruction.Units);
                            break;
                        case Direction.N or Direction.S:
                            waypointNewVerticalPos += (direction is Direction.N ? -instruction.Units : instruction.Units);
                            break;
                    }
                    break;
                case (_, _, isRotation: true):
                    foreach (var _ in Enumerable.Range(0, instruction.Units / 4))
                    {
                        var rotationSteps = (instruction.Units / 90) % 4;
                        var rotationDirection = instruction.Action.GetRotation();
                        switch (rotationSteps)
                        {
                            case 1:
                                waypointNewHorizontalPos = (rotationDirection is Rotation.R) ? -waypoint.VerticalPos : waypoint.VerticalPos;
                                waypointNewVerticalPos = (rotationDirection is Rotation.R) ? waypoint.HorizontalPos : -waypoint.HorizontalPos;
                                break;
                            case 2:
                                waypointNewHorizontalPos = -waypoint.HorizontalPos;
                                waypointNewVerticalPos = -waypoint.VerticalPos;
                                break;
                            case 3:
                                waypointNewHorizontalPos = (rotationDirection is Rotation.R) ? waypoint.VerticalPos : -waypoint.VerticalPos;
                                waypointNewVerticalPos = (rotationDirection is Rotation.R) ? -waypoint.HorizontalPos : waypoint.HorizontalPos;
                                break;
                            default:
                                throw new ArgumentException();
                        }
                    }
                    break;
            }
            
            ship = new Ship(shipNewDirection, shipNewPosition);
            waypoint = new Waypoint(waypointNewHorizontalPos, waypointNewVerticalPos);
            Console.WriteLine();
        }
        AssignAnswer2(Math.Abs(ship.Position.HorizontalPos) + Math.Abs(ship.Position.VerticalPos));

        return Answers;
    }
}
