using System.Text.RegularExpressions;

namespace Challenges2022; 

public partial class Solution16 {
    private List<Path> ParsePaths() {
        var paths = new List<Path>();
        var allPaths = new List<Path>();
        const string pattern = @"Valve (?<x>.+) has flow rate=(?<y>.+); tunnel(?:\w*) lead(?:\w*) to valve(?:\w*) (?<z>.+)";
        foreach (var line in InputNlSplit) {
            var matches = Regex.Match(line, pattern).Groups;
            var name = matches[1].Value.Trim();
            var value = int.Parse(matches[2].Value);
            var toPaths = matches[3].Value.Split(',').Select(x => x.Trim());

            var alreadyT1 = (allPaths.Where(x => x.Name == name)).ToList();
            if (alreadyT1.Any()) {
                var p = alreadyT1.Single();
                p.Value = value;

                var alreadyTX = allPaths.Where(x => toPaths.Any(y => x.Name == y));
                foreach (var px in alreadyTX) {
                    p.Paths.Add(px);
                }

                foreach (var pathNotAddedYet in toPaths) {
                    if (allPaths.All(x => x.Name != pathNotAddedYet)) {
                        allPaths.Add(new Path(new List<Path>(), 0, pathNotAddedYet));
                        p.Paths.Add(allPaths.Last());
                    }
                }
            }

            var nPath = new Path(new List<Path>(), value, name);

            foreach (var toPath in toPaths) {
                var alreadyT2 = allPaths.Where(x => x.Name == toPath).ToArray();
                if (alreadyT2.Any()) {
                    nPath.Paths.AddRange(alreadyT2);
                }
                else {
                    allPaths.Add(new Path(new List<Path>(), 0, toPath));
                    nPath.Paths.Add(allPaths.Last());
                }
            }

            if (allPaths.All(x => nPath.Name != x.Name)) {
                allPaths.Add(nPath);
                paths.Add(allPaths.Last());
            }
            else {
                paths.Add(allPaths.Single(x => x.Name == nPath.Name));
            }
        }

        return paths;
    }
}
