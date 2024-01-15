using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LipUI.Models.Lip;

public class LipCommandSyntaxException : Exception
{
    public LipCommandSyntaxException(string? message) : base(message)
    {
    }
}

public class LipCommandContext
{
    private readonly List<LipCommand> commands = new();
    private readonly List<LipCommandOption> options = new();

    private LipCommand? current;
    private string? parameter;

    public LipCommandContext() { }

    public LipCommandContext Append(LipCommand command)
    {
        if (current is null || (command.Parent is not null && command.Parent == current))
        {
            commands.Add(command);
            current = command;
            return this;
        }

        throw new LipCommandSyntaxException(this);
    }

    public LipCommandContext Append(LipCommandOption option)
    {
        if (current is not null && option.AvailableCommands.First(c => c == current) is not null)
        {
            options.Add(option);
            return this;
        }

        throw new LipCommandSyntaxException(this);
    }

    public LipCommandContext Commands(params LipCommand[] commands)
    {
        foreach (var option in commands) Append(option);
        return this;
    }

    public LipCommandContext Options(params LipCommandOption[] options)
    {
        foreach (var option in options) Append(option);
        return this;
    }

    public LipCommandContext Parameter(string param)
    {
        parameter = param;
        return this;
    }

    public static LipCommandContext operator +(LipCommandContext context, LipCommand command)
        => context.Append(command);

    public static LipCommandContext operator +(LipCommandContext context, LipCommandOption option)
        => context.Append(option);

    public static LipCommandContext operator +(LipCommandContext context, string param)
        => context.Parameter(param);

    public override string ToString()
    {
        var bulider = new StringBuilder();
        foreach (var cmd in commands)
            bulider.Append($"{cmd} ");
        foreach (var option in options)
            bulider.Append($"{option} ");
        if (parameter is not null)
            bulider.Append(parameter);
        return bulider.ToString().Trim();
    }

    public static implicit operator string(LipCommandContext command)
        => command.ToString();
}
