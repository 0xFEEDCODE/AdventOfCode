using System.Net.WebSockets;
using System.Text.RegularExpressions;

using Framework;

namespace Challenges2021;

public class Solution19 : SolutionFramework {
    public Solution19() : base(19) { }

    public record struct Coord3D(int X, int Y, int Z)  {
        public bool Equals(Coord3D other) => (X, Y, Z) == (other.X, other.Y, other.Z);
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);
    }

    public record struct Scanner(int Id, List<ScannerReading> ScannerReadings);
    
    public record struct ScannerReading(Coord3D Coord) {
        public ICollection<Coord3D> Rotations() {
            var rot = new HashSet<Coord3D>();

            rot.Add(Coord with { X = -Coord.X });
            rot.Add(Coord with { X = +Coord.X });
            rot.Add(Coord with { Y = -Coord.Y });
            rot.Add(Coord with { Y = +Coord.Y });
            rot.Add(Coord with { Z = -Coord.Z });
            rot.Add(Coord with { Z = +Coord.Z });

            var previousCount = -1;
            var currentCount = rot.Count;
            while (previousCount != currentCount) {
                previousCount = rot.Count;
                foreach (var r in rot.ToList()) {
                    var c1 = r with { X = -r.X };
                    var c2 = r with { X = +r.X };
                    var c3 = r with { Y = +r.Y };
                    var c4 = r with { Y = -r.Y };
                    var c5 = r with { Z = +r.Z };
                    var c6 = r with { Z = -r.Z };
                    var r1 = r with { X = r.Z, Y = r.X, Z = r.Y};
                    var r2 = r with { X = r.Y, Y = r.Z, Z = r.X};
                    rot.Add(r1);
                    rot.Add(r2);
                    rot.Add(c1);
                    rot.Add(c2);
                    rot.Add(c3);
                    rot.Add(c4);
                    rot.Add(c5);
                    rot.Add(c6);
                }
                currentCount = rot.Count;
            }
            return rot;
        }
    }

    public override string[] Solve() {
        var beaconPos = new Dictionary<Coord3D, int>();
        var scanners = new List<Scanner>();
        var allPossibilities = new List<Coord3D>();

        var scannerNo = 0;
        Scanner scanner = default;
        foreach (var line in RawInputSplitByNl) {
            var matches = Regex.Matches(line, @"\d+");
            if(line.Contains("scanner"))
            {
                if (scanner != default) {
                    scanners.Add(scanner);
                }
                scanner = new Scanner(scannerNo++, new List<ScannerReading>());
            }
            if (matches.Count() == 3) {
                scanner.ScannerReadings.Add(new ScannerReading(
                    new Coord3D(int.Parse(matches.ElementAt(0).Value),
                        int.Parse(matches.ElementAt(1).Value),
                        int.Parse(matches.ElementAt(2).Value)))
                );
            }
        }

        foreach (var sc in scanners) {
            foreach (var sr in sc.ScannerReadings.ToList()) {
                sc.ScannerReadings.AddRange(sr.Rotations().Select(x=>new ScannerReading(x)));
            }
        }
        
        if (scanner != default) {
            scanners.Add(scanner);
        }

        var nBeacons = 0;
        foreach (var sc in scanners) {
            Console.WriteLine($"Processing scanner {sc.Id}");
            for (var x = -500; x <= 500; x++) {
                for (var y = -500; y <= 500; y++) {
                    for (var z = -500; z <= 500; z++) {
                        foreach (var otherScanner in scanners.Where(_sc => _sc != sc)) {
                            var identicalReadings = otherScanner.ScannerReadings
                                .Select(sr => new ScannerReading(new Coord3D(sr.Coord.X + x, sr.Coord.Y + y, sr.Coord.Z + z)))
                                .Intersect(sc.ScannerReadings).ToArray();
                            if (identicalReadings.Count() >= 12) {
                                nBeacons+=identicalReadings.Count();
                                Console.WriteLine("FOUND, "+nBeacons);
                            }
                        }
                    }
                }
                Console.WriteLine($"Finished X: {x}");
            }
        }
        
        return Answers;
    }
}
