using Framework;

namespace Challenges2021;

public class Solution18 : SolutionFramework {
    public Solution18() : base(18) { }
    public class PairOrRNum {
        public Pair? Pair;
        public int? RegularNum;

        public bool IsRegularNum() {
            if (Pair is not null && RegularNum.HasValue) {
                throw new InvalidOperationException();
            }
            return RegularNum.HasValue;
        }

        public bool IsPair() {
            if (Pair is not  null && RegularNum.HasValue) {
                throw new InvalidOperationException();
            }

            return Pair is not null;
        }
    }
    
    public class Pair {
        public PairOrRNum? Left;
        public PairOrRNum? Right;
        public Pair? Parent;

        public Pair GetRootParent() {
            var curr = this;
            while (curr.Parent != null) {
                curr = curr.Parent;
            }
            return curr;
        }

        public Pair Copy() {
            var copy = new Pair {
                Left = Left,
                Right = Right,
                Parent = Parent
            };
            if (copy.Left is not null && copy.Left.IsPair()) {
                copy.Left.Pair!.Parent = copy;
            }
            if (copy.Right is not null && copy.Right.IsPair()) {
                copy.Right.Pair!.Parent = copy;
            }

            return copy;
        }

        public int GetNestLevel() {
            var curr = this;
            var nest = 0;
            while (curr != null) {
                curr = curr.Parent;
                nest++;
            }

            return nest;
        }

        public void AssignToAvailableValueSlot(PairOrRNum val) {
            if (Left is not null && Right is not null) {
                throw new InvalidOperationException();
            }

            if (Left is null) {
                Left = val;
            }
            else if (Right is null) {
                Right = val;
            }
        }
    }

    private static bool HandleSplit(Pair? pair) {
        var leftmost = Flattened(pair!.GetRootParent()).FirstOrDefault(x => (x.Left!.IsRegularNum() && x.Left.RegularNum >= 10) || (x.Right!.IsRegularNum() && x.Right.RegularNum >= 10));
        if (leftmost == null) {
            return false;
        }

        var leftmostValue = leftmost.Left?.RegularNum >= 10 ? leftmost.Left : leftmost.Right;
        if (leftmostValue!.RegularNum.GetValueOrDefault() >= 10) {
            var nSplit = leftmostValue.RegularNum.GetValueOrDefault() / 2f;
            var newPair = new Pair() {
                Left = new PairOrRNum() { RegularNum = (int)Math.Floor(nSplit) }, 
                Right = new PairOrRNum() { RegularNum = (int)Math.Ceiling(nSplit) },
                Parent = leftmost
            };
            
            if (newPair.Left.RegularNum > newPair.Right.RegularNum) {
                throw new InvalidOperationException();
            }
            
            if (leftmost.Left == leftmostValue) {
                leftmost.Left.RegularNum = null;
                leftmost.Left.Pair = newPair;
            }
            else if (leftmost.Right == leftmostValue) {
                leftmost.Right.RegularNum = null;
                leftmost.Right.Pair = newPair;
            } else {
                throw new InvalidOperationException();
            }

            return true;
        }

        if (pair.Left is not null && pair.Left.IsPair()) {
            if (HandleSplit(pair.Left.Pair!)) {
                return true;
            } 
        }
        if (pair.Right is not null && pair.Right.IsPair()) {
            if (HandleSplit(pair.Right.Pair!)) {
                return true;
            }
        }

        return false;
    }

    private static bool HandleExplosions(Pair? pair) {
        var leftmost = Flattened(pair!.GetRootParent()).FirstOrDefault(p => (p.GetNestLevel() > 4 && p.Left!.IsRegularNum() && p.Right!.IsRegularNum()));
        if (leftmost is null) {
            return false;
        }

        if (leftmost.GetNestLevel() > 4) {
            if (!(leftmost.Left!.IsRegularNum() && leftmost.Right!.IsRegularNum())) {
                throw new InvalidOperationException();
            }

            var flattened = Flattened(leftmost.GetRootParent());

            PairOrRNum? closestLeftN = null;
            PairOrRNum? closestRightN = null;
            
            for (var i = 0; i < flattened.FindIndex(x=>x==leftmost); i++) {
                if (flattened[i].Right!.IsRegularNum()) {
                    closestLeftN = flattened[i].Right!;
                }
                else if (flattened[i].Left!.IsRegularNum()) {
                    closestLeftN = flattened[i].Left!;
                }
            }
            for (var i = flattened.FindIndex(x=>x==leftmost)+1; i < flattened.Count; i++) {
                if (flattened[i].Left!.IsRegularNum()) {
                    closestRightN = flattened[i].Left!;
                }
                else if (flattened[i].Right!.IsRegularNum()) {
                    closestRightN = flattened[i].Right!;
                }

                if (closestRightN != null) {
                    break;
                }
            }
            
            if (closestLeftN is not null && leftmost.Left!.IsRegularNum()) {
                closestLeftN.RegularNum += leftmost.Left?.RegularNum;
            } 

            if (closestRightN is not null && leftmost.Right!.IsRegularNum()) {
                closestRightN.RegularNum += leftmost.Right?.RegularNum;
            }

            var parent = leftmost.Parent;
            if (parent?.Left != null && parent.Left.IsPair() && leftmost == parent.Left.Pair) {
                parent.Left.Pair = null;
                parent.Left.RegularNum = 0;
            }
            else if (parent?.Right != null && parent.Right.IsPair() && leftmost == parent.Right.Pair) {
                parent.Right.Pair = null;
                parent.Right.RegularNum = 0;
            } else {
                throw new InvalidOperationException();
            }
            return true;
        }

        if (pair.Left is not null && pair.Left.IsPair()) {
            if (HandleExplosions(pair.Left.Pair!)) {
                return true;
            }
        }
        if (pair.Right is not null && pair.Right.IsPair()) {
            if (HandleExplosions(pair.Right.Pair!)) {
                return true;
            }
        }

        return false;
    }

