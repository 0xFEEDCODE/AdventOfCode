using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

using Framework;

using Microsoft.VisualBasic;

namespace Challenges2022;

public static partial class Ext
{
}

public class Solution15 : SolutionFramework {
    public static char BeaconRep => 'B';
    public static char SensorRep => 'S';
    public static char SignalRep => '#';
    
    public class Coord  : IComparable<Coord>, IEquatable<Coord> {
        public (int, int) Coords;
        public int Row {
            get => Coords.Item1;
            set => Coords.Item1 = value;
        }

        public int Col {
            get => Coords.Item2;
            set => Coords.Item2 = value;
        }

        public Coord(int row, int col) {
            Coords = (row, col);
        }

        public Coord Copy() => new Coord(Row, Col);
        public int CompareTo(Coord other) => Coords.CompareTo(other.Coords);
        public bool Equals(Coord? other) => Row == other.Row && Col == other.Col;
        public override int GetHashCode() => (Row.GetHashCode()*2) + (Col.GetHashCode()*7);
        public static bool operator ==(Coord? a, Coord b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.Row == b.Row && a.Col == b.Col;
        }

        public static bool operator !=(Coord a, Coord b) => !(a == b);
    }
    public struct Beacon {
        public Coord Coords;
        public char Rep => BeaconRep;
    }
    public struct Sensor {
        public Coord Coords;
        public Beacon Beacon;
        public char Rep => SensorRep;
    }

    class Outline {
        public List<Coord> Coords = new List<Coord>();
    }
    struct Edge {
        public Coord Top;
        public Coord Bot;
        public Coord Right;
        public Coord Left;
    }
    
    public Solution15() : base(15) { }
    List<Coord> GetAllCoordsBetweenSensorAndBeaconOld(Sensor sensor) {
        var sRow = sensor.Coords.Row;
        var sCol = sensor.Coords.Col;

        var foundAtYCoords = new List<Coord>();
        int y = 100;
        
        var found = false;
        var l = 0;
        while (!found) {
            l+=1;
            for (var i = 0; i <= l; i++) {
                /*
                if (sRow + i == y) {foundAtYCoords.Add(new Coord(sRow + i, sCol - l + i));}
                if (sRow - i == y) {foundAtYCoords.Add(new Coord(sRow - i, sCol + l - i));}
                if (sRow + i == y) {foundAtYCoords.Add(new Coord(sRow + i, sCol + l - i));}
                if (sRow - i == y) {foundAtYCoords.Add(new Coord(sRow - i, sCol - l + i));}
                */
                {foundAtYCoords.Add(new Coord(sRow + i, sCol - l + i));}
                {foundAtYCoords.Add(new Coord(sRow - i, sCol + l - i));}
                {foundAtYCoords.Add(new Coord(sRow + i, sCol + l - i));}
                {foundAtYCoords.Add(new Coord(sRow - i, sCol - l + i));}

                if(!found) found = (sRow + i == sensor.Beacon.Coords.Row && sCol - l + i == sensor.Beacon.Coords.Col);
                if(!found) found = (sRow - i == sensor.Beacon.Coords.Row && sCol + l - i == sensor.Beacon.Coords.Col);
                if(!found) found = sRow + i == sensor.Beacon.Coords.Row && sCol + l - i == sensor.Beacon.Coords.Col;
                if(!found) found = sRow - i == sensor.Beacon.Coords.Row && sCol - l + i == sensor.Beacon.Coords.Col;
            }
        }

        foundAtYCoords = foundAtYCoords.Where(x => !(x.Col == sensor.Beacon.Coords.Col && x.Row == sensor.Beacon.Coords.Row)).Distinct().ToList();
        return foundAtYCoords;
    }
    
