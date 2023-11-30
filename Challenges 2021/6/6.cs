using System.Text.RegularExpressions;

using Framework;

namespace Challenges2021;

public class Solution6 : SolutionFramework {
    public Solution6() : base(6) { }

    public class LanternFish {
        public int Timer;

        public LanternFish? DailyTurn() {
            Timer--;
            var spawned = Timer < 0 ? new LanternFish(8) : null;
            if(spawned != null){
                Timer = 6;
            }

            return spawned;
        }

        public LanternFish(int timer = 6) {
            Timer = timer;
        }
    }

    public override string[] Solve() {
        var fishes = new List<LanternFish>();
        foreach (var line in RawInputSplitByNl) {
            foreach (var n in Regex.Matches(line, @"\d+").Select(x => int.Parse(x.Value))) {
                fishes.Add(new LanternFish(n));
            }
        }
        var fishesOriginal = fishes.Select(x=>new LanternFish(x.Timer)).ToList();

        for (var i = 0; i < 18; i++) {
            fishes.ToList().ForEach(f => {
                //Console.Write($"{f.Timer},");
                var spawned = f.DailyTurn();
                if(spawned != null)
                    fishes.Add(spawned);
            });
        }
        AssignAnswer1(fishes.Count);
        
        //pt2
        var fishesDict = new Dictionary<int, long>();
        for (var i = 0; i <= 8; i++) {
            fishesDict.Add(i, 0);
        }
        foreach (var f in fishesOriginal) {
            fishesDict[f.Timer]++;
        }

        for (var i = 0; i < 256; i++) {
            var carryOver = fishesDict[8];
            for (var j = 7; j > 0; j--) {
                (fishesDict[j], carryOver) = (carryOver, fishesDict[j]);
            }
            fishesDict[8] = fishesDict[0];
            fishesDict[6] += fishesDict[0];
            fishesDict[0] = carryOver;
        }

        AssignAnswer2(fishesDict.Values.Sum());
        
        return Answers;
    }
}
