void RunChallengeSolutions(bool onlyRunLastChallenge)
{
    var challenges = new List<Func<string[]>>
    {
        // new Challenges2021.Solution25().Solve,
        // new Challenges2021.Solution24().Solve,
        //new Challenges2021.Solution1().Solve,
        //new Challenges2021.Solution2().Solve,
        //new Challenges2021.Solution3().Solve,
        //new Challenges2021.Solution4().Solve,
        //new Challenges2021.Solution5().Solve,
        //new Challenges2021.Solution6().Solve,
        //new Challenges2021.Solution7().Solve,
        //new Challenges2021.Solution8().Solve,
        //new Challenges2021.Solution9().Solve,
        // new Challenges2021.Solution10().Solve,
        // new Challenges2021.Solution11().Solve,
        //new Challenges2021.Solution12().Solve,
        //new Challenges2021.Solution13().Solve,
        //new Challenges2021.Solution14().Solve,
        //new Challenges2021.Solution15().Solve,
        //new Challenges2021.Solution16().Solve,
        //new Challenges2021.Solution17().Solve,
        //new Challenges2021.Solution18().Solve,
        //new Challenges2021.Solution19().Solve,
        //new Challenges2021.Solution20().Solve,
        // new Challenges2021.Solution21().Solve,
        new Challenges2021.Solution22().Solve,
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
