using System.Diagnostics;
using System.Numerics;
using System.Text;

using Framework;

namespace Challenges2022;

public class Solution25 : SolutionFramework {
    public Solution25() : base(25) { }
    
    public override string[] Solve() {
        long sum = 0;
        var nums = new List<long>();
        foreach (var line in InputNlSplit) {
            var num = line.Reverse();
            var c = Calc(new string(num.ToArray()));
            nums.Add(c);
            sum += c;
        }

        var ans = new List<char>();
        long ansSum = 0;
        long mul = 1;
        while (ansSum < sum) {
            ans.Add('2');
            ansSum += mul * 2;
            mul *= 5;
        }

        var pos = ans.Count-1;
        var ansValue = Calc(new string(ans.ToArray()));


        var r = decToSnafuWeird();
        Console.WriteLine();
        
        string decToSnafuWeird() {
            var resSB = new StringBuilder();
            while (ansValue != sum) {
                if (pos < 0) {
                    pos = ans.Count - 1;
                }

                ansValue = Calc(new string(ans.ToArray()));
                if (ansValue > sum) {
                    if (ans[pos] == '=') {
                        pos--;
                        continue;
                    }

                    var beforeChange = ans[pos];
                    var change = ans[pos] switch {
                        '2' => '1',
                        '1' => '0',
                        '0' => '-',
                        '-' => '=',
                    };
                    ans[pos] = change;
                    ansValue = Calc(new string(ans.ToArray()));
                    if (ansValue < sum) {
                        ans[pos] = beforeChange;
                        pos--;
                    }
                }
            }

            foreach (var item in ans) {
                resSB.Insert(0, item);
            }

            return resSB.ToString();
        }

        string decToSnafuNormal(long n) {
            var resSB = new StringBuilder();
            while (n > 0) {
                var rem = n % 5;
                n /= 5;
                resSB.Insert(0,
                    rem switch {
                        0 => '0',
                        1 => '1',
                        2 => '2',
                        3 => '=',
                        4 => '-',
                    }
                );
                if (rem > 2)
                    n += 1;
            }

            return resSB.ToString();
        }

        return Answers;
    }


    private static long Calc(string num) {
        long mul = 1;
        long numInDec = 0;
        foreach (var digit in num) {
            var dec = digit switch {
                '=' => -2 * mul,
                '-' => -1 * mul,
                _ => int.Parse(digit.ToString()) * mul
            };
            numInDec += dec;
            mul *= 5;
        }

        return numInDec;
    }
}