    List<Coord> GetAllCoordsBetweenSensorAndBeacon(Sensor sensor) {
        var sRow = sensor.Coords.Row;
        var sCol = sensor.Coords.Col;

        var foundAtYCoords = new List<Coord>();
        int y = 2000000;
        
        y = 2000000;
        var distance = Math.Abs(sRow - sensor.Beacon.Coords.Row) + Math.Abs(sCol - sensor.Beacon.Coords.Col);
        var diff = Math.Abs(distance - Math.Abs(y-sRow));
        
        var res = (diff * 2)+1;
        if (sensor.Beacon.Coords.Row == y)
            res--;
        
        if (y > sRow && sRow + distance < y)
            res = 0;
        
        if (y < sRow && sRow - distance > y)
            res = 0;

        if (res > 0) {
            if (!SensorsAndBeaconCoords.Contains((y, sensor.Coords.Col))) 
                Results.Add((y, sensor.Coords.Col));
            for (var i = 1; i < (res/2)+1; i++) {
                if (!SensorsAndBeaconCoords.Contains((y, sensor.Coords.Col-i))) 
                    Results.Add((y, sensor.Coords.Col-i));
                if (!SensorsAndBeaconCoords.Contains((y, sensor.Coords.Col+i))) 
                    Results.Add((y, sensor.Coords.Col+i));
                //Console.WriteLine(((y, sensor.Coords.Col-i), (y, sensor.Coords.Col+i)));
            }
        }
        
        Console.WriteLine((sensor.Coords.Coords, sensor.Beacon.Coords.Coords, foundAtYCoords.Count, res));
        return foundAtYCoords;
    }
    
    Edge GetEdgeBetweenSensorAndBeacon(Sensor sensor) {
        var distance = Math.Abs(sensor.Coords.Row - sensor.Beacon.Coords.Row) + Math.Abs(sensor.Coords.Col - sensor.Beacon.Coords.Col);
        var edge = new Edge() {
            Top = new Coord(sensor.Coords.Row - (distance), sensor.Coords.Col),
            Bot = new Coord(sensor.Coords.Row + (distance), sensor.Coords.Col),
            Left = new Coord(sensor.Coords.Row, sensor.Coords.Col - (distance)),
            Right = new Coord(sensor.Coords.Row, sensor.Coords.Col + (distance))
        };
        return edge;
    }
    

    private List<(int, int)> SensorsAndBeaconCoords;
    private HashSet<(int, int)> Results = new HashSet<(int, int)>();

