using Framework;

namespace Challenges2022;

public enum OpenRequest { Yes, No };
public partial class Ext {
    public static bool IsOpenRequestYes(this (Solution16.Path, OpenRequest) p) => p.Item2 == OpenRequest.Yes;
}

public partial class Solution16 : SolutionFramework {
    public record struct PathWithTraversedHistory(Path Path, List<(Path, OpenRequest)> Traversed);

    private static List<Path> ValvesLeft = new();

    public int TimeElapsed;
    public readonly int TimeTotal = 26;
    public int LastWinnerSum;

    public Solution16() : base(16) {
    }

    public PathWithTraversedHistory NotAnOptionPath;
    
    private PathWithTraversedHistory CreateWithValveOpen(PathWithTraversedHistory path) {
        var withValveOpen = new List<(Path, OpenRequest)>(path.Traversed) { (path.Path, OpenRequest.Yes) };
        return path with { Traversed = withValveOpen };
    }

    public PathWithTraversedHistory GetBestPath(PathWithTraversedHistory curr) {
        if (curr.Path.Paths.All(p=>curr.Traversed.Any(x => x.Item1.Name == p.Name))) {
            if (curr.Path is { ValveOpen: false, Value: > 0})
                return CreateWithValveOpen(curr);
            return curr;
        }
        
        var results = new List<PathWithTraversedHistory> {};
        
        foreach (var path in curr.Path.Paths.Where(p => curr.Traversed.All(x => x.Item1.Name != p.Name))) {
            var withValveClosed = new List<(Path, OpenRequest)>(curr.Traversed) { (curr.Path, OpenRequest.No) };
            results.Add(GetBestPath(new PathWithTraversedHistory(new Path(path.Paths, path.Value, path.Name){ValveOpen = path.ValveOpen}, withValveClosed)));

            if (curr is { Path: { ValveOpen: false, Value: > 0} } ) {
                var withValveOpen = new List<(Path, OpenRequest)>(curr.Traversed) { (curr.Path, OpenRequest.Yes) };
                results.Add(CreateWithValveOpen(new PathWithTraversedHistory(new Path(path.Paths, path.Value, path.Name){ValveOpen = path.ValveOpen}, withValveOpen)));
                results.Add(GetBestPath(CreateWithValveOpen(new PathWithTraversedHistory(new Path(path.Paths, path.Value, path.Name){ValveOpen = path.ValveOpen}, withValveOpen))));
            }
        }

        if (results.Count > 1)
            return DetermineBestPath(results);
        return results.First();
    }

    private PathWithTraversedHistory DetermineBestPath(List<PathWithTraversedHistory> res) {
        var winner = res.First();

        var maxFound = int.MinValue;
        foreach (var path in res) {
            var traversedWithOpenRequest = path.Traversed.Where(x=>x.IsOpenRequestYes()).DistinctBy(x => x.Item1.Name).Select(x=>x.Item1).ToArray();
            if (NotAnOptionPath != default) {
                var notAnOptionWithOpenRequest = NotAnOptionPath.Traversed.Where(x => x.IsOpenRequestYes()).Select(x => x.Item1).ToArray();
                if (traversedWithOpenRequest.Any(x=>notAnOptionWithOpenRequest.Any(y=>x.Name==y.Name))){
                    continue;
                }
            }

            var summed = GetPathValue(traversedWithOpenRequest, path);

            if (summed > maxFound) {
                winner = path;
                maxFound = summed;
                LastWinnerSum = maxFound;
            }
        }
        return winner;
    }

    private int GetPathValue(Path[] traversedWithOpenRequest, PathWithTraversedHistory path) {
        var opened = 0;
        var summed = 0;

        foreach (var twor in traversedWithOpenRequest) {
            opened++;
            var idx = path.Traversed.FindIndex(x => x.Item1.Name == twor.Name);
            summed += ((TimeTotal  - 1 - Math.Min(TimeTotal, (idx+opened) + TimeElapsed)) * twor.Value);
        }

        if (opened > 0) {
            summed /= path.Traversed.Count - 1 + opened;
        }
        
        return summed;
    }

    public override string[] Solve() {

        var paths = ParsePaths();
        ValvesLeft = paths.Where(p=>p.Value>0).ToList();

        var valvePressures = new List<int>();
        var opened = -1;

        foreach (var p in paths.Where(p => p.Value == 0)) {
            p.ValveOpen = false;
        }
        
        var localTotal = 0;
        var  total = 0;
        
        var path1 = paths.Single(x=>x.Name=="AA");
        var path2 = paths.Single(x=>x.Name=="AA");
        Path prev = null;
        var opening1 = false;
        var opening2 = false;
        for (var i = 0; i < TimeTotal; i++) {
            (path1, path2) = MakeMoveTwo(path1, path2, paths, i, valvePressures, ref opening1, ref opening2, ref total, ref localTotal);
            TimeElapsed++;
        }
        Console.WriteLine(total);

        return Answers;
    }

