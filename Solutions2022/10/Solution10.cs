using Framework;

namespace Challenges2022;


public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }
    
    struct Sprite
    {
        public int StartPos => Pos-1;
        public int Pos;
        public int EndPos => Pos+1;
        public int[] AllPos => new int[] { StartPos, Pos, EndPos };

        public Sprite(int pos)
        {
            Pos = pos;
        }
    }

    public override string[] Solve()
    {
        bool Magic(List<int> ints, int cycle1, bool added, List<int> list, int regX1)
        {
            if (ints.Contains(cycle1))
            {
                if (!added) list.Add(regX1 * cycle1);
                added = true;
            }

            return added;
        }

        var regX = 1;
        var mul = new List<int>()
        {
            20,
            60,
            100,
            140,
            180,
            220
        };
        var ans = new List<int>();

        var cycle = 0;
        foreach (var line in RawInput.SplitByNewline())
        {
            var added = false;
            if(line.StartsWith("noop"))
                cycle++;
            else if (line.StartsWith("addx"))
            {
                cycle++;
                added = Magic(mul, cycle, added, ans, regX);

                cycle++;
                added = Magic(mul, cycle, added, ans, regX);
                regX += int.Parse(line.Split(" ").Last());
            }
            Magic(mul, cycle, added, ans, regX);
        }

        AssignAnswer1(ans.Sum());

        regX = 1;
        var crt = (6, 40).CreateGrid<char>();
        crt.ForEachCell((i,j)=>crt[i][j]='.');


        cycle = 0;
        var sprite = new Sprite(regX);
        foreach (var line in RawInput.SplitByNewline())
        {
            var added = false;
            if (line.StartsWith("noop"))
            {
                updateCrt(crt, sprite, cycle);
                cycle++;
            }
            else if (line.StartsWith("addx"))
            {
                updateCrt(crt, sprite, cycle);
                cycle++;
                
                updateCrt(crt, sprite, cycle);
                cycle++;
                
                regX += int.Parse(line.Split(" ").Last());
                sprite = new Sprite(regX);
                
            }
        }
        
        printCrt(crt);
        var answ2 = new string(crt.SelectMany(x => new string(x.ToArray())+"\n").ToArray());
        Answers[1] = answ2;

        return Answers;
    }

    void updateCrt(char[][] crt, Sprite sprite, int pos)
    {
        var _pos = 0;
        for (var i = 0; i < crt.Length; i++)
        {
            for (var j = 0; j < crt[0].Length; j++)
            {
                if (pos == _pos)
                {
                    if (sprite.AllPos.Contains(pos%40))
                    {
                        crt[i][j] = '#';
                        return;
                    }

                    return;
                }

                _pos++;
            }
        }
    }
    
    void printCrt(char[][] crt)
    {
        var pos = 0;
        for (var i = 0; i < crt.Length; i++)
        {
            Console.WriteLine("");
            for (var j = 0; j < crt[0].Length; j++)
            {
                var c = crt[i][j];
                Console.Write(c);

                pos++;
            }
        }
    }
}