    public readonly int Offset = 0;
    public override string[] Solve() {
        int H = 1000,  W = 1000;
        var grid = (H, W).CreateGrid<char>();
        grid.SetAllCellsToValue('.');

        var sensors = new List<Sensor>();
        foreach (var line in RawInputSplitByNl) {
            var split = line.Split(':');
            var cSensor = Regex.Matches(split[0], @"-?\d*\.{0,1}\d+").ToList().Select(x=>int.Parse(x.Value)).ToArray();
            var cBeacon = Regex.Matches(split[1], @"-?\d*\.{0,1}\d+").ToList().Select(x=>int.Parse(x.Value)).ToArray();
            //var sensor = new Sensor() { Coords = new Coord((cSensor[1]), (cSensor[0])), Beacon = { Coords = new Coord((cBeacon[1]), (cBeacon[0])) } };
            var sensor = new Sensor() { Coords = new Coord((cSensor[1]+Offset), (cSensor[0]+Offset)), Beacon = { Coords = new Coord((cBeacon[1]+Offset), (cBeacon[0]+Offset)) } };
            sensors.Add(sensor);
        }

        sensors = sensors.OrderBy(x => x.Coords.Coords).ToList();
        
        //print(grid, sensors, sensors.SelectMany(GetAllCoordsBetweenSensorAndBeaconOld).ToList());
        SensorsAndBeaconCoords = sensors.Select(x => x.Beacon.Coords.Coords).ToList();
        SensorsAndBeaconCoords.AddRange(sensors.Select(x => x.Coords.Coords));
        
        var edges = sensors.Select(GetEdgeBetweenSensorAndBeacon).ToList();
        var dict = new Dictionary<(int, int), int>();

        var outlines = new List<Coord>();
        foreach (var e in edges) {
            var outline = new Outline();
            var r = e.Left.Row;
            var c = e.Left.Col;
            while ((r--, c++) != e.Top.Coords) {
                //outline.Coords.Add(new Coord(r, c));
                if (!dict.ContainsKey((r, c - 1)))
                    dict.Add((r, c - 1), 0);
                dict[(r, c-1)]++;
                if (!dict.ContainsKey((r, c)))
                    dict.Add((r, c), 0);
                dict[(r, c)]--;
                if (!dict.ContainsKey((r, c+1)))
                    dict.Add((r, c+1), 0);
                dict[(r, c+1)]--;
            }
            r++; c--;
            
            while ((r++, c++) != e.Right.Coords) {
                //outline.Coords.Add(new Coord(r, c));
                if (!dict.ContainsKey((r, c + 1)))
                    dict.Add((r, c + 1), 0);
                dict[(r, c+1)]++;
                if (!dict.ContainsKey((r, c)))
                    dict.Add((r, c), 0);
                dict[(r, c)]--;
                if (!dict.ContainsKey((r, c-1)))
                    dict.Add((r, c-1), 0);
                dict[(r, c-1)]--;
            }
            r--; c--;

            while ((r++, c--) != e.Bot.Coords) {
                //outline.Coords.Add(new Coord(r, c));
                if (!dict.ContainsKey((r, c+1)))
                    dict.Add((r, c+1), 0);
                dict[(r, c+1)]++;
                if (!dict.ContainsKey((r, c)))
                    dict.Add((r, c), 0);
                dict[(r, c)]--;
                if (!dict.ContainsKey((r, c-1)))
                    dict.Add((r, c-1), 0);
                dict[(r, c-1)]--;
            }
            r--; c++;

            while ((r--, c--) != e.Left.Coords) {
                //outline.Coords.Add(new Coord(r, c));
                if (!dict.ContainsKey((r, c-1)))
                    dict.Add((r, c-1), 0);
                dict[(r, c-1)]++;
                if (!dict.ContainsKey((r, c)))
                    dict.Add((r, c), 0);
                dict[(r, c)]--;
                if (!dict.ContainsKey((r, c+1)))
                    dict.Add((r, c+1), 0);
                dict[(r, c+1)]--;
            }

            /*
            outlines.AddRange(outline.Coords);
            outlines.DistinctBy(x => x.Coords);
        */
        }


        //print(grid, sensors, outlines);
        var v = dict.Where(kv => kv.Value == 4).Where(kv=>!SensorsAndBeaconCoords.Contains(kv.Key)).ToList();
        var res = v.Select(kv => (kv.Key.Item2 * 4000000) + kv.Key.Item1).ToList();
        //print(grid, sensors, v.Select(kv => new Coord(kv.Key.Item1, kv.Key.Item2)).ToList());
        //print(grid, sensors, outlines);
        var limit = 20;
        var found = 0;
        Coord pos;

        
        var ans2 = Results.Count;
        //var ans = signals.Where(s => s.Row == (((2000000+Offset))/Div)).Distinct().Count()*Div;
        Console.WriteLine(ans2);
        return Answers;
    }
    
    void print(char[][] grid, List<Sensor> sensors, List<Coord> signals){
        Console.WriteLine();
        for (var i = Offset-30; i < Offset+100; i++) {
            Console.WriteLine();
            Console.Write(i-Offset-30 + " = ");
            for (var j = Offset-30; j < Offset+100; j++) {
                var s = sensors.Where(s => s.Coords.Row == i && s.Coords.Col == j);
                var b = sensors.Where(s => s.Beacon.Coords.Row == i && s.Beacon.Coords.Col == j).Select(s => s.Beacon);
                var sig = signals.Where(s => s.Row == i && s.Col == j);
                if(s.Any())
                    Console.Write(s.First().Rep);
                else if(b.Any())
                    Console.Write(b.First().Rep);
                else if(sig.Any())
                    Console.Write(SignalRep);
                else
                    Console.Write(grid[i][j]);
            }
        }
    }
}
