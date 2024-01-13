using LipUI.Pages.Home.Modules;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace LipUI.Models;

public interface ILipUIModulesNonGeneric
{
    public string ModuleName { get; }

    public Type PageType { get; }

    public FrameworkElement? IconContent { get => null; }

    public Brush? IconBackground { get => null; }

    public void OnIconInitialze(ModuleIcon icon) { }

    public void OnExit() { }

    public void OnEnable() { }

    public void OnDisable() { }
}

public interface ILipUIModules<TSelf> : ILipUIModulesNonGeneric
    where TSelf : ILipUIModules<TSelf>, new()
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class LipUIModuleAttribute : Attribute
{
}
