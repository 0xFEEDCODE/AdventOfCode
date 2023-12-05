using System.Collections;
using System.Numerics;
using System.Text.RegularExpressions;

using Framework;

namespace Challenges2022;

public static partial class Ext
{
}

public partial class Solution21 : SolutionFramework {
    public Solution21() : base(21) {
    }

    public override string[] Solve() {
        var dict = new Dictionary<string, Func<long>>();
        
        const string pattern = @"(?<x>.+) (?<y>.+) (?<z>.+)";
        foreach (var line in InputNlSplit) {
            var split = line.Split(':');
            var name = split[0];
            var value = split[1].Trim();
            if (char.IsDigit(value[0])) {
                dict.Add(name, () => long.Parse(value));
            } else {
                var r = Regex.Match(value, pattern).Groups;
                var n1 = r[1].Value;
                var op = r[2].Value;
                var n2 = r[3].Value;
                switch (op) {
                    case "+":
                        dict.Add(name, () => dict[n1].Invoke() + dict[n2].Invoke());
                        break;
                    case "-":
                        dict.Add(name, () => dict[n1].Invoke() - dict[n2].Invoke());
                        break;
                    case "*":
                        dict.Add(name, () => dict[n1].Invoke() * dict[n2].Invoke());
                        break;
                    case "/":
                        dict.Add(name, () => dict[n1].Invoke() / dict[n2].Invoke());
                        break;
                    case "=":
                        dict.Add(name, () => {
                            long num1 = dict[n1].Invoke();
                            long num2 = dict[n2].Invoke();
                            //Console.WriteLine($"N1 {num1} == N2 {num2}");
                            return (num1==num2) ? 0 : Math.Abs(num1-num2);
                        });
                        break;
                }
            }
        }

        var ans = dict["root"].Invoke();
        Console.WriteLine();
        for (long i = 3721298272960; i > 0; i-=1) {
            dict["humn"] = () => i;
            var r = dict["root"].Invoke();

            if (r == 0) {
                Console.WriteLine();
                Console.WriteLine("ANSWER IS : " + i);
            }

        }
        Console.WriteLine();
        return Answers;
    }
}
