using Framework;


namespace Challenges2019;

public class Solution10 : SolutionFramework
{
    public Solution10() : base(10) { }
    
    enum Field { Empty, Asteroid }

    public override string[] Solve()
    {
        var temp = InpGr<char>();

        var gr = (temp.Length, temp[0].Length).CreateGrid<Field>();

        for (var i = 0; i < temp.Length; i++)
        {
            for (var j = 0; j < temp[i].Length; j++)
            {
                gr[i][j] = temp[i][j] == '#' ? Field.Asteroid : Field.Empty;
            }
        }

        var score = new List<(Pos2D pos, int score)>();

        for (var i = 0; i < gr.Length; i++)
        {
            for (var j = 0; j < gr[0].Length; j++)
            {
                var pos = new Pos2D(i, j);
                if (gr.GetCell(pos) is Field.Asteroid)
                {
                    
                    
                }
            }
        }


        void Find(Pos2D pos)
        {
            var adjacents = gr.GetAllAdjacentCells(pos, true);
            foreach (var adj in adjacents)
            {
                Find(adj.pos);
            }
            
        }
        
        
        return Answers;
    }
    
}

