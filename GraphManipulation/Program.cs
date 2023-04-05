﻿// See https://aka.ms/new-console-template for more information

using GraphManipulation.Helpers;
using GraphManipulation.Logging;

namespace GraphManipulation;

public static class Program
{
    private static readonly string ProjectFolder =
        Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

    private static readonly string FilePath = Path.Combine(ProjectFolder, "config.json");
    private static readonly ConfigManager Cf = new(FilePath);

    public static void Main()
    {
        Interactive();
    }

    private static void Interactive()
    {
        var interactive = new InteractiveMode(Cf, new PlaintextLogger(Cf));
        interactive.Run();
    }
}