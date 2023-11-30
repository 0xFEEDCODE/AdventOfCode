namespace Challenges2022; 

public partial class Solution16 {
    public class Path {
        public Path(List<Path> paths, int value, string name) {
            Paths = paths;
            Value = value;
            Name = name;
            ValveOpen = false;
        }

        public List<Path> Paths;
        public int Value;
        public bool ValveOpen;
        public string Name;
    };
}
