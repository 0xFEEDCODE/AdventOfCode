using System.Collections.Concurrent;
using System.Reflection.PortableExecutable;

using Framework;

using static Challenges2022.Solution19;

namespace Challenges2022;

public static partial class Ext{
    public static MiningRates IncrementMiningRate(this MiningRates mr, Mineral mineral) {
        var updatedMiningRate = mr.Copy();
        if(mineral == Mineral.Ore) updatedMiningRate.Ore++;
        else if(mineral == Mineral.Cla) updatedMiningRate.Cla++;
        else if(mineral == Mineral.Obs) updatedMiningRate.Obs++;
        else if(mineral == Mineral.Geo) updatedMiningRate.Geo++;
        return updatedMiningRate;
    }
    public static Resources DecrementResource(this Resources res, Mineral mineral, int amount = 1) {
        var updatedRes = res.Copy();
        if (mineral == Mineral.Ore) updatedRes.Ore-=amount;
        else if (mineral == Mineral.Cla) updatedRes.Cla-=amount;
        else if (mineral == Mineral.Obs) updatedRes.Obs-=amount;
        else if (mineral == Mineral.Geo) updatedRes.Geo-=amount;
        return updatedRes;
    }
}

public partial class Solution19 : SolutionFramework {
    public Solution19() : base(19) {
    }

    public int OreRobotProductionPayoff(Blueprint bp, MiningRates mr, int timeLeft) =>
          bp.Geo.OreCost * ((mr.Ore * timeLeft) + 1);

    public int ClaRobotProductionPayoff(Blueprint bp, MiningRates mr, int timeLeft) =>
         (bp.Cla.OreCost * OreRobotProductionPayoff(bp, mr, timeLeft)) * ((mr.Cla * timeLeft) + 1);

    public int ObsRobotProductionPayoff(Blueprint bp, MiningRates mr, int timeLeft) =>
        ((bp.Obs.OreCost * OreRobotProductionPayoff(bp, mr, timeLeft)) + (bp.Obs.ClaCost * ClaRobotProductionPayoff(bp, mr, timeLeft))) * ((mr.Obs * timeLeft) + 1);

    public int GeoRobotProductionPayoff(Blueprint bp, MiningRates mr, int timeLeft) =>
        ((bp.Geo.OreCost * OreRobotProductionPayoff(bp, mr, timeLeft)) + (bp.Geo.ObsCost * ObsRobotProductionPayoff(bp, mr, timeLeft))) * ((mr.Geo * timeLeft) + 1);

    public static int TimeTotal = 24;
    public static int TimeElapsed;
    public int GetTimeLeft() => TimeTotal - TimeElapsed;

    public Dictionary<(Resources, MiningRates, MiningRates, int), Results> Memoized = new();
    public Dictionary<int, List<Results>> BPResults = new();


