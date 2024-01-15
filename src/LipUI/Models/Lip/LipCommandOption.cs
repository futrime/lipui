using System;
using static LipUI.Models.Lip.LipCommand;

namespace LipUI.Models.Lip;

public class LipCommandOption
{
    public readonly string? Abbreviation;
    public readonly string Option;
    public readonly LipCommand[] AvailableCommands;

    private LipCommandOption() => throw new NotSupportedException();

    public LipCommandOption(string? abbreviation, string option, params LipCommand[] availableCommands)
    {
        Abbreviation = abbreviation;
        Option = option;
        AvailableCommands = availableCommands;
    }

    public static readonly LipCommandOption Help
        = new("h", "help", LipCommand.Lip, AutoRemove, Cache, Purge, Install, List, Show, Tooth, Init, Pack, Uninstall);

    public static readonly LipCommandOption Version
        = new("V", "version", LipCommand.Lip);

    public static readonly LipCommandOption Verbose
        = new("v", "verbose", LipCommand.Lip);

    public static readonly LipCommandOption Quiet
        = new("q", "quiet", LipCommand.Lip);

    public static readonly LipCommandOption Yes
        = new("y", "yes", AutoRemove, Install, Uninstall);

    public static readonly LipCommandOption Upgrade
        = new(null, "upgrade", Install);

    public static readonly LipCommandOption ForceReinstall
        = new(null, "force-reinstall", Install);

    public static readonly LipCommandOption NoDependencies
        = new(null, "no-dependencies", Install);

    public static readonly LipCommandOption Upgradable
        = new(null, "upgradable", List);

    public static readonly LipCommandOption Json
        = new(null, "json", List, Show);

    public static readonly LipCommandOption Available
        = new(null, "available", Show);

    public static readonly LipCommandOption KeepPossession
        = new(null, "keep-possession", Uninstall);

    public override string ToString()
    {
        if (Abbreviation is not null)
            return $"-{Abbreviation}";
        else
            return $"--{Option}";
    }

    public static explicit operator string(LipCommandOption option) => option.ToString();
}
