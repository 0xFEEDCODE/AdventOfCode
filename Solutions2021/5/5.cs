using System.Text.RegularExpressions;

using Framework;

namespace Challenges2021;

public class Solution5 : SolutionFramework {
    public Solution5() : base(5) { }
    
    readonly record struct Line(int X1, int Y1, int X2, int Y2);

    public override string[] Solve() {
        var lines = new List<Line>();
        var linesMap = new Dictionary<int, Dictionary<int, int>>();
        foreach (var line in RawInputSplitByNl) {
            var coords = Regex.Matches(line, @"\d+").Select(x=>int.Parse(x.Value)).ToArray();
            lines.Add(new Line(coords[0], coords[1], coords[2], coords[3]));
        }

        foreach (var (x1, y1, x2, y2) in lines) {
            if (x1 == y1 && x2 == y2) {
                for (var i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++) {
                    if (!linesMap.ContainsKey(i)) {
                        linesMap.Add(i, new Dictionary<int, int>());
                    }
                    if (!linesMap[i].ContainsKey(i)) {
                        linesMap[i].Add(i, 0);
                    }

                    linesMap[i][i]++;
                }
            }
            else if (Math.Abs(x1-x2) == Math.Abs(y1-y2)) {
                if (y1 > y2) {
                    var incr = 0;
                    for (var y = y1; y >= y2; y--) {
                        if (!linesMap.ContainsKey(y)) {
                            linesMap.Add(y, new Dictionary<int, int>());
                        }

                        var x = x1 + (x1 > x2 ? -incr : incr);
                        if (!linesMap[y].ContainsKey(x)) {
                            linesMap[y].Add(x, 0);
                        }

                        linesMap[y][x]++;
                        incr++;
                    }
                } else {
                    var incr = 0;
                    for (var y = y1; y <= y2; y++) {
                        if (!linesMap.ContainsKey(y)) {
                            linesMap.Add(y, new Dictionary<int, int>());
                        }

                        var x = x1 + (x1 > x2 ? -incr : incr);
                        if (!linesMap[y].ContainsKey(x)) {
                            linesMap[y].Add(x, 0);
                        }

                        linesMap[y][x]++;
                        incr++;
                    }
                    
                }
            }
            else if (x1 == x2 ) {
                for (var y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++) {
                    if (!linesMap.ContainsKey(y)) {
                        linesMap.Add(y, new Dictionary<int, int>());
                    }
                    if (!linesMap[y].ContainsKey(x1)) {
                        linesMap[y].Add(x1, 0);
                    }
                    linesMap[y][x1]++;
                }
            }
            else if (y1 == y2) {
                for (var x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++) {
                    if (!linesMap.ContainsKey(y1)) {
                        linesMap.Add(y1, new Dictionary<int, int>());
                    }
                    if (!linesMap[y1].ContainsKey(x)) {
                        linesMap[y1].Add(x, 0);
                    }

                    linesMap[y1][x]++;
                }
            }
        }

        var c = 0;
        for (var i = 0; i <= 10; i++) {
            Console.WriteLine();
            for (var j = 0; j <= 10; j++) {
                if (linesMap.ContainsKey(i) && linesMap[i].ContainsKey(j)) {
                    Console.Write(linesMap[i][j]);
                } else {
                    Console.Write('0');
                }
            }
        }
        
        foreach (var kv in linesMap) {
            foreach (var kv2 in kv.Value) {
                if(kv2.Value > 1) {
                    c++;
                }
            }
        }
        
        AssignAnswer1(c);

        
        return Answers;
    }
}
