using Framework;

namespace Challenges2022;

public class Solution6 : SolutionFramework
{
    public Solution6() : base(6) { }

    public override string[] Solve()
    {
        var fourteenLetters = new char[4];
        var i = 0;
        foreach (var letter in RawInput)
        {
            fourteenLetters[i % 4] = letter;
            if (i >= 3 && fourteenLetters.HasNumberOfDistinct(4))
            {
                AssignAnswer1(i+1);
                break;
            }

            i++;
        }

        fourteenLetters = new char[14];
        i = 0;
        foreach (var letter in RawInput)
        {
            fourteenLetters[i % 14] = letter;
            if (i >= 13 && fourteenLetters.HasNumberOfDistinct(14))
            {
                AssignAnswer2(i+1);
                break;
            }

            i++;
        }

        return Answers;
    }
}
