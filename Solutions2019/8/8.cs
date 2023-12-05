using Framework;

namespace Challenges2019;

public class Solution8 : SolutionFramework
{
    public Solution8() : base(8) { }
    
    public override string[] Solve()
    {
        var layers = new List<int[][]>();
        
        var w = 25;
        var h = 6;
        var layerLen = h * w;

        var inp = InpR.Slice(layerLen).ToArray();
        
        for (var idx = 0; idx < inp.Count(); idx++)
        {
            var layer = (h, w).CreateGrid<int>();
            layers.Add(layer);
            
            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    layer[i][j] = inp[idx].ElementAt((i*w)+j).ParseInt();
                }
            }
        }

        var flattened = layers.Select(l => l.SelectMany(x => x)).ToArray();
        var fewest0 = flattened.OrderBy(x => x.Count(y => y == 0)).First().ToArray();
        NSlot = fewest0.Count(x => x == 1) * fewest0.Count(x => x == 2);
        AssignAnswer1();

        flattened = flattened.Reverse().ToArray();
        var resultingLayer = flattened[0].ToArray();
        
        for (var idx = 1; idx < flattened.Length; idx++)
        {
            var currentLayer = flattened[idx].ToArray();
            for (var i = 0; i < layerLen; i++)
            {
                var nv = currentLayer[i];

                if (nv is 2)
                {
                    continue;
                }
                resultingLayer[i] = nv;
            }
        }

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        for (var i = 0; i < h; i++)
        {
            for (var j = 0; j < w; j++)
            {
                var v = resultingLayer[(i * w) + j];
                var outp = v is 0 ? ' ' : '\u2588';
                Console.Write(outp);
            }
            Console.WriteLine();
        }
        
        
        return Answers;
    }
}

