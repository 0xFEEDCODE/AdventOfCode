using Framework;

namespace Challenges2020;

public class Solution15 : SolutionFramework
{
    public Solution15() : base(15) { }


    record SpokenNumber(int Number, SpokenNumberHistory History);
    class SpokenNumberHistory
    {
        public SpokenNumberHistory(int spokenFirstTime, int? spokenLastTime = null)
        {
            SpokenFirstTime = spokenFirstTime;
            SpokenLastTime = spokenLastTime;
        }
        public int SpokenFirstTime;
        public int? SpokenLastTime;
    };
    
    public override string[] Solve()
    {
        var game = new Dictionary<int, SpokenNumberHistory>();
        var round = 1;
        foreach (var line in RawInput.Split(','))
        {
            game.Add(int.Parse(line), new SpokenNumberHistory(round));
            round++;
        }

        var lastSpoken = new SpokenNumber(0, game.Values.Last());
        
        while (round <= 30000000)
        {
            var numberSpoken = 0;
            if (lastSpoken.History.SpokenLastTime != null)
            {
                numberSpoken = lastSpoken.History.SpokenLastTime.Value - lastSpoken.History.SpokenFirstTime;
                lastSpoken.History.SpokenFirstTime = lastSpoken.History.SpokenLastTime.Value;
            }


            SpokenNumberHistory numberHistory;
            if (game.ContainsKey(numberSpoken))
            {
                game[numberSpoken].SpokenLastTime = round;
                numberHistory = game[numberSpoken];
            }
            else
            {
                var newEntry = new SpokenNumberHistory(round);
                game.Add(numberSpoken, newEntry);
                numberHistory = newEntry;
            }

            
            lastSpoken = new SpokenNumber(numberSpoken, numberHistory);
            switch (round)
            {
                case 2020:
                    AssignAnswer1(lastSpoken.Number);
                    break;
                case 30000000:
                    AssignAnswer2(lastSpoken.Number);
                    break;
            }
            
            round++;
        }
        
        return Answers;
    }
}
