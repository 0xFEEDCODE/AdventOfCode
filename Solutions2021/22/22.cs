using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using Framework;

namespace Challenges2021;

public record struct Region(Vector3 Min, Vector3 Max, bool On) {
    public Vector3 Min = Min;
    public Vector3 Max = Max;
    public bool On = On;

    public int Volume() {
        var size = Max - Min;
        return (int)((size.X + 1) * (size.Y + 1) * (size.Z + 1));
    }

    public bool Intersects(Region other) =>
        (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
        (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
        (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);

    public Region? GetIntersection(Region other) {
        if (!Intersects(other)) {
            return null;
        }

        var iMin = Vector3.Max(Min, other.Min);
        var iMax = Vector3.Min(Max, other.Max);
        return new Region(iMin, iMax, other.On);
    }
    
    public override int GetHashCode() => Min.GetHashCode() + Max.GetHashCode();
}


public class Solution22 : SolutionFramework {
    public Solution22() : base(22) { }

    public record struct ProcessedItem(Region Region, Dictionary<int, Region> Intersecting);
    public record struct Range(int Start, int End);

    public record struct OnOffInstruction(bool On, Region Region);

    public override string[] Solve() {
        var rebootInstructions = new Queue<OnOffInstruction>();
        foreach (var line in InputNlSplit) {
            var matches = Regex.Matches(line, @"-?\d+").Select(x => int.Parse(x.Value)).ToArray();
            var onOff = line.Contains("on");
            rebootInstructions.Enqueue(new OnOffInstruction(onOff, new Region(new Vector3(matches[0], matches[2], matches[4]), new Vector3(matches[1], matches[3], matches[5]), onOff)));
        }

        var processed = new List<ProcessedItem>();

        var on = 0;
        while (rebootInstructions.TryDequeue(out var instr)) {
            var isOnInstruction = instr.On;
            var region = instr.Region;
            
            var vol = isOnInstruction ? region.Volume() : 0;

            foreach (var reg in processed) {
                var intersection = region.GetIntersection(reg.Region);
                if (intersection.HasValue) {
                    var volChange = 0;
                    //search if there is any OFF intersection
                    foreach (var offIntersectionEntry in reg.Intersecting.Where(i => !i.Value.On)) {
                        var offIntersection = region.GetIntersection(offIntersectionEntry.Value);
                        if (!offIntersection.HasValue) {
                            continue;
                        }

                        //every intersection that comes after the off intersection
                        foreach (var subsequentIntersection in reg.Intersecting.Where(i => i.Key >= offIntersectionEntry.Key)) {
                            if (offIntersection.Value.Intersects(subsequentIntersection.Value)) {
                                //If its ON entry, update the intersection
                                if (subsequentIntersection.Value.On) {
                                    offIntersection = offIntersection.Value.GetIntersection(subsequentIntersection.Value)!.Value;
                                }
                            }
                        }

                        volChange += offIntersection.Value.Volume();
                    }

                    foreach (var onIntersectionEntry in reg.Intersecting.Where(i => i.Value.On)) {
                        var onIntersection = region.GetIntersection(onIntersectionEntry.Value);
                        if (!onIntersection.HasValue) {
                            continue;
                        }

                        //every intersection that comes after the off intersection
                        foreach (var subsequentIntersection in reg.Intersecting.Where(i => i.Key >= onIntersectionEntry.Key)) {
                            if (onIntersection.Value.Intersects(subsequentIntersection.Value)) {
                                //If its OFF entry, update the intersection
                                if (!subsequentIntersection.Value.On) {
                                    onIntersection = onIntersection.Value.GetIntersection(subsequentIntersection.Value)!.Value;
                                }
                            }
                        }

                        volChange -= onIntersection.Value.Volume();
                    }

                    if (isOnInstruction) {
                        vol += volChange;
                    } else {
                        vol -= volChange;
                    }
                    reg.Intersecting.Add(reg.Intersecting.Count + 1, region);
                }
            }

            if (isOnInstruction) {
                on += vol;
            } else {
                on -= vol;
            }
            processed.Add(new ProcessedItem(region, new Dictionary<int, Region>(){{0, region}}));
        }
        
        return Answers;
    }

    private static List<Region> GetOffIntersects(Region region, List<Region> turnedOffRegions) {
        var offIntersects = new HashSet<Region>();

        foreach (var offRegion in turnedOffRegions) {
            var intersectingRegion = region.GetIntersection(offRegion);
            if (intersectingRegion is not null) {
                offIntersects.Add(intersectingRegion.Value);
            }
        }

        return offIntersects.ToList();
    }

    private static List<Region> GetOnIntersects(Region region, List<Region> turnedOnRegions) {
        var onIntersects = new HashSet<Region>();

        foreach (var onRegion in turnedOnRegions) {
            var intersectingRegion = region.GetIntersection(onRegion);
            if (intersectingRegion is not null) {
                onIntersects.Add(intersectingRegion.Value);
            }
        }

        return onIntersects.ToList();
    }

    private record struct RebootInstructionProcessResult(int VolOfIntersectsIntersectingWithOneAnother, int TotalVolumeOfIntersections);

    private static int ProcessOff(List<Region> offIntersects) {
        var volOfIntersectsIntersectingWithOneAnother = 0;
        var processedIntersects = new List<Region>();
        foreach (var intersectingRegion in offIntersects) {
            var vol = 0;
            foreach (var otherIntersectingRegion in processedIntersects) {
                var intersectWithOtherIntersect = intersectingRegion.GetIntersection(otherIntersectingRegion);
                if (intersectWithOtherIntersect is not null) {
                    vol += intersectWithOtherIntersect.Value.Volume();
                }
            }

            volOfIntersectsIntersectingWithOneAnother += vol;
            processedIntersects.Add(intersectingRegion);
        }

        return volOfIntersectsIntersectingWithOneAnother;
    }

    private static int ProcessOn(List<Region> onIntersects) {
        var volOfIntersectsIntersectingWithOneAnother = 0;
        var processedIntersects = new List<Region>();
        foreach (var intersectingRegion in onIntersects) {
            var vol = 0;
            foreach (var otherIntersectingRegion in processedIntersects) {
                var intersectWithOtherIntersect = intersectingRegion.GetIntersection(otherIntersectingRegion);
                if (intersectWithOtherIntersect is not null) {
                    vol += intersectWithOtherIntersect.Value.Volume();
                }
            }

            volOfIntersectsIntersectingWithOneAnother += vol;
            processedIntersects.Add(intersectingRegion);
        }

        return volOfIntersectsIntersectingWithOneAnother;
    }
}
