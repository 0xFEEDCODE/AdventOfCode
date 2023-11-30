void RunChallengeSolutions(bool onlyRunLastChallenge)
{
    var challenges = new List<Func<string[]>>
    {
        /*
        new Challenges2022.Solution1().Solve,
        new Challenges2022.Solution2().Solve,
        new Challenges2022.Solution3().Solve,
        new Challenges2022.Solution4().Solve,
        new Challenges2022.Solution5().Solve,
        new Challenges2022.Solution6().Solve,
        new Challenges2022.Solution7().Solve,
        new Challenges2022.Solution8().Solve,
        new Challenges2022.Solution9().Solve,
        new Challenges2022.Solution10().Solve,
        new Challenges2022.Solution11().Solve,
        new Challenges2022.Solution12().Solve,
        new Challenges2022.Solution13().Solve,
        new Challenges2022.Solution14().Solve,
        new Challenges2022.Solution15().Solve,
        new Challenges2022.Solution16().Solve,
        new Challenges2022.Solution17().Solve,
        new Challenges2022.Solution18().Solve,
        new Challenges2022.Solution19().Solve,
        new Challenges2022.Solution20().Solve,
        new Challenges2022.Solution21().Solve,
        new Challenges2022.Solution22().Solve,
        new Challenges2022.Solution23().Solve,
        new Challenges2022.Solution24().Solve,
        */
        new Challenges2022.Solution25().Solve,
        // INSERTION POINT MARKER (Used with script)
    };

    // Time to get cooking 🍳
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
