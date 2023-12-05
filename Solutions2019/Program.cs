void RunChallengeSolutions(bool onlyRunLastChallenge)
{
    var challenges = new List<Func<string[]>>
    {
        new Challenges2019.Solution1().Solve,
         new Challenges2019.Solution2().Solve,
        new Challenges2019.Solution3().Solve,
        new Challenges2019.Solution4().Solve,
        new Challenges2019.Solution5().Solve,
        new Challenges2019.Solution6().Solve,
        new Challenges2019.Solution7().Solve,
        new Challenges2019.Solution8().Solve,
    };

    for (int i = 0; i < challenges.Count; i++)
    {
        if (onlyRunLastChallenge && i != challenges.Count - 1)
            continue;

        var answers = challenges[i].Invoke();

        Console.WriteLine($@"
                {'\r'}-----------------------
                {'\r'}Challenge {i+1}:
                {'\r'}Answer 1: {answers[0]}
                {'\r'}Answer 2: {answers[1]}");
    }
}

RunChallengeSolutions(true);
