using System.Diagnostics;

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
        new Solutions2023.Solution7().Solve,
        new Solutions2023.Solution9().Solve,
        new Solutions2023.Solution8().Solve,
        new Solutions2023.Solution10().Solve,
        new Solutions2023.Solution11().Solve,
        new Solutions2023.Solution12().Solve,
        new Solutions2023.Solution13().Solve,
        new Solutions2023.Solution14().Solve,
        new Solutions2023.Solution15().Solve,
        new Solutions2023.Solution16().Solve,
        new Solutions2023.Solution17().Solve,
        new Solutions2023.Solution18().Solve,
        // INSERTION POINT MARKER (Used with script)
    };

    var st = new Stopwatch();
    for (var i = 0; i < challenges.Count; i++)
    {
        if (onlyRunLastChallenge && i != challenges.Count - 1)
            continue;

        st.Start();
        var answers = challenges[i].Invoke();
        st.Stop();

        Console.WriteLine($@"
                {'\r'}-----------------------
                {'\r'}Solution {i + 1}:
                {'\r'}Answer 1: {answers[0]}
                {'\r'}Answer 2: {answers[1]}");
        Console.WriteLine($"\nExecution time: {st.Elapsed:m\\:ss\\.ff}");
    }
}

RunChallengeSolutions(true);