    public override string[] Solve() {
        var blueprints = ParseInput();

        var bestResults = new Dictionary<Blueprint, Results>();
        var resultSum = 0;
        var resultPt2 = 0;

        /*
        Parallel.ForEach(blueprints, bp => {
            var mr = new MiningRates(1, 0, 0, 0);
            var res = new Resources(0, 0, 0, 0);
            MiningRates updatedMr = default;
            var id = bp.Id;
            Console.WriteLine($"Starting computing for BP {id}");
            GetOptimalProductionConfiguration(bp, res, mr, new List<Results>(), 24);
            var bestFound = BPResults[id].MaxBy(x => x.Geo);
            bestResults.TryAdd(bp, bestFound);
            Console.WriteLine($"RESULTS for BP {id} complete");
            resultSum += id * bestFound.Geo;
        });
        */
        foreach (var bp in blueprints.Take(3)) {
            var mr = new MiningRates(1, 0, 0, 0);
            var res = new Resources(0, 0, 0, 0);

            MiningRates updatedMr = default;
            var id = bp.Id;
            Memoized.Clear();
            BPResults.Add(id, new List<Results>());
            GetOptimalProductionConfiguration(bp, res, mr, new Results(), 32);
            var bestFound = BPResults[id].MaxBy(x => x.Geo);
            resultSum += id * bestFound.Geo;
            if (resultPt2 != 0)
                resultPt2 *= bestFound.Geo;
            else
                resultPt2 = bestFound.Geo;
            bestResults.Add(bp, bestFound);
            Console.WriteLine($"RESULTS for BP {bp}");
            Console.WriteLine($"SUM is {resultSum}");
            Console.WriteLine($"SUM pt2 is {resultPt2}");
        }

        var result = bestResults.Select(x => x.Key.Id * x.Value.Geo).Sum();

        Console.WriteLine(resultPt2);
        Console.WriteLine(result);

            /*
            var config = GetOptimalProductionConfiguration(bp, res, mr, mr, GetTimeLeft());
            for (var i = 0; i < config.ProductionPlan[Mineral.Ore]; i++)
                res = StartRobotProductionAndUpdateResources(bp, Mineral.Ore, res, pl);
            for (var i = 0; i < config.ProductionPlan[Mineral.Cla]; i++) 
                res = StartRobotProductionAndUpdateResources(bp, Mineral.Cla, res, pl);
            for (var i = 0; i < config.ProductionPlan[Mineral.Obs]; i++)
                res = StartRobotProductionAndUpdateResources(bp, Mineral.Obs, res, pl);
            for (var i = 0; i < config.ProductionPlan[Mineral.Geo]; i++)
                res = StartRobotProductionAndUpdateResources(bp, Mineral.Geo, res, pl);
            */
                /*
                res = UpdateResourcesBasedOnMiningRate(res , mr);

                Console.WriteLine($"At Minute {TimeElapsed + 1}");
                Console.WriteLine($"Mining rates = ORE: {mr.Ore} | CLA: {mr.Cla} | OBS: {mr.Obs} | GEO: {mr.Geo}");
                Console.WriteLine($"AvailableRes = ORE: {res.Ore} | CLA: {res.Cla} | OBS: {res.Obs} | GEO: {res.Geo}");
            */
        return Answers;
    }

    private Resources StartRobotProductionAndUpdateResources(Blueprint bp, Mineral mineral, Resources res, ProductionLine pl) {
        var updatedRes = res.Copy();
        switch (mineral) {
            case Mineral.Ore:
                pl.StartProduction(Mineral.Ore);
                updatedRes = updatedRes.DecrementResource(Mineral.Ore, bp.Ore.OreCost);
                break;
            case Mineral.Cla:
                pl.StartProduction(Mineral.Cla);
                updatedRes = updatedRes.DecrementResource(Mineral.Ore, bp.Cla.OreCost);
                break;
            case Mineral.Obs:
                pl.StartProduction(Mineral.Obs);
                updatedRes = updatedRes.DecrementResource(Mineral.Ore, bp.Obs.OreCost).DecrementResource(Mineral.Cla, bp.Obs.OreCost);
                break;
            case Mineral.Geo:
                pl.StartProduction(Mineral.Geo);
                updatedRes = updatedRes.DecrementResource(Mineral.Ore, bp.Geo.OreCost).DecrementResource(Mineral.Obs, bp.Geo.ObsCost);
                break;
        }

        return updatedRes;
    }

    private MiningRates GetAllAvailableRobotsOffTheLine(ProductionLine pl, MiningRates mr) {
        var updatedMiningRates = mr;
        if (pl.AnyProducing()) {
            if (pl.TryGetOreRobotOffLine())
                updatedMiningRates = mr.IncrementMiningRate(Mineral.Ore);
            if (pl.TryGetClaRobotOffLine())
                updatedMiningRates = mr.IncrementMiningRate(Mineral.Cla);
            if (pl.TryGetObsRobotOffLine())
                updatedMiningRates = mr.IncrementMiningRate(Mineral.Obs);
            if (pl.TryGetGeoRobotOffLine())
                updatedMiningRates = mr.IncrementMiningRate(Mineral.Geo);
        }

        return updatedMiningRates;
    }

    private static Resources UpdateResourcesBasedOnMiningRate(Resources res, MiningRates mr) {
        var updatedRes = res.Copy();
        updatedRes.Ore += mr.Ore;
        updatedRes.Cla += mr.Cla;
        updatedRes.Obs += mr.Obs;
        updatedRes.Geo += mr.Geo;
        return updatedRes;
    }

