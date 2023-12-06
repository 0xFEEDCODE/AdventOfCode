using Framework;

namespace Solutions2023;

public class Solution6 : SolutionFramework
{
    public Solution6() : base(6) { }

    record Race(double Time, double Record);
    record State(double Speed = 0, double TimeTaken = 0, double Progress = 0, double ButtonHolds = 0);
    record Range(double Start, double End);
    
    public override string[] Solve()
    {
        var times = InpNl[0].Split(" ").Where(x => x.Length > 0 && Char.IsDigit(x[0])).Select(double.Parse).ToArray();
        var distances = InpNl[1].Split(" ").Where(x => x.Length > 0 && Char.IsDigit(x[0])).Select(double.Parse).ToArray();
        var races = new Race[times.Length];
        
        for (var i = 0; i < times.Count(); i++)
        {
            races[i] = new Race(times[i], distances[i]);
        }

        var l = new object();
        Parallel.For(0, races.Length, (i, _) =>
        {
            var bhRange = GetButtonHoldRange(new State(), races[i].Time, races[i].Record);
            var nWays = bhRange.End - bhRange.Start;
            lock (l)
            {
                if (NSlot is 0)
                {
                    NSlot = nWays;
                } else
                {
                    NSlot *= nWays;
                }
            }
        });

        AssignAnswer1();
        
        var mergedTime = times.Aggregate((acc, x) => double.Parse(acc.ToString() + x));
        var mergedDist = distances.Aggregate((acc, x) => double.Parse(acc.ToString() + x));
        var bhRange = GetButtonHoldRange(new State(), mergedTime, mergedDist);
        NSlot = bhRange.End - bhRange.Start;
        AssignAnswer2();

        return Answers;

        Range GetButtonHoldRange(State initialState, double timeLimit, double recordDistance)
        {
            var stack = new Stack<State>();
            for (var i = 0; i < (recordDistance/timeLimit)/2; i++)
            {
                initialState = HoldButton(initialState);
            }
            stack.Push(initialState);

            var minBh = double.MinValue;
            
            while (stack.Any())
            {
                var state = stack.Pop();

                if (state.TimeTaken > recordDistance)
                {
                    break;
                }
                
                var timeLeft = timeLimit - state.TimeTaken;
                var distLeft = recordDistance - state.Progress;
                var velocity = state.Speed;
                
                if (state.Speed < recordDistance && distLeft >= 0 && timeLeft > 0)
                {
                    if (minBh is double.MinValue)
                    {
                        if (state.Progress == 0)
                        {
                            var m1 = HoldButton(state);
                            stack.Push(m1);
                        }
                        if (timeLeft * velocity > distLeft && state.ButtonHolds > minBh)
                        {
                            var m2 = Move(state);
                            stack.Push(m2);
                        }
                    } else
                    {
                        var hold = HoldButton(state);
                        while(hold.ButtonHolds * (timeLimit - hold.TimeTaken) > recordDistance)
                        {
                            hold = HoldButton(hold);
                        }
                        return new Range(minBh, hold.ButtonHolds);
                    }


                } else if(distLeft < 0)
                {
                    if (state.Progress > recordDistance)
                    {
                        if (minBh is double.MinValue)
                        {
                            minBh = state.ButtonHolds;
                        }
                    }
                }
            }
            throw new InvalidOperationException();
        }

        State HoldButton(State state)
        {
            return state with { TimeTaken = state.TimeTaken+1, Speed = state.Speed+1, ButtonHolds = state.ButtonHolds + 1};
        }
        State Move(State state)
        {
            return state with { TimeTaken = state.TimeTaken+1, Progress = state.Progress + state.Speed};
        }
    }
}