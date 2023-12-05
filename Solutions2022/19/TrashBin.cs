using System.Text.RegularExpressions;

namespace Challenges2022;

public partial class Solution19 {
    private List<Blueprint> ParseInput() {
        int id = 1;
        var blueprints = new List<Blueprint>();
        const string patternOre = @".* costs (?<x>.+) ore";
        const string patternClay = @".* costs (?<x>.+) ore";
        const string patternObsidian = @".* costs (?<x>.+) ore and (?<y>.+) clay";
        const string patternGeode = @".* costs (?<x>.+) ore and (?<y>.+) obsidian";
        foreach (var line in InputNlSplit) {
            var split = line.Split('.');
            var ore = Regex.Match(split[0], patternOre).Groups;
            var clay = Regex.Match(split[1], patternClay).Groups;
            var obsidian = Regex.Match(split[2], patternObsidian).Groups;
            var geode = Regex.Match(split[3], patternGeode).Groups;
            var oreR = new OreRobot(int.Parse(ore[1].Value));
            var clayR = new ClaRobot(int.Parse(clay[1].Value));
            var obsidianR = new ObsRobot(int.Parse(obsidian[1].Value), int.Parse(obsidian[2].Value));
            var geodeR = new GeoRobot(int.Parse(geode[1].Value), int.Parse(geode[2].Value));
            var bp = new Blueprint(oreR, clayR, obsidianR, geodeR){Id = id++};
            blueprints.Add(bp);
        }

        return blueprints;
    }
    
