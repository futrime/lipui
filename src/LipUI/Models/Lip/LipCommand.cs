namespace LipUI.Models.Lip;

public class LipCommand
{
    public readonly string Command;
    public readonly LipCommand? Parent;

    private LipCommand() => throw new NotSupportedException();

    private LipCommand(string command, LipCommand? parent = null)
    {
        Command = command;
        Parent = parent;
    }



    public static readonly LipCommand Lip = new("lip");

    public static readonly LipCommand AutoRemove = new("autoremove", Lip);

    public static readonly LipCommand Cache = new("cache", Lip);

    public static readonly LipCommand Purge = new("purge", Cache);

    public static readonly LipCommand Install = new("install", Lip);

    public static readonly LipCommand List = new("list", List);

    public static readonly LipCommand Show = new("show", Lip);

    public static readonly LipCommand Tooth = new("tooth", Lip);

    public static readonly LipCommand Init = new("init", Tooth);

    public static readonly LipCommand Pack = new("pack", Tooth);

    public static readonly LipCommand Uninstall = new("uninstall", Lip);

    public static readonly LipCommand Config = new("config", Lip);

    public override string ToString() => Command;

    public static explicit operator string(LipCommand command) => command.Command;

    public static bool operator ==(LipCommand left, LipCommand right)
        => left.Equals(right);

    public static bool operator !=(LipCommand left, LipCommand right)
        => !left.Equals(right);

    public static LipCommandContext CreateCommand() => new();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        if (obj is LipCommand command)
            return Command == command.Command;

        return false;
    }

    public override int GetHashCode() => Command.GetHashCode();
}
