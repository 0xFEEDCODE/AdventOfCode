using Facet.Combinatorics;

using Framework;

namespace Solutions2023;

record ConnectionNode(string Name, ICollection<ConnectionNode> Connected);

class TBF
{
    private int id;
    public int SccCount;

    private string C1;
    private string C2;
    private string C3;
    private string C4;
    private string C5;
    private string C6;
    
    private const int _unvisited = -1;

    private readonly Dictionary<string, int> ids = new();
    private readonly Dictionary<string, int> low = new();
    private readonly Dictionary<string, bool> onStack = new();
    private readonly Stack<string> stack = new();

    private ICollection<ConnectionNode> graph;
    
    public Dictionary<string, int> FindSccs(ICollection<ConnectionNode> connectionNodes, string c1 = "", string c2 = "", string c3 = "", string c4 = "", string c5 = "", string c6 = "")
    {
        C1 = c1;
        C2 = c2;
        C3 = c3;
        C4 = c4;
        C5 = c5;
        C6 = c6;
        
        id = 0;
        SccCount = 0;
        ids.Clear();
        low.Clear();
        onStack.Clear();
        stack.Clear();
        graph = connectionNodes;

        foreach (var node in graph) { ids[node.Name] = _unvisited; }
        foreach (var node in graph)
        {
            if (ids[node.Name] == _unvisited)
            {
                Dfs(node.Name);
            }
        }
        return low;
    }

    private void Dfs(string at, List<string> component = null)
    {
        stack.Push(at);
        onStack[at] = true;
        ids[at] = id;
        low[at] = id;
        id++;
        
        var currentNode = graph.Single(i => i.Name == at);

        foreach (var (to, _) in currentNode.Connected)
        {
            if ((at == C1 && to == C2) ||
                (at == C2 && to == C1) ||
                (at == C3 && to == C4) ||
                (at == C4 && to == C3) ||
                (at == C5 && to == C6) ||
                (at == C6 && to == C5) ||
                to == at
               )
            {
                continue;
            }
            
            if (ids[to] == _unvisited)
            {
                component?.Add(to);
                Dfs(to);
            }
            if (onStack[to])
            {
                low[at] = Math.Min(low[at], low[to]);
            }
        }

        if (ids[at] == low[at])
        {
            while (stack.Any())
            {
                var node = stack.Pop();
                onStack[node] = false;
                low[node] = ids[at];
                if (node == at)
                {
                    break;
                }
            }
            SccCount++;
        }
    }
}

class TarjanBridgeFinder
{
    private int order;
    private readonly Dictionary<string, int> orderMap = new();
    private readonly Dictionary<string, int> lowLinkMap = new();
    private readonly Dictionary<string, object?> visited = new();
    private string C1;
    private string C2;
    private string C3;
    private string C4;

    //public List<(ConnectionNode, ConnectionNode)> FindBridges(ConnectionNode startNode, (ConnectionNode C1, ConnectionNode C2) filter1, (ConnectionNode C1, ConnectionNode C2) filter2)
    public (ConnectionNode, ConnectionNode)[]? FindBridges(ConnectionNode startNode, string c1, string c2, string c3, string c4)
    {
        C1 = c1; C2 = c2; C3 = c3; C4 = c4;
        order = 0;
        orderMap.Clear();
        lowLinkMap.Clear();
        visited.Clear();
        var bridge = FindBridgesUtil(startNode, null);
        return bridge != null ? [bridge.Value] : null;
    }
    
