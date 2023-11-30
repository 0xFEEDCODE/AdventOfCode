using Framework;

namespace Challenges2022;

public class Solution2 : SolutionFramework
{
    public Solution2() : base(2) { }

    private enum Shape
    {
        Rock, Paper, Scissors
    }
    private enum Outcome
    {
        Win, Loss, Draw
    }

    public override string[] Solve()
    {
        var scoringOutcome = new Dictionary<Outcome, int>()
        {
            { Outcome.Win, 6 },
            { Outcome.Loss, 0 },
            { Outcome.Draw, 3 },
        };
        var scoringShape = new Dictionary<Shape, int>()
        {
            { Shape.Rock, 1 },
            { Shape.Paper, 2 },
            { Shape.Scissors, 3 },
        };

        var mappings = new Dictionary<string, int>()
        {
            { "A X", scoringOutcome[Outcome.Draw] + scoringShape[Shape.Rock] },
            { "B Y", scoringOutcome[Outcome.Draw] + scoringShape[Shape.Paper] },
            { "C Z", scoringOutcome[Outcome.Draw] + scoringShape[Shape.Scissors] },

            { "B X", scoringOutcome[Outcome.Loss] + scoringShape[Shape.Rock] },
            { "C X", scoringOutcome[Outcome.Win] + scoringShape[Shape.Rock] },

            { "A Y", scoringOutcome[Outcome.Win] + scoringShape[Shape.Paper] },
            { "C Y", scoringOutcome[Outcome.Loss] + scoringShape[Shape.Paper] },

            { "B Z", scoringOutcome[Outcome.Win] + scoringShape[Shape.Scissors] },
            { "A Z", scoringOutcome[Outcome.Loss] + scoringShape[Shape.Scissors] },
        };

        var totalScore = 0;
        foreach (var line in RawInput.SplitByNewline())
        {
            if (line != String.Empty)
            {
                totalScore += mappings[line];
            }
        }

        AssignAnswer1(totalScore);

        mappings = new Dictionary<string, int>()
        {
            { "A X", scoringOutcome[Outcome.Loss] + scoringShape[Shape.Scissors] },
            { "B Y", scoringOutcome[Outcome.Draw] + scoringShape[Shape.Paper] },
            { "C Z", scoringOutcome[Outcome.Win] + scoringShape[Shape.Rock] },

            { "B X", scoringOutcome[Outcome.Loss] + scoringShape[Shape.Rock] },
            { "C X", scoringOutcome[Outcome.Loss] + scoringShape[Shape.Paper] },

            { "A Y", scoringOutcome[Outcome.Draw] + scoringShape[Shape.Rock] },
            { "C Y", scoringOutcome[Outcome.Draw] + scoringShape[Shape.Scissors] },

            { "B Z", scoringOutcome[Outcome.Win] + scoringShape[Shape.Scissors] },
            { "A Z", scoringOutcome[Outcome.Win] + scoringShape[Shape.Paper] },
        };

        totalScore = 0;
        foreach (var line in RawInput.SplitByNewline())
        {
            if (line != String.Empty)
            {
                totalScore += mappings[line];
            }
        }

        AssignAnswer2(totalScore);

        return Answers;
    }
}
