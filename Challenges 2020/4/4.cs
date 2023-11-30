using Framework;

namespace Challenges2020;

public class Solution4 : SolutionFramework {
    public Solution4() : base(4) { }

    record struct Passport(Dictionary<string, string> Fields);
    public override string[] Solve() {
        var passports = new List<Passport>();
        var requiredFields = new[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
        
        var fields = new Dictionary<string, string>();
        foreach (var line in RawInputSplitByNl) {
            if (line == String.Empty) {
                if (fields.Count != 0) {
                    passports.Add(new Passport(fields));
                    fields = new Dictionary<string, string>();
                }

                continue;
            }
            
            var splitByFields = line.Split(' ');
            foreach (var field in splitByFields) {
                var fieldSplit = field.Split(":");
                fields.Add(fieldSplit[0], fieldSplit[1]);
            }
        }
        //save Last entry
        if (fields.Count != 0) {
            passports.Add(new Passport(fields));
        }
        AssignAnswer1(passports.Count(p => { return requiredFields.All(rf => p.Fields.Keys.Contains(rf)); }));

        var validPasswords = 0;
        var passportsWithReqFields = passports.Where(p => { return requiredFields.All(rf => p.Fields.Keys.Contains(rf)); });
        foreach (var pass in passportsWithReqFields) {
            var isValid = true;
            foreach (var field in pass.Fields) {
                var val = 0;
                isValid = (field.Key) switch {
                    "byr" => ValidateYear(field.Value, 1920, 2002),
                    "iyr" => ValidateYear(field.Value, 2010, 2020),
                    "eyr" => ValidateYear(field.Value, 2020, 2030),
                    "hgt" => ValidateHeight(field.Value),
                    "hcl" => ValidateHairColor(field.Value),
                    "ecl" => ValidateEyeColor(field.Value),
                    "pid" => ValidatePasswordId(field.Value),
                    "cid" => true,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!isValid) {
                    break;
                }
            }

            if (isValid) {
                validPasswords++;
            }
        }
        AssignAnswer2(validPasswords);
        
        
        return Answers;
    }

    private static bool ValidatePasswordId(string value) => value.Length == 9 && value.All(char.IsDigit);

    private static bool ValidateHairColor(string value) {
        if (value[0] != '#') {
            return false;
        }
        value = value.TrimStart('#');
        return value.Length == 6 && value.All(x => char.IsDigit(x) || (x is >= 'a' and <= 'f'));
    }

    private static readonly string[] _eyeColors = new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
    private static bool ValidateEyeColor(string value) => _eyeColors.Contains(value);

    private static bool ValidateHeight(string value) {
        if (value.Contains("in")) {
            if (int.TryParse(value[..^2], out var height)) {
                return height is >= 59 and <= 76;
            }

            return false;
        }
        
        if (value.Contains("cm")) {
            if (int.TryParse(value[..^2], out var height)) {
                return height is >= 150 and <= 193;
            }

            return false;
        }

        return false;
    }

    private static bool ValidateYear(string value, int lowLimit, int highLimit) {
        if (int.TryParse(value, out var val)) {
            return val >= lowLimit && val <= highLimit;
        }

        return false;
    }
}
