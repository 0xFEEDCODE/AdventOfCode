using System.Text;

using Framework;

using static Challenges2021.BaseConversion;

namespace Challenges2021;

public static partial class Ext {
    public static string AsString(this IEnumerable<char> charEnumerable) => new(charEnumerable.ToArray());
    public static string HexToBin(this string bin) => BaseConversion.HexToBin(bin);
}

public static class BaseConversion {
    public static string HexToBin(string hex) {
        var result = new StringBuilder();
        foreach (var c in hex) {
            result.Append(HexCharacterToBinary[char.ToLower(c)]);
        }
        return result.ToString();
    }

    public static int BinToDec(string bits) {
        var dec = 0;
        var mul = 1;
        foreach (var bit in bits.Reverse()) {
            dec += int.Parse(bit.ToString()) * mul;
            mul *= 2;
        }

        return dec;
    }
    public static long BinToDecLong(string bits) {
        long dec = 0;
        long mul = 1;
        foreach (var bit in bits.Reverse()) {
            dec += long.Parse(bit.ToString()) * mul;
            mul *= 2;
        }

        return dec;
    }
    public static readonly Dictionary<char, string> HexCharacterToBinary = new Dictionary<char, string> {
        { '0', "0000" },
        { '1', "0001" },
        { '2', "0010" },
        { '3', "0011" },
        { '4', "0100" },
        { '5', "0101" },
        { '6', "0110" },
        { '7', "0111" },
        { '8', "1000" },
        { '9', "1001" },
        { 'a', "1010" },
        { 'b', "1011" },
        { 'c', "1100" },
        { 'd', "1101" },
        { 'e', "1110" },
        { 'f', "1111" }
    };
}


public class Solution16 : SolutionFramework {
    public Solution16() : base(16) { }

    public record struct PacketHeader(string RepresentationInBits) {
        public int GetPacketVersion() => BinToDec(RepresentationInBits[..3]);
        public int GetTypeId() => BinToDec(RepresentationInBits[3..6]);
    }

    public record struct Packet(PacketHeader Header, string Content) {
        public int GetLenTypeId() => BinToDec(Content[0].ToString());
        public int GetLenTypeValueBitLen() => GetLenTypeId() == 1 ? 11 : 15;
        public bool IsLiteralValuePacket() => Header.GetTypeId() == 4;
        public bool IsOperatorPacket() => Header.GetTypeId() != 4;

        public long GetLiteralValue() {
            if (!IsLiteralValuePacket()) {
                throw new InvalidOperationException();
            }

            var groupStart = 0;
            var value = new StringBuilder();
            var endFound = false;
            var group = new string(Content[groupStart..(groupStart+5)].ToArray());
            while (true) {
                value.Append(new string(group.Skip(1).ToArray()));
                groupStart+=5;
                if (group.First() == '0') {
                    break;
                }
                group = new string(Content[groupStart..(groupStart+5)].ToArray());
            }
            
            return BinToDecLong(value.ToString());
        }

