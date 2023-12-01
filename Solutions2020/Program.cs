void RunChallengeSolutions(bool onlyRunLastChallenge)
{
    var challenges = new List<Func<string[]>>
    {
        new Challenges2020.Solution1().Solve,
        new Challenges2020.Solution2().Solve,
        new Challenges2020.Solution3().Solve,
        new Challenges2020.Solution4().Solve,
        new Challenges2020.Solution5().Solve,
        new Challenges2020.Solution6().Solve,
        new Challenges2020.Solution7().Solve,
        new Challenges2020.Solution8().Solve,
        new Challenges2020.Solution9().Solve,
        new Challenges2020.Solution10().Solve,
        new Challenges2020.Solution11().Solve,
        new Challenges2020.Solution12().Solve,
        new Challenges2020.Solution13().Solve,
        new Challenges2020.Solution14().Solve,
        new Challenges2020.Solution15().Solve,
        new Challenges2020.Solution16().Solve,
        new Challenges2020.Solution17().Solve,
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