    private List<Results> GlobalResults = new List<Results>();

    private void GetOptimalProductionConfiguration(Blueprint bp, Resources res, MiningRates mr, Results endResults, int timeLeft, MiningRates mrAfterRobotsProduced = null) {
        timeLeft--;
        if (timeLeft < 0) {
            if (endResults.Geo == 0)
                return;
            BPResults[bp.Id].Add(endResults);
            return;
        }

        if (res.Ore > 20) {
            return;
        }

        if (Memoized.ContainsKey((res, mr, mrAfterRobotsProduced, timeLeft))) {
            BPResults[bp.Id].Add(endResults);
            return;
        }
        Memoized.Add((res, mr, mrAfterRobotsProduced, timeLeft), endResults);

        //Console.WriteLine($"MINUTE: {TimeTotal-timeLeft}");
        res = UpdateResourcesBasedOnMiningRate(res, mr);
        endResults = new Results(res.Ore, res.Cla, res.Obs, res.Geo);
        if(mrAfterRobotsProduced!=null)
            mr = mrAfterRobotsProduced;

        var hasEnoughResForOre = HasEnoughResourcesForProduction(res, bp, Mineral.Ore);
        var hasEnoughResForCla = HasEnoughResourcesForProduction(res, bp, Mineral.Cla);
        var hasEnoughResForObs = HasEnoughResourcesForProduction(res, bp, Mineral.Obs);
        var hasEnoughResForGeo = HasEnoughResourcesForProduction(res, bp, Mineral.Geo);

        if (hasEnoughResForGeo) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Geo.OreCost).DecrementResource(Mineral.Obs, bp.Geo.ObsCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Geo);
            GetOptimalProductionConfiguration(bp, updatedRes, mr, endResults, timeLeft, updatedMr);
        }

        if (hasEnoughResForObs) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Obs.OreCost).DecrementResource(Mineral.Cla, bp.Obs.ClaCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Obs);
            if (updatedRes.Obs <= bp.BiggestObsCost())
                GetOptimalProductionConfiguration(bp, updatedRes, mr, endResults, timeLeft, updatedMr);
        }

        if (hasEnoughResForOre) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Ore.OreCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Ore);
            if (updatedRes.Ore <= bp.BiggestOreCost())
                GetOptimalProductionConfiguration(bp, updatedRes, mr, endResults, timeLeft, updatedMr);
        }

        if (hasEnoughResForCla) {
            var updatedRes = res.DecrementResource(Mineral.Ore, bp.Cla.OreCost);
            var updatedMr = mr.IncrementMiningRate(Mineral.Cla);
            if (updatedRes.Cla <= bp.BiggestClaCost())
                GetOptimalProductionConfiguration(bp, updatedRes, mr, endResults, timeLeft, updatedMr);
        }

        GetOptimalProductionConfiguration(bp, res, mr, endResults, timeLeft, mr);
    }

    /*
    private void DoStuff(Blueprint bp, Results results, int timeLeft, Resources updatedRes, MiningRates updatedMr) {
        Results result;
        if (Memoized.ContainsKey((updatedRes, updatedMr, timeLeft))) {
        } else {
            GetOptimalProductionConfiguration(bp, updatedRes, updatedMr, results, timeLeft);
        }
    }
    */

    private static bool HasEnoughForOreAndCla(Resources res, Blueprint bp) {
        return res.Ore >= bp.Ore.OreCost && (res.Ore - bp.Ore.OreCost) >= bp.Cla.OreCost;
    }

    private static bool HasEnoughResourcesForProduction(Resources res, Blueprint bp, Mineral mineral) {
        var hasEnoughResourcesForProduction = mineral switch {
            Mineral.Ore => res.Ore >= bp.Ore.OreCost,
            Mineral.Cla => res.Ore >= bp.Cla.OreCost,
            Mineral.Obs => res.Ore >= bp.Obs.OreCost && res.Cla >= bp.Obs.ClaCost,
            Mineral.Geo => res.Ore >= bp.Geo.OreCost && res.Obs >= bp.Geo.ObsCost,
            _ => throw new ArgumentOutOfRangeException(nameof(mineral), mineral, null)
        };
        return hasEnoughResourcesForProduction;
    }
}