    private (Path, Path) MakeMoveTwo(Path path1, Path path2, List<Path> paths, int i, List<int> valvePressures, ref bool opening1, ref bool opening2, ref int total, ref int localTotal) {
        if (ValvesLeft.Count < 2)
            NotAnOptionPath = default;
        var findOptimalPath1 = GetBestPath(new PathWithTraversedHistory(path1, new List<(Path, OpenRequest)>()));
        
        Path pathToTake1;
        if (findOptimalPath1.Traversed.Count == 1) {
            pathToTake1 = findOptimalPath1.Path;
        }
        else {
            if (!opening1)
                pathToTake1 = paths.Single(x => x.Name == findOptimalPath1.Traversed.First(p => p.Item1.Name != path1.Name).Item1.Name);
            else {
                pathToTake1 = path1;
            }
        }

        NotAnOptionPath = findOptimalPath1;
        if (ValvesLeft.Count < 2)
            NotAnOptionPath = default;
        var findOptimalPath2 = GetBestPath(new PathWithTraversedHistory(path2, new List<(Path, OpenRequest)>()));
        NotAnOptionPath = findOptimalPath2;
        
        Path pathToTake2;
        if (findOptimalPath2.Traversed.Count == 1) {
            pathToTake2 = findOptimalPath2.Path;
        }
        else {
            if (!opening2)
                pathToTake2 = paths.Single(x => x.Name == findOptimalPath2.Traversed.First(p => p.Item1.Name != path2.Name).Item1.Name);
            else {
                pathToTake2 = path2;
            }
        }

        Console.WriteLine($"\n== Minute {i+1}, Releasing {valvePressures.Sum()}==");
        Console.WriteLine($"P1 at {pathToTake2.Name}");
        Console.WriteLine($"P2 at {pathToTake1.Name}");

        total += localTotal;

        if (findOptimalPath1.Traversed.Count >= 1) {
            if (findOptimalPath1.Traversed.First().IsOpenRequestYes() && path1 is { ValveOpen: false, Value: > 0 } && opening1)
                OpenValve(path1, valvePressures, out opening1, out localTotal, pathToTake1);
        }
        if (findOptimalPath2.Traversed.Count >= 1) {
            if (findOptimalPath2.Traversed.First().IsOpenRequestYes() && path2 is { ValveOpen: false, Value: > 0 } && opening2)
                OpenValve(path2, valvePressures, out opening2, out localTotal, pathToTake2);
        }
        if (findOptimalPath1.Traversed.Count >= 2) {
            if (findOptimalPath1.Traversed.Skip(1).First().IsOpenRequestYes() && !opening1 && !pathToTake1.ValveOpen & (pathToTake1.Value > 0))
                opening1 = true;
        }
        if (findOptimalPath2.Traversed.Count >= 2) {
            if (findOptimalPath2.Traversed.Skip(1).First().IsOpenRequestYes() && !opening2 && !pathToTake2.ValveOpen & (pathToTake2.Value > 0))
                opening2 = true;
        }
        path1 = paths.Single(p => p.Name == pathToTake1.Name);
        path2 = paths.Single(p => p.Name == pathToTake2.Name);
        return (path1, path2);
    }

    private static void OpenValve(Path path, List<int> valvePressures, out bool opening, out int localTotal, Path pathToTake) {
        Console.WriteLine($"OPENING {path.Name}");
        valvePressures.Add(path.Value);
        localTotal = valvePressures.Sum();

        path.ValveOpen = true;
        pathToTake.Value = 0;
        opening = false;
        ValvesLeft = ValvesLeft.Where(x=>x.Name!=path.Name).ToList();
    }

    private Path MakeMove(Path path, List<Path> paths, int i, List<int> valvePressures, ref bool opening, ref int total, ref int localTotal) {
        var foundPath = GetBestPath(new PathWithTraversedHistory(path, new List<(Path, OpenRequest)>()));

        Path pathToTake = null;
        if (foundPath.Traversed.Count < 2) {
            pathToTake = foundPath.Path;
        }
        else {
            if (!opening)
                pathToTake = paths.Single(x => x.Name == foundPath.Traversed.First(p => p.Item1.Name != path.Name).Item1.Name);
            else {
                pathToTake = path;
            }
        }

        Console.WriteLine($"Minute {i+1} at {pathToTake.Name} | Releasing: " + valvePressures.Sum());

        total += localTotal;

        if (pathToTake is { ValveOpen: false, Value: > 0 } && opening) {
            Console.WriteLine($"OPENING {path.Name}");

            valvePressures.Add(path.Value);
            localTotal = valvePressures.Sum();

            path.ValveOpen = true;
            pathToTake.Value = -1;
            opening = false;
        }

        if (!opening && !pathToTake.ValveOpen & (pathToTake.Value > 0)) {
            opening = true;
        }
        path = paths.Single(p => p.Name == pathToTake.Name);
        return path;
    }
}
