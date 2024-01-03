using Framework;

namespace Solutions2023;

public class Solution20() : SolutionFramework(20)
{

    enum ModuleT { FlipFlop, Conjunction, Broadcaster, Dummy };
    enum Pulse { Low, High };

    class Module(ModuleT t, IEnumerable<string> output)
    { private Dictionary<string, Pulse> inputState = new(); private bool isOn; private Pulse? lastPulseReceived = null; public ModuleT Type = t; public IEnumerable<string> OutputTo = output; public IDictionary<string, Pulse> InputState { get { if (t is not ModuleT.Conjunction) { throw new InvalidOperationException(); } return inputState; } } public bool IsOn { get { if (t is not ModuleT.FlipFlop) { throw new InvalidOperationException(); } return isOn; } } public void FlipOnOff() { if (t is not ModuleT.FlipFlop) { throw new InvalidOperationException(); } isOn = !isOn; } public Pulse LastReceivedPulse { set { if (t is not ModuleT.Broadcaster) { throw new InvalidOperationException(); } lastPulseReceived = value; } } public Pulse GetAndResetLastReceivedPulse() { if (t is not ModuleT.Broadcaster) { throw new InvalidOperationException(); } var v = lastPulseReceived.GetValueOrDefault(); lastPulseReceived = null; return v; } }
    
    public async Task X()
    {
        await Task.Yield();
    }
    
    public override string[] Solve()
    {
        var modulesState = new Dictionary<string, Module>();

        //Input parse
        foreach (var l in InpNl) { var spl = l.Split("->"); var type = ModuleT.Broadcaster; if (spl[0][0] is '%' or '&') { type = spl[0][0] == '%' ? ModuleT.FlipFlop : ModuleT.Conjunction; } var name = type is ModuleT.Broadcaster ? spl[0].Trim() : spl[0].Trim().Substring(1); var outp = spl[1].Split(',').Select(x => x.Trim()).ToArray(); var module = new Module(type, outp); modulesState.Add(name, module); }
        // Handle dummy
        foreach (var v in modulesState.Values.ToArray()) { foreach (var outp in v.OutputTo) { if (!modulesState.ContainsKey(outp)) { modulesState.Add(outp, new Module(ModuleT.Dummy, Array.Empty<string>())); } } }
        // Initialize conjunction
        foreach (var (k, v) in modulesState) { foreach (var outp in v.OutputTo) { var target = modulesState[outp]; if (target.Type != ModuleT.Conjunction) { continue; } target.InputState.Add(k, Pulse.Low); } }

        var debug = false;

        var lowPN = 0;
        var highPN = 0;
        double i = -1d;

        while(true)
        {
            i++;
            var f = 0;
            // Initialize AllModules
            //allModules.Clear(); foreach (var (k,v) in initialState) { var m = new Module(v.Type, v.OutputTo.ToArray()); if (m.Type is ModuleT.Conjunction) { foreach (var (k1, v2) in v.InputState) { m.InputState.Add(k1, Pulse.Low); } } allModules.Add(k, m); }

            //Push button
            var broadcastModule = modulesState["broadcaster"]; broadcastModule.LastReceivedPulse = Pulse.Low; lowPN++; var processingQ = new Queue<(string ModuleName, Module module)>(); processingQ.Enqueue(("broadcaster", broadcastModule));
            
            while (processingQ.Any())
            {
                var (moduleName, module) = processingQ.Dequeue();
                Pulse? pulse = null;
                switch (module.Type)
                {
                    case ModuleT.Broadcaster:
                        pulse = module.GetAndResetLastReceivedPulse();
                        if(debug) { Console.WriteLine("broadcaster -" + pulse + ", " + module.OutputTo.Aggregate((acc, x) => acc + ", " + x)); }
                        SendSignalToAllOutputs(moduleName, module, pulse.Value);
                        break;
                    case ModuleT.Conjunction:
                        pulse = module.InputState.All(kv => kv.Value == Pulse.High) ? Pulse.Low : Pulse.High;
                        if ((moduleName is ("kd" or "zf" or "vg" or "gs")) && pulse is Pulse.High)
                        {
                            Console.Write($"\n{i} {moduleName}");
                        }
                        if (moduleName is "rg" && module.InputState.Any(x => x.Value is Pulse.High) && f == 0)
                        {
                            f++;
                            Console.Write("\n"+i+" "+moduleName);
                            foreach (var (n,s) in module.InputState)
                            {
                                Console.Write(" " + n + " " + s + ",");
                            }
                        }
                        if(debug) { Console.WriteLine($"&{moduleName} -" + pulse + ", " + module.OutputTo.Aggregate((acc, x) => acc + ", " + x)); }
                        SendSignalToAllOutputs(moduleName, module, pulse.Value);
                        break;
                    case ModuleT.FlipFlop:
                        pulse = module.IsOn ? Pulse.Low : Pulse.High;
                        if(debug) { Console.WriteLine($"%{moduleName} -" + pulse + ", " + module.OutputTo.Aggregate((acc, x) => acc + ", " + x)); }
                        SendSignalToAllOutputs(moduleName, module, pulse.Value);
                        module.FlipOnOff();
                        break;
                }
                
                if(debug) { Task.Delay(1000).GetAwaiter().GetResult();}
                foreach (var outp in module.OutputTo)
                {
                    var target = modulesState[outp];
                    if (target.Type is ModuleT.FlipFlop && pulse is Pulse.High)
                    {
                        continue;
                    }
                    if (target.Type is ModuleT.Dummy)
                    {
                        if (pulse is Pulse.Low)
                        {
                            AssignAnswer2(i);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    processingQ.Enqueue((outp, modulesState[outp]));
                }
                if (pulse is Pulse.Low)
                {
                    lowPN += module.OutputTo.Count();
                }
                else
                {
                    highPN += module.OutputTo.Count();
                }
            }

            if (debug)
            {
                foreach (var (k, v) in modulesState)
                {
                    if (v.Type is ModuleT.Conjunction)
                    {
                        Console.Write("\n" + k + " CONJ: ");
                        foreach (var (k1, v1) in v.InputState)
                        {
                            Console.Write($"{k1} - {v1}, ");
                        }
                    }
                    else if (v.Type is ModuleT.FlipFlop)
                    {
                        Console.Write("\n" + k + $" FL: {v.IsOn}");
                    }
                }
            }
        }
        AssignAnswer1(lowPN*highPN);
        
        return Answers;
        
        void SendSignalToAllOutputs(string moduleName, Module module, Pulse pulse)
        {
            foreach (var targetName in module.OutputTo)
            {
                var target = modulesState[targetName];
                switch (target.Type)
                {
                    case ModuleT.Conjunction:
                        target.InputState[moduleName] = pulse;
                        break;
                    case ModuleT.FlipFlop when pulse is Pulse.Low:
                        break;
                    case ModuleT.Broadcaster:
                        target.LastReceivedPulse = pulse;
                        break;
                }
            }
        }
    }
    
}