        public long DOSTUFF(List<long> subpacketsLiteralValues) {
            if (!IsOperatorPacket()) {
                throw new InvalidOperationException();
            }

            if (Header.GetTypeId() is 5 or 6 or 7 && subpacketsLiteralValues.Count > 2) {
                throw new InvalidOperationException();
            }

            return (Header.GetTypeId()) switch {
                //SUM
                0 => subpacketsLiteralValues.Sum(),
                //PROD
                1 => subpacketsLiteralValues.Aggregate((x, acc) => x * acc),
                //MIN
                2 => subpacketsLiteralValues.Min(),
                //MAX
                3 => subpacketsLiteralValues.Max(),
                //GT
                5 => (subpacketsLiteralValues[0] > subpacketsLiteralValues[1]) ? 1 : 0,
                //LT
                6 => (subpacketsLiteralValues[0] < subpacketsLiteralValues[1]) ? 1 : 0,
                //EQ
                7 => (subpacketsLiteralValues[0] == subpacketsLiteralValues[1]) ? 1 : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public int GetSubPacketCount() {
            if (GetLenTypeId() != 1) {
                throw new InvalidOperationException();
            }
            if (!IsOperatorPacket()) {
                throw new InvalidOperationException();
            }
            return BinToDec(Content[1..(GetLenTypeValueBitLen()+1)]);
        }

        public string GetSubPacketContent() {
            if (!IsOperatorPacket()) {
                throw new InvalidOperationException();
            }

            return Content[(len_LenTypeId + GetLenTypeValueBitLen())..];
        }
        
        public int GetLenType0SubPacketLen() {
            if (!IsOperatorPacket()) {
                throw new InvalidOperationException();
            }
            if (GetLenTypeId() != 0) {
                throw new InvalidOperationException();
            }
            return BinToDec(Content[1..(GetLenTypeValueBitLen()+1)]);
        }
        
        public int GetPacketContentLen() {
            if (IsLiteralValuePacket()) {
                var groupStart = 0;
                var group = new string(Content[groupStart..(groupStart+5)].ToArray());
                while (true){
                    groupStart+=5;
                    if (group.First() == '0') {
                        break;
                    }
                    group = new string(Content[groupStart..(groupStart+5)].ToArray());
                }

                return groupStart;
            }
            if (IsOperatorPacket()) {
                throw new NotImplementedException();
            }

            throw new InvalidOperationException();
        }
    }

    

    private const int len_LenTypeId = 1;
    private const int len_Header = 6;
    private const int len_type0ValueBitLen = 15;
    private const int len_type1ValueBitLen = 11;

    public int VersionSum = 0;
    
    public record struct PacketLenAndValue(int Length, long Value);

    public PacketLenAndValue ProcessLiteralValuePacket(Packet packet) {
        if (!packet.IsLiteralValuePacket()) {
            throw new InvalidOperationException();
        }

        VersionSum += packet.Header.GetPacketVersion();
        return new PacketLenAndValue(len_Header + packet.GetPacketContentLen(), packet.GetLiteralValue());
    }

    public PacketLenAndValue ProcessType0Packet(Packet packet) {
        if (!packet.IsOperatorPacket() && packet.GetLenTypeId() != 0) {
            throw new InvalidOperationException();
        }

        VersionSum += packet.Header.GetPacketVersion();

        var processedSubpacketsLen = 0;
        var totalSubpacketLen = packet.GetLenType0SubPacketLen();
        var subpacketContent = packet.GetSubPacketContent();
        var subpacketValues = new List<long>();
        while(processedSubpacketsLen != totalSubpacketLen) {
            var subpacket = new Packet(new PacketHeader(subpacketContent[processedSubpacketsLen..(processedSubpacketsLen+len_Header)]), subpacketContent[(processedSubpacketsLen+len_Header)..]);
            var processedSubpacket = ProcessPacket(subpacket);

            processedSubpacketsLen += processedSubpacket.Length;
            subpacketValues.Add(processedSubpacket.Value);
        }

        return new PacketLenAndValue(len_Header + len_LenTypeId + len_type0ValueBitLen + processedSubpacketsLen, packet.DOSTUFF(subpacketValues));
    }

    public PacketLenAndValue ProcessType1Packet(Packet packet) {
        if (!packet.IsOperatorPacket() && packet.GetLenTypeId() != 1) {
            throw new InvalidOperationException();
        }
        
        VersionSum += packet.Header.GetPacketVersion();

        var accumulatedSubpacketLen = 0;
        var subpacketCount = packet.GetSubPacketCount();
        var subpacketsProcessed = 0;
        var subpacketContent = packet.GetSubPacketContent();
        var subpacketValues = new List<long>();
        while(subpacketsProcessed < subpacketCount) {
            var subpacket = new Packet(new PacketHeader(subpacketContent[accumulatedSubpacketLen..(accumulatedSubpacketLen+len_Header)]), subpacketContent[(accumulatedSubpacketLen+len_Header)..]);
            var processedSubpacket = ProcessPacket(subpacket);

            accumulatedSubpacketLen += processedSubpacket.Length;
            subpacketValues.Add(processedSubpacket.Value);
            subpacketsProcessed++;
        }

        return new PacketLenAndValue(len_Header + len_LenTypeId + len_type1ValueBitLen + accumulatedSubpacketLen, packet.DOSTUFF(subpacketValues));
    }
    
    public PacketLenAndValue ProcessPacket(Packet packet) {
        if (packet.IsLiteralValuePacket()) {
            return ProcessLiteralValuePacket(packet);
        }

        return packet.GetLenTypeId() switch {
            0 => ProcessType0Packet(packet),
            1 => ProcessType1Packet(packet),
            _ => throw new ArgumentOutOfRangeException()
        };
    }


    public override string[] Solve() {
        var parsingPos = 0;
        var packetInBin = string.Join("", RawInputSplitByNl.ToArray()).HexToBin();

        var packet = new Packet(new PacketHeader(packetInBin[..6].AsString()), packetInBin[6..]);
        var processedPacket = ProcessPacket(packet);
        AssignAnswer1(VersionSum);
        AssignAnswer2(processedPacket.Value);
        
 
        return Answers;
    }
}
