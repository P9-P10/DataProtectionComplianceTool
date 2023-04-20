// See https://aka.ms/new-console-template for more information

using GraphManipulation.Helpers;
using GraphManipulation.Linking;
using GraphManipulation.Logging;
using GraphManipulation.Managers;

namespace GraphManipulation;

public static class Program
{
    private static readonly string ProjectFolder =
        Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

    private static readonly string FilePath = Path.Combine(ProjectFolder, "config.json");
    private static readonly ConfigManager Cf = new(FilePath);

    public static void Main()
    {
        var demonstration = new Demonstration<int, string, string, int>(
            new MetadataManager<int>(),
            new VacuumingManager<string>(),
            new PurposeManager<string>(),
            new OriginManager<int>(),
            new PlaintextLogger(new ConfigManager("")),
            new VacuumingPurposeLinker<string, string>(),
            new PurposeMetadataLinker<string, int>(),
            new OriginMetadataLinker<int, int>());
        
        Interactive();
    }

    private static void Interactive()
    {
        var interactive = new InteractiveMode(Cf, new PlaintextLogger(Cf));
        interactive.Run();
    }
}