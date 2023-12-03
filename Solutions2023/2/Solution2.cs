using Framework;

namespace Solutions2023;

public class Solution2 : SolutionFramework
{
    public Solution2() : base(2) { }
    
    public override string[] Solve()
    {
        Part1(); 
        Part2();
        return Answers;
    }

    enum Color { red, green, blue };

    record Set(Color Color, int Quantity);

    static bool isPossible(List<Set> sets)
    {
        var bl = sets.Where(x => x.Color is Color.blue).Max(x => x.Quantity);
        var gr = sets.Where(x => x.Color is Color.green).Max(x => x.Quantity);
        var re = sets.Where(x => x.Color is Color.red).Max(x => x.Quantity);
        return (bl <= 14 && gr <= 13 && re <= 12);
    }

    static (int Blue, int Green, int Red) Fewest(List<Set> sets)
    {
        var bl = sets.Where(x => x.Color is Color.blue).Max(x => x.Quantity);
        var gr = sets.Where(x => x.Color is Color.green).Max(x => x.Quantity);
        var re = sets.Where(x => x.Color is Color.red).Max(x => x.Quantity);
        return (bl, gr, re);
    }
    
    private void Part1()
    {
        var i = 1;
        foreach (var l in RawInputSplitByNl)
        {
            var sets = new List<Set>();
            
            foreach (var s in l.Split(','))
            {
                var w = s.Split(' ');
                var color = (Color)Enum.Parse(typeof(Color), w[^1], true);
                var quantity = int.Parse(w[^2]);
                sets.Add(new Set(color, quantity));
            }
            if (isPossible(sets))
            {
                NumSlot += i;
            }
            
            i++;
        }
        
        AssignAnswer1(NumSlot);
    }
    
    private void Part2()
    {
        var i = 1;
        foreach (var l in RawInputSplitByNl)
        {
            var sets = new List<Set>();
            
            foreach (var s in l.Split(','))
            {
                var w = s.Split(' ');
                var color = (Color)Enum.Parse(typeof(Color), w[^1], true);
                var quantity = int.Parse(w[^2]);
                sets.Add(new Set(color, quantity));
            }
            var f = Fewest(sets);
            NumSlot += f.Blue * f.Green * f.Red;
            
            i++;
        }
        
        AssignAnswer2(NumSlot, true);
    }
}