    private (ConnectionNode, ConnectionNode)? FindBridgesUtil(ConnectionNode currentNode, ConnectionNode parentNode)
    {
        visited.Add(currentNode.Name, null);
        orderMap[currentNode.Name] = order;
        lowLinkMap[currentNode.Name] = order;
        order++;

        foreach (var neighbor in currentNode.Connected)
        {
            if ((currentNode.Name == C1&& neighbor.Name == C2) ||
                (currentNode.Name == C2&& neighbor.Name == C1) ||
                (currentNode.Name == C3&& neighbor.Name == C4) ||
                (currentNode.Name == C4&& neighbor.Name == C3)
               )
            {
                continue;
            }
            
            if (neighbor.Name == parentNode?.Name)
            {
                continue;
            }

            if (!visited.ContainsKey(neighbor.Name))
            {
                var bridge = FindBridgesUtil(neighbor, currentNode);
                if (bridge != null)
                {
                    return bridge;
                }

                lowLinkMap[currentNode.Name] = Math.Min(lowLinkMap[currentNode.Name], lowLinkMap[neighbor.Name]);

                if (orderMap[currentNode.Name] < lowLinkMap[neighbor.Name])
                {
                    return (currentNode, neighbor);
                }
            }
            else
            {
                lowLinkMap[currentNode.Name] = Math.Min(lowLinkMap[currentNode.Name], orderMap[neighbor.Name]);
            }
        }
        return null;
    }
}