    private static List<Pair> Flattened(Pair pair, List<Pair>? pairs = null) {
        pairs ??= new List<Pair>();
        if (pair.Left is not null && pair.Left.IsPair()) {
            Flattened(pair.Left.Pair!, pairs);
        }

        if (!pairs.Contains(pair)) {
            pairs.Add(pair);
        }
        
        if (pair.Right is not null && pair.Right.IsPair()) {
            Flattened(pair.Right.Pair!, pairs);
        }

        if (!pairs.Contains(pair)) {
            pairs.Add(pair);
        }

        return pairs;
    }

    private static void Print(Pair pair) {
        Console.Write("[");
        if (pair.Left is not null && pair.Left.IsPair()) {
            Print(pair.Left.Pair!);
        }
        if (pair.Left is not null && pair.Left.IsRegularNum()) {
            Console.Write(pair.Left.RegularNum + ",");
        }
        
        if (pair.Right is not null && pair.Right.IsPair()) {
            Print(pair.Right.Pair!);
        }
        if (pair.Right is not null && pair.Right.IsRegularNum()) {
            Console.Write(pair.Right.RegularNum);
        }

        Console.Write("]");
    }

    public override string[] Solve() {
        var numbers = RawInputSplitByNl.ToArray();
        
        Pair? pair = null;
        foreach (var n in numbers) {
            var newPair = ParseNum(n);

            if (pair is null) {
                pair = newPair;
                continue;
            }

            pair = CombineTwoPairs(pair, newPair);
            
            var stuffHappened = true;
            while (stuffHappened) {
                while (HandleExplosions(pair)) { }
                stuffHappened = HandleSplit(pair);
            }
        }
        
        AssignAnswer1(GetTotalMagnitude(pair!.GetRootParent()));

        var highestMagnitudeRecorded = 0;
        foreach (var n in numbers) {
            foreach (var otherN in numbers.Where(x => x != n)) {
                var newPair1 = ParseNum(n);
                var newPair2 = ParseNum(otherN);
                
                pair = CombineTwoPairs(newPair1, newPair2);
                var stuffHappened = true;
                while (stuffHappened) {
                    while (HandleExplosions(pair)) { }
                    stuffHappened = HandleSplit(pair);
                }

                highestMagnitudeRecorded.AssignIfBigger(GetTotalMagnitude(pair.GetRootParent()));
            }
        }
        AssignAnswer2(highestMagnitudeRecorded);

        return Answers;
    }

    private static Pair CombineTwoPairs(Pair? pair1, Pair? pair2) {
        var combined = new Pair();
        if (pair1!.Left is not null && pair1.Right is null) {
            combined.Left = pair1.Left;
        }
        else if (pair1.Left is null && pair1.Right is not null) {
            combined.Left = pair1.Right;
        }
        else {
            combined.Left = new PairOrRNum() { Pair = pair1 };
        }

        if (pair2!.Left is not null && pair2.Right is null) {
            combined.Right = pair2.Left;
        }
        else if (pair2.Left is null && pair2.Right is not null) {
            combined.Right = pair2.Right;
        }
        else {
            combined.Right = new PairOrRNum() { Pair = pair2 };
        }

        combined.Left.Pair!.Parent = combined;
        combined.Right.Pair!.Parent = combined;
        return combined;
    }

    private static Pair? ParseNum(string number) {
        var pair = new Pair();
        foreach (var ch in number) {
            switch (ch) {
                case '[': {
                    if (pair.Left is null) {
                        pair.Left = new PairOrRNum() { Pair = new Pair() { Parent = pair } };
                        pair = pair.Left.Pair;
                    }
                    else if (pair.Right is null) {
                        pair.Right = new PairOrRNum() { Pair = new Pair() { Parent = pair } };
                        pair = pair.Right.Pair;
                    }

                    break;
                }
                case ']': {
                    //At end
                    if (pair.Parent is null) {
                        break;
                    }

                    pair = pair.Parent;
                    break;
                }
                default: {
                    if (char.IsDigit(ch)) {
                        var value = new PairOrRNum() { RegularNum = int.Parse(ch.ToString()) };
                        pair.AssignToAvailableValueSlot(value);
                    }

                    break;
                }
            }
        }

        return pair;
    }

    private static int GetTotalMagnitude(Pair pair) {
        var total = 0;
        var flat = Flattened(pair.GetRootParent());
        while (flat.Any(x => x.Left!.IsPair() || x.Right!.IsPair())) {
            foreach (var p in flat.Where(p => p != pair.GetRootParent())) {
                if (p.Left!.IsRegularNum() && p.Right!.IsRegularNum()) {
                    var magnitude = 0;
                    if (p.Left.IsRegularNum()) {
                        magnitude += (p.Left.RegularNum!.Value * 3);
                    }

                    if (p.Right != null && p.Right.IsRegularNum()) {
                        magnitude += (p.Right.RegularNum!.Value * 2);
                    }

                    if (p.Parent?.Left?.Pair == p) {
                        p.Parent.Left.Pair = null;
                        p.Parent.Left.RegularNum = magnitude;
                    }
                    else if (p.Parent?.Right?.Pair == p) {
                        p.Parent.Right!.Pair = null;
                        p.Parent.Right.RegularNum = magnitude;
                    }
                    else {
                        throw new InvalidOperationException();
                    }
                }
            }
            flat = Flattened(pair.GetRootParent());
        }
        var root = pair.GetRootParent();
        total += (root.Left!.RegularNum!.Value * 3);
        total += (root.Right!.RegularNum!.Value * 2);

        return total;
    }
}
