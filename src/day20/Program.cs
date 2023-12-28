// https://adventofcode.com/2023/day/20
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Net.NetworkInformation;

int aocPart = 1;
string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DanTh\github\aoc2023\inputs\day20.txt");

//// Example 1 input
//lines = new string[] {
//"broadcaster -> a, b, c",
//"%a -> b",
//"%b -> c",
//"%c -> inv",
//"&inv -> a",
//};

// Example 2 input
//lines = new string[] {
//    "broadcaster -> a",
//    "%a -> inv, con",
//    "&inv -> b",
//    "%b -> con",
//    "&con -> output",
//};

Dictionary<string, Module> modules = Network.Components;
List<string> conjunctionModules = new();

// Build up network (Modules and "wires"/connections)
modules.Add("button", new Button(new string[] {"broadcaster"}, modules));
modules.Add("output", new Output(modules));
modules.Add("rx", new Output(modules));
foreach (var line in lines)
{
    string[] halves = line.Split(" -> ");
    string name = halves[0];
    char? typeChar = null;
    if (name[0] == '%' || name[0] == '&')
    {
        typeChar = name[0];
        name = name[1..];
    }
    string[] outputs = halves[1].Split(",",StringSplitOptions.TrimEntries);
    switch (typeChar.ToModule())
    {
        case ModuleType.Button:
            modules.Add(name, new Button(name, outputs, modules));
            break;
        case ModuleType.Broadcaster:
            modules.Add(name, new Broadcaster(name, outputs, modules));
            break;
        case ModuleType.FlipFlop:
            modules.Add(name, new FlipFlop(name, outputs, modules));
            break;
        case ModuleType.Conjunction:
            modules.Add(name, new Conjunction(name, outputs, modules));
            conjunctionModules.Add(name);
            break;
        default:
            Debug.Assert(false, "Unknown Module type");
            break;
    }
    Console.WriteLine(line);
}

// Find, and register, all input connections to each Conjunction module
foreach (string cjName in conjunctionModules)
{
    Conjunction cjMod = (Conjunction)modules[cjName];
    foreach ((string from, Module m) in modules)
    {
        foreach (string destination in m.Outputs)
        {
            if (destination == cjName) cjMod.RegisterInput(from);
        }
    }
}

Button button = (Button)modules["button"];
Output output = (Output)modules["rx"];

long? maybeAnsPart2 = null;

for (int i = 0; i < 1000000000; i++)
{
    button.Press();
    button.ProcessBacklog();
    if (maybeAnsPart2 is null && !output.State)
    {
        maybeAnsPart2 = i + 1;
        break;
    }
}
long ansPart1 = Module.PulseCountLow * Module.PulseCountHigh;
long ansPart2 = maybeAnsPart2 is null ? -1 : (int)maybeAnsPart2;

Console.WriteLine($"The answer for Part {1} is {ansPart1}");
Console.WriteLine($"The answer for Part {2} is {ansPart2}");

Console.ReadKey();

// End
// End
// End

static class Network
{
    public static Dictionary<string, Module> Components = new();
    public static Queue<(string From, string To, bool Polarity)> Backlog = new();
}

/// <summary>
/// Static var Network holds list of all Modules by name.
/// A Module, when Pulse'd, passes along a modified (?) pulse to each of it's outputs
/// State management (see "state" variable) is handled in concrete classes and must
/// resolve to a single boolean value ("state") to be passed along to all outputs
/// </summary>
abstract class Module
{
    Dictionary<string, Module> modules;
    public static long PulseCountLow { get; private set; }
    public static long PulseCountHigh { get; private set; }
    public string Name { get; init; }
    public List<string> Outputs;

    public Module(string name, IEnumerable<string> outputs, Dictionary<string, Module> network)
    {
        Name = name;
        Outputs = outputs.ToList();
        modules = network;
        PulseCountLow = 0;
        PulseCountHigh = 0;
    }

    protected void CountPulses(bool isHigh)
    {
        if (isHigh) PulseCountHigh++;
        else PulseCountLow++;
    }
    /// <summary>
    /// Modify state, then send new pulse to all outputs.
    /// </summary>
    /// <param name="_from">Unused unless concrete Conjunction Module</param>
    /// <param name="_isHigh">The (originally) received pulse</param>
    protected abstract void Pulse(string from, bool isHigh);
    /// <summary>
    /// Send pulse to all outputs
    /// </summary>
    protected void SendPulse(bool pulsePolarity)
    {
        foreach (string output in Outputs)
        {
            Network.Backlog.Enqueue((Name, output, pulsePolarity));
        }
    }