public class Solution25() : SolutionFramework(25)
{
    public override string[] Solve()
    {
        var allWires = new List<string>();
        var components = new Dictionary<string, ICollection<string>>();

        foreach (var l in InpNl)
        {
            var spl = l.Split(':');
            var connectedWires = spl[1].Split(' ').Skip(1).Select(s => s.Trim()).ToArray();
            foreach (var cw in connectedWires)
            {
                allWires.Add(cw);
            }
            allWires.Add(spl[0]);
            components[spl[0]] = connectedWires;
        }

        allWires = allWires.ToHashSet().ToList();

        allWires.Sort();
        var connectionNodes = allWires.Select(w => new ConnectionNode(w, new List<ConnectionNode>())).ToArray();
        
        foreach (var node in connectionNodes)
        {
            if (components.ContainsKey(node.Name))
            {
                foreach (var w in components[node.Name])
                {
                    var connectedNode = connectionNodes.Single(n => n.Name == w);
                    if (!node.Connected.Contains(connectedNode))
                    {
                        node.Connected.Add(connectedNode);
                    }
                    if (!connectedNode.Connected.Contains(node))
                    {
                        connectedNode.Connected.Add(node);
                    }
                }
            }
            else
            {
                var connections = components.Where(kv => kv.Value.Contains(node.Name));
                foreach (var w in connections)
                {
                    var connectedNode = connectionNodes.Single(n => n.Name == w.Key);
                    if (!node.Connected.Contains(connectedNode))
                    {
                        node.Connected.Add(connectedNode);
                    }
                    if (!connectedNode.Connected.Contains(node))
                    {
                        connectedNode.Connected.Add(node);
                    }
                }
            }
        }

        var considered = 0;
        
        var n = 44646096;

        var bridgeFinder = new TarjanBridgeFinder();

        
        var tx = new System.Timers.Timer();
        tx.Stop();
        tx.Interval = 10000;
        tx.Elapsed += (_,__) =>
        {
            Console.WriteLine((considered, n-considered));
        };
        tx.Start();


        /*
        var bmz = connectionNodes.Single(x => x.Name == "bmz");
        var dbp = connectionNodes.Single(x => x.Name == "dbp");
        var mvs = connectionNodes.Single(x => x.Name == "mvs");
        var vjg = connectionNodes.Single(x => x.Name == "vjg");
        var rkh = connectionNodes.Single(x => x.Name == "rkh");
        var ldz = connectionNodes.Single(x => x.Name == "ldz");
        bmz.Connected.Remove(dbp);
        dbp.Connected.Remove(bmz);
        mvs.Connected.Remove(vjg);
        vjg.Connected.Remove(mvs);
        rkh.Connected.Remove(ldz);
        ldz.Connected.Remove(rkh);
        */

        /*
        var bridges = bridgeFinder.FindBridges(connectionNodes.First(), "bmz", "dbp", "mvs", "vjg");
        
        var tbf = new TBF();
        var sccs = tbf.FindSccs(connectionNodes);
        Console.WriteLine();
        */

        foreach(var n1 in connectionNodes) 
        {
            foreach (var n2 in n1.Connected)
            {
                if (n1.Name == n2.Name)
                {
                    continue;
                }

                foreach (var n3 in connectionNodes)
                {
                    if (n2.Name == n3.Name || n1.Name == n3.Name)
                    {
                        continue;
                    }

                    foreach (var n4 in n3.Connected)
                    {
                        if (n1.Name == n4.Name || n2.Name == n4.Name || n3.Name == n4.Name)
                        {
                            continue;
                        }

                        considered++;
                        var bridges = bridgeFinder.FindBridges(connectionNodes.First(), n1.Name, n2.Name, n3.Name, n4.Name);
                        if (bridges?.Length > 0)
                        {
                            Console.WriteLine("Found");
                            Console.WriteLine((n1.Name, n2.Name));
                            Console.WriteLine((n3.Name, n4.Name));
                            Console.WriteLine((bridges[0].Item1.Name, bridges[0].Item2.Name));

                            n1.Connected.Remove(n2);
                            n2.Connected.Remove(n1);
                            n3.Connected.Remove(n4);
                            n4.Connected.Remove(n3);
                            bridges[0].Item1.Connected.Remove(bridges[0].Item2);
                            bridges[0].Item2.Connected.Remove(bridges[0].Item1);

                            var g1 = GetConnectionGroup(bridges[0].Item1).Split('_').Length - 1;
                            var g2 = GetConnectionGroup(bridges[0].Item2).Split('_').Length - 1;
                            if (g1 > 1 && g2 > 1)
                            {
                                var r = g1 * g2;
                                AssignAnswer1(r);
                                return Answers;
                            }
                        }
                    }
                }
            }
        }
        
        /*
        foreach (var n1 in connectionNodes)
        {
            foreach (var n2 in n1.Connected)
            {
                if (n1 == n2)
                {
                    continue;
                }
                
                foreach (var n3 in connectionNodes)
                {
                    if (n2 == n3 || n1 == n3)
                    {
                        continue;
                    }

                    foreach (var n4 in n3.Connected)
                    {
                        if (n1 == n4 || n2 == n4 || n3 == n4)
                        {
                            continue;
                        }

                        foreach (var n5 in connectionNodes)
                        {
                            if (n1 == n5 || n2 == n5 || n3 == n5 || n4 == n5)
                            {
                                continue;
                            }
                            foreach (var n6 in n5.Connected)
                            {
                                /*
                                considered++;
                                var sccs = tbf.FindSccs(connectionNodes, n1.Name, n2.Name, n3.Name, n4.Name, n5.Name, n6.Name);
                                //var bridges = bridgeFinder.FindBridges(connectionNodes.First(), n1.Name, n2.Name, n3.Name, n4.Name);
                                if (sccs.DistinctBy(x => x.Value).Count() > 1)
                                {
                                    var distinct = sccs.DistinctBy(x => x.Value).ToArray();
                                    n1.Connected.Remove(n2);
                                    n2.Connected.Remove(n1);
                                    n3.Connected.Remove(n4);
                                    n4.Connected.Remove(n3);
                                    /*
                                    bridges[0].Item1.Connected.Remove(bridges[0].Item2);
                                    bridges[0].Item2.Connected.Remove(bridges[0].Item1);
                                    #2#
                                    
                                    
                                    Console.WriteLine(sccs);
                                    Console.WriteLine("Found");
                                    Console.WriteLine((n1.Name, n2.Name));
                                    Console.WriteLine((n3.Name, n4.Name));

                                    n1.Connected.Remove(n2);
                                    n2.Connected.Remove(n1);
                                    n3.Connected.Remove(n4);
                                    n4.Connected.Remove(n3);
                                    AssignAnswer1(sccs.Count(x => x.Value == distinct[0].Value) * sccs.Count(x => x.Value == distinct.Last().Value));
                                    return Answers;
                                }
                            #1#
                                
                        /*
                        considered++;
                        var bridges = bridgeFinder.FindBridges(connectionNodes.First(), n1.Name, n2.Name, n3.Name, n4.Name);
                        if (bridges.Count > 0)
                        {
                            Console.WriteLine("Found");
                            Console.WriteLine((n1.Name, n2.Name));
                            Console.WriteLine((n3.Name, n4.Name));
                            Console.WriteLine((bridges[0].Item1.Name, bridges[0].Item2.Name));

                            n1.Connected.Remove(n2);
                            n2.Connected.Remove(n1);
                            n3.Connected.Remove(n4);
                            n4.Connected.Remove(n3);
                            bridges[0].Item1.Connected.Remove(bridges[0].Item2);
                            bridges[0].Item2.Connected.Remove(bridges[0].Item1);
                            
                            var g1 = GetConnectionGroup(bridges[0].Item1).Split('_').Length - 1;
                            var g2 = GetConnectionGroup(bridges[0].Item2).Split('_').Length - 1;
                            if (g1 > 1 && g2 > 1)
                            {
                                var r = g1 * g2;
                                AssignAnswer1(r);
                                return Answers;
                            }
                        }
                    #1#
                            }
                        }
                        

                    }
                }
            }
        }
        */
        Console.WriteLine(considered);
        return Answers;
        //var combinations = (from cn in connectionNodes from connected in cn.Connected select (cn, connected)).ToList();
        List<List<ConnectionNode>> GetPathsToNode(ConnectionNode start, ConnectionNode end)
        {
            var paths = new List<List<ConnectionNode>>();
            
            var q = new Queue<ConnectionNode>();
            q.Enqueue(start);
            var cameFrom = new Dictionary<ConnectionNode, ConnectionNode>
            {
                [start] = null
            };

            while (q.Any())
            {
                var curr = q.Dequeue();

                if (curr == end)
                {
                    var path = new List<ConnectionNode>();
                    var temp = curr;
                    while (temp != curr)
                    {
                        path.Add(temp);
                        temp = cameFrom[temp];
                    }
                    continue;
                }
                
                foreach (var connected in curr.Connected)
                {
                    q.Enqueue(connected);
                    cameFrom[connected] = curr;
                }
            }
            
            return paths;
        }
        
        string GetConnectionGroup(ConnectionNode node, ICollection<(ConnectionNode C1, ConnectionNode C2)>? filter = null)
        {
            var st = new Stack<ConnectionNode>();
            st.Push(node);

            var visited = new List<ConnectionNode>();

            while (st.Any())
            {
                var current = st.Pop();

                var filterMatch = filter?.Where(x => x.C1 == current);
                
                if (visited.Contains(current))
                {
                    continue;
                }
                visited.Add(current);
                foreach (var connectionNode in current.Connected)
                {
                    if (filterMatch != null)
                    {
                        if (filterMatch.Any(m => m.C2.Name == connectionNode.Name))
                        {
                            continue;
                        }
                    }
                    st.Push(connectionNode);
                }
            }
            var visitedNames = visited.Select(x => x.Name).ToList();
            visitedNames.Sort();
            
            return visitedNames.Aggregate(string.Empty, (acc, x) => acc + "_" + x);
        }
    }
}

        /*
        foreach (var ignoredCombination in combinationsToRemove)
        {
            i++;
            Console.WriteLine(combinationsToRemove.Count - i);
            var groups = new List<ICollection<string>>();
            HashSet<string>? currentGroup = null;
            var wireConnections = new Stack<(string F, string T)>();
            foreach (var conn in allWireConnections)
            {
                if (ignoredCombination.Contains(conn))
                {
                    continue;
                }
                wireConnections.Push(conn);
            }
            
            /*
            while (wireConnections.Any())
            {
                var current = wireConnections.Pop();

                if (currentGroup is null || !currentGroup.Contains(current.F) && !currentGroup.Contains(current.T))
                {
                    currentGroup = new HashSet<string>();
                    groups.Add(currentGroup);
                }

                currentGroup.Add(current.F);
                currentGroup.Add(current.T);
            }

            var flag = true;
            while (flag)
            {
                flag = false;
                foreach (var wire in allWires)
                {
                    var existingGroups = groups.Where(g => g.Contains(wire));
                    if (existingGroups.Count() > 1)
                    {
                        var merged = existingGroups.SelectMany(x => x).ToHashSet();
                        foreach (var existingGroup in existingGroups.ToArray())
                        {
                            groups.Remove(existingGroup);
                        }
                        groups.Add(merged);
                        flag = true;
                    }
                }
            }
            #1#
            if (groups.Count == 2)
            {
                AssignAnswer1(groups[0].Count*groups[1].Count);
                return Answers;
            }
        }
        */


