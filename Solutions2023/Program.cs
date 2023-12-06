void RunChallengeSolutions(bool onlyRunLastChallenge)
{
    var challenges = new List<Func<string[]>>
    {
        new Solutions2023.Solution1().Solve,
        new Solutions2023.Solution2().Solve,
        new Solutions2023.Solution3().Solve,
        new Solutions2023.Solution4().Solve,
        new Solutions2023.Solution5().Solve,
        new Solutions2023.Solution6().Solve,
        // INSERTION POINT MARKER (Used with script)
    };

    for (var i = 0; i < challenges.Count; i++)
    {
        if (onlyRunLastChallenge && i != challenges.Count - 1)
            continue;

        var answers = challenges[i].Invoke();

        Console.WriteLine($@"
                {'\r'}-----------------------
                {'\r'}Solution {i + 1}:
                {'\r'}Answer 1: {answers[0]}
                {'\r'}Answer 2: {answers[1]}");
    }
}

RunChallengeSolutions(true);