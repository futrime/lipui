using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Language;
using LipUI.Models;

namespace LipUI.ViewModels;
public class LanguageSelectorViewModel : ObservableObject
{
    public AppConfig Config => Global.Config;
    public Utils.LanguageDescriptionItem[] AvailableLanguages => Language.Utils.AvailableLanguages;
    public Utils.LanguageDescriptionItem? CurrentLanguage
    {
        get
        {
            return AvailableLanguages.FirstOrDefault(x => x.Id == Config.Language);
        }
        set
        {
            if (value is not null)
            {
                Utils.CurrentLangId = value.Id;
            }
        }
    }
}
