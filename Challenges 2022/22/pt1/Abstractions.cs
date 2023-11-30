namespace Archive;
public partial class Solution22 {
    
    public enum Rotation { Right, Left };
    public enum Direction { Up, Right, Down, Left };
    public enum Tile { Wall, Open, BoundaryWall, Player }

    public record struct Instruction(Rotation? Rotation = null, int? Steps = null) {
        public bool IsRotation => Rotation.HasValue;
        public bool IsSteps => Steps.HasValue;
        public Rotation GetRotation => Rotation!.Value;
        public int GetSteps => Steps!.Value;
    }

}
