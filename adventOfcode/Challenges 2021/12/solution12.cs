using Framework;

namespace Challenges2021;

public static partial class Ext
{
    public static bool IsStart(this string str) => str == "start";
    public static bool IsEnd(this string str) => str == "end";
    public static bool IsSmallCave(this string str) => str.All(char.IsLower) && str != "start" && str != "end";
}

public class Solution12 : SolutionFramework
{
    public Solution12() : base(12) { }

    public record struct CavePath(ICollection<PathNode> Path) {
        public PathNode Last() => Path.Last();
        public void Add(PathNode node) => Path.Add(node);
        public int Count(Func<PathNode, bool> predicate) => Path.Count(predicate);
        public bool VisitedSmallCaveMoreTwice = false;
    }
    public record struct PathNode(string Name, ICollection<PathNode> Connected) {
        public bool IsStart => Name.IsStart();
        public bool IsEnd => Name.IsEnd();
        public bool IsSmallCave => Name.IsSmallCave();
    }
    
    public override string[] Solve()
    {
        var connections = new Dictionary<string, List<string>>();

        foreach (var line in RawInput.SplitByNewline())
        {
            var split = line.Split('-');
            if (!connections.ContainsKey(split[0])) {
                connections.Add(split[0], new List<string>());
            }
            if (!connections.ContainsKey(split[1])) {
                connections.Add(split[1], new List<string>());
            }
            connections[split[0]].Add(split[1]);
            connections[split[1]].Add(split[0]);
        }

        var pNodes = connections.Select(kv=>new PathNode(kv.Key, new List<PathNode>())).ToArray();
        foreach (var node in pNodes) {
            foreach (var conn in connections[node.Name]) {
                node.Connected.Add(pNodes.Single(p=>p.Name == conn));
            }
        }

        Console.WriteLine();
        

        foreach (var startNode in pNodes.Where(n => n.IsStart)) {
            FindPaths(new CavePath(new List<PathNode>(){startNode}));
        }

        AssignAnswer1(PathsFound.Count(cp => cp.Path.Any(n => n.Name.IsSmallCave())));
        
        //pt 2
        PathsFound.Clear();
        Pt2Flavor = true;
        foreach (var startNode in pNodes.Where(n => n.IsStart)) {
            FindPaths(new CavePath(new List<PathNode>(){startNode}));
        }

        AssignAnswer2(PathsFound.Count());

        return Answers;
    }

    public HashSet<CavePath> PathsFound = new();
    public bool Pt2Flavor = false;
    public void FindPaths(CavePath currentPath) {
        var currentNode = currentPath.Last();

        if (currentNode.IsSmallCave && currentPath.Count(n => n.Name == currentNode.Name) > 1) {
            if(!Pt2Flavor || currentPath.VisitedSmallCaveMoreTwice) {
                return;
            }

            currentPath.VisitedSmallCaveMoreTwice = true;
        }
        
        if (currentNode.IsEnd) {
            PathsFound.Add(currentPath);
            return;
        }

        foreach (var conn in currentNode.Connected.Where(c => !c.IsStart)) {
            var nPath = currentPath with { Path = currentPath.Path.Select(n => n with { }).ToList() };
            nPath.Add(conn);
            FindPaths(nPath);
        }
    }
}
