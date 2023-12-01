void RunChallengeSolutions(bool onlyRunLastChallenge)
{
    var challenges = new List<Func<string[]>>
    {
        new Challenges2019.Solution1().Solve,

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
