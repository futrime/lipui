using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Models;

namespace LipUI.ViewModels;
public class LanguageSelectorViewModel : ObservableObject
{
    public record LanguageDescriptionItem(string Id, string Name);
    public AppConfig Config => Global.Config;
    public ObservableCollection<LanguageDescriptionItem> AvailableLanguages => new()
    {
        new("zh_Hans","简体中文"),
        new("en","English")
    };
}
