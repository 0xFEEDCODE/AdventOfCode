using System.Text;

using Framework;

namespace Challenges2021;

public class Solution3 : SolutionFramework {
    public Solution3() : base(3) { }

    enum Bit { Zero, One }

    public override string[] Solve() {
        var bitsByCol = new Dictionary<int, List<Bit>>();
        var bitsByRow = new Dictionary<int, string>();
        var row = 0;
        foreach (var line in RawInputSplitByNl) {
            for (var i = 0; i < line.Length; i++) {
                if (!bitsByCol.ContainsKey(i)) {
                    bitsByCol.Add(i, new List<Bit>());
                }
                bitsByCol[i].Add(line[i] switch { '0' => Bit.Zero, '1' => Bit.One });
            }
            bitsByRow.Add(row++, line);
        }

        var bitStringSb = new StringBuilder();
        foreach (var kv in bitsByCol) {
            var zeroC = kv.Value.Count(x => x == Bit.Zero);
            var oneC = kv.Value.Count(x => x == Bit.One);
            bitStringSb.Append((zeroC > oneC) switch { true => '0', false => '1' });
        }

        var bitString = bitStringSb.ToString();
        var gRate = binToDec(bitString);
        var eRate = binToDec(bitString.Select(x => x == '1' ? '0' : '1'));
        var pConsumption = gRate * eRate;
        AssignAnswer1(pConsumption);

        var consideredORating = new List<int>();
        var consideredSRating = new List<int>();
        for (var i = 0; i < bitsByRow.Keys.Count; i++) {
            consideredORating.Add(i);
            consideredSRating.Add(i);
        }

        var bitCol = 0;
        while (!(consideredORating.Count == 1 && consideredSRating.Count == 1)) {
            var oneC = consideredORating.Count(bitRow => bitsByRow[bitRow][bitCol] == '1');
            var zeroC = consideredORating.Count - oneC;
            if (oneC == zeroC) {
                oneC++;
            }
            var mcb = (oneC > zeroC) switch { true => Bit.One, _ => Bit.Zero };

            if (consideredORating.Count != 1) {
                foreach (var bitRow in consideredORating.ToList()) {
                    switch (mcb) {
                        case Bit.One when bitsByRow[bitRow][bitCol] != '1':
                        case Bit.Zero when bitsByRow[bitRow][bitCol] != '0':
                            consideredORating.Remove(bitRow);
                            break;
                    }
                }
            }
            
            oneC = consideredSRating.Count(bitRow => bitsByRow[bitRow][bitCol] == '1');
            zeroC = consideredSRating.Count - oneC;
            if (oneC == zeroC) {
                oneC++;
            }
            mcb = (oneC > zeroC) switch { true => Bit.One, _ => Bit.Zero };

            if (consideredSRating.Count != 1) {
                foreach (var bitRow in consideredSRating.ToList()) {
                    switch (mcb) {
                        case Bit.One when bitsByRow[bitRow][bitCol] != '0':
                        case Bit.Zero when bitsByRow[bitRow][bitCol] != '1':
                            consideredSRating.Remove(bitRow);
                            break;
                    }
                }
            }

            bitCol++;
        }

        var rO = bitsByRow[consideredORating.Single()];
        var rS = bitsByRow[consideredSRating.Single()];
        var oRating = binToDec(bitsByRow[consideredORating.Single()]);
        var sRating = binToDec(bitsByRow[consideredSRating.Single()]);

        var lsRating = oRating * sRating;
        AssignAnswer2(lsRating);
        
        return Answers;
    }

    private static long binToDec(IEnumerable<char> binaryN) {
        long res = 0;
        long mul = 1;
        foreach (var bit in binaryN.Reverse()) {
            if (bit == '1')
                res += mul;
            mul *= 2;
        }

        return res;
    }
}
