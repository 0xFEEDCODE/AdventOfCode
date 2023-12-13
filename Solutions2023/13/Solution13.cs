using Framework;

namespace Solutions2023;

public class Solution13 : SolutionFramework
{
    public Solution13() : base(13) { }
    
    public override string[] Solve()
    {
        var grids = new List<char[][]>();

        var temp = new List<string>();
        foreach (var s in InpNl)
        {
            if (s != string.Empty)
            {
                temp.Add(s);
            }
            else
            {
                ParseSection(grids, temp);
                temp = new List<string>();
            }
        }
        ParseSection(grids, temp);
        
        List<(string A, int[] B)> conditionRecords = new string[]{}.Select(line => //File.ReadAllLines(@"..\..\..\..\day-12.txt").Select(line =>
        {
            var parts = line.Split(" ");
            //return new { springs = parts[0], groups = parts[1].Split(',').Select(int.Parse).ToArray() };
            return (parts[0], parts[1].Split(',').Select(int.Parse).ToArray());
        }).ToList(); 
        
        var r = new List<int>();
        var c = new List<int>();
        foreach (var gr in grids)
        {
            for (var i = 0; i < gr.Length-1; i++)
            {
                if (IsSame(gr,i, i+1))
                {
                    var offset = 0;
                    var isSame = true;
                    while (i+1+offset < gr.Length-1 && i - ++offset >= 0 && (isSame = IsSame(gr, i-offset, i + 1 + offset))){ }
                    if (isSame)
                    {
                        r.Add(i+1);
                    }
                }
            }
            for (var j = 0; j < gr[0].Length-1; j++)
            {
                var (vl1, vl2) = GetVerticalLines(gr, j, j + 1);
                if(IsSame(vl1, vl2))
                {
                    var offset = 0;
                    var isSame = true;
                    while (j + 1 + offset < gr[0].Length - 1 && j - ++offset >= 0)
                    {
                        (vl1, vl2) = GetVerticalLines(gr, j-offset, j + 1 + offset);
                        isSame = IsSame(vl1, vl2);
                        if (!isSame)
                        {
                            break;
                        }
                    }
                    if (isSame)
                    {
                        c.Add(j+1);
                    }
                }
            }
        }
        Console.WriteLine();
        AssignAnswer1(r.Sum()*100 + c.Sum());
        
        r = new List<int>();
        c = new List<int>();
        foreach (var gr in grids)
        {
            var found = false;
            for (var i = 0; i < gr.Length-1; i++)
            {
                var diffUsed = Has1Diff(gr, i, i + 1);
                if (IsSame(gr,i, i+1) || diffUsed)
                {
                    var offset = 0;
                    var isSame = true;
                    while (i+1+offset < gr.Length-1 && i - ++offset >= 0)
                    {
                        isSame = IsSame(gr, i - offset, i + 1 + offset);
                        if (!isSame)
                        {
                            if (!diffUsed && Has1Diff(gr, i - offset, i + 1 + offset))
                            {
                                isSame = true;
                                diffUsed = true;
                            }
                        }
                        if (!isSame)
                        {
                            break;
                        }
                    }
                    if (isSame && diffUsed)
                    {
                        r.Add(i+1);
                        found = true;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            if (found)
            {
                continue;
            }
            found = false;
            for (var j = 0; j < gr[0].Length-1; j++)
            {
                var (vl1, vl2) = GetVerticalLines(gr, j, j + 1);
                var diffUsed = Has1Diff(vl1,vl2);
                if(IsSame(vl1, vl2) || diffUsed)
                {
                    var offset = 0;
                    var isSame = true;
                    while (j + 1 + offset < gr[0].Length - 1 && j - ++offset >= 0)
                    {
                        (vl1, vl2) = GetVerticalLines(gr, j-offset, j + 1 + offset);
                        isSame = IsSame(vl1 ,vl2);
                        if (!isSame)
                        {
                            if (!diffUsed && Has1Diff(vl1,vl2))
                            {
                                isSame = true;
                                diffUsed = true;
                            }
                        }                    
                        if (!isSame)
                        {
                            break;
                        }
                    }
                    if (isSame && diffUsed)
                    {
                        c.Add(j+1);
                        found = true;
                    }
                }
                if (found)
                {
                    break;
                }
            }
        }
        
        
        AssignAnswer2(r.Sum()*100 + c.Sum());
        return Answers;
    }
    
    private static (char[] vl1, char[] vl2) GetVerticalLines(char[][] gr, int c1, int c2)
    {
        var vl1 = new char[gr.Length];
        var vl2 = new char[gr.Length];
        for (var i = 0; i < gr.Length; i++)
        {
            vl1[i] = gr[i][c1];
            vl2[i] = gr[i][c2];
        }
        return (vl1, vl2);
    }

    private static bool IsSame(char[][] gr, int r1, int r2)
    {
        var s1 = new string(gr[r1]);
        var s2 = new string(gr[r2]);
        return s1 == s2;
    }
    private static bool IsSame(char[] line1, char[] line2)
    {
        var s1 = new string(line1);
        var s2 = new string(line2);
        return s1 == s2;
    }
    
    private static bool Has1Diff(char[][] gr, int r1, int r2)
    {
        var s1 = new string(gr[r1]);
        var s2 = new string(gr[r2]);
        return has1Diff(s1, s2);
    }
    private static bool Has1Diff(char[] line1, char[] line2)
    {
        var s1 = new string(line1);
        var s2 = new string(line2);
        return has1Diff(s1, s2);
    }
    
    private static bool has1Diff(string s1, string s2)
    {
        var diff = 0;
        for (var i = 0; i < s1.Length; i++)
        {
            if (s1[i] != s2[i])
            {
                diff++;
            }
            if (diff > 1)
            {
                return false;

            }
        }
        return diff == 1;
    }

    private static void ParseSection(List<char[][]> grids, List<string> temp)
    {
        grids.Add((temp.Count, temp[0].Length).CreateGrid<char>());
        for (var i = 0; i < temp.Count(); i++)
        {
            for (var j = 0; j < temp[0].Length; j++)
            {
                grids.Last()[i][j] = temp[i][j];
            }
        }
    }
}