    public void ProcessBacklog()
    {
        while (Network.Backlog.Count > 0)
        {
            (string origin, string destination, bool cachedPolarity) = Network.Backlog.Dequeue();
            if (!Network.Components.ContainsKey(destination))
            {
                modules.Add(destination, new Output(modules));
            }
            Network.Components[destination].Pulse(origin, cachedPolarity);
        }
    }
    protected virtual void PulseStatsOnly(string _from, bool isHigh)
    {
        if (isHigh) PulseCountHigh++;
        else PulseCountLow++;
    }
}

class Output : Module
{
    public bool State { get; private set; } = true;
    public Output(Dictionary<string, Module> network) :
        base("output", new string[] { }, network) { }
    public Output(string name, IEnumerable<string> outputs, Dictionary<string, Module> network) :
        base(name, outputs, network)
    {
        if (outputs.Count() > 0)
            throw new Exception("Output should have no outputs!");
    }

    protected override void Pulse(string _from, bool isHigh)
    {
        base.CountPulses(isHigh);
        State = isHigh;
    }
}
/// <summary>
/// A very simple module that Pulse()'s a low pulse when Press()'ed.
/// Cannot be Pulse()'d
/// </summary>
class Button : Module
{
    public Button(IEnumerable<string> outputs, Dictionary<string, Module> network) : 
        base("Button", outputs, network) { }
    public Button(string name, IEnumerable<string> outputs, Dictionary<string, Module> network) :
        base(name, outputs, network) { }

    protected override void Pulse(string _from, bool _isHigh)
    {
        throw new Exception("Buttons cannot be Pulse'd. Use Press() instead.");
    }
    public void Press()
    {
        base.SendPulse(false); // Hardcoded low pulse
    }
}

/// <summary>
/// A Module that passes along any pulse, unmodified, to all outputs.
/// </summary>
class Broadcaster : Module
{
    public Broadcaster(string name, IEnumerable<string> outputs, Dictionary<string, Module> network) :
        base(name, outputs, network) { }

    protected override void Pulse(string from, bool isHigh)
    {
        base.CountPulses(isHigh);
        base.SendPulse(isHigh);
    }
}
/// <summary>
/// A Module whose state toggles on each low pulse
/// High pulses are counted, but otherwise ignored
/// </summary>
class FlipFlop : Module
{
    bool state;
    public FlipFlop(string name, IEnumerable<string> outputs, Dictionary<string, Module> network) :
        base(name, outputs, network)
    {
        state = false;
    }

    protected override void Pulse(string from, bool isHigh)
    {
        base.CountPulses(isHigh);
        if (!isHigh)
        {
            state = !state;
            base.SendPulse(state);
        }
    }
}
/// <summary>
/// A Module whose multiple input (history) must all be high in order to Pulse low.
/// Aka a "NAND gate" -- or, with a single input, an inverter.
/// All inputs must be Registered() before any Pulse can be received.
/// Default history for each input registered is a low pulse.
/// When a pulse is received, the conjunction module first updates its
/// memory for that input.
/// Then, if all inputs (history) are high,
/// it sends a low pulse; Otherwise, it sends a high pulse.
/// </summary>
class Conjunction : Module
{
    Dictionary<string, bool> history = new();
    bool activityStarted;
    public Conjunction(string name, IEnumerable<string> outputs, Dictionary<string, Module> network) :
        base(name, outputs, network)
    {
        history = new();
        activityStarted = false;
    }

    public void RegisterInput(string name)
    {
        if (activityStarted) throw new Exception("Cannot register input after a Pulse has been received.");
        history.Add(name, default);
    }

    protected override void Pulse(string from, bool isHigh)
    {
        base.CountPulses(isHigh);
        activityStarted = true;
        if (history.Count == 0) throw new Exception("Must register inputs before use of Conjunction Modules");
        if (!history.ContainsKey(from)) throw new Exception($"Unregistered input {from} for ConjunctionModule {Name}");
        history[from] = isHigh;
        bool newPulse = history.Any(kv => !kv.Value);
        base.SendPulse(newPulse);
    }
}
public enum ModuleType
{
    Button,
    Broadcaster,
    FlipFlop,
    Conjunction,
}

public static class MyExtensions
{
    public static ModuleType ToModule(this char? c)
    {
        switch (c)
        {
            case null:
                return ModuleType.Broadcaster;
            case 'B':
                return ModuleType.Button;
            case '&':
                return ModuleType.Conjunction;
            case '%':
                return ModuleType.FlipFlop;
            default:
                throw new Exception("Unknown module character: '{c}'");
        }
    }
} 