    /*
    private void ManageRobotProduction2(Blueprint bp, Resources res, MiningRates mr, ProductionLine pl) {
        var hasEnoughResForOre = HasEnoughResourcesForProduction(res, bp, Mineral.Ore);
        var hasEnoughResForCla = HasEnoughResourcesForProduction(res, bp, Mineral.Cla);
        var hasEnoughResForObs = HasEnoughResourcesForProduction(res, bp, Mineral.Obs);
        var hasEnoughResForGeo = HasEnoughResourcesForProduction(res, bp, Mineral.Geo);

        var payoffs = new Dictionary<Mineral, (int, bool)>();
        var currentPayoff = GeoRobotProductionPayoff(bp, mr);
        
        var mrWithOre = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
        mrWithOre.IncrementMiningRate(Mineral.Ore);
        var payoffAfterProducingRobot = GeoRobotProductionPayoff(bp, mrWithOre);
        if (payoffAfterProducingRobot > currentPayoff) 
            payoffs.Add(Mineral.Ore, (payoffAfterProducingRobot, true));
        else
            payoffs.Add(Mineral.Ore, (currentPayoff, false));
            
        var mrWithCla = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
        mrWithCla.IncrementMiningRate(Mineral.Cla);
        payoffAfterProducingRobot = GeoRobotProductionPayoff(bp, mrWithCla);
        if (payoffAfterProducingRobot > currentPayoff) 
            payoffs.Add(Mineral.Cla, (payoffAfterProducingRobot, true));
        else
            payoffs.Add(Mineral.Cla, (currentPayoff, false));
            
        var mrWithObs = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
        mrWithObs.IncrementMiningRate(Mineral.Obs);
        payoffAfterProducingRobot = GeoRobotProductionPayoff(bp, mrWithObs);
        if (payoffAfterProducingRobot > currentPayoff) 
            payoffs.Add(Mineral.Obs, (payoffAfterProducingRobot, true));
        else
            payoffs.Add(Mineral.Obs, (currentPayoff, false));
            
        var mrWithGeo = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
        mrWithGeo.IncrementMiningRate(Mineral.Geo);
        payoffAfterProducingRobot = GeoRobotProductionPayoff(bp, mrWithGeo);
        if (payoffAfterProducingRobot > currentPayoff) 
            payoffs.Add(Mineral.Geo, (payoffAfterProducingRobot, true));
        else
            payoffs.Add(Mineral.Geo, (currentPayoff, false));
        Console.WriteLine();

}

    private void StartRobotProductionIfProfitable(Resources res, Blueprint bp, MiningRates mr, ProductionLine pl, Mineral mineral) {
        var hasEnoughResourcesForProduction = HasEnoughResourcesForProduction(res, bp, mineral);

        if (hasEnoughResourcesForProduction) {
            var values = new List<(Mineral, (int, bool))>();
            foreach (var m in Enum.GetValues(typeof(Mineral)).Cast<Mineral>().Reverse()) {
                var mrWithAdditionalRobot1 = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
                
                var currentRobotValue1 = GeoRobotProductionPayoff(bp, mr);
                var additionalRobotValue1 = GeoRobotProductionPayoff(bp, mrWithAdditionalRobot1);
                if (m == Mineral.Ore) {
                    mrWithAdditionalRobot1 = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
                    mrWithAdditionalRobot1.IncrementMiningRate(Mineral.Geo);
                    currentRobotValue1 = OreRobotProductionPayoff(bp, mr);
                    additionalRobotValue1 = GeoRobotProductionPayoff(bp, mrWithAdditionalRobot1);
                } else if (m == Mineral.Cla) {
                    mrWithAdditionalRobot1 = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
                    mrWithAdditionalRobot1.IncrementMiningRate(Mineral.Cla);
                    currentRobotValue1 = GeoRobotProductionPayoff(bp, mr);
                    additionalRobotValue1 = GeoRobotProductionPayoff(bp, mrWithAdditionalRobot1);
                } else if (m == Mineral.Obs) {
                    mrWithAdditionalRobot1 = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
                    mrWithAdditionalRobot1.IncrementMiningRate(Mineral.Obs);
                    currentRobotValue1 = GeoRobotProductionPayoff(bp, mr);
                    additionalRobotValue1 = GeoRobotProductionPayoff(bp, mrWithAdditionalRobot1);
                } else if (m == Mineral.Geo) {
                    mrWithAdditionalRobot1 = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
                    mrWithAdditionalRobot1.IncrementMiningRate(Mineral.Geo);
                    currentRobotValue1 = GeoRobotProductionPayoff(bp, mr);
                    additionalRobotValue1 = GeoRobotProductionPayoff(bp, mrWithAdditionalRobot1);
                }

                if (additionalRobotValue1 > currentRobotValue1) {
                    values.Add((m, (additionalRobotValue1, true)));
                } else {
                    values.Add((m, (currentRobotValue1, false)));
                }
            }
            
            var mrWithAdditionalRobot = new MiningRates(mr.Ore, mr.Cla, mr.Obs, mr.Geo);
            mrWithAdditionalRobot.IncrementMiningRate(mineral);
            
            var currentRobotValue = GeoRobotProductionPayoff(bp, mr);
            
            var additionalRobotValue = GeoRobotProductionPayoff(bp, mrWithAdditionalRobot);
            if (additionalRobotValue > currentRobotValue) {
                pl.StartProduction(mineral);
                switch (mineral) {
                    case Mineral.Ore:
                        res.Ore -= bp.Ore.OreCost;
                        break;
                    case Mineral.Cla:
                        res.Ore -= bp.Cla.OreCost;
                        break;
                    case Mineral.Obs:
                        res.Ore -= bp.Obs.OreCost;
                        res.Cla -= bp.Obs.ClaCost;
                        break;
                    case Mineral.Geo:
                        res.Ore -= bp.Geo.OreCost;
                        res.Obs -= bp.Geo.ObsCost;
                        break;
                }
            }
        }
    }
    */
       /*
       private Configuration GetOptimalProductionConfiguration(Blueprint bp, Resources res, MiningRates mr, MiningRates origMr, int timeLeft) {
        var configs = new List<Configuration>() { new() {
            Payoff = GeoRobotProductionPayoff(bp, origMr, timeLeft),
            ProductionPlan = new Dictionary<Mineral, int>() {
                { Mineral.Ore, 0 },
                { Mineral.Cla, 0 },
                { Mineral.Obs, 0 },
                { Mineral.Geo, 0 },
            } }
        };

        if (timeLeft <= 0) {
            return configs.First();
        }
        
        var hasEnoughResForOre = HasEnoughResourcesForProduction(res, bp, Mineral.Ore);
        var hasEnoughResForCla = HasEnoughResourcesForProduction(res, bp, Mineral.Cla);
        var hasEnoughResForObs = HasEnoughResourcesForProduction(res, bp, Mineral.Obs);
        var hasEnoughResForGeo = HasEnoughResourcesForProduction(res, bp, Mineral.Geo);

        if (hasEnoughResForOre) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Ore.OreCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Ore);
            DoStuff(bp, updatedRes, updatedMr, origMr, configs, timeLeft);
        }

        if (hasEnoughResForCla) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Cla.OreCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Cla);
            DoStuff(bp, updatedRes, updatedMr, origMr, configs, timeLeft);
        }

        if (hasEnoughResForObs) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Obs.OreCost).DecrementResource(Mineral.Cla, bp.Obs.ClaCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Obs);
            DoStuff(bp, updatedRes, updatedMr, origMr, configs, timeLeft);
        }

        if (hasEnoughResForGeo) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Geo.OreCost).DecrementResource(Mineral.Obs, bp.Geo.ObsCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Geo);
            DoStuff(bp, updatedRes, updatedMr, origMr, configs, timeLeft);
        }

        configs.Add(GetOptimalProductionConfiguration(bp, res, mr, origMr, timeLeft-1));

        configs.Add(new Configuration() {
            Payoff = GeoRobotProductionPayoff(bp, mr, timeLeft),
            ProductionPlan = new Dictionary<Mineral, int>() {
                { Mineral.Ore, (mr.Ore - origMr.Ore) },
                { Mineral.Cla, (mr.Cla - origMr.Cla) },
                { Mineral.Obs, (mr.Obs - origMr.Obs) },
                { Mineral.Geo, (mr.Geo - origMr.Geo) },
            }
        });

        if (mr.Obs - origMr.Obs > 0) {
            var a = 5;
        }
        
        return configs.MaxBy(x => x.Payoff);
    }
    */
 

    /*
    private void DoStuff(Blueprint bp, Resources updatedRes, MiningRates updatedMr, MiningRates origMr, List<Configuration> configs, int timeLeft) {
        if (Memoized.ContainsKey((updatedRes, updatedMr))) {
            configs.Add(Memoized[(updatedRes, updatedMr)]);
        }
        else {
            var result = GetOptimalProductionConfiguration(bp, updatedRes, updatedMr, origMr, timeLeft);
            Memoized.Add((updatedRes, updatedMr), result);
            configs.Add(result);
        }
    }

*/
}
