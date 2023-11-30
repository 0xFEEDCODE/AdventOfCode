using System.Text.RegularExpressions;

using Framework;

namespace Challenges2020;

public class Solution7 : SolutionFramework {
    public Solution7() : base(7) { }

    public record struct BagEntry(int Quantity, Bag Bag);

    public record struct Bag(string Name);

    public override string[] Solve() {
        var bags = new Dictionary<Bag, List<BagEntry>>();
        const string currentBagPattern = @"(?<x>.*) bags contain";
        const string bagsContainedPattern = @"\d .+?(?=bag)";
        foreach (var line in RawInputSplitByNl) {
            var currentBag = Regex.Match(line, currentBagPattern).Groups.Values.Skip(1).Single().Value;
            var containingBags = Regex.Matches(line, bagsContainedPattern).Select(m => m.Value);
            var quantityAndBagPattern = @"(?<x>\d+) (?<y>[\w\s]+)\s";
            var bagsContained = new List<BagEntry>();
            foreach (var bag in containingBags) {
                var quantityAndBag = Regex.Match(bag, quantityAndBagPattern).Groups.Values.Select(x => x.Value).ToArray();
                var bagEntry = new BagEntry(int.Parse(quantityAndBag[1]), new Bag(quantityAndBag[2]));
                bagsContained.Add(bagEntry);
            }

            bags.Add(new Bag(currentBag), bagsContained);
        }

        var shinyGoldBagsFound = 0;
        foreach (var bag in bags) {
            var stack = new Stack<BagEntry>(bag.Value);
            while (stack.Any()) {
                var currentBag = stack.Pop();
                if (currentBag.Bag.Name == "shiny gold") {
                    shinyGoldBagsFound++;
                    break;
                }

                if (!bags.ContainsKey(currentBag.Bag)) {
                    continue;
                }

                foreach (var containedBag in bags[currentBag.Bag]) {
                    stack.Push(containedBag);
                }
            }
        }

        AssignAnswer1(shinyGoldBagsFound);

        int GetBagsContainedCount(BagEntry bag) {
            var st = new Stack<BagAccumulator>(new[] { new BagAccumulator(0, bag.Bag) });
            var total = 0;
            
            while (st.Any()) {
                var currentBag = st.Pop();
                if (!bags.ContainsKey(currentBag.Bag)) {
                    continue;
                }

                foreach (var containedBag in bags[currentBag.Bag]) {
                    var acc = (currentBag.Acc is 0 ? 1 : currentBag.Acc) * containedBag.Quantity;
                    total += acc;
                    st.Push(new BagAccumulator(acc, containedBag.Bag));
                }
            }

            return total;
        }

        var bagsInShinyGold = GetBagsContainedCount(new BagEntry(1, bags.Single(kv => kv.Key.Name == "shiny gold").Key));
        AssignAnswer2(bagsInShinyGold);

        return Answers;
    }

    public record struct BagAccumulator(int Acc, Bag Bag);
}
