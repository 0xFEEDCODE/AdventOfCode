using System.Text;
using Framework;

namespace Challenges2022;
public class Solution13 : SolutionFramework {

    private class PacketComparer : IComparer<Packet> {
        public int Compare(Packet? p1, Packet? p2) =>
            GetPacketValidity(p1!, p2!) switch {
                Validity.NotConcluded => 0,
                Validity.Valid => -1,
                Validity.Invalid => 1,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    public Solution13() : base(13) { }

    public enum Validity {
        Valid,
        Invalid,
        NotConcluded
    }

    public class PairOfPackets {
        public Packet Left = new();
        public Packet Right = new();
    }

    public struct PacketItem {
        public int Value;
        public Packet? Packet;

        public bool IsValue => Packet == null;
    }

    public class Packet {
        public List<PacketItem> Values = new();
    }

    public override string[] Solve() {
        var packetGroups = new List<PairOfPackets>();
        ParseInput(packetGroups);

        var idxWhereRightOrder = new List<int>();
        for (var i = 0; i < packetGroups.Count; i++) {
            if (GetPacketValidity(packetGroups[i].Left, packetGroups[i].Right) == Validity.Valid)
                idxWhereRightOrder.Add(i+1);
        }
        AssignAnswer1(idxWhereRightOrder.Sum());

        var allPacketPairs = new List<Packet>();
        packetGroups.ForEach(e => {
            allPacketPairs.Add(e.Left);
            allPacketPairs.Add(e.Right);
        });

        Packet dividerPacket2 = new() { Values = new() { new() { Packet = CreatePacketWithValue(2) } } };
        Packet dividerPacket6 = new() { Values = new() { new() { Packet = CreatePacketWithValue(6) } } };
        allPacketPairs.Add(dividerPacket2); allPacketPairs.Add(dividerPacket6);

        allPacketPairs.Sort(new PacketComparer());
        
        var dividerPack2Idx = int.MaxValue;
        var dividerPack6Idx = int.MaxValue;
        for(var i = 0 ; i < allPacketPairs.Count; i++) {
            if(allPacketPairs[i] == dividerPacket2)
                dividerPack2Idx.AssignIfLower(i+1);
            if(allPacketPairs[i] == dividerPacket6)
                dividerPack6Idx.AssignIfLower(i+1);
        }

        AssignAnswer2(checked(dividerPack2Idx * dividerPack6Idx));
        return Answers;
    }
    
    private void ParseInput(List<PairOfPackets> entries) {
        var i = 0;
        var packetPair = new PairOfPackets();
        foreach (var line in RawInputSplitByNl) {
            if (i == 2) {
                if (line != "")
                    throw new InvalidOperationException();
                i = 0;
                continue;
            }

            var packetsStack= new Stack<Packet>();
            packetsStack.Push(i == 0 ? packetPair.Left : packetPair.Right);

            var numSB = new StringBuilder();
            foreach (var ch in line) {
                switch (ch) {
                    case '[':
                        var newPacket = new Packet();
                        packetsStack.Peek().Values.Add(new PacketItem { Packet = newPacket });
                        packetsStack.Push(newPacket);
                        break;
                    case ']':
                        AddNumToPacket(numSB, packetsStack.Pop());
                        break;
                    case ',':
                        AddNumToPacket(numSB, packetsStack.Peek());
                        break;
                    default:
                        numSB.Append(ch);
                        break;
                }
            }
            
            void AddNumToPacket(StringBuilder numSB, Packet packet) {
                if (numSB.Length == 0) { return; }
                packet.Values.Add(new PacketItem() { Value = int.Parse(numSB.ToString()) });
                numSB.Clear();
            }
            
            i = (i + 1) % 3;
            //Group processed
            if (i == 2) {
                entries.Add(packetPair);
                packetPair = new PairOfPackets();
            }
        }
    }
    
    static Validity GetPacketItemValidity(PacketItem p1, PacketItem p2) =>
        (p1.IsValue, p2.IsValue) switch {
            (true, true) => (p1.Value - p2.Value) switch {
                > 0 => Validity.Invalid,
                < 0 => Validity.Valid,
                0 => Validity.NotConcluded
            },
            (true, false) => GetPacketValidity(CreatePacketWithValue(p1.Value), p2.Packet!),
            (false, true) => GetPacketValidity(p1.Packet!, CreatePacketWithValue(p2.Value)),
            (false, false) => GetPacketValidity(p1.Packet!, p2.Packet!)
        };

    static Packet CreatePacketWithValue(int value) => new () { Values = new() { new () { Value = value }}};

    static Validity GetPacketValidity(Packet p1, Packet p2) {
        var validity = Validity.NotConcluded;
        for (var i = 0; i < p1.Values.Count && i < p2.Values.Count; i++) {
            validity = GetPacketItemValidity(p1.Values[i], p2.Values[i]);
            if (validity != Validity.NotConcluded)
                return validity;
        }
        if (p1.Values.Count > p2.Values.Count)
            return Validity.Invalid;
        if (p2.Values.Count > p1.Values.Count)
            return Validity.Valid;
        return validity;
    }
}

