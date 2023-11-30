using Framework;

namespace Challenges2022; 

public partial class Solution19 {
    public record OreRobot(int OreCost);
    public record ClaRobot(int OreCost);
    public record ObsRobot(int OreCost, int ClaCost);
    public record GeoRobot(int OreCost, int ObsCost);

    public record struct Results(int Ore, int Cla, int Obs, int Geo);
    public record struct Configuration() {
        public Dictionary<Mineral, int> ProductionPlan = new();
        public int Payoff = 0;
    }

    public enum Mineral {
        Ore, Cla, Obs, Geo
    }

    public record Blueprint(OreRobot Ore, ClaRobot Cla, ObsRobot Obs, GeoRobot Geo) {
        public int Id;
        public int BiggestOreCost() {
            var cost = Ore.OreCost;
            cost.AssignIfBigger(Cla.OreCost);
            cost.AssignIfBigger(Obs.OreCost);
            cost.AssignIfBigger(Geo.OreCost);
            return cost;
        }
        public int BiggestObsCost() => Geo.ObsCost;
        public int BiggestClaCost() => Obs.ClaCost;
    }

    public class MiningRates : IEquatable<MiningRates> {
        public MiningRates(int ore, int cla, int obs, int geo) {
            Ore = ore; Cla = cla; Obs = obs; Geo = geo;
        }
        public int Ore;
        public int Cla;
        public int Obs;
        public int Geo;
        
        public MiningRates Copy() => new MiningRates(Ore, Cla, Obs, Geo);

        public bool Equals(MiningRates? other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Ore == other.Ore && Cla == other.Cla && Obs == other.Obs && Geo == other.Geo;
        }

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != this.GetType()) {
                return false;
            }

            return Equals((MiningRates)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Ore, Cla, Obs, Geo);
    }

    public class ProductionLine {
        private bool producingOreRobot;
        private bool producingClaRobot;
        private bool producingObsRobot;
        private bool producingGeoRobot;
        public bool AnyProducing() => producingOreRobot || producingClaRobot || producingObsRobot || producingGeoRobot;

        public void StartProduction(Mineral mineral) {
            Console.WriteLine($"Starting {mineral.ToString().ToUpper()} robot production.");
            switch (mineral) {
                case Mineral.Ore:
                    producingOreRobot = true;
                    break;
                case Mineral.Cla:
                    producingClaRobot = true;
                    break;
                case Mineral.Obs:
                    producingObsRobot = true;
                    break;
                case Mineral.Geo:
                    producingGeoRobot = true;
                    break;
            }
        }
        public bool TryGetOreRobotOffLine() {
            if (producingOreRobot) {
                Console.WriteLine($"Getting ORE robot off the line.");
                producingOreRobot = false;
                return true;
            }
            return false;
        }
        public bool TryGetClaRobotOffLine() {
            if (producingClaRobot) {
                Console.WriteLine($"Getting CLA robot off the line.");
                producingClaRobot = false;
                return true;
            }
            return false;
        }
        public bool TryGetObsRobotOffLine() {
            if (producingObsRobot) {
                Console.WriteLine($"Getting OBS robot off the line.");
                producingObsRobot = false;
                return true;
            }
            return false;
        }
        public bool TryGetGeoRobotOffLine() {
            if (producingGeoRobot) {
                Console.WriteLine($"Getting GEO robot off the line.");
                producingGeoRobot = false;
                return true;
            }
            return false;
        }
    }

    public record Resources(int Ore, int Cla, int Obs, int Geo) : IEquatable<Resources> {
        public int Ore { get; set; } = Ore;
        public int Cla { get; set; } = Cla;
        public int Obs { get; set; } = Obs;
        public int Geo { get; set; } = Geo;

        public Resources Copy() => new Resources(Ore, Cla, Obs, Geo);

        public virtual bool Equals(Resources? other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Ore == other.Ore && Cla == other.Cla && Obs == other.Obs && Geo == other.Geo;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Ore, Cla, Obs, Geo);
        }
    }
}
