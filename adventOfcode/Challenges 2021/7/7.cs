using Framework;

namespace Challenges2021;

public class Solution7 : SolutionFramework {
    public Solution7() : base(7) { }

    public override string[] Solve() {
        var positions = RawInputSplitByNl.Single().Split(',').Select(long.Parse).ToList();

        long smallestFuelCost = int.MaxValue;
        foreach (var pos in positions) {
            var fuelCost = positions.Where(p => p != pos).Sum(otherPos => Math.Abs(pos - otherPos));
            smallestFuelCost.AssignIfLower(fuelCost);
        }
        AssignAnswer1(smallestFuelCost);
        
        //pt2
        smallestFuelCost = int.MaxValue;
        for(var pos = positions.Min(); pos < positions.Max(); pos++) {
            var fuelCost = positions.Where(p => p != pos).Sum(otherPos => {
                var n = Math.Abs(pos - otherPos);
                return (n * (n + 1)) / 2;
            });
            smallestFuelCost.AssignIfLower(fuelCost);
        }
        AssignAnswer2(smallestFuelCost);
        
        return Answers;
    }
}
