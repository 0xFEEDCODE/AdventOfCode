using System.Globalization;

using Framework;

namespace Solutions2023;

public class Solution18() : SolutionFramework(18)
{

    enum Dir { R, L, U, D };

    record Range(int S, int E);

    public override string[] Solve()
    {
        int offset = 0;
        int wh = 25;
        var grid = (wh, wh).CreateGrid<char>();
        grid.SetAllCellsToValue('.');

        var startPos = new GridPos(offset, offset);
        grid.SetCell(startPos, '#');

        var instructions = InputNlSplit.Select(l =>
        {
            var spl = l.Split(' ');
            return new { dir = (Dir)Enum.Parse(typeof(Dir), spl[0]), amount = spl[1].ParseInt(), hex = spl[2].Substring(2, 6) };
        }).ToList();
        instructions.Clear();

        var totalSurface = 0;

        var id = 0;
        var currentPos = startPos;
        foreach (var instruction in instructions)
        {
            var dir = instruction.dir;

            for (var i = 0; i < instruction.amount; i++)
            {

                currentPos = dir switch
                {
                    Dir.D => new GridPos(currentPos.R + 1, currentPos.C),
                    Dir.U => new GridPos(currentPos.R - 1, currentPos.C),
                    Dir.R => new GridPos(currentPos.R, currentPos.C + 1),
                    Dir.L => new GridPos(currentPos.R, currentPos.C - 1),
                    _ => throw new ArgumentOutOfRangeException()
                };


                grid.SetCell(currentPos, 'O');
            }
        }


        for (var i = 0; i < grid.Length; i++)
        {
            var nEdges = 0;
            var parsingEdge = false;
            var acc = 0;

            for (var j = 0; j < grid[i].Length; j++)
            {
                var p = new GridPos(i, j);
                var cell = grid.GetCell(p);

                if (cell is '#')
                {
                    totalSurface++;
                }
                if (cell is '#' && (grid.TryGetNeighborCellUp(p, out var u) && u == '#'))
                {
                    nEdges++;
                }
                if (cell is '.')
                {
                    if (nEdges % 2 != 0)
                    {
                        grid[i][j] = 'x';
                        totalSurface++;
                    }
                }
            }
        }

        AssignAnswer1(totalSurface);

        instructions = InputNlSplit.Select(l =>
        {
            var spl = l.Split(' ');
            var hexV = spl[2].Substring(2, 6);
            var d = hexV[^1];
            var n = int.Parse("0" + hexV.Substring(0, hexV.Length - 1), NumberStyles.HexNumber);
            var dir = d switch
            {
                '0' => Dir.R,
                '1' => Dir.D,
                '2' => Dir.L,
                '3' => Dir.U,
                _ => throw new ArgumentOutOfRangeException()
            };


            //return new { dir = (Dir)Enum.Parse(typeof(Dir), spl[0]), amount = int.Parse(spl[1]), hex = hexV };
            return new { dir = dir, amount = n, hex = hexV };
        }).ToList();


        currentPos = new GridPos(offset, offset);
        var lines = new List<(Range Rows, Range Cols, Dir Dir)>();

        totalSurface = 0;

        var hlines = new List<(int R, Range Cols)>();
        var inc = 0;

        (double X, double Y) pos = (0d, 0d);
        var points = new List<(double X, double Y)>{ pos };
        var nBoundaries = 0d;
        
        foreach (var instruction in instructions)
        {
            var dir = instruction.dir;
            var n = instruction.amount;
            var newPos = dir switch
            {
                Dir.D => (pos.X, pos.Y + n),
                Dir.U => (pos.X, pos.Y - n),
                Dir.R => (pos.X + n, pos.Y),
                Dir.L => (pos.X - n, pos.Y),
                _ => throw new ArgumentOutOfRangeException()
            };

            nBoundaries += instruction.amount;
            
            points.Add(newPos);
            pos = newPos;
        }

        // Calculate interior area only using Shoelace formula
        var interiorArea = Math.Abs(points.Select((p, i) => p.X * (points[(i > 0 ? i : points.Count) - 1].Y - points[(i + 1) % points.Count].Y)).Sum())/2;
        
        // Calculate the area using Pick's theorem
        var area = interiorArea - (nBoundaries / 2) + 1;
        
        AssignAnswer1(area + nBoundaries);
        return Answers;

        /*
        var dU = 0;
        var dD = 0;
        var verticalLines = new List<(int C, Range R, Dir? Dir)>();
        
        Dir? prevDir = null;
        foreach (var vl in lines)
        {
            var dir = vl.Dir;

            if (dir is Dir.U or Dir.D)
            {
                var s = Math.Min(vl.Rows.S, vl.Rows.E);
                var e = Math.Max(vl.Rows.S, vl.Rows.E);

                if (dir is Dir.D)
                {
                    e++;
                }
                if (dir is Dir.D && prevDir is Dir.L)
                {
                    s++;
                }
                if (dir is Dir.D && prevDir is Dir.R && verticalLines.Any(vl => vl.Dir is Dir.D))
                {
                    var prevVl = verticalLines.Last();
                    verticalLines.Remove(prevVl);
                    prevVl = (prevVl.C, new Range(prevVl.R.S, prevVl.R.E - 1), prevVl.Dir);
                    verticalLines.Add(prevVl);
                }
                
                if (dir is Dir.U)
                {
                    e++;
                }
                if (dir is Dir.U && prevDir is Dir.R)
                {
                    e--;
                }
                if (dir is Dir.U && prevDir is Dir.L && verticalLines.Any(vl => vl.Dir is Dir.U))
                {
                    var prevVl = verticalLines.Last();
                    verticalLines.Remove(prevVl);
                    prevVl = (prevVl.C, new Range(prevVl.R.S + 1, prevVl.R.E ), prevVl.Dir);
                    verticalLines.Add(prevVl);
                }
                /*
                if (dir is Dir.U && prevDir is Dir.R && verticalLines.Any())
                {
                    var prevVl = verticalLines.Last();
                    verticalLines.Remove(prevVl);
                    prevVl = (prevVl.C, new Range(prevVl.R.S - 1, prevVl.R.E));
                    verticalLines.Add(prevVl);
                    e--;
                }
                #1#
                
                verticalLines.Add((vl.Cols.S, new Range(s, e), dir));
            }

            prevDir = dir;
        }
        
        foreach (var vl in verticalLines)
        {
            var s = Math.Min(vl.R.S, vl.R.E);
            var e = Math.Max(vl.R.S, vl.R.E);

            for (var i = s; i < e; i++)
            {
                grid[i][vl.C] = '|';
            }
        }
        
        while (verticalLines.Any())
        {
            var vl1 = verticalLines.MinBy(x=>x.C);
            var c1 = vl1.C;

            var f = 0;
            foreach (var vl2 in verticalLines.Where(vt2 => vt2.C > c1).OrderBy(vt2 => vt2.C).ToArray())
            {
                if (!GetIntersection(vl1.R, vl2.R, out var prefix, out var intersection, out var suffix))
                {
                    continue;
                }

                f++;
                var c2 = vl2.C;
                
                Console.WriteLine((c1, c2, vl1.R, vl2.R));

                var horDist = c2 - c1 - 1;
                totalSurface += horDist * (intersection.E - intersection.S);

                /*
                for (var i = intersection.S; i < intersection.E; i++)
                {
                    for (var j = c1 ; j < c2+1; j++)
                    {
                        grid[i][j] = '#';
                    }
                }
                if(intersection.S == intersection.E)
                {
                    for (var j = c1 ; j < c2+1; j++)
                    {
                        grid[intersection.S][j] = '#';
                    }
                }
                #1#

                verticalLines.Remove(vl1);
                verticalLines.Remove(vl2);
                
                if (prefix.E - prefix.S >= 0)
                {
                    if (RangeContains(vl1.R, prefix))
                    {
                        verticalLines.Add((c1, prefix, null));
                    }
                    if (RangeContains(vl2.R, prefix))
                    {
                        verticalLines.Add((c2, prefix, null));
                    }
                }
                if (suffix.E - suffix.S >= 0)
                {
                    if (RangeContains(vl1.R, suffix))
                    {
                        verticalLines.Add((c1, suffix, null));
                    }
                    if (RangeContains(vl2.R, suffix))
                    {
                        verticalLines.Add((c2, suffix, null));
                    }
                }
                break;
            }
            grid.PrintGrid();
            if (f==0)
            {
                Console.WriteLine();
            }
        }
        
        AssignAnswer2(totalSurface);
        
        foreach (var vt in lines.Where(x => x.Dir is Dir.D or Dir.U).OrderBy(x => Math.Min(x.Rows.S, x.Rows.E)))
        {
            var s = Math.Min(vt.Rows.S, vt.Rows.E);
            var e = Math.Max(vt.Rows.S, vt.Rows.E);

            for (var i = s; i < e; i++)
            {
                //grid[i][vt.Cols.S] = '|';

            }
        }
        
        
        grid.PrintGrid();
        return Answers;


        AssignAnswer2(totalSurface);


        return Answers;
    */
    }

    private static bool RangeContains(Range r1, Range r2) => (r1.S <= r2.S && r1.E >= r2.E);

    private static bool GetIntersection(Range r1, Range r2, out Range prefix, out Range intersection, out Range suffix)
    {
        var greatestStart = Math.Max(r1.S, r2.S);
        var smallestEnd = Math.Min(r1.E, r2.E);
        
        intersection = new Range(0, 0);
        prefix = new Range(0, 0);
        suffix = new Range(0, 0);

        if (greatestStart > smallestEnd)
        {
            return false;
        }

        intersection = new Range(greatestStart, smallestEnd);
        prefix = new Range(Math.Min(r1.S, r2.S), intersection.S - 1);
        suffix = new Range(intersection.E + 1, Math.Max(r1.E, r2.E));
        return true;
    }
}